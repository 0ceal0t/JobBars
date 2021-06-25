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
    public class GaugeStacks : Gauge {
        private int MaxStacks;

        public static GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Arrow, GaugeVisualType.Bar, GaugeVisualType.Diamond };

        public GaugeStacks(string name, int maxStacks) : base(name) {
            MaxStacks = maxStacks;
            DefaultVisual = Visual = new GaugeVisual
            {
                Type = GaugeVisualType.Diamond,
                Color = White
            };
        }

        public override void SetupVisual(bool resetValue = true) {
            UI?.SetColor(Visual.Color);
            if(resetValue) {
                if (UI is UIDiamond diamond) {
                    diamond.SetParts(MaxStacks);
                }
                else if (UI is UIArrow arrow) {
                    arrow.SetMaxValue(MaxStacks);
                }
                else if (UI is UIGauge gauge) {
                    gauge.SetTextColor(NoColor);
                }
                SetValue(0);
            }
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
                gauge.SetPercent(((float)value) / MaxStacks);
            }
        }

        public override void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict) {
            foreach(var trigger in Triggers) {
                if(buffDict.TryGetValue(trigger, out var elem)) {
                    SetValue(elem.StackCount);
                }
                else {
                    SetValue(0);
                }
            }
        }

        public override void ProcessAction(Item action) { }

        public override int GetHeight() {
            return UI == null ? 0 : UI.GetHeight(0);
        }

        public override int GetWidth() {
            return UI == null ? 0 : UI.GetWidth(MaxStacks);
        }

        // ===== BUILDER FUNCS =====
        public GaugeStacks WithTriggers(Item[] triggers) {
            Triggers = triggers;
            return this;
        }

        public GaugeStacks WithVisual(GaugeVisual visual) {
            DefaultVisual = Visual = visual;
            return this;
        }
    }
}
