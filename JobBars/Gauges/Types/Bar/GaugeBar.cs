using JobBars.Atk;

namespace JobBars.Gauges.Types.Bar {
    public class GaugeBar<T> : Gauge<AtkBar, T> where T : GaugeTracker, IGaugeBarInterface {
        public GaugeBar( T tracker, int idx ) {
            Tracker = tracker;
            UI = JobBars.Builder.Bars[idx];
            UI.SetSegments( Tracker.GetBarSegments() );
            UI.SetTextColor( Tracker.GetBarDanger() ? AtkColor.Red : AtkColor.NoColor );
            UI.SetPercent( 0 );
            UI.SetText( "0" );
        }

        protected override int GetHeightGauge() => Tracker.GetVertical() ? 160 : 46;

        protected override int GetWidthGauge() => Tracker.GetVertical() ? 55 : 160;

        public override int GetYOffset() => 0;

        protected override void TickGauge() {
            UI.SetTextColor( Tracker.GetBarDanger() ? AtkColor.Red : AtkColor.NoColor );
            UI.SetText( Tracker.GetBarText() );

            var value = Tracker.GetBarPercent();
            UI.SetPercent( value );
            UI.SetIndicatorPercent( Tracker.GetBarIndicatorPercent(), value );
        }

        protected override void UpdateVisualGauge() {
            UI.SetColor( Tracker.GetColor() );
            UI.SetTextVisible( Tracker.GetBarTextVisible() );
            UI.SetLayout( Tracker.GetBarTextSwap(), Tracker.GetVertical() );
        }
    }
}
