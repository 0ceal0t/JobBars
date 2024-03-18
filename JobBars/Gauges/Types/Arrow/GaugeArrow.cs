using JobBars.Atk;

namespace JobBars.Gauges.Types.Arrow {
    public class GaugeArrow<T> : Gauge<AtkArrow, T> where T : GaugeTracker, IGaugeArrowInterface {
        private readonly int MaxStacks;

        public GaugeArrow( T tracker, int idx ) {
            Tracker = tracker;
            UI = JobBars.Builder.Arrows[idx];
            MaxStacks = Tracker.GetTotalMaxTicks();
            UI.SetMaxValue( Tracker.GetCurrentMaxTicks() );
            UI.Clear();
        }

        protected override int GetHeightGauge() => 32;

        protected override int GetWidthGauge() => 32 + 18 * ( MaxStacks - 1 );

        public override int GetYOffset() => -3;

        private bool Reverse => Tracker.GetReverseFill();
        private int Size => Tracker.GetCurrentMaxTicks();
        private int Index( int i ) => Reverse ? ( Size - i - 1 ) : i;

        protected override void TickGauge() {
            for( var i = 0; i < Size; i++ ) {
                UI.SetValue( i, Tracker.GetTickValue( Index( i ) ) );
            }
        }

        protected override void UpdateVisualGauge() {
            UI.SetMaxValue( Tracker.GetCurrentMaxTicks() );

            for( var i = 0; i < Size; i++ ) {
                UI.SetColor( i, Tracker.GetTickColor( Index( i ) ) );
            }
        }
    }
}
