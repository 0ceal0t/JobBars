using JobBars.UI;

namespace JobBars.Gauges.Types.Bar {
    public interface IGaugeBarInterface {
        // ===== SETUP ======

        public float[] GetBarSegments();

        public bool GetBarTextVisible();

        public bool GetBarTextSwap();

        public bool GetVertical();

        public ElementColor GetColor();

        // ===== TICK =======

        public bool GetBarDanger();

        public string GetBarText();

        public float GetBarPercent();

        public float GetBarIndicatorPercent();
    }
}
