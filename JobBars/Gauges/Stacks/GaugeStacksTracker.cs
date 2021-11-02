using JobBars.Helper;
using JobBars.UI;
using JobBars.Gauges.Types.Arrow;
using JobBars.Gauges.Types.Bar;
using JobBars.Gauges.Types.Diamond;

namespace JobBars.Gauges.Stacks {
    public class GaugeStacksTracker : GaugeTracker, IGaugeBarInterface, IGaugeArrowInterface, IGaugeDiamondInterface {
        private readonly GaugeStacksConfig Config;
        private int Value = 0;

        public GaugeStacksTracker(GaugeStacksConfig config, int idx) {
            Config = config;
            LoadUI(Config.TypeConfig switch {
                GaugeBarConfig _ => new GaugeBar<GaugeStacksTracker>(this, idx),
                GaugeArrowConfig _ => new GaugeArrow<GaugeStacksTracker>(this, idx),
                GaugeDiamondConfig _ => new GaugeDiamond<GaugeStacksTracker>(this, idx),
                _ => null
            });
        }

        public override GaugeConfig GetConfig() => Config;

        public override bool GetActive() => Value > 0;

        public override void ProcessAction(Item action) { }

        protected override void TickTracker() {
            int currentValue = 0;
            foreach (var trigger in Config.Triggers) {
                var value = UIHelper.PlayerStatus.TryGetValue(trigger, out var elem) ? elem.StackCount : 0;
                currentValue = value > currentValue ? value : currentValue;
            }

            if (currentValue != Value) {
                if (currentValue == 0) {
                    if (Config.CompletionSound == GaugeCompleteSoundType.When_Empty || Config.CompletionSound == GaugeCompleteSoundType.When_Empty_or_Full)
                        UIHelper.PlaySeComplete();
                }
                else if (currentValue == Config.MaxStacks) {
                    if (Config.CompletionSound == GaugeCompleteSoundType.When_Full || Config.CompletionSound == GaugeCompleteSoundType.When_Empty_or_Full)
                        UIHelper.PlaySeComplete();
                }
                else {
                    if (Config.ProgressSound) UIHelper.PlaySeProgress();
                }
            }
            Value = currentValue;
        }

        public float GetBarPercent() => ((float)Value) / Config.MaxStacks;

        public string GetBarText() => Value.ToString();

        public int GetCurrentMaxTicks() => Config.MaxStacks;

        public int GetTotalMaxTicks() => Config.MaxStacks;

        public bool GetTickValue(int idx) => idx < Value;

        public bool GetBarDanger() => false;

        public string GetDiamondText(int idx) => "";

        public float[] GetBarSegments() => null;

        public ElementColor GetColor() => Config.Color;

        public ElementColor GetTickColor(int _) => Config.Color;

        public bool GetDiamondTextVisible() => false;

        public bool GetBarTextSwap() => Config.TypeConfig switch {
            GaugeBarConfig barConfig => barConfig.SwapText,
            _ => false
        };

        public bool GetBarTextVisible() => Config.TypeConfig switch {
            GaugeBarConfig barConfig => barConfig.ShowText,
            _ => false
        };

        public bool GetReverseFill() => Config.ReverseFill;
    }
}
