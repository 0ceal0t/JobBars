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

        private readonly float[] Segments;
        private ElementColor Color;

        public GaugeResources(string name, ResourceFunction function, GaugeResourcesProps props) : base(name) {
            Segments = props.Segments;
            Color = JobBars.Config.GaugeColor.Get(Name, props.Color);
            Function = function;
        }

        protected override void LoadUIImpl() {
            if (UI is UIBar gauge) {
                gauge.SetSegments(Segments);
                gauge.SetTextColor(UIColor.NoColor);
            }
            SetValue(0, "0");
        }

        protected override void ApplyUIConfigImpl() {
            if (UI is UIBar gauge) {
                gauge.SetTextVisible(ShowText);
                gauge.SetTextSwap(SwapText);
            }
            UI.SetColor(Color);
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

        protected override bool GetActive() => true;

        public override void ProcessAction(Item action) { }

        public override GaugeVisualType GetVisualType() => GaugeVisualType.Bar;
        protected override int GetHeight() => UI.GetHeight(0);
        protected override int GetWidth() => UI.GetWidth(0);

        protected override void DrawGauge(string _ID, JobIds job) {
            if (JobBars.Config.GaugeColor.Draw($"Color{_ID}", Name, Color, out var newColor)) {
                Color = newColor;
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
