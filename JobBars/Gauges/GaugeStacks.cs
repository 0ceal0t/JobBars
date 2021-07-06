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
    public struct GaugeStacksProps {
        public int MaxStacks;
        public Item[] Triggers;
        public GaugeVisualType Type;
        public ElementColor Color;
    }

    public class GaugeStacks : Gauge {
        private GaugeStacksProps Props;

        public static GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Arrow, GaugeVisualType.Bar, GaugeVisualType.Diamond };

        public GaugeStacks(string name, GaugeStacksProps props) : base(name) {
            Props = props;
            Props.Type = Configuration.Config.GaugeTypeOverride.TryGetValue(Name, out var newType) ? newType : Props.Type;
            Props.Color = Configuration.Config.GetColorOverride(name, out var newColor) ? newColor : Props.Color;
        }

        public override void Setup() {
            UI.SetColor(Props.Color);
            if (UI is UIDiamond diamond) {
                diamond.SetMaxValue(Props.MaxStacks);
            }
            else if (UI is UIArrow arrow) {
                arrow.SetMaxValue(Props.MaxStacks);
            }
            else if (UI is UIGauge gauge) {
                gauge.SetTextColor(NoColor);
            }
            SetValue(0);
        }

        private void SetValue(int value) {
            if (UI is UIArrow arrows) {
                arrows.SetValue(value);
            }
            else if (UI is UIDiamond diamond) {
                diamond.SetValue(value);
            }
            else if (UI is UIGauge gauge) {
                gauge.SetText(value.ToString());
                gauge.SetPercent(((float)value) / Props.MaxStacks);
            }
        }

        public override void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict) {
            foreach(var trigger in Props.Triggers) {
                SetValue(buffDict.TryGetValue(trigger, out var elem) ? elem.StackCount : 0);
            }
        }

        public override void ProcessAction(Item action) { }

        public override int GetHeight() {
            return UI == null ? 0 : UI.GetHeight(0);
        }

        public override int GetWidth() {
            return UI == null ? 0 : UI.GetWidth(Props.MaxStacks);
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
