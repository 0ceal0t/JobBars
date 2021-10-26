using JobBars.UI;

namespace JobBars.Gauges.Types.Arrow {
    public class GaugeArrow<T> : Gauge<UIArrow, T> where T : GaugeTracker, IGaugeArrowInterface {
        private readonly int MaxStacks;

        public GaugeArrow(T tracker, int idx) {
            Tracker = tracker;
            UI = JobBars.Builder.Arrows[idx];
            MaxStacks = Tracker.GetTotalMaxTicks();
            UI.SetMaxValue(Tracker.GetCurrentMaxTicks());
            UI.SetValue(0);
        }

        protected override int GetHeightGauge() => UI.GetHeight(0);

        protected override int GetWidthGauge() => UI.GetWidth(MaxStacks);

        protected override void TickGauge() {
            UI.SetValue(Tracker.GetTicks());
        }

        protected override void UpdateVisualGauge() {
            UI.SetMaxValue(Tracker.GetCurrentMaxTicks());
            UI.SetColor(Tracker.GetColor());
        }
    }
}
