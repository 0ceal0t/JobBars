using JobBars.Atk;

namespace JobBars.Gauges.Types.Diamond {
    public interface IGaugeDiamondInterface {
        // ===== SETUP ======

        public int GetTotalMaxTicks();

        public int GetCurrentMaxTicks();

        public ElementColor GetTickColor(int idx);

        public bool GetDiamondTextVisible();

        // ===== TICK =======

        public bool GetTickValue(int idx);

        public string GetDiamondText(int idx);

        public bool GetReverseFill();
    }
}
