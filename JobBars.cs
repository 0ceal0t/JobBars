﻿using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
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
using FFXIVClientInterface;
using Dalamud.Game.ClientState;

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
        private JobIds CurrentJob = JobIds.OTHER;
        private HashSet<uint> GCDs = new HashSet<uint>();

        private delegate void ReceiveActionEffectDelegate(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private Hook<ReceiveActionEffectDelegate> receiveActionEffectHook;

        private delegate void ActorControlSelfDelegate(uint entityId, uint id, uint arg0, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, UInt64 targetId, byte a10);
        private Hook<ActorControlSelfDelegate> actorControlSelfHook;

        private PList Party; // TEMP
        private int LastPartyCount = 0;

        private bool Ready => (PluginInterface.ClientState != null && PluginInterface.ClientState.LocalPlayer != null);
        private bool Init = false;

        private Vector2 LastPosition;
        private Vector2 LastScale;

        public static ClientInterface Client;

        /*
         * SIG LIST:
         *  Lord have mercy when 6.0 comes
         * =================================
         * 
         *  FFXIVClientInterface/ClientInterface.cs
         *      Get UI Module (E8 ?? ?? ?? ?? 48 8B C8 48 8B 10 FF 52 40 80 88 ?? ?? ?? ?? 01 E9)
         *  FFXIVClientInterface/Client/UI/Agent/AgentModule.cs
         *      getAgentByInternalID (E8 ?? ?? ?? ?? 83 FF 0D)
         *  FFXIVClientInterface/Client/Game/ActionManager.cs
         *      BaseAddress (E8 ?? ?? ?? ?? 33 C0 E9 ?? ?? ?? ?? 8B 7D 0C)
         *      GetRecastGroup (E8 ?? ?? ?? ?? 8B D0 48 8B CD 8B F0) 
         *      GetGroupTimer (E8 ?? ?? ?? ?? 0F 57 FF 48 85 C0)
         *      GetAdjustedActionId (E8 ?? ?? ?? ?? 8B F8 3B DF) 
         *      
         *  Helper/UiHelper.GameFunctions.cs
         *      _atkTextNodeSetText (E8 ?? ?? ?? ?? 49 8B FC)
         *      _gameAlloc (E8 ?? ?? ?? ?? 45 8D 67 23)
         *      _getGameAllocator (E8 ?? ?? ?? ?? 8B 75 08)
         *      _playSe (E8 ?? ?? ?? ?? 4D 39 BE ?? ?? ?? ??)
         *      
         *  JobBars.cs
         *      receiveActionEffectFuncPtr (4C 89 44 24 18 53 56 57 41 54 41 57 48 81 EC ?? 00 00 00 8B F9)
         *      actorControlSelfPtr (E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64)
         *  UIIconManager.cs
         *      setIconRecastPtr (40 53 48 83 EC 20 48 8B D9 E8 ?? ?? ?? ?? 48 8B 4B 10 48 85 C9 74 23)
         *      setIconRecastPtr2 (40 53 48 83 EC 20 0F B6 81 ?? ?? ?? ?? 48 8B D9 48 83 C1 08 A8 01 74 1E 48 83 79 ?? ?? 74 17 A8 08 75 0E 48 83 79 ?? ?? 75 07 E8 ?? ?? ?? ?? EB 05 E8 ?? ?? ?? ?? F6 83 ?? ?? ?? ?? ?? 0F 84 ?? ?? ?? ?? 48 8B 93 ?? ?? ?? ??)
         *      setIconTextPtr (55 57 48 83 EC 28 0F B6 44 24 ?? 8B EA 48 89 5C 24 ?? 48 8B F9)
         *      setIconTextPtr2 (4C 8B DC 53 55 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 49 89 73 18 48 8B EA)
         *  UIBuilder.cs
         *      loadAssetsPtr (E8 ?? ?? ?? ?? 48 8B 84 24 ?? ?? ?? ?? 41 B9 ?? ?? ?? ??)
         *      loadTexAllocPtr (48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8B 01 49 8B D8 48 8B FA 48 8B F1 FF 50 48)
         *      texUnallocPtr (E8 ?? ?? ?? ?? C6 43 10 02)
         *      
         *  PartyList/PartyList.cs
         *      GroupManager (48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 80 B8 ?? ?? ?? ?? ?? 76 50)
         *      CrossRealmGroupManagerPtr (77 71 48 8B 05)
         *      CompanionManagerPtr (4C 8B 15 ?? ?? ?? ?? 4C 8B C9)
         *      GetCrossRealmMemberCountPtr (E8 ?? ?? ?? ?? 3C 01 77 4B)
         *      GetCrossMemberByGrpIndexPtr (E8 ?? ?? ?? ?? 44 89 7C 24 ?? 4C 8B C8)
         *      GetCompanionMemberCountPtr (E8 ?? ?? ?? ?? 8B D3 85 C0)
         */

        public void Initialize(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;
            Client = new ClientInterface(pluginInterface.TargetModuleScanner, pluginInterface.Data);
            UiHelper.Setup(pluginInterface.TargetModuleScanner);
            UIColor.SetupColors();

            Party = new PList(pluginInterface, pluginInterface.TargetModuleScanner); // TEMP

            Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Config.Initialize(PluginInterface);
            SetupActions();
            UI = new UIBuilder(pluginInterface);

            IntPtr receiveActionEffectFuncPtr = PluginInterface.TargetModuleScanner.ScanText("4C 89 44 24 18 53 56 57 41 54 41 57 48 81 EC ?? 00 00 00 8B F9");
            receiveActionEffectHook = new Hook<ReceiveActionEffectDelegate>(receiveActionEffectFuncPtr, (ReceiveActionEffectDelegate)ReceiveActionEffect);
            receiveActionEffectHook.Enable();

            IntPtr actorControlSelfPtr = pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64");
            actorControlSelfHook = new Hook<ActorControlSelfDelegate>(actorControlSelfPtr, (ActorControlSelfDelegate)ActorControlSelf);
            actorControlSelfHook.Enable();

            PluginInterface.UiBuilder.OnBuildUi += BuildSettingsUI;
            PluginInterface.UiBuilder.OnBuildUi += Animate;
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

            PluginInterface.UiBuilder.OnBuildUi -= BuildSettingsUI;
            PluginInterface.UiBuilder.OnBuildUi -= Animate;
            PluginInterface.UiBuilder.OnOpenConfigUi -= OnOpenConfig;
            PluginInterface.Framework.OnUpdateEvent -= FrameworkOnUpdate;
            PluginInterface.ClientState.TerritoryChanged -= ZoneChanged;

            Animation.Cleanup();
            UI.Dispose();
            Client.Dispose();

            RemoveCommands();
        }

        private void SetupActions() {
            var sheet = PluginInterface.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where(x => !string.IsNullOrEmpty(x.Name) && (x.IsPlayerAction || x.ClassJob.Value != null) && !x.IsPvP);
            foreach(var item in sheet) {
                var name = item.Name.ToString();
                var attackType = item.ActionCategory.Value.Name.ToString();
                var actionId = item.ActionCategory.Value.RowId;
                if (actionId == 2 || actionId == 3) { // spell or weaponskill
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

            var selfId = PluginInterface.ClientState.LocalPlayer.ActorId;
            var isSelf = sourceId == selfId;
            var isPet = !isSelf && ((GManager?.CurrentJob == JobIds.SMN || GManager?.CurrentJob == JobIds.SCH) ? IsPet(sourceId, selfId) : false);
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

                if (entries[i].type == ActionEffectType.Gp_Or_Status || entries[i].type == ActionEffectType.ApplyStatusEffectTarget) {
                    // idk what's up with Gp_Or_Status. sometimes the enum is wrong?

                    var buffItem = new Item
                    {
                        Id = entries[i].value,
                        Type = ItemType.Buff
                    };

                    if(!isParty) { // don't let party members affect our gauge
                        GManager?.PerformAction(buffItem);
                    }
                    if((int) tTarget == selfId) { // only really care about buffs on us
                        BManager?.PerformAction(buffItem);
                    }
                }
            }
            receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
        }

        private void Animate() {
            Animation.Tick();
        }

        private void ActorControlSelf(uint entityId, uint id, uint arg0, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, UInt64 targetId, byte a10) {
            actorControlSelfHook.Original(entityId, id, arg0, arg1, arg2, arg3, arg4, arg5, targetId, a10);
            if (arg1 == 0x40000010) { // WIPE
                Reset();
            }
        }

        private void ZoneChanged(object sender, ushort e) {
            Reset();
        }

        private bool IsPet(int actorId, int ownerId) {
            foreach (Actor actor in PluginInterface.ClientState.Actors) {
                if(actor?.ActorId == actorId) {
                    if(actor is BattleNpc npc) {
                        if (npc.Address == IntPtr.Zero) return false;
                        return npc.OwnerId == ownerId;
                    }
                    return false;
                }
            }
            return false;
        }

        private bool IsInParty(int actorId) {
            foreach(var pMember in Party) {
                if (pMember.Actor == null) continue;
                if(pMember.Actor.ActorId == actorId) {
                    return true;
                }
            }
            return false;
        }

        private void FrameworkOnUpdate(Framework framework) {
            if (!Ready) {
                if(Init && !UI.IsInitialized()) { // a logout, need to recreate everything once we're done
                    PluginLog.Log("LOGOUT");
                    Animation.Cleanup();
                    Init = false;
                    CurrentJob = JobIds.OTHER;
                }
                return;
            }
            var addon = UI._ADDON;
            if (!Init) {
                if (addon == null) return;
                PluginLog.Log("INIT");
                UI.Setup();
                GManager = new GaugeManager(PluginInterface, UI);
                BManager = new BuffManager(UI);
                SetupSettings();
                UI.HideAllBuffs();
                UI.HideAllGauges();
                Init = true;
                return;
            }

            SetJob(PluginInterface.ClientState.LocalPlayer.ClassJob);
            GManager.Tick();
            BManager.Tick();

            if (Party.Count < LastPartyCount) {
                BManager.SetJob(CurrentJob);
            }
            LastPartyCount = Party.Count;

            var currentPosition = UiHelper.GetNodePosition(addon->RootNode);
            var currentScale = UiHelper.GetNodeScale(addon->RootNode);
            if(currentPosition != LastPosition || currentScale != LastScale) {
                GManager.SetPositionScale();
                BManager.SetPositionScale();
            }
            LastPosition = currentPosition;
            LastScale = currentScale;
        }

        private void SetJob(ClassJob jobId) {
            JobIds job = jobId.Id < 19 ? JobIds.OTHER : (JobIds)jobId.Id;
            if (job != CurrentJob) {
                CurrentJob = job;
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

        public Item(ActionIds action) {
            Id = (uint)action;
            Type = ItemType.Action;
        }

        public Item(BuffIds buff) {
            Id = (uint)buff;
            Type = ItemType.Buff;
        }

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
