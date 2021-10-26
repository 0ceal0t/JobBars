using System;
using System.Collections.Generic;
using System.Linq;

using JobBars.Helper;
using JobBars.UI;
using JobBars.Gauges.Types.Bar;
using JobBars.Gauges.Types.BarDiamondCombo;
using JobBars.Gauges.Types.Diamond;

namespace JobBars.Gauges.Charges {
    public class GaugeChargesTracker : GaugeTracker, IGaugeBarInterface, IGaugeDiamondInterface, IGaugeBarDiamondComboInterface {
        private readonly GaugeChargesConfig Config;
        private readonly int TotalCharges;
        private readonly List<bool> ChargesActive = new();

        private int ChargesActiveTotal = 0;
        private float BarTextValue = 0;
        private float BarPercentValue = 0;

        public GaugeChargesTracker(GaugeChargesConfig config, int idx) {
            Config = config;
            TotalCharges = Config.Parts.Where(p => p.Diamond).Select(d => d.MaxCharges).Sum();
            LoadUI(Config.TypeConfig switch {
                GaugeBarConfig _ => new GaugeBar<GaugeChargesTracker>(this, idx),
                GaugeDiamondConfig _ => new GaugeDiamond<GaugeChargesTracker>(this, idx),
                GaugeBarDiamondComboConfig _ => new GaugeBarDiamondCombo<GaugeChargesTracker>(this, idx),
                _ => null
            });
        }

        public override GaugeConfig GetConfig() => Config;

        public override bool GetActive() => true;

        public override void ProcessAction(Item action) { }

        protected override void TickTracker() {
            ChargesActive.Clear();
            var barAssigned = false;
            var currentChargesValue = 0;

            foreach (var part in Config.Parts) {
                foreach (var trigger in part.Triggers) {
                    if (trigger.Type == ItemType.Buff) {
                        var buffExists = UIHelper.PlayerStatus.TryGetValue(trigger, out var buff);
                        var buffValue = buffExists ? buff.StackCount : 0;

                        if (part.Bar && !barAssigned && buffExists) {
                            barAssigned = true;
                            BarPercentValue = buff.RemainingTime / part.Duration;
                            BarTextValue = buff.RemainingTime;
                        }
                        if (part.Diamond) {
                            currentChargesValue += buffValue;
                            AddToActive(buffValue, part.MaxCharges);
                        }
                        if (buffExists) break;
                    }
                    else {
                        var recastActive = UIHelper.GetRecastActive(trigger.Id, out var timeElapsed);
                        var actionValue = recastActive ? (int)Math.Floor(timeElapsed / part.CD) : part.MaxCharges;

                        if (part.Bar && !barAssigned && recastActive) {
                            barAssigned = true;
                            var currentTime = timeElapsed % part.CD;
                            var timeLeft = part.CD - currentTime;

                            BarPercentValue = currentTime / part.CD;
                            BarTextValue = timeLeft;
                        }
                        if (part.Diamond) {
                            currentChargesValue += actionValue;
                            AddToActive(actionValue, part.MaxCharges);
                        }
                        if (recastActive) break;
                    }
                }
                AddToActive(0, part.MaxCharges); // part is empty
            }
            if (!barAssigned) BarTextValue = BarPercentValue = 0;

            if (currentChargesValue != ChargesActiveTotal) {
                if (currentChargesValue == 0) {
                    if (Config.CompletionSound == GaugeCompleteSoundType.When_Empty || Config.CompletionSound == GaugeCompleteSoundType.When_Empty_or_Full)
                        UIHelper.PlaySeComplete();
                }
                else if (currentChargesValue == TotalCharges) {
                    if (Config.CompletionSound == GaugeCompleteSoundType.When_Full || Config.CompletionSound == GaugeCompleteSoundType.When_Empty_or_Full)
                        UIHelper.PlaySeComplete();
                }
                else if (Config.ProgressSound) UIHelper.PlaySeProgress();
            }
            ChargesActiveTotal = currentChargesValue;
        }

        private void AddToActive(int count, int max) {
            for (int i = 0; i < count; i++) ChargesActive.Add(true);
            for (int i = count; i < max; i++) ChargesActive.Add(false);
        }

        public float[] GetBarSegments() => null;

        public bool GetBarTextVisible() => Config.TypeConfig switch {
            GaugeBarConfig barConfig => barConfig.ShowText,
            GaugeBarDiamondComboConfig comboConfig => comboConfig.ShowText,
            _ => false
        };

        public bool GetBarTextSwap() => false;

        public ElementColor GetColor() => Config.BarColor;

        public bool GetBarDanger() => false;

        public string GetBarText() => $"{(int)Math.Round(BarTextValue)}";

        public float GetBarPercent() => BarPercentValue;

        public int GetCurrentMaxTicks() => TotalCharges;

        public int GetTotalMaxTicks() => TotalCharges;

        public ElementColor[] GetDiamondColors() {
            var ret = new List<ElementColor>();
            foreach (var part in Config.Parts) {
                for (int i = 0; i < part.MaxCharges; i++) ret.Add(Config.SameColor ? Config.BarColor : part.Color);
            }
            return ret.ToArray();
        }

        public bool GetDiamondTextVisible() => false;

        public bool[] GetDiamondValue() => ChargesActive.ToArray();

        public string[] GetDiamondText() => null;
    }
}
