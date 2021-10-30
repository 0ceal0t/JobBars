using JobBars.UI;

namespace JobBars.Gauges.Types.Diamond {
    public interface IGaugeDiamondInterface {
        // ===== SETUP ======

        public int GetTotalMaxTicks();

        public int GetCurrentMaxTicks();

        public ElementColor[] GetDiamondColors();

        public bool GetDiamondTextVisible();

        // ===== TICK =======

        public bool[] GetDiamondValue();

        public string[] GetDiamondText();
    }
}
