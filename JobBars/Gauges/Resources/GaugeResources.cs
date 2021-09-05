using JobBars.Data;
using JobBars.UI;

namespace JobBars.Gauges {
    public struct GaugeResourcesProps {
        public float[] Segments;
        public ElementColor Color;
    }

    public class GaugeResources : Gauge {

        public delegate void ResourceFunction(out float percent, out string text);
        private readonly ResourceFunction Function;
        private GaugeResourcesProps Props;

        public GaugeResources(string name, ResourceFunction function, GaugeResourcesProps props) : base(name) {
            Props = props;
            Function = function;
        }

        protected override void LoadUI_() {
            if (UI is UIBar gauge) {
                gauge.SetSegments(Props.Segments);
                gauge.SetTextColor(UIColor.NoColor);
            }

            SetValue(0, "0");
        }

        protected override void ApplyUIConfig_() {
            if (UI is UIBar gauge) {
                gauge.SetTextVisible(ShowText);
            }
            UI.SetColor(Props.Color);
        }

        private void SetValue(float value, string text) {
            if (UI is UIBar gauge) {
                gauge.SetText(text);
                gauge.SetPercent(value);
            }
        }

        public override void Tick() {
            Function(out var value, out var text);
            SetValue(value, text);
        }

        public override void ProcessAction(Item action) { }

        public override GaugeVisualType GetVisualType() => GaugeVisualType.Bar;
        protected override int GetHeight() => UI.GetHeight(0);
        protected override int GetWidth() => UI.GetWidth(0);

        protected override void DrawGauge(string _ID, JobIds job) {
            if (JobBars.Config.GaugeColor.Draw($"Color{_ID}", Name, Props.Color, out var newColor)) {
                Props.Color = newColor;
                ApplyUIConfig();
            }
        }

        // =====================

        public static void DrkMp(out float percent, out string text) {
            var mp = JobBars.ClientState.LocalPlayer.CurrentMp;
            percent = mp / 10000f;
            text = ((int)(mp / 100)).ToString();
        }
    }
}
