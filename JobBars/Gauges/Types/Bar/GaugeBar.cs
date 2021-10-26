using JobBars.UI;

namespace JobBars.Gauges.Types.Bar {
    public class GaugeBar<T> : Gauge<UIBar, T> where T : GaugeTracker, IGaugeBarInterface {
        public GaugeBar(T tracker, int idx) {
            Tracker = tracker;
            UI = JobBars.Builder.Bars[idx];
            UI.SetSegments(Tracker.GetBarSegments());
            UI.SetTextColor(Tracker.GetBarDanger() ? UIColor.Red : UIColor.NoColor);
            UI.SetPercent(0);
            UI.SetText("0");
        }

        protected override int GetHeightGauge() => UI.GetHeight(0);

        protected override int GetWidthGauge() => UI.GetWidth(0);

        protected override void TickGauge() {
            UI.SetTextColor(Tracker.GetBarDanger() ? UIColor.Red : UIColor.NoColor);
            UI.SetText(Tracker.GetBarText());
            UI.SetPercent(Tracker.GetBarPercent());
        }

        protected override void UpdateVisualGauge() {
            UI.SetColor(Tracker.GetColor());
            UI.SetTextVisible(Tracker.GetBarTextVisible());
            UI.SetTextSwap(Tracker.GetBarTextSwap());
        }
    }
}
