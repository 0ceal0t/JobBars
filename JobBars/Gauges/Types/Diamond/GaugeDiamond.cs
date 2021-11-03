using JobBars.UI;

namespace JobBars.Gauges.Types.Diamond {
    public class GaugeDiamond<T> : Gauge<UIDiamond, T> where T : GaugeTracker, IGaugeDiamondInterface {
        private readonly int MaxStacks;

        public GaugeDiamond(T tracker, int idx) {
            Tracker = tracker;
            UI = JobBars.Builder.Diamonds[idx];
            MaxStacks = Tracker.GetTotalMaxTicks();
            UI.SetMaxValue(Tracker.GetCurrentMaxTicks());
            UI.Clear();
        }

        protected override int GetHeightGauge() => Tracker.GetDiamondTextVisible() ? 40 : 32;

        protected override int GetWidthGauge() => 32 + 20 * (MaxStacks - 1);

        public override int GetYOffset() => -3;

        private bool Reverse => Tracker.GetReverseFill();
        private int Size => Tracker.GetCurrentMaxTicks();
        private int Index(int i) => Reverse ? (Size - i - 1) : i;

        protected override void TickGauge() {
            for (var i = 0; i < Size; i++) {
                UI.SetValue(i, Tracker.GetTickValue(Index(i)));
            }

            if (!Tracker.GetDiamondTextVisible()) return;
            for (var i = 0; i < Size; i++) {
                UI.SetText(i, Tracker.GetDiamondText(Index(i)));
            }
        }

        protected override void UpdateVisualGauge() {
            UI.SetTextVisible(Tracker.GetDiamondTextVisible());
            UI.SetMaxValue(Tracker.GetCurrentMaxTicks());

            for (var i = 0; i < Size; i++) {
                UI.SetColor(i, Tracker.GetTickColor(Index(i)));
            }
        }
    }
}
