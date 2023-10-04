using JobBars.Gauges.MP;
using JobBars.Atk;

namespace JobBars.Gauges.Custom {
    public struct GaugeDrkMpProps {
        public float[] Segments;
        public ElementColor Color;
        public ElementColor DarkArtsColor;
    }

    public class GaugeDrkMPConfig : GaugeMPConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Bar, GaugeVisualType.BarDiamondCombo, GaugeVisualType.Diamond, GaugeVisualType.Arrow };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public ElementColor DarkArtsColor { get; private set; }

        private string DarkArtsName => Name + "/DarkArts";

        public GaugeDrkMPConfig(string name, GaugeVisualType type, GaugeDrkMpProps props) : base(name, type, props.Segments) {
            DarkArtsColor = JobBars.Configuration.GaugeColor.Get(DarkArtsName, props.DarkArtsColor);
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeDrkMPTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newVisual, ref bool reset) {
            base.DrawConfig(id, ref newVisual, ref reset);

            if (JobBars.Configuration.GaugeColor.Draw($"Dark Arts color{id}", Name, Color, out var newDarkArtsColor)) {
                DarkArtsColor = newDarkArtsColor;
                newVisual = true;
            }
        }
    }
}
