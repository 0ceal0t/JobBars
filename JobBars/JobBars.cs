using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Plugin;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Game;
using JobBars.Helper;
using JobBars.UI;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Buffs;
using JobBars.Cooldowns;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Threading;
using System.Runtime.InteropServices;

namespace JobBars {
    public unsafe partial class JobBars : IDalamudPlugin {
        public string Name => "JobBars";
        public DalamudPluginInterface PluginInterface { get; private set; }
        public string AssemblyLocation { get; private set; } = Assembly.GetExecutingAssembly().Location;

        private GaugeManager GManager;
        private BuffManager BManager;
        private CooldownManager CDManager;
        private Configuration Config;

        private JobIds CurrentJob = JobIds.OTHER;

        private delegate void ReceiveActionEffectDelegate(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private Hook<ReceiveActionEffectDelegate> receiveActionEffectHook;

        private delegate void ActorControlSelfDelegate(uint entityId, uint id, uint arg0, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, UInt64 targetId, byte a10);
        private Hook<ActorControlSelfDelegate> actorControlSelfHook;

        private int LastPartyCount = 0;

        private bool PlayerExists => PluginInterface.ClientState?.LocalPlayer != null;
        private bool Initialized = false;
        private bool LoggedOut = false;

        private Vector2 LastPosition;
        private Vector2 LastScale;

        /*
         * SIG LIST:
         *      
         *  Helper/UiHelper.GameFunctions.cs
         *      _playSe (E8 ?? ?? ?? ?? 4D 39 BE ?? ?? ?? ??)
         *      
         *  JobBars.cs
         *      receiveActionEffectFuncPtr (4C 89 44 24 18 53 56 57 41 54 41 57 48 81 EC ?? 00 00 00 8B F9)
         *      actorControlSelfPtr (E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64)
         *      
         *  GaugeManager (TargetManager from Dalamud)
         *      48 8B 05 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? FF 50 ?? 48 85 DB
         */

        public void Initialize(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;

            if (!FFXIVClientStructs.Resolver.Initialized) FFXIVClientStructs.Resolver.Initialize();

            UIIconManager.Initialize(pluginInterface);
            DataManager.Initialize(pluginInterface);

            UIHelper.Setup(pluginInterface);
            UIColor.SetupColors();

            Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Config.Initialize(PluginInterface);

            IntPtr receiveActionEffectFuncPtr = PluginInterface.TargetModuleScanner.ScanText("4C 89 44 24 18 53 56 57 41 54 41 57 48 81 EC ?? 00 00 00 8B F9");
            receiveActionEffectHook = new Hook<ReceiveActionEffectDelegate>(receiveActionEffectFuncPtr, ReceiveActionEffect);
            receiveActionEffectHook.Enable();

            IntPtr actorControlSelfPtr = pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64");
            actorControlSelfHook = new Hook<ActorControlSelfDelegate>(actorControlSelfPtr, ActorControlSelf);
            actorControlSelfHook.Enable();

            PluginInterface.UiBuilder.Draw += BuildSettingsUI;
            PluginInterface.UiBuilder.Draw += Animate;
            PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfig;
            PluginInterface.Framework.OnUpdateEvent += FrameworkOnUpdate;
            PluginInterface.ClientState.TerritoryChanged += ZoneChanged;
            SetupCommands();
        }

        public void Dispose() {
            receiveActionEffectHook?.Disable();
            actorControlSelfHook?.Disable();

            Thread.Sleep(500);

            receiveActionEffectHook?.Dispose();
            actorControlSelfHook?.Dispose();
            receiveActionEffectHook = null;
            actorControlSelfHook = null;

            PluginInterface.UiBuilder.Draw -= BuildSettingsUI;
            PluginInterface.UiBuilder.Draw -= Animate;
            PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfig;
            PluginInterface.Framework.OnUpdateEvent -= FrameworkOnUpdate;
            PluginInterface.ClientState.TerritoryChanged -= ZoneChanged;

            GaugeManager.Dispose();
            BuffManager.Dispose();
            CooldownManager.Dispose();
            GManager = null;
            BManager = null;
            CDManager = null;

            Animation.Dispose();
            UIIconManager.Dispose();
            UIBuilder.Dispose();
            UIHelper.Dispose();
            DataManager.Dispose();

            RemoveCommands();
        }

        private void Animate() {
            Animation.Tick();
        }

        private void FrameworkOnUpdate(Framework framework) {
            var addon = UIHelper.ParameterAddon;

            if (!PlayerExists) {
                if (Initialized && !LoggedOut && addon == null) Logout();
                return;
            }

            if (addon == null || addon->RootNode == null) return;

            if (!Initialized) {
                InitializeUI();
                return;
            }

            if(LoggedOut) {
                UIBuilder.Builder.Attach();
                LoggedOut = false;
                return;
            }

            CheckForJobChange();
            Tick();
            CheckForPartyChange();
            CheckForHUDChange(addon);
        }

        private void Logout() {
            PluginLog.Log("==== LOGOUT ===");
            UIIconManager.Manager.Reset();
            Animation.Dispose();

            LoggedOut = true;
            CurrentJob = JobIds.OTHER;
        }

        private void InitializeUI() {
            PluginLog.Log("==== INIT ====");
            UIIconManager.Manager.Reset();

            BManager = new BuffManager();
            CDManager = new CooldownManager(PluginInterface);
            UIBuilder.Initialize(PluginInterface);
            GManager = new GaugeManager(PluginInterface);
            BManager.GetIconsUI();

            if (!Configuration.Config.GaugesEnabled) UIBuilder.Builder.HideGauges();
            if (!Configuration.Config.BuffBarEnabled) UIBuilder.Builder.HideBuffs();
            UIBuilder.Builder.HideAllBuffs();
            UIBuilder.Builder.HideAllGauges();

            Initialized = true;
        }

        private void CheckForPartyChange() {
            // someone left the party. this means that some buffs might not make sense anymore, so reset it
            var partyCount = DataManager.GetPartyCount();
            if (partyCount < LastPartyCount) BManager.Reset();
            LastPartyCount = partyCount;
        }

        private void CheckForJobChange() {
            var jobId = PluginInterface.ClientState.LocalPlayer.ClassJob;
            JobIds job = DataManager.IdToJob(jobId.Id);

            if (job != CurrentJob) {
                CurrentJob = job;
                PluginLog.Log($"SWITCHED JOB TO {CurrentJob}");
                BManager.Reset();
                GManager.SetJob(CurrentJob);
            }
        }

        private void Tick() {
            var inCombat = PluginInterface.ClientState.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat];
            GManager.Tick(inCombat);
            BManager.Tick(inCombat);
            CDManager.Tick(inCombat);
        }

        private void CheckForHUDChange(AtkUnitBase* addon) {
            var currentPosition = UIHelper.GetNodePosition(addon->RootNode); // changing HP/MP in hud layout
            var currentScale = UIHelper.GetNodeScale(addon->RootNode);

            if (currentPosition != LastPosition || currentScale != LastScale) {
                GManager.UpdatePositionScale();
                BManager.UpdatePositionScale();
            }
            LastPosition = currentPosition;
            LastScale = currentScale;
        }

        // ======= COMMANDS ============

        private void SetupCommands() {
            PluginInterface.CommandManager.AddHandler("/jobbars", new Dalamud.Game.Command.CommandInfo(OnCommand) {
                HelpMessage = $"Open config window for {Name}",
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
}
