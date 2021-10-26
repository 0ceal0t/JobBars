using JobBars.UI;

namespace JobBars.Gauges.Types.BarDiamondCombo {
    public interface IGaugeBarDiamondComboInterface {
        // ===== SETUP ======

        public float[] GetBarSegments();

        public bool GetBarTextVisible();

        public bool GetBarTextSwap();

        public ElementColor GetColor();

        public int GetCurrentMaxTicks();

        public ElementColor[] GetDiamondColors();

        // ===== TICK =======

        public bool GetBarDanger();

        public string GetBarText();

        public float GetBarPercent();

        public bool[] GetDiamondValue();
    }
}
