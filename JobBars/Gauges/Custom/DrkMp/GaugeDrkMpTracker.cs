using Dalamud.Game.ClientState.JobGauge.Types;
using JobBars.Atk;
using JobBars.Gauges.MP;
using JobBars.Gauges.Types.Arrow;
using JobBars.Gauges.Types.Bar;
using JobBars.Gauges.Types.BarDiamondCombo;
using JobBars.Gauges.Types.Diamond;

namespace JobBars.Gauges.Custom {
    public class GaugeDrkMPTracker : GaugeMPTracker, IGaugeBarDiamondComboInterface, IGaugeArrowInterface, IGaugeDiamondInterface {
        private readonly GaugeDrkMPConfig Config;
        private bool DarkArts = false;

        public GaugeDrkMPTracker( GaugeDrkMPConfig config, int idx ) : base( config, idx, true ) {
            Config = config;
            LoadUI( Config.TypeConfig switch {
                GaugeBarConfig _ => new GaugeBar<GaugeDrkMPTracker>( this, idx ),
                GaugeArrowConfig _ => new GaugeArrow<GaugeDrkMPTracker>( this, idx ),
                GaugeDiamondConfig _ => new GaugeDiamond<GaugeDrkMPTracker>( this, idx ),
                GaugeBarDiamondComboConfig _ => new GaugeBarDiamondCombo<GaugeDrkMPTracker>( this, idx ),
                _ => new GaugeBarDiamondCombo<GaugeDrkMPTracker>( this, idx ) // DEFAULT
            } );
        }

        public override GaugeConfig GetConfig() => Config;

        protected override void TickTracker() {
            base.TickTracker();

            var drkGauge = Dalamud.JobGauges.Get<DRKGauge>();
            DarkArts = drkGauge != null && drkGauge.HasDarkArts;
        }

        public override bool GetBarTextVisible() => Config.TypeConfig switch {
            GaugeBarConfig barConfig => barConfig.ShowText,
            GaugeBarDiamondComboConfig comboConfig => comboConfig.ShowText,
            _ => false
        };

        // ======== DIAMOND ============

        public int GetCurrentMaxTicks() => 1;

        public ElementColor GetTickColor( int idx ) => Config.DarkArtsColor;

        public bool GetTickValue( int idx ) => DarkArts;

        public bool GetReverseFill() => false; // doesn't matter anyway, only 1 charge

        public int GetTotalMaxTicks() => 1;

        public bool GetDiamondTextVisible() => false;

        public string GetDiamondText( int idx ) => "";
    }
}
