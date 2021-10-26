namespace JobBars.Gauges.Types {
    public abstract class GaugeTypeConfig {
        public readonly string Name;

        public GaugeTypeConfig(string name) {
            Name = name;
        }

        public abstract void Draw(string id, out bool newPos, out bool newVisual, out bool reset);
    }
}
