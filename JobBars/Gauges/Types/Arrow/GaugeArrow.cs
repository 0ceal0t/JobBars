using JobBars.Nodes.Gauge.Arrow;

namespace JobBars.Gauges.Types.Arrow {
    public class GaugeArrow<T> : Gauge<ArrowNode, T> where T : GaugeTracker, IGaugeArrowInterface {
        private readonly int MaxStacks;

        public GaugeArrow( T tracker, int idx ) {
            Tracker = tracker;
            Node = JobBars.NodeBuilder.GaugeRoot.Arrows[idx];
            MaxStacks = Tracker.GetTotalMaxTicks();
            Node.SetMaxValue( Tracker.GetCurrentMaxTicks() );
            Node.Clear();
        }

        protected override int GetHeightGauge() => 32;

        protected override int GetWidthGauge() => 32 + 18 * ( MaxStacks - 1 );

        public override int GetYOffset() => -3;

        private bool Reverse => Tracker.GetReverseFill();
        private int Size => Tracker.GetCurrentMaxTicks();
        private int Index( int i ) => Reverse ? ( Size - i - 1 ) : i;

        protected override void TickGauge() {
            for( var i = 0; i < Size; i++ ) {
                Node.SetValue( i, Tracker.GetTickValue( Index( i ) ) );
            }
        }

        protected override void UpdateVisualGauge() {
            Node.SetMaxValue( Tracker.GetCurrentMaxTicks() );

            for( var i = 0; i < Size; i++ ) {
                Node.SetColor( i, Tracker.GetTickColor( Index( i ) ) );
            }
        }
    }
}
