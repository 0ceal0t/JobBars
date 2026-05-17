using JobBars.Atk;
using JobBars.Gauges.Types.Bar;
using JobBars.Helper;

namespace JobBars.Gauges.Rolling {
    public class GaugeRollingTracker : GaugeTracker, IGaugeBarInterface {
        private readonly GaugeRollingConfig Config;

        protected float Value;
        protected float IndicatorValue;
        protected string TextValue;

        public GaugeRollingTracker( GaugeRollingConfig config ) {
            Config = config;
        }

        public override GaugeConfig GetConfig() => Config;

        public override bool GetActive() => true;

        public override void ProcessAction( Item action ) { }

        public override void TickTracker() {
            if( Config.RollingType == GaugeGCDRollingType.GCD ) {
                Value = UiHelper.GetGCD( out var timeElapsed, out var total );
                IndicatorValue = 0f;
                TextValue = ( total - timeElapsed ).ToString( "0.00" );
            }
            else if( Config.RollingType == GaugeGCDRollingType.CastTime ) {
                Value = UiHelper.GetCastTime( out var timeElapsed, out var total );
                IndicatorValue = ( JobBars.Configuration.GaugeSlidecastTime > 0f && total > 0f ) ? JobBars.Configuration.GaugeSlidecastTime / total : 0f;
                TextValue = ( total - timeElapsed ).ToString( "0.00" );
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
