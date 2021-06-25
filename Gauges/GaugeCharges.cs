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
                }
                else if(UI is UIDiamond diamond) {
                    diamond.SetMaxValue(MaxCharges);
                }
                else if (UI is UIGauge gauge) {
                    gauge.SetTextColor(UIColor.NoColor);
                }
                SetValue(MaxCharges, 0, 0);
            }
        }

        public unsafe override void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict) {
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

                    SetValue(currentCharges, currentTime, (int)timeLeft);
                    return;
                }
            }
            SetValue(MaxCharges, 0, 0); // none triggered
        }

        public override void ProcessAction(Item action) { }

        private void SetValue(int diamondValue, float value, int textValue) {
            if (UI is UIGaugeDiamondCombo combo) {
                combo.SetDiamondValue(diamondValue);
                combo.SetText(textValue.ToString());
                combo.SetPercent((float)value / CD);
            }
            else if (UI is UIGauge gauge) {
                gauge.SetText(textValue.ToString());
                gauge.SetPercent((float)value / CD);
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

        // ===== BUILDER FUNCS =====
        public GaugeCharges WithTriggers(Item[] triggers) {
            Triggers = triggers;
            return this;
        }

        public GaugeCharges WithVisual(GaugeVisual visual) {
            DefaultVisual = Visual = visual;
            return this;
        }
    }
}
