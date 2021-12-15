using JobBars.Gauges.Types.Bar;
using JobBars.Helper;
using JobBars.UI;

namespace JobBars.Gauges.Rolling {
    public class GaugeRollingTracker : GaugeTracker, IGaugeBarInterface {
        private readonly GaugeRollingConfig Config;

        protected float Value;
        protected string TextValue;

        public GaugeRollingTracker(GaugeRollingConfig config, int idx) {
            Config = config;
            LoadUI(Config.TypeConfig switch {
                GaugeBarConfig _ => new GaugeBar<GaugeRollingTracker>(this, idx),
                _ => new GaugeBar<GaugeRollingTracker>(this, idx) // DEFAULT
            });
        }

        public override GaugeConfig GetConfig() => Config;

        public override bool GetActive() => true;

        public override void ProcessAction(Item action) { }

        protected override void TickTracker() {
            Value = UIHelper.GetGCD(out var timeElapsed, out var total);
            var timeLeft = total - timeElapsed;
            TextValue = timeLeft.ToString("0.00");
        }

        public virtual float[] GetBarSegments() => null;

        public virtual bool GetBarTextVisible() => Config.TypeConfig switch {
            GaugeBarConfig barConfig => barConfig.ShowText,
            _ => false
        };

        public virtual bool GetBarTextSwap() => Config.TypeConfig switch {
            GaugeBarConfig barConfig => barConfig.SwapText,
            _ => false
        };

        public virtual bool GetVertical() => Config.TypeConfig switch {
            GaugeBarConfig barConfig => barConfig.Vertical,
            _ => false
        };

        public virtual ElementColor GetColor() => Config.Color;

        public virtual bool GetBarDanger() => false;

        public virtual string GetBarText() => TextValue;

        public virtual float GetBarPercent() => Value;
    }
}
