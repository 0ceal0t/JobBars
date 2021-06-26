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
        private Proc[] Procs;
        private int Size;

        public GaugeProc(string name, GaugeProcProps props) : base(name) {
            Procs = props.Procs;
            for (int idx = 0; idx < Procs.Length; idx++) {
                Procs[idx].Idx = idx;
            }
            Size = Procs.Length;
        }

        public override void Setup() {
            if (UI is UIDiamond diamond) {
                diamond.SetParts(Size);
                foreach (var proc in Procs) {
                    diamond.SetColor(proc.Color, proc.Idx);
                }
            }
            foreach (var proc in Procs) {
                SetValue(proc.Idx, false);
            }
        }

        public unsafe override void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict) {
            foreach (var proc in Procs) {
                if(proc.Trigger.Type == ItemType.Buff) {
                    SetValue(proc.Idx, buffDict.ContainsKey(proc.Trigger));
                }
                else {
                    SetValue(proc.Idx, JobBars.GetRecast(proc.Trigger.Id, out var timer) ? timer->IsActive != 1  : false);
                }
            }
        }

        private void SetValue(int idx, bool value) {
            if(UI is UIDiamond diamond) {
                if (value) {
                    diamond.SelectPart(idx);
                }
                else {
                    diamond.UnselectPart(idx);
                }
            }
        }

        public override void ProcessAction(Item action) { }

        public override int GetHeight() {
            return UI == null ? 0 : UI.GetHeight(0);
        }

        public override int GetWidth() {
            return UI == null ? 0 : UI.GetWidth(Size);
        }

        public override GaugeVisualType GetVisualType() {
            return GaugeVisualType.Diamond;
        }

        public override void DrawGauge(string _ID, JobIds job) {
        }
    }
}
