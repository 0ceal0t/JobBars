using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Newtonsoft.Json.Linq;
using JobBars.Helper;
using Dalamud.Game.Internal;
using JobBars.UI;
using JobBars.GameStructs;
using Dalamud.Hooking;
using JobBars.Data;
using Dalamud.Game.ClientState.Actors.Types;
using Dalamud.Game.ClientState.Actors.Types.NonPlayer;
using JobBars.Gauges;
using Dalamud.Game.ClientState.Actors.Resolvers;
using JobBars.Buffs;
using JobBars.PartyList;
using System.Runtime.InteropServices;

#pragma warning disable CS0659
namespace JobBars {
    public unsafe partial class JobBars : IDalamudPlugin {
        public string Name => "JobBars";
        public DalamudPluginInterface PluginInterface { get; private set; }
        public string AssemblyLocation { get; private set; } = Assembly.GetExecutingAssembly().Location;

        private UIBuilder UI;
        private GaugeManager GManager;
        private BuffManager BManager;
        private Configuration Config;

        private delegate void ReceiveActionEffectDelegate(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private Hook<ReceiveActionEffectDelegate> receiveActionEffectHook;

        private delegate void ActorControlSelfDelegate(uint entityId, uint id, uint arg0, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, UInt64 targetId);
        private Hook<ActorControlSelfDelegate> actorControlSelfHook;

        private PList Party; // TEMP
        private int LastPartyCount = 0;

        private HashSet<uint> GCDs = new HashSet<uint>();

        private bool Ready => (PluginInterface.ClientState != null && PluginInterface.ClientState.LocalPlayer != null);
        private bool Init = false;

        public void Initialize(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;
            UiHelper.Setup(pluginInterface.TargetModuleScanner);
            UIColor.SetupColors();
            SetupActions();

            Party = new PList(pluginInterface, pluginInterface.TargetModuleScanner); // TEMP

            Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Config.Initialize(PluginInterface);
            UI = new UIBuilder(pluginInterface);

            IntPtr receiveActionEffectFuncPtr = PluginInterface.TargetModuleScanner.ScanText("4C 89 44 24 18 53 56 57 41 54 41 57 48 81 EC ?? 00 00 00 8B F9");
            receiveActionEffectHook = new Hook<ReceiveActionEffectDelegate>(receiveActionEffectFuncPtr, (ReceiveActionEffectDelegate)ReceiveActionEffect);
            receiveActionEffectHook.Enable();

            IntPtr actorControlSelfPtr = pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64");
            actorControlSelfHook = new Hook<ActorControlSelfDelegate>(actorControlSelfPtr, (ActorControlSelfDelegate)ActorControlSelf);
            actorControlSelfHook.Enable();

            PluginInterface.UiBuilder.OnBuildUi += BuildUI;
            PluginInterface.UiBuilder.OnOpenConfigUi += OnOpenConfig;
            PluginInterface.Framework.OnUpdateEvent += FrameworkOnUpdate;
            PluginInterface.ClientState.TerritoryChanged += ZoneChanged;
            SetupCommands();
        }

        public void Dispose() {
            receiveActionEffectHook?.Disable();
            receiveActionEffectHook?.Dispose();
            receiveActionEffectHook = null;

            actorControlSelfHook?.Disable();
            actorControlSelfHook?.Dispose();
            actorControlSelfHook = null;

            PluginInterface.UiBuilder.OnBuildUi -= BuildUI;
            PluginInterface.UiBuilder.OnOpenConfigUi -= OnOpenConfig;
            PluginInterface.Framework.OnUpdateEvent -= FrameworkOnUpdate;
            PluginInterface.ClientState.TerritoryChanged -= ZoneChanged;

            UI.Dispose();

            RemoveCommands();
        }

        // ========= HOOKS ===========
        private void SetupActions() {
            var sheet = PluginInterface.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where(x => !string.IsNullOrEmpty(x.Name) && x.IsPlayerAction);
            foreach(var item in sheet) {
                var attackType = item.ActionCategory.Value.Name.ToString();
                if(attackType == "Spell" || attackType == "Weaponskill") {
                    GCDs.Add(item.RowId);
                }
            }
        }
        private void ReceiveActionEffect(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail) {
            if (!Ready || !Init) {
                receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
                return;
            }

            uint id = *((uint*)effectHeader.ToPointer() + 0x2);
            ushort op = *((ushort*)effectHeader.ToPointer() - 0x7);

            var isSelf = sourceId == PluginInterface.ClientState.LocalPlayer.ActorId;
            var isPet = (GManager?.CurrentJob == JobIds.SMN || GManager?.CurrentJob == JobIds.SCH) ? sourceId == FindCharaPet() : false;
            var isParty = !isSelf && !isPet && IsInParty(sourceId);
            if(!(isSelf || isPet || isParty)) {
                receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
                return;
            }

            var actionItem = new Item
            {
                Id = id,
                Type = (GCDs.Contains(id) ? ItemType.GCD : ItemType.OGCD)
            };

            if(!isParty) { // don't let party members affect our gauge
                GManager?.PerformAction(actionItem);
            }
            BManager?.PerformAction(actionItem);

            byte targetCount = *(byte*)(effectHeader + 0x21);
            int effectsEntries = 0;
            int targetEntries = 1;
            if (targetCount == 0) {
                effectsEntries = 0;
                targetEntries = 1;
            }
            else if (targetCount == 1) {
                effectsEntries = 8;
                targetEntries = 1;
            }
            else if (targetCount <= 8) {
                effectsEntries = 64;
                targetEntries = 8;
            }
            else if (targetCount <= 16) {
                effectsEntries = 128;
                targetEntries = 16;
            }
            else if (targetCount <= 24) {
                effectsEntries = 192;
                targetEntries = 24;
            }
            else if (targetCount <= 32) {
                effectsEntries = 256;
                targetEntries = 32;
            }

            List<EffectEntry> entries = new List<EffectEntry>(effectsEntries);
            for (int i = 0; i < effectsEntries; i++) {
                entries.Add(*(EffectEntry*)(effectArray + i * 8));
            }
            ulong[] targets = new ulong[targetEntries];
            for (int i = 0; i < targetCount; i++) {
                targets[i] = *(ulong*)(effectTrail + i * 8);
            }
            for (int i = 0; i < entries.Count; i++) {
                ulong tTarget = targets[i / 8];
                if(entries[i].type == ActionEffectType.Gp_Or_Status || entries[i].type == ActionEffectType.ApplyStatusEffectTarget) {
                    var buffItem = new Item
                    {
                        Id = entries[i].value,
                        Type = ItemType.Buff
                    };

                    if(!isParty) { // don't let party members affect our gauge
                        GManager?.PerformAction(buffItem);
                    }
                    if((int) tTarget == PluginInterface.ClientState.LocalPlayer.ActorId) { // only really care about buffs on us
                        BManager?.PerformAction(buffItem);
                    }
                }
            }

            receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
        }
        private void ActorControlSelf(uint entityId, uint id, uint arg0, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, UInt64 targetId) {
            actorControlSelfHook.Original(entityId, id, arg0, arg1, arg2, arg3, arg4, arg5, targetId);
            if (arg1 == 0x40000010) { // WIPE
                Reset();
            }
        }
        private void ZoneChanged(object sender, ushort e) {
            Reset();
        }

        // ======= DATA ==========
        private int GetCharacterActorId() {
            if (PluginInterface.ClientState.LocalPlayer != null)
                return PluginInterface.ClientState.LocalPlayer.ActorId;
            return 0;
        }
        private int FindCharaPet() {
            int charaId = GetCharacterActorId();
            foreach (Actor a in PluginInterface.ClientState.Actors) {
                if (!(a is BattleNpc npc)) continue;
                IntPtr actPtr = npc.Address;
                if (actPtr == IntPtr.Zero) continue;
                if (npc.OwnerId == charaId)
                    return npc.ActorId;
            }
            return -1;
        }
        private bool IsInParty(int actorId) {
            foreach(var pMember in Party) {
                if(pMember.Actor != null && pMember.Actor.ActorId == actorId) {
                    return true;
                }
            }
            return false;
        }

        // ======== UPDATE =========
        private void FrameworkOnUpdate(Framework framework) {
            if (!Ready) {
                if(Init && !UI.IsInitialized()) { // a logout, need to recreate everything once we're done
                    PluginLog.Log("LOGOUT");
                    Init = false;
                    CurrentJob = JobIds.OTHER;
                }
                return;
            }
            if (!Init) {
                if(UI._ADDON == null) {
                    return;
                }
                UI.SetupTex();
                UI.SetupPart();
                UI.Init();
                GManager = new GaugeManager(PluginInterface, UI);
                BManager = new BuffManager(UI);
                UI.HideAllBuffs();
                UI.HideAllGauges();
                Init = true;
                return;
            }

            SetJob(PluginInterface.ClientState.LocalPlayer.ClassJob);
            GManager?.Tick();
            BManager?.Tick();
            Animation.Tick();

            if(Party.Count < LastPartyCount) {
                BManager?.SetJob(CurrentJob);
            }
            LastPartyCount = Party.Count;
        }

        JobIds CurrentJob = JobIds.OTHER;
        private void SetJob(ClassJob job) {
            JobIds _job = job.Id < 19 ? JobIds.OTHER : (JobIds)job.Id;
            if (_job != CurrentJob) {
                CurrentJob = _job;
                PluginLog.Log($"SWITCHED JOB TO {CurrentJob}");
                Reset();
            }
        }
        private void Reset() {
            GManager?.SetJob(CurrentJob);
            BManager?.SetJob(CurrentJob);
        }

        // ======= COMMANDS ============
        public void SetupCommands() {
            PluginInterface.CommandManager.AddHandler("/jobbars", new Dalamud.Game.Command.CommandInfo(OnCommand) {
                HelpMessage = $"Open config window for {this.Name}",
                ShowInHelp = true
            });
        }
        private void OnOpenConfig(object sender, EventArgs eventArgs) {
            Visible = true;
        }
        public void OnCommand(object command, object args) {
            Visible = !Visible;
        }
        public void RemoveCommands() {
            PluginInterface.CommandManager.RemoveHandler("/jobbars");
        }
    }

    // ===== BUFF OR ACTION ======
    public enum ItemType {
        Buff,
        Action, // either GCD or OGCD
        GCD,
        OGCD
    }
    public struct Item {
        public uint Id;
        public ItemType Type;

        // GENERATORS
        public Item(ActionIds action) {
            Id = (uint)action;
            Type = ItemType.Action;
        }
        public Item(BuffIds buff) {
            Id = (uint)buff;
            Type = ItemType.Buff;
        }

        // EQUALITY
        public override bool Equals(object obj) {
            return obj is Item overrides && Equals(overrides);
        }
        public bool Equals(Item other) {
            return (Id == other.Id) && ((Type == ItemType.Buff) == (other.Type == ItemType.Buff));
        }
        public static bool operator ==(Item left, Item right) {
            return left.Equals(right);
        }
        public static bool operator !=(Item left, Item right) {
            return !(left == right);
        }
        public override int GetHashCode() {
            int hash = 13;
            hash = (hash * 7) + Id.GetHashCode();
            hash = (hash * 7) + (Type == ItemType.Buff).GetHashCode();
            return hash;
        }
    }
}
