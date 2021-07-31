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
    public struct GaugeProcProps {
        public bool ShowText;
        public Proc[] Procs;
    }

    public struct Proc {
        public Item Trigger;
        public ElementColor Color;
        public int Idx;

        public Proc(BuffIds buff, ElementColor color) {
            Trigger = new Item(buff);
            Color = color;
            Idx = 0;
        }
        public Proc(ActionIds action, ElementColor color) {
            Trigger = new Item(action);
            Color = color;
            Idx = 0;
        }
    }

    public class GaugeProc : Gauge {
        private GaugeProcProps Props;
        private readonly int Size;

        public GaugeProc(string name, GaugeProcProps props) : base(name) {
            Props = props;
            for (int idx = 0; idx < Props.Procs.Length; idx++) {
                Props.Procs[idx].Idx = idx;
            }
            Size = Props.Procs.Length;
        }

        protected override void Setup() {
            if (UI is UIDiamond diamond) {
                diamond.SetMaxValue(Size, showText: Props.ShowText);
                foreach (var proc in Props.Procs) {
                    diamond.SetColor(proc.Color, proc.Idx);
                }
            }
            foreach (var proc in Props.Procs) {
                SetValue(proc.Idx, false);
            }
        }

        public unsafe override void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict) {
            foreach (var proc in Props.Procs) {
                if (proc.Trigger.Type == ItemType.Buff) {
                    SetValue(proc.Idx, buffDict.TryGetValue(proc.Trigger, out var buff), buff.Duration);
                }
                else {
                    SetValue(proc.Idx, JobBars.GetRecast(proc.Trigger.Id, out var timer) && timer->IsActive != 1);
                }
            }
        }

        private void SetValue(int idx, bool value, float duration = 0) {
            if (UI is UIDiamond diamond) {
                if (value) {
                    diamond.SelectPart(idx);
                    if (Props.ShowText) {
                        diamond.SetText(idx, ((int)Math.Round(duration)).ToString());
                    }
                }
                else {
                    diamond.UnselectPart(idx);
                }
            }
        }

        public override void ProcessAction(Item action) { }

        protected override int GetHeight() {
            return UI == null ? 0 : UI.GetHeight(0);
        }

        protected override int GetWidth() {
            return UI == null ? 0 : UI.GetWidth(Size);
        }

        public override GaugeVisualType GetVisualType() {
            return GaugeVisualType.Diamond;
        }

        protected override void DrawGauge(string _ID, JobIds job) {
        }
    }
}
