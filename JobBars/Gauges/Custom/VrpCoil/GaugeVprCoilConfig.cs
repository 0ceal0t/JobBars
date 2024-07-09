using JobBars.Atk;
using JobBars.Gauges.Stacks;

namespace JobBars.Gauges.Custom.VrpCoil {
    public class GaugeVprCoilConfig : GaugeStacksConfig {
        public GaugeVprCoilConfig( string name, GaugeVisualType type ) : base( name, type, new() {
            MaxStacks = 3,
            Triggers = [],
            Color = ColorConstants.Red
        } ) {
        }

        public override GaugeTracker GetTracker( int idx ) => new GaugeVprCoilTracker( this, idx );
    }
}
