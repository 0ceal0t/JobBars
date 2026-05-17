using JobBars.Atk;
using JobBars.Gauges.Types.Bar;

namespace JobBars.Gauges.MP {
    public class GaugeMPTracker : GaugeTracker, IGaugeBarInterface {
        protected readonly GaugeMpConfig Config;

        protected float Value;
        protected string TextValue;

        public GaugeMPTracker( GaugeMpConfig config, bool noInit = false ) {
            Config = config;
            if( noInit ) return;
        }

        public override GaugeConfig GetConfig() => Config;

        public override bool GetActive() => Value < 1f;

        public override void ProcessAction( Item action ) { }

        public override void TickTracker() {
            var mp = Dalamud.Objects.LocalPlayer?.CurrentMp ?? 0;
            Value = mp / 10000f;
            TextValue = ( ( int )( mp / 100 ) ).ToString();
        }

        public virtual float[] GetBarSegments() => Config.ShowSegments ? Config.Segments : null;

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

        public float GetBarIndicatorPercent() => 0;
    }
}
