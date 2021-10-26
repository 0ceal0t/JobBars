using JobBars.UI;

namespace JobBars.Gauges.Types.Arrow {
    public interface IGaugeArrowInterface {
        // ===== SETUP ======

        public int GetTotalMaxTicks();

        public int GetCurrentMaxTicks();

        public ElementColor GetColor();

        // ===== TICK =======

        public int GetTicks();
    }
}
