using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
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
using JobBars.Nodes.Builder;
using KamiToolKit;
using System;

namespace JobBars {
    public unsafe partial class JobBars : IDalamudPlugin {
        public static Configuration Configuration { get; private set; }
        public static NodeBuilder NodeBuilder { get; private set; }
        public static GaugeManager GaugeManager { get; private set; }
        public static BuffManager BuffManager { get; private set; }
        public static CooldownManager CooldownManager { get; private set; }
        public static CursorManager CursorManager { get; private set; }
        public static IconManager IconManager { get; private set; }
        public static NativeController NativeController { get; private set; }

        public static JobIds CurrentJob { get; private set; } = JobIds.OTHER;

        public static AttachAddon AttachAddon { get; private set; } = AttachAddon.Chatbox;
        public static AttachAddon CooldownAttachAddon { get; private set; } = AttachAddon.PartyList;

        public static uint NodeId { get; set; } = 89990001;

        public JobBars( IDalamudPluginInterface pluginInterface ) {
            pluginInterface.Create<Service>();

            NativeController = new( pluginInterface );

            UiHelper.Setup();
            ColorConstants.SetupColors();

            try {
                Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            }
            catch( Exception e ) {
                Service.Error( e, "Error loading config" );
                Configuration = new Configuration();
                Configuration.Save();
            }
            if( Configuration.Version < 1 ) {
                Service.Log( "Old config version found" );
                Configuration = new Configuration();
                Configuration.Save();
            }

            ReceiveActionEffectHook = Service.Hooks.HookFromSignature<ReceiveActionEffectDelegate>( Constants.ReceiveActionEffectSig, ReceiveActionEffect );
            ActorControlSelfHook = Service.Hooks.HookFromSignature<ActorControlSelfDelegate>( Constants.ActorControlSig, ActorControlSelf );
            ReceiveActionEffectHook.Enable();
            ActorControlSelfHook.Enable();

            AttachAddon = Configuration.AttachAddon;
            CooldownAttachAddon = Configuration.CooldownAttachAddon;

            NodeBuilder = new NodeBuilder();
            BuffManager = new BuffManager();
            CooldownManager = new CooldownManager();
            GaugeManager = new GaugeManager();
            CursorManager = new CursorManager();
            IconManager = new IconManager();

            Service.PluginInterface.UiBuilder.Draw += BuildSettingsUi;
            Service.PluginInterface.UiBuilder.OpenMainUi += OpenConfig;
            Service.PluginInterface.UiBuilder.OpenConfigUi += OpenConfig;
            SetupCommands();

            if( Service.ClientState.IsLoggedIn ) OnLogin();
            Service.Framework.Update += OnFrameworkUpdate;
            Service.ClientState.Login += OnLogin;
            Service.ClientState.Logout += OnLogout;
            Service.ClientState.TerritoryChanged += OnZoneChange;

        }

        public void Dispose() {
            ReceiveActionEffectHook?.Dispose();
            ActorControlSelfHook?.Dispose();

            Service.PluginInterface.UiBuilder.Draw -= BuildSettingsUi;
            Service.PluginInterface.UiBuilder.OpenMainUi -= OpenConfig;
            Service.PluginInterface.UiBuilder.OpenConfigUi -= OpenConfig;
            RemoveCommands();

            Service.Framework.Update -= OnFrameworkUpdate;
            Service.ClientState.Login -= OnLogin;
            Service.ClientState.Logout -= OnLogout;
            Service.ClientState.TerritoryChanged -= OnZoneChange;

            Animation.Dispose();
            NodeBuilder?.Dispose();
            NativeController?.Dispose();
        }

        private void OnFrameworkUpdate( IFramework framework ) {
            if( Service.ClientState.IsPvP ||
                !Service.ClientState.IsLoggedIn ||
                Service.Condition[ConditionFlag.BetweenAreas] || Service.Condition[ConditionFlag.BetweenAreas51] || Service.Condition[ConditionFlag.CreatingCharacter] ||
                !NodeBuilder.IsLoaded ) {

                if( NodeBuilder.GaugeRoot != null ) NodeBuilder.GaugeRoot.IsVisible = false;
                if( NodeBuilder.BuffRoot != null ) NodeBuilder.BuffRoot.IsVisible = false;
                if( NodeBuilder.CooldownRoot != null ) NodeBuilder.CooldownRoot.IsVisible = false;

                return;
            }

            UiHelper.UpdateMp( Service.ClientState.LocalPlayer.CurrentMp );
            UiHelper.UpdatePlayerStatus();

            Animation.Tick();
            CheckForJobChange();
            UpdatePartyMembers();

            GaugeManager.Tick();
            BuffManager.Tick();
            CooldownManager.Tick();
            CursorManager.Tick();
            IconManager.Tick();

            var time = DateTime.Now;
            var millis = time.Second * 1000 + time.Millisecond;
            var percent = ( float )( millis % 1000 ) / 1000;

            NodeBuilder.Tick( Configuration.GaugePulse ? percent : 0f );
            GaugeManager.UpdatePositionScale();
            BuffManager.UpdatePositionScale();
            CooldownManager.UpdatePositionScale();
        }

        private void OnLogin() {
            NodeBuilder.Load();
        }

        private void OnLogout( int type, int code ) {
            Service.Log( "==== LOGOUT ====" );
            Animation.Dispose();
            CurrentJob = JobIds.OTHER;
        }

        private void OnZoneChange( ushort newZoneId ) {
            if( !NodeBuilder.IsLoaded ) return;

            GaugeManager?.Reset();
            IconManager?.Reset();
            BuffManager?.Reset();
            UiHelper.ResetTicks();
        }

        private static void CheckForJobChange() {
            var job = UiHelper.IdToJob( Service.ClientState.LocalPlayer.ClassJob.RowId );
            if( job != CurrentJob ) {
                CurrentJob = job;
                Service.Log( $"SWITCHED JOB TO {CurrentJob}" );
                GaugeManager.SetJob( CurrentJob );
                CursorManager.SetJob( CurrentJob );
                IconManager.SetJob( CurrentJob );
            }
        }

        // ======= COMMANDS ============

        private void SetupCommands() {
            Service.CommandManager.AddHandler( "/jobbars", new CommandInfo( OnCommand ) {
                HelpMessage = $"Open config window for VFXEditor",
                ShowInHelp = true
            } );
        }

        private void OpenConfig() {
            if( !NodeBuilder.IsLoaded ) return;
            Visible = true;
        }

        public void OnCommand( object command, object args ) {
            Visible = !Visible;
        }

        public static void RemoveCommands() {
            Service.CommandManager.RemoveHandler( "/jobbars" );
        }
    }
}
