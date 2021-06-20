using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Gauges {
    public class GaugeCharges : Gauge {
        private float CD;
        private int MaxCharges;

        public static GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.BarDiamondCombo, GaugeVisualType.Bar, GaugeVisualType.Diamond };

        public GaugeCharges(string name, float cd, int maxCharges) : base(name) {
            CD = cd;
            MaxCharges = maxCharges;
            DefaultVisual = Visual = new GaugeVisual
            {
                Type = GaugeVisualType.BarDiamondCombo,
                Color = UIColor.LightBlue
            };
        }

        public override void SetupVisual(bool resetValue = true) {
            UI?.SetColor(Visual.Color);
            if (resetValue) {
                if (UI is UIGaugeDiamondCombo combo) {
                    combo.SetMaxValue(MaxCharges);
                    combo.SetDiamondValue(MaxCharges);
                    combo.SetText("0");
                    combo.SetPercent(0);
                }
                else if(UI is UIGauge gauge) {
                    gauge.SetText("0");
                    gauge.SetPercent(0);
                }
                else if(UI is UIDiamond diamond) {
                    diamond.SetMaxValue(MaxCharges);
                    diamond.SetValue(MaxCharges);
                }
            }
        }

        public unsafe override void Tick(DateTime time, Dictionary<Item, float> buffDict) {
            foreach(var trigger in Triggers) {
                if (trigger.Type == ItemType.Buff) continue;

                var adjustedActionId = JobBars.Client.ActionManager.GetAdjustedActionId(trigger.Id);
                var recastGroup = (int)JobBars.Client.ActionManager.GetRecastGroup(0x01, adjustedActionId) + 1;
                if (recastGroup == 0 || recastGroup == 58) continue;
                var recastTimer = JobBars.Client.ActionManager.GetGroupRecastTime(recastGroup);

                if(recastTimer->IsActive == 1) {
                    var currentCharges = (int)Math.Floor(recastTimer->Elapsed / CD);
                    var currentTime = recastTimer->Elapsed % CD;
                    var timeLeft = CD - currentTime;

                    if (UI is UIGaugeDiamondCombo combo) {
                        combo.SetDiamondValue(currentCharges);
                        combo.SetText(((int)timeLeft).ToString());
                        combo.SetPercent((float)currentTime / CD);
                    }
                    else if(UI is UIGauge gauge) {
                        gauge.SetText(((int)timeLeft).ToString());
                        gauge.SetPercent((float)currentTime / CD);
                    }
                    else if(UI is UIDiamond diamond) {
                        diamond.SetValue(currentCharges);
                    }

                    return;
                }
            }

            if (UI is UIGaugeDiamondCombo comboInactive) {
                comboInactive.SetDiamondValue(MaxCharges);
                comboInactive.SetText("0");
                comboInactive.SetPercent(0);
            }
            else if (UI is UIGauge gaugeInactive) {
                gaugeInactive.SetText("0");
                gaugeInactive.SetPercent(0);
            }
            else if (UI is UIDiamond diamondInactive) {
                diamondInactive.SetValue(MaxCharges);
            }
        }

        public override void ProcessAction(Item action) { }

        public override int GetHeight() {
            return UI == null ? 0 : UI.GetHeight(0);
        }

        public override int GetWidth() {
            return UI == null ? 0 : UI.GetWidth(0);
        }

        // ===== BUILDER FUNCS =====
        public GaugeCharges WithTriggers(Item[] triggers) {
            Triggers = triggers;
            return this;
        }

        public GaugeCharges WithVisual(GaugeVisual visual) {
            DefaultVisual = Visual = visual;
            GetVisualConfig();
            return this;
        }
    }
}
