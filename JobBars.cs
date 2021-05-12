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

        private bool _Ready => (PluginInterface.ClientState != null && PluginInterface.ClientState.LocalPlayer != null);

        public void Initialize(DalamudPluginInterface pluginInterface) {
            PluginLog.Log("=== INIT 1 ====");
            PluginInterface = pluginInterface;
            UiHelper.Setup(pluginInterface.TargetModuleScanner);
            _Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            _Config.Initialize(PluginInterface);
            UI = new UIBuilder(pluginInterface);
            PluginLog.Log("=== INIT 2 ====");
            IntPtr receiveActionEffectFuncPtr = PluginInterface.TargetModuleScanner.ScanText("4C 89 44 24 18 53 56 57 41 54 41 57 48 81 EC ?? 00 00 00 8B F9");
            receiveActionEffectHook = new Hook<ReceiveActionEffectDelegate>(receiveActionEffectFuncPtr, (ReceiveActionEffectDelegate)ReceiveActionEffect);
            receiveActionEffectHook.Enable();
            IntPtr addStatusFuncPtr = PluginInterface.TargetModuleScanner.ScanText("40 53 55 56 48 83 EC 60 40 32 ED 41 0F B7 F0 48 8B D9");
            addStatusHook = new Hook<AddStatusDelegate>(addStatusFuncPtr, (AddStatusDelegate)AddStatus);
            addStatusHook.Enable();

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

            PluginInterface.UiBuilder.OnBuildUi -= BuildUI;
            PluginInterface.Framework.OnUpdateEvent -= FrameworkOnUpdate;
            UI.Dispose();
            RemoveCommands();
        }

        // ========= HOOKS ===========
        private byte AddStatus(IntPtr statusEffectList, uint slot, UInt16 statusId, float duration, UInt16 a5, uint sourceActorId, byte newStatus) { // STATUS DURATION SENT SEPARATELY
            if (_Ready && STEP == STEPS.READY && sourceActorId == PluginInterface.ClientState.LocalPlayer.ActorId) {
                GManager?.SetBuffDuration(new Item
                {
                    Id = statusId,
                    IsBuff = true
                }, duration, newStatus == 0);
            }
            return addStatusHook.Original(statusEffectList, slot, statusId, duration, a5, sourceActorId, newStatus);
        }

        private void ReceiveActionEffect(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail) {
            if (!_Ready || STEP != STEPS.READY) {
                receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
                return;
            }

            uint id = *((uint*)effectHeader.ToPointer() + 0x2);
            ushort op = *((ushort*)effectHeader.ToPointer() - 0x7);

            var isSelf = sourceId == PluginInterface.ClientState.LocalPlayer.ActorId;
            var isPet = (GManager?.CurrentJob == JobIds.SMN || GManager?.CurrentJob == JobIds.SCH) ? sourceId == FindCharaPet() : false;
            if(!isSelf || isPet) {
                receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
                return;
            }

            var item = new Item
            {
                Id = id,
                IsBuff = false
            };
            GManager?.PerformAction(item);
            BManager?.PerformAction(item);

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
                    GManager?.PerformAction(new Item
                    {
                        Id = entries[i].value,
                        IsBuff = true
                    });
                }
            }
            receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
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
        enum STEPS {
            INIT_TEXTURES,
            DONE_INIT_TEXTURES,
            INIT_GAUGES,
            DONE_INIT_GAUGES,
            READY
        };
        STEPS STEP = STEPS.INIT_TEXTURES;
        DateTime STEP_TIME;

        private void FrameworkOnUpdate(Framework framework) {
            if (!_Ready) return;
            if (STEP == STEPS.INIT_TEXTURES) {
                PluginLog.Log("===== INIT TEXTURES =====");
                UI.SetupTex();
                STEP = STEPS.DONE_INIT_TEXTURES;
                STEP_TIME = DateTime.Now;
                return;
            }
            else if (STEP == STEPS.INIT_GAUGES) {
                PluginLog.Log("===== INIT GAUGES ======");
                UI.Init();
                GManager = new GaugeManager(UI);
                BManager = new BuffManager(UI);
                STEP = STEPS.DONE_INIT_GAUGES;
                STEP_TIME = DateTime.Now;
                return;
            }
            else if (STEP == STEPS.DONE_INIT_TEXTURES) {
                if ((DateTime.Now - STEP_TIME).TotalSeconds > 0.5) { STEP = STEPS.INIT_GAUGES; }
                return;
            }
            else if (STEP == STEPS.DONE_INIT_GAUGES) {
                if ((DateTime.Now - STEP_TIME).TotalSeconds > 0.5) {
                    PluginLog.Log("====== READY =======");
                    STEP = STEPS.READY;
                }
                return;
            }

            SetJob(PluginInterface.ClientState.LocalPlayer.ClassJob);
            GManager.Tick();
            BManager.Tick();
        }

        JobIds CurrentJob = JobIds.OTHER;
        public void SetJob(ClassJob job) {
            JobIds _job = job.Id < 19 ? JobIds.OTHER : (JobIds)job.Id;
            if (_job != CurrentJob) {
                CurrentJob = _job;
                GManager.SetJob(_job);
                BManager.SetJob(_job);
            }
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
