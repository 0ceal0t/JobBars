using System;
using System.Numerics;
using System.Reflection;
using System.Threading;

using JobBars.Helper;
using JobBars.UI;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;

using FFXIVClientStructs.FFXIV.Component.GUI;

using Dalamud.Plugin;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Data;
using JobBars.Icons;

namespace JobBars {
    public unsafe partial class JobBars : IDalamudPlugin {
        public static DalamudPluginInterface PluginInterface    { get; private set; }
        public static ClientState ClientState                   { get; private set; }
        public static Framework Framework                       { get; private set; }
        public static Condition Condition                       { get; private set; }
        public static CommandManager CommandManager             { get; private set; }
        public static ObjectTable Objects                       { get; private set; }
        public static SigScanner SigScanner                     { get; private set; }
        public static DataManager DataManager                   { get; private set; }

        public static Configuration Config                      { get; private set; }
        public static UIBuilder Builder                         { get; private set; }
        public static UIIconManager IconBuilder                 { get; private set; }

        public static GaugeManager GaugeManager                 { get; private set; }
        public static BuffManager BuffManager                   { get; private set; }
        public static CooldownManager CooldownManager           { get; private set; }
        public static CursorManager CursorManager               { get; private set; }
        public static IconManager IconManager                   { get; private set; }

        public string Name => "JobBars";
        public string AssemblyLocation { get; private set; } = Assembly.GetExecutingAssembly().Location;

        private JobIds CurrentJob = JobIds.OTHER;

        private delegate void ReceiveActionEffectDelegate(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private Hook<ReceiveActionEffectDelegate> ReceiveActionEffectHook;

        private delegate void ActorControlSelfDelegate(uint entityId, uint id, uint arg0, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, UInt64 targetId, byte a10);
        private Hook<ActorControlSelfDelegate> ActorControlSelfHook;

        private static bool PlayerExists => ClientState?.LocalPlayer != null;
        private bool LoggedOut = true;

        private Vector2 LastPosition;
        private Vector2 LastScale;

        /*
         * SIG LIST:
         *      
         *  UiHelper
         *      PlaySoundEffect (E8 ?? ?? ?? ?? 4D 39 BE ?? ?? ?? ??)
         *      TextureLoadPath (E8 ?? ?? ?? ?? 4C 8B 6C 24 ?? 4C 8B 5C 24 ??)
         *      TargetAddress (48 8B 05 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? FF 50 ?? 48 85 DB)
         *      
         *  JobBars
         *      receiveActionEffectFuncPtr (4C 89 44 24 18 53 56 57 41 54 41 57 48 81 EC ?? 00 00 00 8B F9)
         *      actorControlSelfPtr (E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64)
         */

        public JobBars(
                DalamudPluginInterface pluginInterface,
                ClientState clientState,
                CommandManager commandManager,
                Condition condition,
                Framework framework,
                ObjectTable objects,
                SigScanner sigScanner,
                DataManager dataManager
            ) {
            PluginInterface = pluginInterface;
            ClientState = clientState;
            Framework = framework;
            Condition = condition;
            CommandManager = commandManager;
            Objects = objects;
            SigScanner = sigScanner;
            DataManager = dataManager;

            if (!FFXIVClientStructs.Resolver.Initialized) FFXIVClientStructs.Resolver.Initialize();

            UIHelper.Setup();
            UIColor.SetupColors();
            Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            if (Config.Version == 0) Config = new Configuration(); // remove outdated

            IconBuilder = new UIIconManager();

            InitializeUI(); // <======= TEXTURES AND UI INITIALIZED HERE

            IntPtr receiveActionEffectFuncPtr = SigScanner.ScanText("4C 89 44 24 18 53 56 57 41 54 41 57 48 81 EC ?? 00 00 00 8B F9");
            ReceiveActionEffectHook = new Hook<ReceiveActionEffectDelegate>(receiveActionEffectFuncPtr, ReceiveActionEffect);
            ReceiveActionEffectHook.Enable();

            IntPtr actorControlSelfPtr = SigScanner.ScanText("E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64");
            ActorControlSelfHook = new Hook<ActorControlSelfDelegate>(actorControlSelfPtr, ActorControlSelf);
            ActorControlSelfHook.Enable();

            PluginInterface.UiBuilder.Draw += BuildSettingsUI;
            PluginInterface.UiBuilder.Draw += Animate;
            PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfig;
            Framework.Update += FrameworkOnUpdate;
            ClientState.TerritoryChanged += ZoneChanged;
            SetupCommands();
        }

        private void InitializeUI() { // this only ever gets run ONCE
            // these are created before the addons are even visible, so they aren't attached yet
            PluginLog.Log("==== INIT ====");
            IconBuilder.Reset();

            Builder = new UIBuilder();
            BuffManager = new BuffManager();
            CooldownManager = new CooldownManager();
            GaugeManager = new GaugeManager();
            CursorManager = new CursorManager();
            IconManager = new IconManager();
        }

        public void Dispose() {
            ReceiveActionEffectHook?.Disable();
            ActorControlSelfHook?.Disable();

            Thread.Sleep(500);

            ReceiveActionEffectHook?.Dispose();
            ActorControlSelfHook?.Dispose();
            ReceiveActionEffectHook = null;
            ActorControlSelfHook = null;

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

        private void Animate() => Animation.Tick();

        private void FrameworkOnUpdate(Framework framework) {
            var addon = UIHelper.ChatLogAddon;

            if (!PlayerExists) {
                if (!LoggedOut && addon == null) Logout();
                return;
            }

            if (addon == null || addon->RootNode == null) return;

            if(LoggedOut) {
                Builder.Attach(); // re-attach after addons have been created
                LoggedOut = false;
                return;
            }

            CheckForJobChange();
            Tick();
            CheckForHUDChange(addon);
        }

        private void Logout() {
            PluginLog.Log("==== LOGOUT ===");
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
            var inCombat = Condition[ConditionFlag.InCombat];
            UIHelper.UpdateMp(ClientState.LocalPlayer.CurrentMp);
            UIHelper.UpdatePlayerStatus();

            UpdatePartyMembers();
            GaugeManager.Tick(inCombat);
            BuffManager.Tick(inCombat);
            CooldownManager.Tick(inCombat);
            CursorManager.Tick();
            IconManager.Tick();
        }

        private void CheckForHUDChange(AtkUnitBase* addon) {
            var currentPosition = UIHelper.GetNodePosition(addon->RootNode);
            var currentScale = UIHelper.GetNodeScale(addon->RootNode);

            if (currentPosition != LastPosition || currentScale != LastScale) {
                GaugeManager.UpdatePositionScale();
                BuffManager.UpdatePositionScale();
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
