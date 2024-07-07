using JobBars.Atk;
using JobBars.Nodes.Gauge.BarDiamondCombo;

namespace JobBars.Gauges.Types.BarDiamondCombo {
    public class GaugeBarDiamondCombo<T> : Gauge<BarDiamondComboNode, T> where T : GaugeTracker, IGaugeBarDiamondComboInterface {
        public GaugeBarDiamondCombo( T tracker, int idx ) {
            Tracker = tracker;
            Node = new( JobBars.NodeBuilder.GaugeRoot.Bars[idx], JobBars.NodeBuilder.GaugeRoot.Diamonds[idx] );
            Node.SetSegments( Tracker.GetBarSegments() );
            Node.SetTextColor( Tracker.GetBarDanger() ? ColorConstants.Red : ColorConstants.NoColor );
            Node.SetPercent( 0 );
            Node.SetText( "0" );

            Node.SetMaxValue( Tracker.GetCurrentMaxTicks() );
            Node.Clear();
        }

        protected override int GetHeightGauge() => 50;

        protected override int GetWidthGauge() => 160;

        public override int GetYOffset() => 0;

        private bool Reverse => Tracker.GetReverseFill();
        private int Size => Tracker.GetCurrentMaxTicks();
        private int Index( int i ) => Reverse ? ( Size - i - 1 ) : i;

        protected override void TickGauge() {
            Node.SetTextColor( Tracker.GetBarDanger() ? ColorConstants.Red : ColorConstants.NoColor );
            Node.SetText( Tracker.GetBarText() );
            Node.SetPercent( Tracker.GetBarPercent() );

            for( var i = 0; i < Size; i++ ) {
                Node.SetDiamondValue( i, Tracker.GetTickValue( Index( i ) ) );
            }
        }

        protected override void UpdateVisualGauge() {
            Node.SetBarColor( Tracker.GetColor() );
            Node.SetBarTextVisible( Tracker.GetBarTextVisible() );

            for( var i = 0; i < Size; i++ ) {
                Node.SetDiamondColor( i, Tracker.GetTickColor( Index( i ) ) );
            }
        }
    }
}
