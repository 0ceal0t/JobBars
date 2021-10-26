using JobBars.UI;
using JobBars.Gauges.Types.Bar;

namespace JobBars.Gauges.Resources {
    public class GaugeResourcesTracker : GaugeTracker, IGaugeBarInterface {
        private readonly GaugeResourcesConfig Config;
        private float Value;
        private string TextValue;

        public GaugeResourcesTracker(GaugeResourcesConfig config, int idx) {
            Config = config;
            LoadUI(Config.TypeConfig switch {
                GaugeBarConfig _ => new GaugeBar<GaugeResourcesTracker>(this, idx),
                _ => null
            });
        }

        public override GaugeConfig GetConfig() => Config;

        public override bool GetActive() => true;

        public override void ProcessAction(Item action) { }

        protected override void TickTracker() {
            Config.Function(out var value, out var text);
            Value = value;
            TextValue = text;
        }

        public float[] GetBarSegments() => Config.Segments;

        public bool GetBarTextSwap() => Config.TypeConfig switch {
            GaugeBarConfig barConfig => barConfig.SwapText,
            _ => false
        };

        public bool GetBarTextVisible() => Config.TypeConfig switch {
            GaugeBarConfig barConfig => barConfig.ShowText,
            _ => false
        };

        public ElementColor GetColor() => Config.Color;

        public bool GetBarDanger() => false;

        public string GetBarText() => TextValue;

        public float GetBarPercent() => Value;
    }
}
