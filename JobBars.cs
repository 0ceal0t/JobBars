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
using FFXIVClientInterface;
using JobBars.GameStructs;
using Dalamud.Hooking;
using JobBars.Data;
using Dalamud.Game.ClientState.Actors.Types;
using Dalamud.Game.ClientState.Actors.Types.NonPlayer;
using JobBars.Gauges;

#pragma warning disable CS0659
namespace JobBars {
    public unsafe partial class JobBars : IDalamudPlugin {
        public string Name => "JobBars";
        public DalamudPluginInterface PluginInterface { get; private set; }
        public string AssemblyLocation { get; private set; } = Assembly.GetExecutingAssembly().Location;

        public UIBuilder _UI;
        public ActionGaugeManager _GManager;
        public Configuration _Config;

        private delegate void ReceiveActionEffectDelegate(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private Hook<ReceiveActionEffectDelegate> receiveActionEffectHook;
        private delegate byte AddStatusDelegate(IntPtr statusEffectList, uint slot, UInt16 statusId, float duration, UInt16 a5, uint sourceActorId, byte newStatus);
        private Hook<AddStatusDelegate> addStatusHook;
        private delegate IntPtr RemoveStatusDelegate(IntPtr a1, uint statusId, IntPtr a3, uint sourceActorId, byte a5, byte a6);
        private Hook<RemoveStatusDelegate> removeStatusHook;

        private bool _Ready => (PluginInterface.ClientState != null && PluginInterface.ClientState.LocalPlayer != null);

        public void Initialize(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;
            UiHelper.Setup(pluginInterface.TargetModuleScanner);
            _Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            _Config.Initialize(PluginInterface);
            _UI = new UIBuilder(pluginInterface);

            IntPtr receiveActionEffectFuncPtr = PluginInterface.TargetModuleScanner.ScanText("4C 89 44 24 18 53 56 57 41 54 41 57 48 81 EC ?? 00 00 00 8B F9");
            receiveActionEffectHook = new Hook<ReceiveActionEffectDelegate>(receiveActionEffectFuncPtr, (ReceiveActionEffectDelegate)ReceiveActionEffect);
            receiveActionEffectHook.Enable();
            IntPtr addStatusFuncPtr = PluginInterface.TargetModuleScanner.ScanText("40 53 55 56 48 83 EC 60 40 32 ED 41 0F B7 F0 48 8B D9");
            addStatusHook = new Hook<AddStatusDelegate>(addStatusFuncPtr, (AddStatusDelegate)AddStatus);
            addStatusHook.Enable();
            IntPtr removeStatusFuncPtr = PluginInterface.TargetModuleScanner.ScanText("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 30 80 7C 24 ?? ?? 41 8B D9 8B FA");
            removeStatusHook = new Hook<RemoveStatusDelegate>(removeStatusFuncPtr, (RemoveStatusDelegate)RemoveStatus);
            removeStatusHook.Enable();

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

            removeStatusHook?.Disable();
            removeStatusHook?.Dispose();

            PluginInterface.UiBuilder.OnBuildUi -= BuildUI;
            PluginInterface.Framework.OnUpdateEvent -= FrameworkOnUpdate;
            _UI.Dispose();

            RemoveCommands();
        }

        private byte AddStatus(IntPtr statusEffectList, uint slot, UInt16 statusId, float duration, UInt16 a5, uint sourceActorId, byte newStatus) { // STATUS DURATION SENT SEPARATELY
            if (_Ready && sourceActorId == PluginInterface.ClientState.LocalPlayer.ActorId && newStatus == 1) {
                _GManager?.GetBuffDuration(new Item
                {
                    Id = statusId,
                    IsBuff = true
                }, duration);
            }
            return addStatusHook.Original(statusEffectList, slot, statusId, duration, a5, sourceActorId, newStatus);
        }

        private IntPtr RemoveStatus(IntPtr a1, uint statusId, IntPtr a3, uint sourceActorId, byte a5, byte a6) { // NOT USING THIS FOR NOW
            return removeStatusHook.Original(a1, statusId, a3, sourceActorId, a5, a6);
        }

        private void ReceiveActionEffect(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail) {
            if (!_Ready) {
                receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
                return;
            }

            uint id = *((uint*)effectHeader.ToPointer() + 0x2);
            ushort op = *((ushort*)effectHeader.ToPointer() - 0x7);

            var isSelf = sourceId == PluginInterface.ClientState.LocalPlayer.ActorId;
            var isPet = (_GManager?.CurrentJob == JobIds.SMN || _GManager?.CurrentJob == JobIds.SCH) ? sourceId == FindCharaPet() : false;

            if(!isSelf || isPet) {
                receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
                return;
            }

            _GManager?.PerformAction(new Item
            {
                Id = id,
                IsBuff = false
            });

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
                if(entries[i].type == ActionEffectType.Gp_Or_Status) {
                    _GManager?.PerformAction(new Item
                    {
                        Id = entries[i].value,
                        IsBuff = true
                    });
                }
            }

            receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
        }

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

        private int FrameCount = 0;
        private void FrameworkOnUpdate(Framework framework) {
            if (!_Ready) return;
            if (FrameCount == 0) { // idk lmao
                _UI.SetupTex();
                FrameCount++;
                return;
            }
            else if (FrameCount == 1) {
                _UI.Init();
                _GManager = new ActionGaugeManager(_UI);
                FrameCount++;
                return;
            }

            _GManager.SetJob(PluginInterface.ClientState.LocalPlayer.ClassJob);
            _GManager.Tick();
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
