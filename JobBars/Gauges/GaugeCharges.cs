using Dalamud.Plugin;
using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars.Gauges {
    public struct GaugesChargesPartProps {
        public Item[] Triggers;
        public float Duration;
        public float CD;
        public bool Bar;
        public bool Diamond;
        public int MaxCharges;
        public ElementColor Color;
    }

    public struct GaugeChargesProps {
        public GaugesChargesPartProps[] Parts;
        public GaugeVisualType Type;
        public ElementColor BarColor;
        public bool SameColor;
    }

    public class GaugeCharges : Gauge {
        private GaugeChargesProps Props;
        private int TotalDiamonds;

        public static GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.BarDiamondCombo, GaugeVisualType.Bar, GaugeVisualType.Diamond };

        public GaugeCharges(string name, GaugeChargesProps props) : base(name) {
            Props = props;
            Props.Type = Configuration.Config.GaugeTypeOverride.TryGetValue(Name, out var newType) ? newType : Props.Type;
            Props.BarColor = Configuration.Config.GetColorOverride(name, out var newColor) ? newColor : Props.BarColor;
            RefreshSameColor();

            TotalDiamonds = 0;
            foreach(var part in Props.Parts) {
                if(part.Diamond) {
                    TotalDiamonds += part.MaxCharges;
                }
            }
        }

        protected override void Setup() {
            if (UI is UIGaugeDiamondCombo combo) {
                combo.SetGaugeColor(Props.BarColor);
                SetupDiamondColors();
                combo.SetTextColor(NoColor);
                combo.SetMaxValue(TotalDiamonds);
            }
            else if (UI is UIDiamond diamond) {
                SetupDiamondColors();
                diamond.SetMaxValue(TotalDiamonds);
            }
            else if (UI is UIGauge gauge) {
                gauge.SetColor(Props.BarColor);
                gauge.SetTextColor(NoColor);
            }
            SetGaugeValue(0, 0);
            SetDiamondValue(0, 0, TotalDiamonds);
        }

        private void SetupDiamondColors() {
            int diamondIdx = 0;
            foreach(var part in Props.Parts) {
                if(part.Diamond) {
                    if (UI is UIGaugeDiamondCombo combo) {
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
            if (Props.SameColor) {
                for (int i = 0; i < Props.Parts.Length; i++) {
                    Props.Parts[i].Color = Props.BarColor;
                }
            }
        }

        public unsafe override void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict) {
            bool barAssigned = false;
            int diamondIdx = 0;
            foreach(var part in Props.Parts) {
                foreach (var trigger in part.Triggers) {
                    if (trigger.Type == ItemType.Buff) {
                        var buffExists = buffDict.TryGetValue(trigger, out var buff);
                        if (part.Bar && !barAssigned && buffExists) {
                            barAssigned = true;
                            SetGaugeValue(buff.Duration / part.Duration, (int)Math.Round(buff.Duration));
                        }
                        if (part.Diamond) {
                            SetDiamondValue(buffExists ? buff.StackCount : 0, diamondIdx, part.MaxCharges);
                            diamondIdx += part.MaxCharges;
                        }
                        if (buffExists) break;
                    }
                    else {
                        var foundRecast = JobBars.GetRecast(trigger.Id, out var recastTimer);
                        var recastActive = foundRecast && recastTimer->IsActive == 1;
                        if (part.Bar && !barAssigned && recastActive) {
                            barAssigned = true;
                            var currentTime = recastTimer->Elapsed % part.CD;
                            var timeLeft = part.CD - currentTime;
                            SetGaugeValue(currentTime / part.CD, (int)Math.Round(timeLeft));
                        }
                        if (part.Diamond) {
                            SetDiamondValue(recastActive ? (int)Math.Floor(recastTimer->Elapsed / part.CD) : part.MaxCharges, diamondIdx, part.MaxCharges);
                            diamondIdx += part.MaxCharges;
                        }
                        if (recastActive) break;
                    }
                }
            }
            if(!barAssigned) {
                SetGaugeValue(0, 0);
            }
        }

        public override void ProcessAction(Item action) { }

        private void SetDiamondValue(int value, int start, int max) {
            if (UI is UIGaugeDiamondCombo combo) {
                combo.SetDiamondValue(value, start, max);
            }
            else if (UI is UIDiamond diamond) {
                diamond.SetValue(value, start, max);
            }
        }

        private void SetGaugeValue(float value, int textValue) {
            if (UI is UIGaugeDiamondCombo combo) {
                combo.SetText(textValue.ToString());
                combo.SetPercent(value);
            }
            else if (UI is UIGauge gauge) {
                gauge.SetText(textValue.ToString());
                gauge.SetPercent(value);
            }
        }

        protected override int GetHeight() {
            return UI == null ? 0 : UI.GetHeight(0);
        }

        protected override int GetWidth() {
            return UI == null ? 0 : UI.GetWidth(0);
        }

        public override GaugeVisualType GetVisualType() {
            return Props.Type;
        }

        protected override void DrawGauge(string _ID, JobIds job) {
            // ======== COLOR =============
            if (DrawColorOptions(_ID, Name, Props.BarColor, out var newColor)) {
                Props.BarColor = newColor;
                if (job == GaugeManager.Manager.CurrentJob) {
                    UI?.SetColor(Props.BarColor);
                    RefreshSameColor();
                    SetupDiamondColors();
                }
            }
            // ======== TYPE =============
            if (DrawTypeOptions(_ID, ValidGaugeVisualType, Props.Type, out var newType)) {
                Props.Type = newType;
                GaugeManager.Manager.ResetJob(job);
            }
        }
    }
}
