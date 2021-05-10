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
using FFXIVClientInterface.Client.UI.Misc;
using Dalamud.Hooking;
using JobBars.Gauges;
using Dalamud.Game.Internal.Network;
using JobBars.Data;
using Dalamud.Interface;

#pragma warning disable CS0659
namespace JobBars {
    public unsafe class JobBars : IDalamudPlugin {
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
            uint id = *((uint*)effectHeader.ToPointer() + 0x2);
            ushort op = *((ushort*)effectHeader.ToPointer() - 0x7);
            if (_Ready && IntPtr.Equals(sourceCharacter, PluginInterface.ClientState.LocalPlayer.Address)) {
                _GManager?.PerformAction(new Item
                {
                    Id = id,
                    IsBuff = false
                });
            }

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
                //if (entries[i].type != ActionEffectType.Nothing && entries[i].type != ActionEffectType.Damage) { PluginLog.Log($"{entries[i].type} {tTarget} {entries[i].value} {entries[i].mult} {entries[i].param0} {entries[i].param1} {entries[i].param2}"); }
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

        public void SetupCommands() {
            PluginInterface.CommandManager.AddHandler("/jobbars", new Dalamud.Game.Command.CommandInfo(OnConfigCommandHandler) {
                HelpMessage = $"Open config window for {this.Name}",
                ShowInHelp = true
            });
        }
        public void OnConfigCommandHandler(object command, object args) {
        }
        public void RemoveCommands() {
            PluginInterface.CommandManager.RemoveHandler("/jobbars");
        }

        private bool Visible = true;
        private int FrameCount = 0;
        private bool GAUGE_LOCK = true;

        private void BuildUI() {
            // ====== SETTINGS =======
            string _ID = "##JobBars_Settings";
            ImGuiHelpers.ForceNextWindowMainViewport();
            ImGui.SetNextWindowSize(new Vector2(500, 800), ImGuiCond.FirstUseEver);
            if(ImGui.Begin("Settings", ref Visible)) {
                ImGui.BeginTabBar("Tabs" + _ID);
                if(ImGui.BeginTabItem("Gauges" + _ID)) {
                    DrawGaugeSettings();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Buffs" + _ID)) {
                    DrawBuffSettings();
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
                ImGui.End();
            }
            // ====== GAUGE POSITION =======
            if (!GAUGE_LOCK) {
                ImGuiHelpers.ForceNextWindowMainViewport();
                ImGui.SetNextWindowPos(Configuration.Config.GaugePosition, ImGuiCond.FirstUseEver);
                ImGui.SetNextWindowSize(new Vector2(200, 200));
                ImGui.Begin("##GaugePosition", ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize);
                ImGui.Text("Gauge Bar Position");

                var pos = ImGui.GetWindowPos();
                if(pos != Configuration.Config.GaugePosition) {
                    Configuration.Config.GaugePosition = pos;
                    Configuration.Config.Save();
                    _UI?.SetGaugePosition(pos);
                }

                ImGui.End();
            }
        }
        JobIds G_SelectedJob = JobIds.OTHER;
        private void DrawGaugeSettings() {
            if (_GManager == null) return;

            string _ID = "##JobBars_Gauges";
            if (ImGui.Checkbox("Locked" + _ID, ref GAUGE_LOCK)) {
            }
            if (ImGui.InputFloat("Scale" + _ID, ref Configuration.Config.GaugeScale)) {
                _UI?.SetGaugeScale(Configuration.Config.GaugeScale);
                Configuration.Config.Save();
            }

            var size = ImGui.GetContentRegionAvail() - new Vector2(0, ImGui.GetTextLineHeight() + 10);
            ImGui.BeginChild(_ID + "/Child", size, true);
            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, 150);

            foreach(var job in _GManager.JobToGauges.Keys) {
                if (job == JobIds.OTHER) continue;
                if(ImGui.Selectable(job + _ID + "/Job", G_SelectedJob == job)) {
                    G_SelectedJob = job;
                }
            }

            var spaceLeft = ImGui.GetContentRegionAvail().Y;
            if(spaceLeft > 0) {
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + spaceLeft);
            }

            ImGui.NextColumn();

            if(G_SelectedJob == JobIds.OTHER) {
                ImGui.Text("Select a job...");
            }
            else {
                ImGui.BeginChild(_ID + "Selected");
                foreach(var g_ in _GManager.JobToGauges[G_SelectedJob]) {
                    ImGui.TextColored(new Vector4(0, 1, 0, 1), g_.Name);
                }
                ImGui.EndChild();
            }

            ImGui.Columns(1);
            ImGui.EndChild();
            if(ImGui.SmallButton("SAVE" + _ID)) {

            }
        }
        private void DrawBuffSettings() {

        }

        private void FrameworkOnUpdate(Framework framework) {
            if (!_Ready) return;
            if(FrameCount == 0) { // idk lmao
                _UI.SetupTex();
                FrameCount++;
                return;
            }
            else if(FrameCount == 1) {
                _UI.Init();
                _GManager = new ActionGaugeManager(_UI);
                FrameCount++;
                return;
            }

            _GManager.SetJob(PluginInterface.ClientState.LocalPlayer.ClassJob);
            _GManager.Tick();
        }
    }
}
