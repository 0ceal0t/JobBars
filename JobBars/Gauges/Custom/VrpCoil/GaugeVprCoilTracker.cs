using Dalamud.Game.ClientState.JobGauge.Types;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Types.Arrow;
using JobBars.Gauges.Types.Bar;
using JobBars.Gauges.Types.Diamond;

namespace JobBars.Gauges.Custom.VrpCoil {
    public class GaugeVprCoilTracker : GaugeStacksTracker {
        public GaugeVprCoilTracker( GaugeVprCoilConfig config ) : base( config ) { }

        public override void TickTracker() {
            var vprGauge = Dalamud.JobGauges.Get<VPRGauge>();
            var currentValue = vprGauge == null ? 0 : vprGauge.RattlingCoilStacks;
            PlaySound( currentValue );
            Value = currentValue;
        }
    }
}
