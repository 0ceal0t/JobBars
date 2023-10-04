using JobBars.Atk;

namespace JobBars.Gauges.Types.BarDiamondCombo {
    public interface IGaugeBarDiamondComboInterface {
        // ===== SETUP ======

        public float[] GetBarSegments();

        public bool GetBarTextVisible();

        public bool GetBarTextSwap();

        public ElementColor GetColor();

        public int GetCurrentMaxTicks();

        public ElementColor GetTickColor(int idx);

        // ===== TICK =======

        public bool GetBarDanger();

        public string GetBarText();

        public float GetBarPercent();

        public bool GetTickValue(int idx);

        public bool GetReverseFill();
    }
}
