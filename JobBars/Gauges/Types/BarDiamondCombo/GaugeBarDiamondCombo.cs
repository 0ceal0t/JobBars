using JobBars.UI;

namespace JobBars.Gauges.Types.BarDiamondCombo {
    public class GaugeBarDiamondCombo<T> : Gauge<UIBarDiamondCombo, T> where T : GaugeTracker, IGaugeBarDiamondComboInterface {
        public GaugeBarDiamondCombo(T tracker, int idx) {
            Tracker = tracker;
            UI = new UIBarDiamondCombo(JobBars.Builder.Bars[idx], JobBars.Builder.Diamonds[idx]);
            UI.SetSegments(Tracker.GetBarSegments());
            UI.SetTextColor(Tracker.GetBarDanger() ? UIColor.Red : UIColor.NoColor);
            UI.SetPercent(0);
            UI.SetText("0");

            UI.SetMaxValue(Tracker.GetCurrentMaxTicks());
            UI.SetDiamondValue(0);
        }

        protected override int GetHeightGauge() => UI.GetHeight(0);

        protected override int GetWidthGauge() => UI.GetWidth(0);

        protected override void TickGauge() {
            UI.SetTextColor(Tracker.GetBarDanger() ? UIColor.Red : UIColor.NoColor);
            UI.SetText(Tracker.GetBarText());
            UI.SetPercent(Tracker.GetBarPercent());

            var selected = Tracker.GetDiamondValue();
            for (int i = 0; i < selected.Length; i++) {
                UI.SetDiamondValue(i, selected[i]);
            }
        }

        protected override void UpdateVisualGauge() {
            UI.SetColor(Tracker.GetColor());
            UI.SetBarTextVisible(Tracker.GetBarTextVisible());

            var colors = Tracker.GetDiamondColors();
            for (int i = 0; i < colors.Length; i++) {
                UI.SetDiamondColor(colors[i], i);
            }
        }
    }
}
