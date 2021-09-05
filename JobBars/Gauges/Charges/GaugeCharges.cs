using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;

namespace JobBars.Gauges {
    public struct GaugeChargesProps {
        public GaugesChargesPartProps[] Parts;
        public GaugeVisualType Type;
        public ElementColor BarColor;
        public bool SameColor;
        public bool NoSoundOnFull;
    }

    public struct GaugesChargesPartProps {
        public Item[] Triggers;
        public float Duration;
        public float CD;
        public bool Bar;
        public bool Diamond;
        public int MaxCharges;
        public ElementColor Color;
    }

    public class GaugeCharges : Gauge {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.BarDiamondCombo, GaugeVisualType.Bar, GaugeVisualType.Diamond };

        private GaugeChargesProps Props;
        private readonly int TotalDiamonds = 0;
        private bool GaugeFull = true;

        public GaugeCharges(string name, GaugeChargesProps props) : base(name) {
            Props = props;
            Props.Type = JobBars.Config.GaugeType.Get(Name, Props.Type);
            Props.NoSoundOnFull = JobBars.Config.GaugeNoSoundOnFull.Get(Name, Props.NoSoundOnFull);
            Props.BarColor = JobBars.Config.GaugeColor.Get(Name, Props.BarColor);

            RefreshSameColor();
            foreach (var part in Props.Parts) {
                if (part.Diamond) {
                    TotalDiamonds += part.MaxCharges;
                }
            }
        }

        protected override void LoadUI_() {
            if (UI is UIBarDiamondCombo combo) {
                combo.ClearSegments();
                combo.SetTextColor(UIColor.NoColor);
                combo.SetMaxValue(TotalDiamonds);
            }
            else if (UI is UIDiamond diamond) {
                diamond.SetMaxValue(TotalDiamonds);
            }
            else if (UI is UIBar gauge) {
                gauge.ClearSegments();
                gauge.SetTextColor(UIColor.NoColor);
            }

            GaugeFull = true;
            SetGaugeValue(0, 0);
            SetDiamondValue(0, 0, TotalDiamonds);
        }

        protected override void ApplyUIConfig_() {
            RefreshSameColor();
            if (UI is UIBarDiamondCombo combo) {
                SetupDiamondColors();
                combo.SetGaugeColor(Props.BarColor);
                combo.SetBarTextVisible(ShowText);
            }
            else if (UI is UIDiamond diamond) {
                SetupDiamondColors();
                diamond.SetTextVisible(false);
            }
            else if (UI is UIBar gauge) {
                gauge.SetColor(Props.BarColor);
                gauge.SetTextVisible(ShowText);
            }
        }

        private void SetupDiamondColors() {
            int diamondIdx = 0;
            foreach (var part in Props.Parts) {
                if (part.Diamond) {
                    if (UI is UIBarDiamondCombo combo) {
                        combo.SetDiamondColor(part.Color, diamondIdx, part.MaxCharges);
                    }
                    else if (UI is UIDiamond diamond) {
                        diamond.SetColor(part.Color, diamondIdx, part.MaxCharges);
                    }
                    diamondIdx += part.MaxCharges;
                }
            }
        }

        private void RefreshSameColor() {
            if (!Props.SameColor) return;
            for (int i = 0; i < Props.Parts.Length; i++) {
                Props.Parts[i].Color = Props.BarColor;
            }
        }

        public unsafe override void Tick() {
            bool barAssigned = false;
            int diamondIdx = 0;
            foreach (var part in Props.Parts) {
                foreach (var trigger in part.Triggers) {
                    if (trigger.Type == ItemType.Buff) {
                        var buffExists = UIHelper.PlayerStatus.TryGetValue(trigger, out var buff);

                        if (part.Bar && !barAssigned && buffExists) {
                            barAssigned = true;
                            SetGaugeValue(buff.RemainingTime / part.Duration, (int)Math.Round(buff.RemainingTime));
                        }
                        if (part.Diamond) {
                            SetDiamondValue(buffExists ? buff.StackCount : 0, diamondIdx, part.MaxCharges);
                            diamondIdx += part.MaxCharges;
                        }
                        if (buffExists) break;
                    }
                    else {
                        var recastActive = UIHelper.GetRecastActive(trigger.Id, out var timeElapsed);

                        if (part.Bar && !barAssigned && recastActive) {
                            barAssigned = true;
                            var currentTime = timeElapsed % part.CD;
                            var timeLeft = part.CD - currentTime;
                            SetGaugeValue(currentTime / part.CD, (int)Math.Round(timeLeft));
                        }
                        if (part.Diamond) {
                            SetDiamondValue(recastActive ? (int)Math.Floor(timeElapsed / part.CD) : part.MaxCharges, diamondIdx, part.MaxCharges);
                            diamondIdx += part.MaxCharges;
                        }
                        if (recastActive) break;
                    }
                }
            }
            if (!barAssigned) {
                SetGaugeValue(0, 0);
                if (!GaugeFull && !Props.NoSoundOnFull) UIHelper.PlaySeComplete(); // play the sound effect when full charges
            }
            GaugeFull = !barAssigned;
        }

        public override void ProcessAction(Item action) { }

        private void SetDiamondValue(int value, int start, int max) {
            if (UI is UIBarDiamondCombo combo) {
                combo.SetDiamondValue(value, start, max);
            }
            else if (UI is UIDiamond diamond) {
                diamond.SetValue(value, start, max);
            }
        }

        private void SetGaugeValue(float value, int textValue) {
            if (UI is UIBarDiamondCombo combo) {
                combo.SetText(textValue.ToString());
                combo.SetPercent(value);
            }
            else if (UI is UIBar gauge) {
                gauge.SetText(textValue.ToString());
                gauge.SetPercent(value);
            }
        }

        protected override int GetHeight() => UI.GetHeight(0);
        protected override int GetWidth() => UI.GetWidth(0);
        public override GaugeVisualType GetVisualType() => Props.Type;

        protected override void DrawGauge(string _ID, JobIds job) {
            if (JobBars.Config.GaugeType.Draw($"Type{_ID}", Name, ValidGaugeVisualType, Props.Type, out var newType)) {
                Props.Type = newType;
                JobBars.GaugeManager.ResetJob(job);
            }

            if (JobBars.Config.GaugeColor.Draw($"Color{_ID}", Name, Props.BarColor, out var newColor)) {
                Props.BarColor = newColor;
                ApplyUIConfig();
            }

            if (JobBars.Config.GaugeNoSoundOnFull.Draw($"Don't Play Sound When Full{_ID}", Name, Props.NoSoundOnFull, out var newSound)) {
                Props.NoSoundOnFull = newSound;
            }
        }
    }
}
