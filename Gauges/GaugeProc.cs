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
    public class GaugeProc : Gauge {
        private Proc[] Procs;
        private int Size;

        public GaugeProc(string name) : base(name) {
            Procs = new Proc[0];
            DefaultVisual = Visual = new GaugeVisual
            {
                Type = GaugeVisualType.Diamond,
                Color = White // this doesn't matter
            };
        }

        public override void SetupVisual(bool resetValue = true) {
            if (UI is UIDiamond diamond) {
                diamond.SetParts(Size);
                foreach (var proc in Procs) {
                    diamond.SetColor(proc.Color, proc.Idx);
                }
            }

            if(resetValue) {
                foreach (var proc in Procs) {
                    SetValue(proc.Idx, false);
                }
            }
        }

        public override void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict) {
            foreach (var proc in Procs) {
                SetValue(proc.Idx, buffDict.ContainsKey(proc.Trigger));
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

        // ===== BUILDER FUNCS =====
        public GaugeProc WithProcs(Proc[] procs) {
            Procs = procs;
            for (int idx = 0; idx < Procs.Length; idx++) {
                Procs[idx].Idx = idx;
            }
            Size = Procs.Length;
            return this;
        }
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
    }
}
