using Dalamud.Game.ClientState.JobGauge.Types;
using JobBars.Atk;
using JobBars.Gauges.MP;
using JobBars.Gauges.Types.Arrow;
using JobBars.Gauges.Types.Bar;
using JobBars.Gauges.Types.BarDiamondCombo;
using JobBars.Gauges.Types.Diamond;

namespace JobBars.Gauges.Custom {
    public class GaugeDrkMpTracker : GaugeMPTracker, IGaugeBarDiamondComboInterface, IGaugeArrowInterface, IGaugeDiamondInterface {
        private readonly GaugeDrkMpConfig CustomConfig;
        private bool DarkArts = false;

        public GaugeDrkMpTracker( GaugeDrkMpConfig config ) : base( config, true ) {
            CustomConfig = config;
        }

        public override void TickTracker() {
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

        public ElementColor GetTickColor( int idx ) => CustomConfig.DarkArtsColor;

        public bool GetTickValue( int idx ) => DarkArts;

        public bool GetReverseFill() => false; // doesn't matter anyway, only 1 charge

        public int GetTotalMaxTicks() => 1;

        public bool GetDiamondTextVisible() => false;

        public string GetDiamondText( int idx ) => "";
    }
}
