namespace JobBars.Gauges.Types {
    public abstract class GaugeTypeConfig {
        public readonly string Name;

        public GaugeTypeConfig(string name) {
            Name = name;
        }

        public abstract void Draw(string id, ref bool newPos, ref bool newVisual, ref bool reset);
    }
}
