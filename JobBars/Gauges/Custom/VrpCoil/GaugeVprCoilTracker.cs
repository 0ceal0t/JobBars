using Dalamud.Game.ClientState.JobGauge.Types;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Types.Arrow;
using JobBars.Gauges.Types.Bar;
using JobBars.Gauges.Types.Diamond;

namespace JobBars.Gauges.Custom.VrpCoil {
    public class GaugeVprCoilTracker : GaugeStacksTracker {
        public GaugeVprCoilTracker( GaugeVprCoilConfig config, int idx ) : base( config, idx ) {
            LoadUi( config.TypeConfig switch {
                GaugeBarConfig _ => new GaugeBar<GaugeVprCoilTracker>( this, idx ),
                GaugeArrowConfig _ => new GaugeArrow<GaugeVprCoilTracker>( this, idx ),
                _ => new GaugeDiamond<GaugeVprCoilTracker>( this, idx ), // DEFAULT
            } );
        }


        protected override void TickTracker() {
            var vprGauge = Service.JobGauges.Get<VPRGauge>();
            var currentValue = vprGauge == null ? 0 : vprGauge.RattlingCoilStacks;
            PlaySound( currentValue );
            Value = currentValue;
        }
    }
}
