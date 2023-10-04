using JobBars.Gauges.Types.Bar;
using JobBars.Helper;
using JobBars.Atk;

namespace JobBars.Gauges.Rolling {
    public class GaugeRollingTracker : GaugeTracker, IGaugeBarInterface {
        private readonly GaugeRollingConfig Config;

        protected float Value;
        protected float IndicatorValue;
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
            if (Config.RollingType == GaugeGCDRollingType.GCD) {
                Value = AtkHelper.GetGCD(out var timeElapsed, out var total);
                IndicatorValue = 0f;
                TextValue = (total - timeElapsed).ToString("0.00");
            }
            else if (Config.RollingType == GaugeGCDRollingType.CastTime) {
                Value = AtkHelper.GetCastTime(out var timeElapsed, out var total);
                IndicatorValue = (JobBars.Configuration.GaugeSlidecastTime > 0f && total > 0f) ? JobBars.Configuration.GaugeSlidecastTime / total : 0f;
                TextValue = (total - timeElapsed).ToString("0.00");
            }
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

        public float GetBarIndicatorPercent() => IndicatorValue;
    }
}
