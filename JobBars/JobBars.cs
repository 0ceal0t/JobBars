using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Hooking;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using JobBars.Atk;
using JobBars.Buffs.Manager;
using JobBars.Cooldowns.Manager;
using JobBars.Cursors.Manager;
using JobBars.Data;
using JobBars.Gauges.Manager;
using JobBars.Helper;
using JobBars.Icons.Manager;
using System;
using System.Reflection;

namespace JobBars {
    public unsafe partial class JobBars : IDalamudPlugin {
        public static Configuration Configuration { get; private set; }
        public static AtkBuilder Builder { get; private set; }
        public static AtkIconBuilder IconBuilder { get; private set; }

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

        private static bool PlayerExists => Dalamud.ClientState?.LocalPlayer != null;
        private static bool RecreateUi => Dalamud.Condition[ConditionFlag.CreatingCharacter]; // getting haircut, etc.
        private bool LoggedOut = true;

        public static AttachAddon AttachAddon { get; private set; } = AttachAddon.Chatbox;
        public static AttachAddon CooldownAttachAddon { get; private set; } = AttachAddon.PartyList;

        private bool IsLoaded = false;

        public JobBars(DalamudPluginInterface pluginInterface) {
            pluginInterface.Create<Dalamud>();

            AtkHelper.Setup();
            AtkColor.SetupColors();

            // Upgrade if config is too old
            try {
                Configuration = Dalamud.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            }
            catch (Exception e) {
                Dalamud.Error(e, "Error loading config");
                Configuration = new Configuration();
                Configuration.Save();
            }
            if (Configuration.Version < 1) {
                Dalamud.Log("Old config version found");
                Configuration = new Configuration();
                Configuration.Save();
            }

            AttachAddon = Configuration.AttachAddon;
            CooldownAttachAddon = Configuration.CooldownAttachAddon;
            IconBuilder = new AtkIconBuilder();

            // ==========================

            InitializeUI();

            ReceiveActionEffectHook = Dalamud.Hooks.HookFromSignature<ReceiveActionEffectDelegate>(Constants.ReceiveActionEffectSig, ReceiveActionEffect);
            ReceiveActionEffectHook.Enable();

            ActorControlSelfHook = Dalamud.Hooks.HookFromSignature<ActorControlSelfDelegate>(Constants.ActorControlSig, ActorControlSelf);
            ActorControlSelfHook.Enable();

            IconDimmedHook = Dalamud.Hooks.HookFromSignature<IconDimmedDelegate>(Constants.IconDimmedSig, IconDimmedDetour);
            IconDimmedHook.Enable();

            Dalamud.PluginInterface.UiBuilder.Draw += BuildSettingsUi;
            Dalamud.PluginInterface.UiBuilder.Draw += Animate;
            Dalamud.PluginInterface.UiBuilder.OpenMainUi += OpenConfig;
            Dalamud.PluginInterface.UiBuilder.OpenConfigUi += OpenConfig;
            Dalamud.Framework.Update += FrameworkOnUpdate;
            Dalamud.ClientState.TerritoryChanged += ZoneChanged;
            SetupCommands();
        }

        private void InitializeUI() {
            // these are created before the addons are even visible, so they aren't attached yet
            Dalamud.Log("==== INIT ====");
            IconBuilder.Reset();

            Builder = new AtkBuilder();
            BuffManager = new BuffManager();
            CooldownManager = new CooldownManager();
            GaugeManager = new GaugeManager();
            CursorManager = new CursorManager();
            IconManager = new IconManager();

            IsLoaded = true;
        }

        public void Dispose() {
            ReceiveActionEffectHook?.Dispose();
            ActorControlSelfHook?.Dispose();
            IconDimmedHook?.Dispose();

            ReceiveActionEffectHook = null;
            ActorControlSelfHook = null;
            IconDimmedHook = null;

            Dalamud.PluginInterface.UiBuilder.Draw -= BuildSettingsUi;
            Dalamud.PluginInterface.UiBuilder.Draw -= Animate;
            Dalamud.PluginInterface.UiBuilder.OpenMainUi -= OpenConfig;
            Dalamud.PluginInterface.UiBuilder.OpenConfigUi -= OpenConfig;
            Dalamud.Framework.Update -= FrameworkOnUpdate;
            Dalamud.ClientState.TerritoryChanged -= ZoneChanged;

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
            Configuration = null;

            RemoveCommands();
        }

        private void Animate() {
            if (!IsLoaded) return;
            Animation.Tick();
        }

        private void FrameworkOnUpdate(IFramework framework) {
            if (!IsLoaded) return;

            var addon = AtkHelper.BuffGaugeAttachAddon;

            if (!LoggedOut && RecreateUi) {
                Logout();
                return;
            }

            if (!PlayerExists) {
                if (!LoggedOut && (addon == null)) Logout();
                return;
            }

            if (addon == null || addon->RootNode == null || RecreateUi) return;

            if (LoggedOut) {
                Dalamud.Log("====== REATTACH =======");
                Builder.Attach(); // re-attach after addons have been created
                LoggedOut = false;
                return;
            }

            AtkHelper.TickTextures();
            CheckForJobChange();
            Tick();

            GaugeManager.UpdatePositionScale();
            BuffManager.UpdatePositionScale();
            CooldownManager.UpdatePositionScale();
        }

        private void Logout() {
            Dalamud.Log("==== LOGOUT ====");
            IconBuilder.Reset();
            Animation.Dispose();

            LoggedOut = true;
            CurrentJob = JobIds.OTHER;
        }

        private void CheckForJobChange() {
            JobIds job = AtkHelper.IdToJob(Dalamud.ClientState.LocalPlayer.ClassJob.Id);

            if (job != CurrentJob) {
                CurrentJob = job;
                Dalamud.Log($"SWITCHED JOB TO {CurrentJob}");
                GaugeManager.SetJob(CurrentJob);
                CursorManager.SetJob(CurrentJob);
                IconManager.SetJob(CurrentJob);
            }
        }

        private void Tick() {
            if (!IsLoaded) return;

            AtkHelper.UpdateMp(Dalamud.ClientState.LocalPlayer.CurrentMp);
            AtkHelper.UpdatePlayerStatus();

            UpdatePartyMembers();
            GaugeManager.Tick();
            BuffManager.Tick();
            CooldownManager.Tick();
            CursorManager.Tick();
            IconManager.Tick();

            var time = DateTime.Now;
            var millis = time.Second * 1000 + time.Millisecond;
            var percent = (float)(millis % 1000) / 1000;

            Builder.Tick(Configuration.GaugePulse ? percent : 0f);
        }

        // ======= COMMANDS ============

        private void SetupCommands() {
            Dalamud.CommandManager.AddHandler("/jobbars", new CommandInfo(OnCommand) {
                HelpMessage = $"Open config window for {Name}",
                ShowInHelp = true
            });
        }

        private void OpenConfig() {
            if (!IsLoaded) return;
            Visible = true;
        }

        public void OnCommand(object command, object args) {
            Visible = !Visible;
        }

        public void RemoveCommands() {
            Dalamud.CommandManager.RemoveHandler("/jobbars");
        }
    }
}
