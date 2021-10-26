using JobBars.UI;

namespace JobBars.Gauges.Resources {
    public struct GaugeResourcesProps {
        public float[] Segments;
        public ElementColor Color;
        public ResourceFunction Function;
    }

    public delegate void ResourceFunction(out float percent, out string text);

    public class GaugeResourcesConfig : GaugeConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Bar };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public float[] Segments { get; private set; }
        public ElementColor Color { get; private set; }
        public ResourceFunction Function { get; private set; }

        public GaugeResourcesConfig(string name, GaugeVisualType type, GaugeResourcesProps props) : base(name, type) {
            Segments = props.Segments;
            Function = props.Function;
            Color = JobBars.Config.GaugeColor.Get(Name, props.Color);
        }

        protected override void DrawConfig(string id, out bool newPos, out bool newVisual, out bool reset) {
            newPos = newVisual = reset = false;

            if (JobBars.Config.GaugeColor.Draw($"Color{id}", Name, Color, out var newColor)) {
                Color = newColor;
                newVisual = true;
            }
        }

        // ================================

        public static void DrkMp(out float percent, out string text) {
            var mp = JobBars.ClientState.LocalPlayer.CurrentMp;
            percent = mp / 10000f;
            text = ((int)(mp / 100)).ToString();
        }
    }
}
