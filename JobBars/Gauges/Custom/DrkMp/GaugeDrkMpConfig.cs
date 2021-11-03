using JobBars.UI;

namespace JobBars.Gauges.Custom {
    public struct GaugeDrkMpProps {
        public float[] Segments;
        public ElementColor Color;
        public ElementColor DarkArtsColor;
    }

    public class GaugeDrkMpConfig : GaugeConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Bar, GaugeVisualType.BarDiamondCombo, GaugeVisualType.Diamond, GaugeVisualType.Arrow };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public float[] Segments { get; private set; }
        public ElementColor Color { get; private set; }
        public ElementColor DarkArtsColor { get; private set; }

        private string DarkArtsName => Name + "/DarkArts";

        public GaugeDrkMpConfig(string name, GaugeVisualType type, GaugeDrkMpProps props) : base(name, type) {
            Segments = props.Segments;
            Color = JobBars.Config.GaugeColor.Get(Name, props.Color);
            DarkArtsColor = JobBars.Config.GaugeColor.Get(DarkArtsName, props.DarkArtsColor);
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeDrkMpTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newPos, ref bool newVisual, ref bool reset) {
            if (JobBars.Config.GaugeColor.Draw($"Color{id}", Name, Color, out var newColor)) {
                Color = newColor;
                newVisual = true;
            }

            if (JobBars.Config.GaugeColor.Draw($"Dark Arts Color{id}", Name, Color, out var newDarkArtsColor)) {
                DarkArtsColor = newDarkArtsColor;
                newVisual = true;
            }
        }
    }
}
