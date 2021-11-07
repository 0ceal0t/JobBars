using JobBars.UI;

namespace JobBars.Gauges.Rolling {
    public class GaugeRollingConfig : GaugeConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Bar };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public ElementColor Color { get; private set; }

        public GaugeRollingConfig(string name, GaugeVisualType type) : base(name, type) {
            Enabled = JobBars.Config.GaugeEnabled.Get(Name, false); // default disabled
            Color = JobBars.Config.GaugeColor.Get(Name, UIColor.Yellow);
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeRollingTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newPos, ref bool newVisual, ref bool reset) {
            if (JobBars.Config.GaugeColor.Draw($"Color{id}", Name, Color, out var newColor)) {
                Color = newColor;
                newVisual = true;
            }
        }
    }
}
