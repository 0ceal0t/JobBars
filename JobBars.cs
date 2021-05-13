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

        public UIBuilder UI;
        public GaugeManager GManager;
        public BuffManager BManager;
        public Configuration _Config;

        private delegate void ReceiveActionEffectDelegate(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private Hook<ReceiveActionEffectDelegate> receiveActionEffectHook;
        private delegate byte AddStatusDelegate(IntPtr statusEffectList, uint slot, UInt16 statusId, float duration, UInt16 a5, uint sourceActorId, byte newStatus);
        private Hook<AddStatusDelegate> addStatusHook;

        private delegate void ActorControlSelfDelegate(uint entityId, uint id, uint arg0, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, UInt64 targetId);
        private Hook<ActorControlSelfDelegate> actorControlSelfHook;
        private delegate byte InitZoneDelegate(IntPtr a1, int a2, IntPtr a3);
        private Hook<InitZoneDelegate> initZoneHook;

        private PList Party; // TEMP
        private HashSet<uint> GCDs = new HashSet<uint>();

        private bool _Ready => (PluginInterface.ClientState != null && PluginInterface.ClientState.LocalPlayer != null);
        private bool Init = false;

        public void Initialize(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;
            UiHelper.Setup(pluginInterface.TargetModuleScanner);
            UIColor.SetupColors();
            SetupActions();

            Party = new PList(pluginInterface, pluginInterface.TargetModuleScanner); // TEMP

            _Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            _Config.Initialize(PluginInterface);
            UI = new UIBuilder(pluginInterface);

            IntPtr receiveActionEffectFuncPtr = PluginInterface.TargetModuleScanner.ScanText("4C 89 44 24 18 53 56 57 41 54 41 57 48 81 EC ?? 00 00 00 8B F9");
            receiveActionEffectHook = new Hook<ReceiveActionEffectDelegate>(receiveActionEffectFuncPtr, (ReceiveActionEffectDelegate)ReceiveActionEffect);
            receiveActionEffectHook.Enable();
            IntPtr addStatusFuncPtr = PluginInterface.TargetModuleScanner.ScanText("40 53 55 56 48 83 EC 60 40 32 ED 41 0F B7 F0 48 8B D9");
            addStatusHook = new Hook<AddStatusDelegate>(addStatusFuncPtr, (AddStatusDelegate)AddStatus);
            addStatusHook.Enable();

            IntPtr actorControlSelfPtr = pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64");
            actorControlSelfHook = new Hook<ActorControlSelfDelegate>(actorControlSelfPtr, (ActorControlSelfDelegate)ActorControlSelf);
            actorControlSelfHook.Enable();
            IntPtr initZonePtr = pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 45 33 C0 48 8D 53 10 8B CE E8 ?? ?? ?? ?? 48 8D 4B 60 E8 ?? ?? ?? ?? 48 8D 4B 6C");
            initZoneHook = new Hook<InitZoneDelegate>(initZonePtr, (InitZoneDelegate)InitZone);
            initZoneHook.Enable();
            //IntPtr removeStatusFuncPtr = PluginInterface.TargetModuleScanner.ScanText("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 30 80 7C 24 ?? ?? 41 8B D9 8B FA"); IntPtr a1, uint statusId, IntPtr a3, uint sourceActorId, byte a5, byte a6
            //IntPtr testPtr = PluginInterface.TargetModuleScanner.ScanText("48 8B C4 55 57 41 56 48 83 EC 60 83 3D ?? ?? ?? ?? ?? 41 0F B6 E8"); // IntPtr a1, IntPtr a2, IntPtr a3) <--- status list
            //IntPtr testPtr2 = PluginInterface.TargetModuleScanner.ScanText("48 8B C4 57 41 54 41 57 48 83 EC 60 83 3D ?? ?? ?? ?? ?? 45 0F B6 E0"); // IntPtr a1, IntPtr a2, IntPtr a3) <---- action effect

            PluginInterface.UiBuilder.OnBuildUi += BuildUI;
            PluginInterface.Framework.OnUpdateEvent += FrameworkOnUpdate;
            SetupCommands();
        }

        public void Dispose() {
            receiveActionEffectHook?.Disable();
            receiveActionEffectHook?.Dispose();
            addStatusHook?.Disable();
            addStatusHook?.Dispose();

            actorControlSelfHook?.Disable();
            actorControlSelfHook?.Dispose();
            initZoneHook?.Disable();
            initZoneHook?.Dispose();

            PluginInterface.UiBuilder.OnBuildUi -= BuildUI;
            PluginInterface.Framework.OnUpdateEvent -= FrameworkOnUpdate;

            UI.Dispose();

            RemoveCommands();
        }

        // ========= HOOKS ===========
        private byte AddStatus(IntPtr statusEffectList, uint slot, UInt16 statusId, float duration, UInt16 a5, uint sourceActorId, byte newStatus) { // STATUS DURATION SENT SEPARATELY
            if (_Ready && Init && sourceActorId == PluginInterface.ClientState.LocalPlayer.ActorId) {
                GManager?.SetBuffDuration(new Item
                {
                    Id = statusId,
                    Type = ItemType.BUFF
                }, duration, newStatus == 0);
            }
            return addStatusHook.Original(statusEffectList, slot, statusId, duration, a5, sourceActorId, newStatus);
        }
        private void SetupActions() {
            var _sheet = PluginInterface.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where(x => !string.IsNullOrEmpty(x.Name) && x.IsPlayerAction);
            foreach(var item in _sheet) {
                var attackType = item.ActionCategory.Value.Name.ToString();
                if(attackType == "Spell" || attackType == "Weaponskill") {
                    GCDs.Add(item.RowId);
                }
            }
        }
        private void ReceiveActionEffect(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail) {
            if (!_Ready || !Init) {
                receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
                return;
            }

            uint id = *((uint*)effectHeader.ToPointer() + 0x2);
            ushort op = *((ushort*)effectHeader.ToPointer() - 0x7);

            var isSelf = sourceId == PluginInterface.ClientState.LocalPlayer.ActorId;
            var isPet = (GManager?.CurrentJob == JobIds.SMN || GManager?.CurrentJob == JobIds.SCH) ? sourceId == FindCharaPet() : false;
            if(!(isSelf || isPet)) {
                receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
                return;
            }

            var actionItem = new Item
            {
                Id = id,
                Type = (GCDs.Contains(id) ? ItemType.GCD : ItemType.OGCD)
            };
            GManager?.PerformAction(actionItem);
            BManager?.PerformAction(actionItem); // TODO: only trigger if performed by party member

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
                        Type = ItemType.BUFF
                    };
                    GManager?.PerformAction(buffItem);
                    if((int) tTarget == PluginInterface.ClientState.LocalPlayer.ActorId) {
                        BManager?.PerformAction(buffItem);
                    }
                }
            }
            receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
        }
        private void ActorControlSelf(uint entityId, uint id, uint arg0, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, UInt64 targetId) {
            if(arg1 == 0x40000010) { // it's a wipe!
                Reset();
            }
            actorControlSelfHook.Original(entityId, id, arg0, arg1, arg2, arg3, arg4, arg5, targetId);
        }
        private byte InitZone(IntPtr a1, int a2, IntPtr a3) {
            Reset();
            return initZoneHook.Original(a1, a2, a3);
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

        // ======== UPDATE =========
        private void FrameworkOnUpdate(Framework framework) {
            if (!_Ready) {
                if(Init && !UI.IsInitialized()) { // a logout, need to recreate everything once we're done
                    PluginLog.Log("LOGOUT");
                    Init = false;
                    CurrentJob = JobIds.OTHER;
                }
                return;
            }
            if (!Init) {
                UI.SetupTex();
                UI.SetupPart();
                UI.Init();
                GManager = new GaugeManager(UI);
                BManager = new BuffManager(UI);
                UI.HideAllBuffs();
                UI.HideAllGauges();
                Init = true;
                return;
            }

            SetJob(PluginInterface.ClientState.LocalPlayer.ClassJob);
            GManager?.Tick();
            BManager?.Tick();
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
            PluginInterface.CommandManager.AddHandler("/jobbars", new Dalamud.Game.Command.CommandInfo(OnConfigCommandHandler) {
                HelpMessage = $"Open config window for {this.Name}",
                ShowInHelp = true
            });
        }
        public void OnConfigCommandHandler(object command, object args) {
            Visible = !Visible;
        }
        public void RemoveCommands() {
            PluginInterface.CommandManager.RemoveHandler("/jobbars");
        }
    }
}
