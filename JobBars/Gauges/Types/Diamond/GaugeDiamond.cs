using JobBars.UI;

namespace JobBars.Gauges.Types.Diamond {
    public class GaugeDiamond<T> : Gauge<UIDiamond, T> where T : GaugeTracker, IGaugeDiamondInterface {
        private readonly int MaxStacks;

        public GaugeDiamond(T tracker, int idx) {
            Tracker = tracker;
            UI = JobBars.Builder.Diamonds[idx];
            MaxStacks = Tracker.GetTotalMaxTicks();
            UI.SetMaxValue(Tracker.GetCurrentMaxTicks());
            UI.SetValue(0);
        }

        protected override int GetHeightGauge() => UI.GetHeight(0);

        protected override int GetWidthGauge() => UI.GetWidth(MaxStacks);

        protected override void TickGauge() {
            var selected = Tracker.GetDiamondValue();
            for (int i = 0; i < selected.Length; i++) {
                UI.SetValue(i, selected[i]);
            }

            var text = Tracker.GetDiamondText();
            if (text != null) {
                for (int i = 0; i < text.Length; i++) {
                    UI.SetText(i, text[i]);
                }
            }
        }

        protected override void UpdateVisualGauge() {
            UI.SetTextVisible(Tracker.GetDiamondTextVisible());
            UI.SetMaxValue(Tracker.GetCurrentMaxTicks());

            var colors = Tracker.GetDiamondColors();
            for(int i = 0; i < colors.Length; i++) {
                UI.SetColor(colors[i], i);
            }
        }
    }
}
