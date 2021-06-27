using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars.Gauges {
    public struct GaugeChargesProps {
        public float CD;
        public int MaxCharges;
        public Item[] Triggers;
        public GaugeVisualType Type;
        public ElementColor Color;
    }

    public class GaugeCharges : Gauge {
        private GaugeChargesProps Props;

        public static GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.BarDiamondCombo, GaugeVisualType.Bar, GaugeVisualType.Diamond };

        public GaugeCharges(string name, GaugeChargesProps props) : base(name) {
            Props = props;
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
            SetValue(Props.MaxCharges, 0, 0);
        }

        public unsafe override void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict) {
            foreach(var trigger in Props.Triggers) {
                if (trigger.Type == ItemType.Buff) continue;
                if (!JobBars.GetRecast(trigger.Id, out var recastTimer)) continue;

                if(recastTimer->IsActive == 1) {
                    var currentCharges = (int)Math.Floor(recastTimer->Elapsed / Props.CD);
                    var currentTime = recastTimer->Elapsed % Props.CD;
                    var timeLeft = Props.CD - currentTime;

                    SetValue(currentCharges, currentTime, (int)Math.Round(timeLeft));
                    return;
                }
            }
            SetValue(Props.MaxCharges, 0, 0); // none triggered
        }

        public override void ProcessAction(Item action) { }

        private void SetValue(int diamondValue, float value, int textValue) {
            if (UI is UIGaugeDiamondCombo combo) {
                combo.SetDiamondValue(diamondValue);
                combo.SetText(textValue.ToString());
                combo.SetPercent((float)value / Props.CD);
            }
            else if (UI is UIGauge gauge) {
                gauge.SetText(textValue.ToString());
                gauge.SetPercent((float)value / Props.CD);
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
