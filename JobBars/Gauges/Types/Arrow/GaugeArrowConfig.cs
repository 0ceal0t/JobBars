namespace JobBars.Gauges.Types.Arrow {
    public class GaugeArrowConfig : GaugeTypeConfig {
        public GaugeArrowConfig(string name) : base(name) {
        }

        public override void Draw(string id, out bool newPos, out bool newVisual, out bool reset) {
            newPos = newVisual = reset = false;
        }
    }
}
