using JobBars.UI;
using JobBars.Gauges.Types.Bar;
using JobBars.Gauges.Types.Arrow;
using JobBars.Gauges.Types.Diamond;
using JobBars.Gauges.Types.BarDiamondCombo;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace JobBars.Gauges.Custom {
    public class GaugeDrkMpTracker : GaugeTracker, IGaugeBarInterface, IGaugeBarDiamondComboInterface, IGaugeArrowInterface, IGaugeDiamondInterface {
        private readonly GaugeDrkMpConfig Config;
        private float Value;
        private string TextValue;
        private bool DarkArts = false;

        public GaugeDrkMpTracker(GaugeDrkMpConfig config, int idx) {
            Config = config;
            LoadUI(Config.TypeConfig switch {
                GaugeBarConfig _ => new GaugeBar<GaugeDrkMpTracker>(this, idx),
                GaugeArrowConfig _ => new GaugeArrow<GaugeDrkMpTracker>(this, idx),
                GaugeDiamondConfig _ => new GaugeDiamond<GaugeDrkMpTracker>(this, idx),
                GaugeBarDiamondComboConfig _ => new GaugeBarDiamondCombo<GaugeDrkMpTracker>(this, idx),
                _ => null
            });
        }

        public override GaugeConfig GetConfig() => Config;

        public override bool GetActive() => true;

        public override void ProcessAction(Item action) { }

        protected override void TickTracker() {
            var mp = JobBars.ClientState.LocalPlayer.CurrentMp;
            Value = mp / 10000f;
            TextValue = ((int)(mp / 100)).ToString();

            var drkGauge = JobBars.JobGauges.Get<DRKGauge>();
            DarkArts = drkGauge != null && drkGauge.HasDarkArts;
        }

        public float[] GetBarSegments() => Config.Segments;

        public bool GetBarTextSwap() => Config.TypeConfig switch {
            GaugeBarConfig barConfig => barConfig.SwapText,
            _ => false
        };

        public bool GetBarTextVisible() => Config.TypeConfig switch {
            GaugeBarConfig barConfig => barConfig.ShowText,
            GaugeBarDiamondComboConfig comboConfig => comboConfig.ShowText,
            _ => false
        };

        public bool GetVertical() => Config.TypeConfig switch {
            GaugeBarConfig barConfig => barConfig.Vertical,
            _ => false
        };

        public ElementColor GetColor() => Config.Color;

        public bool GetBarDanger() => false;

        public string GetBarText() => TextValue;

        public float GetBarPercent() => Value;

        public int GetCurrentMaxTicks() => 1;

        public ElementColor GetTickColor(int idx) => Config.DarkArtsColor;

        public bool GetTickValue(int _) => DarkArts;

        public bool GetReverseFill() => false; // doesn't matter anyway, only 1 charge

        public int GetTotalMaxTicks() => 1;

        public bool GetDiamondTextVisible() => false;

        public string GetDiamondText(int idx) => "";
    }
}
