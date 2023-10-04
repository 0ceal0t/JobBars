using JobBars.Atk;

namespace JobBars.Gauges.Types.BarDiamondCombo {
    public class GaugeBarDiamondCombo<T> : Gauge<AtkBarDiamondCombo, T> where T : GaugeTracker, IGaugeBarDiamondComboInterface {
        public GaugeBarDiamondCombo(T tracker, int idx) {
            Tracker = tracker;
            UI = new AtkBarDiamondCombo(JobBars.Builder.Bars[idx], JobBars.Builder.Diamonds[idx]);
            UI.SetSegments(Tracker.GetBarSegments());
            UI.SetTextColor(Tracker.GetBarDanger() ? AtkColor.Red : AtkColor.NoColor);
            UI.SetPercent(0);
            UI.SetText("0");

            UI.SetMaxValue(Tracker.GetCurrentMaxTicks());
            UI.Clear();
        }

        protected override int GetHeightGauge() => 50;

        protected override int GetWidthGauge() => 160;

        public override int GetYOffset() => 0;

        private bool Reverse => Tracker.GetReverseFill();
        private int Size => Tracker.GetCurrentMaxTicks();
        private int Index(int i) => Reverse ? (Size - i - 1) : i;

        protected override void TickGauge() {
            UI.SetTextColor(Tracker.GetBarDanger() ? AtkColor.Red : AtkColor.NoColor);
            UI.SetText(Tracker.GetBarText());
            UI.SetPercent(Tracker.GetBarPercent());

            for (var i = 0; i < Size; i++) {
                UI.SetDiamondValue(i, Tracker.GetTickValue(Index(i)));
            }
        }

        protected override void UpdateVisualGauge() {
            UI.SetGaugeColor(Tracker.GetColor());
            UI.SetBarTextVisible(Tracker.GetBarTextVisible());

            for (var i = 0; i < Size; i++) {
                UI.SetDiamondColor(i, Tracker.GetTickColor(Index(i)));
            }
        }
    }
}
