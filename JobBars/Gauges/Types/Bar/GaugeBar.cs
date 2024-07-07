using JobBars.Atk;
using JobBars.Nodes.Gauge.Bar;

namespace JobBars.Gauges.Types.Bar {
    public class GaugeBar<T> : Gauge<BarNode, T> where T : GaugeTracker, IGaugeBarInterface {
        public GaugeBar( T tracker, int idx ) {
            Tracker = tracker;
            Node = JobBars.NodeBuilder.GaugeRoot.Bars[idx];
            Node.SetSegments( Tracker.GetBarSegments() );
            Node.SetTextColor( Tracker.GetBarDanger() ? ColorConstants.Red : ColorConstants.NoColor );
            Node.SetPercent( 0 );
            Node.SetText( "0" );
        }

        protected override int GetHeightGauge() => Tracker.GetVertical() ? 160 : 46;

        protected override int GetWidthGauge() => Tracker.GetVertical() ? 55 : 160;

        public override int GetYOffset() => 0;

        protected override void TickGauge() {
            Node.SetTextColor( Tracker.GetBarDanger() ? ColorConstants.Red : ColorConstants.NoColor );
            Node.SetText( Tracker.GetBarText() );

            var value = Tracker.GetBarPercent();
            Node.SetPercent( value );
            Node.SetIndicatorPercent( Tracker.GetBarIndicatorPercent(), value );
        }

        protected override void UpdateVisualGauge() {
            Node.SetColor( Tracker.GetColor() );
            Node.SetTextVisible( Tracker.GetBarTextVisible() );
            Node.SetLayout( Tracker.GetBarTextSwap(), Tracker.GetVertical() );
        }
    }
}
