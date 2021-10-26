namespace JobBars.Gauges.Types.Diamond {
    public class GaugeDiamondConfig : GaugeTypeConfig {
        public GaugeDiamondConfig(string name) : base(name) {
        }

        public override void Draw(string id, out bool newPos, out bool newVisual, out bool reset) {
            newPos = newVisual = reset = false;
        }
    }
}
