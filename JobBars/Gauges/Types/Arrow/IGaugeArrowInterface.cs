using JobBars.UI;

namespace JobBars.Gauges.Types.Arrow {
    public interface IGaugeArrowInterface {
        // ===== SETUP ======

        public int GetTotalMaxTicks();

        public int GetCurrentMaxTicks();

        public ElementColor GetTickColor(int idx);

        // ===== TICK =======

        public bool GetTickValue(int idx);

        public bool GetReverseFill();
    }
}
