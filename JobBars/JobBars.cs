using System;
using System.Numerics;
using System.Reflection;

using JobBars.Helper;
using JobBars.UI;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;

using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Threading;

using Dalamud.Plugin;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Data;

namespace JobBars {
    public unsafe partial class JobBars : IDalamudPlugin {
        public static DalamudPluginInterface PluginInterface { get; private set; }
        public static ClientState ClientState { get; private set; }
        public static Framework Framework { get; private set; }
        public static Condition Condition { get; private set; }
        public static CommandManager CommandManager { get; private set; }
        public static ObjectTable Objects { get; private set; }
        public static SigScanner SigScanner { get; private set; }
        public static DataManager DataManager { get; private set; }

        public static Configuration Config { get; private set; }
        public static UIBuilder Builder { get; private set; }
        public static UIIconManager Icon { get; private set;}

        public static GaugeManager GaugeManager { get; private set; }
        public static BuffManager BuffManager { get; private set; }
        public static CooldownManager CooldownManager { get; private set; }
        public static CursorManager CursorManager { get; private set; }

        public string Name => "JobBars";
        public string AssemblyLocation { get; private set; } = Assembly.GetExecutingAssembly().Location;

        private JobIds CurrentJob = JobIds.OTHER;

        private delegate void ReceiveActionEffectDelegate(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private Hook<ReceiveActionEffectDelegate> receiveActionEffectHook;

        private delegate void ActorControlSelfDelegate(uint entityId, uint id, uint arg0, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, UInt64 targetId, byte a10);
        private Hook<ActorControlSelfDelegate> actorControlSelfHook;

        private int LastPartyCount = 0;

        private bool PlayerExists => ClientState?.LocalPlayer != null;
        private bool LoggedOut = true;

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

            Icon = new UIIconManager();

            InitializeUI(); // <======= TEXTURES AND UI INITIALIZED HERE

            IntPtr receiveActionEffectFuncPtr = SigScanner.ScanText("4C 89 44 24 18 53 56 57 41 54 41 57 48 81 EC ?? 00 00 00 8B F9");
            receiveActionEffectHook = new Hook<ReceiveActionEffectDelegate>(receiveActionEffectFuncPtr, ReceiveActionEffect);
            receiveActionEffectHook.Enable();

            IntPtr actorControlSelfPtr = SigScanner.ScanText("E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64");
            actorControlSelfHook = new Hook<ActorControlSelfDelegate>(actorControlSelfPtr, ActorControlSelf);
            actorControlSelfHook.Enable();

            PluginInterface.UiBuilder.Draw += BuildSettingsUI;
            PluginInterface.UiBuilder.Draw += Animate;
            PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfig;
            Framework.OnUpdateEvent += FrameworkOnUpdate;
            ClientState.TerritoryChanged += ZoneChanged;
            SetupCommands();
        }

        private void InitializeUI() { // this only ever gets run ONCE
            // these are created before the addons are even visible, so they aren't attached yet
            PluginLog.Log("==== INIT ====");
            Icon.Reset();

            BuffManager = new BuffManager();
            CooldownManager = new CooldownManager();
            GaugeManager = new GaugeManager();
            CursorManager = new CursorManager();
            Builder = new UIBuilder();
            CooldownManager.SetupUI();
            BuffManager.SetupUI();
            CursorManager.SetupUI();

            if (!Config.GaugesEnabled) Builder.HideGauges();
            if (!Config.BuffBarEnabled) Builder.HideBuffs();
            Builder.HideAllBuffs();
            Builder.HideAllGauges();
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
            Framework.OnUpdateEvent -= FrameworkOnUpdate;
            ClientState.TerritoryChanged -= ZoneChanged;

            GaugeManager = null;
            BuffManager = null;
            CooldownManager = null;
            CursorManager = null;

            Animation.Dispose();
            Icon?.Dispose();
            Builder?.Dispose();
            Icon = null;
            Builder = null;

            PluginInterface = null;
            Config = null;

            RemoveCommands();
        }

        private void Animate() {
            Animation.Tick();
        }

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
            CheckForPartyChange();
            CheckForHUDChange(addon);
        }

        private void Logout() {
            PluginLog.Log("==== LOGOUT ===");
            Icon.Reset();
            Animation.Dispose();

            LoggedOut = true;
            CurrentJob = JobIds.OTHER;
        }

        private void CheckForPartyChange() {
            // someone left the party. this means that some buffs might not make sense anymore, so reset it
            var partyCount = UIHelper.GetPartyCount();
            if (partyCount < LastPartyCount) BuffManager.Reset();
            LastPartyCount = partyCount;
        }

        private void CheckForJobChange() {
            var jobId = ClientState.LocalPlayer.ClassJob;
            JobIds job = UIHelper.IdToJob(jobId.Id);

            if (job != CurrentJob) {
                CurrentJob = job;
                PluginLog.Log($"SWITCHED JOB TO {CurrentJob}");
                BuffManager.Reset();
                GaugeManager.SetJob(CurrentJob);
                CursorManager.SetJob(CurrentJob);
            }
        }

        private void Tick() {
            var inCombat = Condition[ConditionFlag.InCombat];
            UIHelper.UpdateMp(ClientState.LocalPlayer.CurrentMp);
            GaugeManager.Tick(inCombat);
            BuffManager.Tick(inCombat);
            CooldownManager.Tick(inCombat);
            CursorManager.Tick();
        }

        private void CheckForHUDChange(AtkUnitBase* addon) {
            var currentPosition = UIHelper.GetNodePosition(addon->RootNode); // changing HP/MP in hud layout
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

        private void OnOpenConfig(object sender, EventArgs eventArgs) {
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
