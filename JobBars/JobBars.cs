using System;
using System.Numerics;
using System.Reflection;
using System.Threading;

using JobBars.Helper;
using JobBars.UI;
using JobBars.Data;
using JobBars.Buffs.Manager;
using JobBars.Cooldowns.Manager;
using JobBars.Cursors.Manager;
using JobBars.Icons.Manager;
using JobBars.Gauges.Manager;

using FFXIVClientStructs.FFXIV.Component.GUI;

using Dalamud.Plugin;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Game;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Data;

namespace JobBars {
    public unsafe partial class JobBars : IDalamudPlugin {
        public static DalamudPluginInterface PluginInterface { get; private set; }
        public static ClientState ClientState { get; private set; }
        public static JobGauges JobGauges { get; private set; }
        public static Framework Framework { get; private set; }
        public static Condition Condition { get; private set; }
        public static CommandManager CommandManager { get; private set; }
        public static ObjectTable Objects { get; private set; }
        public static SigScanner SigScanner { get; private set; }
        public static DataManager DataManager { get; private set; }

        public static Configuration Config { get; private set; }
        public static UIBuilder Builder { get; private set; }
        public static UIIconManager IconBuilder { get; private set; }

        public static GaugeManager GaugeManager { get; private set; }
        public static BuffManager BuffManager { get; private set; }
        public static CooldownManager CooldownManager { get; private set; }
        public static CursorManager CursorManager { get; private set; }
        public static IconManager IconManager { get; private set; }

        public string Name => "JobBars";
        public string AssemblyLocation { get; private set; } = Assembly.GetExecutingAssembly().Location;

        public static JobIds CurrentJob { get; private set; } = JobIds.OTHER;

        private delegate void ReceiveActionEffectDelegate(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private Hook<ReceiveActionEffectDelegate> ReceiveActionEffectHook;

        private delegate void ActorControlSelfDelegate(uint entityId, uint id, uint arg0, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, UInt64 targetId, byte a10);
        private Hook<ActorControlSelfDelegate> ActorControlSelfHook;

        private delegate IntPtr IconDimmedDelegate(IntPtr iconUnk, byte dimmed);
        private Hook<IconDimmedDelegate> IconDimmedHook;

        private static bool PlayerExists => ClientState?.LocalPlayer != null;
        private static bool RecreateUI => Condition[ConditionFlag.CreatingCharacter]; // getting haircut, etc.
        private bool LoggedOut = true;

        private Vector2 LastPosition;
        private Vector2 LastScale;

        public static AttachAddon AttachAddon { get; private set; } = AttachAddon.Chatbox;
        public static AttachAddon CooldownAttachAddon { get; private set; } = AttachAddon.PartyList;

        private bool IsLoaded = false;
        private DateTime StartTime = DateTime.Now;

        public JobBars(
                DalamudPluginInterface pluginInterface,
                ClientState clientState,
                CommandManager commandManager,
                Condition condition,
                Framework framework,
                ObjectTable objects,
                SigScanner sigScanner,
                DataManager dataManager,
                JobGauges jobGauges
            ) {
            PluginInterface = pluginInterface;
            ClientState = clientState;
            Framework = framework;
            Condition = condition;
            CommandManager = commandManager;
            Objects = objects;
            SigScanner = sigScanner;
            DataManager = dataManager;
            JobGauges = jobGauges;

            if (!FFXIVClientStructs.Resolver.Initialized) FFXIVClientStructs.Resolver.Initialize();

            UIHelper.Setup();
            UIColor.SetupColors();

            // Upgrade if config is too old
            try {
                Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            }
            catch (Exception e) {
                PluginLog.LogError("Error loading config", e);
                Config = new Configuration();
                Config.Save();
            }
            if (Config.Version < 1) {
                PluginLog.Log("Old config version found");
                Config = new Configuration();
                Config.Save();
            }

            AttachAddon = Config.AttachAddon;
            CooldownAttachAddon = Config.CooldownAttachAddon;
            IconBuilder = new UIIconManager();

            // ==========================

            InitializeUI();

            IntPtr receiveActionEffectFuncPtr = SigScanner.ScanText("4C 89 44 24 ?? 55 56 57 41 54 41 55 41 56 48 8D 6C 24");
            ReceiveActionEffectHook = new Hook<ReceiveActionEffectDelegate>(receiveActionEffectFuncPtr, ReceiveActionEffect);
            ReceiveActionEffectHook.Enable();

            IntPtr actorControlSelfPtr = SigScanner.ScanText("E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64");
            ActorControlSelfHook = new Hook<ActorControlSelfDelegate>(actorControlSelfPtr, ActorControlSelf);
            ActorControlSelfHook.Enable();

            IntPtr iconDimmedPtr = SigScanner.ScanText("E8 ?? ?? ?? ?? 49 8D 4D 10 FF C6");
            IconDimmedHook = new Hook<IconDimmedDelegate>(iconDimmedPtr, IconDimmedDetour);
            IconDimmedHook.Enable();

            PluginInterface.UiBuilder.Draw += BuildSettingsUI;
            PluginInterface.UiBuilder.Draw += Animate;
            PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfig;
            Framework.Update += FrameworkOnUpdate;
            ClientState.TerritoryChanged += ZoneChanged;
            SetupCommands();
        }

        private void InitializeUI() {
            // these are created before the addons are even visible, so they aren't attached yet
            PluginLog.Log("==== INIT ====");
            IconBuilder.Reset();

            Builder = new UIBuilder();
            BuffManager = new BuffManager();
            CooldownManager = new CooldownManager();
            GaugeManager = new GaugeManager();
            CursorManager = new CursorManager();
            IconManager = new IconManager();

            IsLoaded = true;
        }

        public void Dispose() {
            ReceiveActionEffectHook?.Disable();
            ActorControlSelfHook?.Disable();
            IconDimmedHook?.Disable();

            Thread.Sleep(500);

            ReceiveActionEffectHook?.Dispose();
            ActorControlSelfHook?.Dispose();
            IconDimmedHook?.Dispose();

            ReceiveActionEffectHook = null;
            ActorControlSelfHook = null;
            IconDimmedHook = null;

            PluginInterface.UiBuilder.Draw -= BuildSettingsUI;
            PluginInterface.UiBuilder.Draw -= Animate;
            PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfig;
            Framework.Update -= FrameworkOnUpdate;
            ClientState.TerritoryChanged -= ZoneChanged;

            GaugeManager = null;
            BuffManager = null;
            CooldownManager = null;
            CursorManager = null;
            IconManager = null;

            Animation.Dispose();
            IconBuilder?.Dispose();
            Builder?.Dispose();
            IconBuilder = null;
            Builder = null;

            PluginInterface = null;
            Config = null;

            RemoveCommands();
        }

        private void Animate() {
            if (!IsLoaded) return;
            Animation.Tick();
        }

        private void FrameworkOnUpdate(Framework framework) {
            if (!IsLoaded) return;

            var addon = UIHelper.BuffGaugeAttachAddon;

            if (!LoggedOut && RecreateUI) {
                Logout();
                return;
            }

            if (!PlayerExists) {
                if (!LoggedOut && (addon == null)) Logout();
                return;
            }

            if (addon == null || addon->RootNode == null || RecreateUI) return;

            if (LoggedOut) {
                PluginLog.Log("====== REATTACH =======");
                Builder.Attach(); // re-attach after addons have been created
                LoggedOut = false;
                return;
            }

            UIHelper.TickTextures();
            CheckForJobChange();
            Tick();
            CheckForHUDChange(addon);
        }

        private void Logout() {
            PluginLog.Log("==== LOGOUT ====");
            IconBuilder.Reset();
            Animation.Dispose();

            LoggedOut = true;
            CurrentJob = JobIds.OTHER;
        }

        private void CheckForJobChange() {
            var jobId = ClientState.LocalPlayer.ClassJob;
            JobIds job = UIHelper.IdToJob(jobId.Id);

            if (job != CurrentJob) {
                CurrentJob = job;
                PluginLog.Log($"SWITCHED JOB TO {CurrentJob}");
                GaugeManager.SetJob(CurrentJob);
                CursorManager.SetJob(CurrentJob);
                IconManager.SetJob(CurrentJob);
            }
        }

        private void Tick() {
            if (!IsLoaded) return;

            UIHelper.UpdateMp(ClientState.LocalPlayer.CurrentMp);
            UIHelper.UpdatePlayerStatus();

            UpdatePartyMembers();
            GaugeManager.Tick();
            BuffManager.Tick();
            CooldownManager.Tick();
            CursorManager.Tick();
            IconManager.Tick();

            var time = DateTime.Now;
            var millis = time.Second * 1000 + time.Millisecond;
            var percent = (float)(millis % 1000) / 1000;

            Builder.Tick( Config.GaugePulse ? percent : 0f );
        }

        private void CheckForHUDChange(AtkUnitBase* addon) {
            var currentPosition = UIHelper.GetNodePosition(addon->RootNode);
            var currentScale = UIHelper.GetNodeScale(addon->RootNode);

            if (currentPosition != LastPosition || currentScale != LastScale) {
                GaugeManager.UpdatePositionScale();
                BuffManager.UpdatePositionScale();
                CooldownManager.UpdatePositionScale();
            }
            LastPosition = currentPosition;
            LastScale = currentScale;
        }

        // ======= COMMANDS ============

        private void SetupCommands() {
            CommandManager.AddHandler("/jobbars", new CommandInfo(OnCommand) {
                HelpMessage = $"Open config window for {Name}",
                ShowInHelp = true
            });
        }

        private void OnOpenConfig() {
            if (!IsLoaded) return;
            Visible = true;
        }

        public void OnCommand(object command, object args) {
            Visible = !Visible;
        }

        public void RemoveCommands() {
            CommandManager.RemoveHandler("/jobbars");
        }
    }
}
