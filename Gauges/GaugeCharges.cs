using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;
using static JobBars.UI.UIColor;

namespace JobBars.Gauges {
    public struct GaugeChargesProps {
        public float CD;
        public int MaxCharges;
        public Item[] Triggers;
        public Item[] DiamondTriggers;
        public Dictionary<Item,float> CdDictionary;
        public GaugeVisualType Type;
        public ElementColor Color;
    }

    public class GaugeCharges : Gauge {
        private GaugeChargesProps Props;

        public static GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.BarDiamondCombo, GaugeVisualType.Bar, GaugeVisualType.Diamond };

        public GaugeCharges(string name, GaugeChargesProps props) : base(name) {
            Props = props;
            Props.CdDictionary ??= new Dictionary<Item, float>();
            Props.Type = Configuration.Config.GaugeTypeOverride.TryGetValue(Name, out var newType) ? newType : Props.Type;
            Props.Color = Configuration.Config.GetColorOverride(name, out var newColor) ? newColor : Props.Color;
        }

        public override void Setup() {
            UI.SetColor(Props.Color);
            if (UI is UIGaugeDiamondCombo combo) {
                combo.SetTextColor(NoColor);
                combo.SetMaxValue(Props.MaxCharges);
            }
            else if (UI is UIDiamond diamond) {
                diamond.SetMaxValue(Props.MaxCharges);
            }
            else if (UI is UIGauge gauge) {
                gauge.SetTextColor(NoColor);
            }
            SetValue(Props.MaxCharges, 0, 0x9999f, 0);

            foreach (var trigger in Props.Triggers)
            {
                if (!Props.CdDictionary.TryGetValue(trigger, out _))
                {
                    Props.CdDictionary.Add(trigger,Props.CD);
                }
            }
            foreach (var trigger in Props.DiamondTriggers)
            {
                if (!Props.CdDictionary.TryGetValue(trigger, out _))
                {
                    Props.CdDictionary.Add(trigger,Props.CD);
                }
            }
        }

        public override unsafe void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict)
        {
            var diamondCount = 0;
            foreach (var trigger in Props.DiamondTriggers)
            {
                if (trigger.Type == ItemType.Buff)
                {
                    if (buffDict.TryGetValue(trigger, out var buffElem))
                    {
                        if (buffElem.Duration > 0)
                        {
                            diamondCount = buffElem.StackCount;
                            if (diamondCount == 0) diamondCount = 1;
                        }
                    }
                }
                else
                {
                    if (!JobBars.GetRecast(trigger.Id, out var recastTimer)) continue;
                    if(recastTimer->IsActive == 1) {

                        var currentCharges = (int)Math.Floor(recastTimer->Elapsed / Props.CD);
                        if (currentCharges > 0) diamondCount = currentCharges;
                    }
                    else
                    {
                        diamondCount = Props.MaxCharges;
                    }
                }
            }

            Item item = new();
            float shortest = 0x9999f; 
            foreach(var trigger in Props.Triggers) {
                if (trigger.Type == ItemType.Buff)
                {
                    if (!Props.CdDictionary.TryGetValue(trigger, out var cd)) continue;
                    if (!buffDict.TryGetValue(trigger, out var buffElem)) continue;
                    var timeLeft = buffElem.Duration;
                    if (timeLeft <= 0) continue;
                    if (timeLeft < shortest)
                    {
                        item = trigger;
                        shortest = timeLeft;
                    }
                }
                else
                {
                    if (!JobBars.GetRecast(trigger.Id, out var recastTimer)) continue;
                    if(recastTimer->IsActive == 1)
                    {
                        if (!Props.CdDictionary.TryGetValue(trigger, out var cd)) continue;
                        var currentTime = recastTimer->Elapsed % cd;
                        var timeLeft = cd - currentTime;
                        if (timeLeft < shortest)
                        {
                            item = trigger;
                            shortest = timeLeft;
                        }
                    }
                }
            }

            if (shortest < 0x9990f)
            {
                Props.CdDictionary.TryGetValue(item, out var cd);
                SetValue(diamondCount, shortest, cd, shortest);
                return;
            }
            SetValue(diamondCount, 0, 0x9999f, 0); // none triggered
        }

        public override void ProcessAction(Item action) { }

        private void SetValue(int diamondValue, float value, float cd , float textValue) {
            if (UI is UIGaugeDiamondCombo combo) {
                combo.SetDiamondValue(diamondValue);
                combo.SetText(textValue.ToString("0.0"));
                combo.SetPercent((float)value / cd);
            }
            else if (UI is UIGauge gauge) {
                gauge.SetText(textValue.ToString("0.0"));
                gauge.SetPercent((float)value / cd);
            }
            else if (UI is UIDiamond diamond) {
                diamond.SetValue(diamondValue);
            }
        }

        public override int GetHeight() {
            return UI == null ? 0 : UI.GetHeight(0);
        }

        public override int GetWidth() {
            return UI == null ? 0 : UI.GetWidth(0);
        }

        public override GaugeVisualType GetVisualType() {
            return Props.Type;
        }

        public override void DrawGauge(string _ID, JobIds job) {
            // ======== COLOR =============
            if (DrawColorOptions(_ID, Name, Props.Color, out var newColor)) {
                Props.Color = newColor;
                if (job == GaugeManager.Manager.CurrentJob) {
                    UI?.SetColor(Props.Color);
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
