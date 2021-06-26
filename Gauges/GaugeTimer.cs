using Dalamud.Plugin;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars.Gauges {
    public class GaugeTimer : Gauge {
        private SubGaugeTimer[] SubGauges;
        public SubGaugeTimer ActiveSubGauge;

        private bool IconEnabled = false;

        public GaugeTimer(string name, SubGaugeTimerProps props) : this(name, new[] { props }) { }
        public GaugeTimer(string name, SubGaugeTimerProps[] props) : base(name) {
            SubGauges = new SubGaugeTimer[props.Length];
            for (int i = 0; i < props.Length; i++) {
                string id = string.IsNullOrEmpty(props[i].SubName) ? Name : Name + "/" + props[i].SubName;
                SubGauges[i] = new SubGaugeTimer(id, this, props[i]);
            }
            RefreshIconEnabled();
        }

        public void RefreshIconEnabled() {
            foreach(var sg in SubGauges) {
                if (!sg.NoIcon()) {
                    IconEnabled = true;
                    return;
                }
            }
            IconEnabled = false;
        }

        public override void Setup() {
            foreach (var sg in SubGauges) {
                sg.Reset();
            }
            ActiveSubGauge = SubGauges[0];
            ActiveSubGauge.UseSubGauge();
        }

        public override void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict) {
            foreach (var sg in SubGauges) {
                sg.Tick(time, buffDict);
            }
        }

        public override void ProcessAction(Item action) {
            foreach (var sg in SubGauges) {
                sg.ProcessAction(action);
            }
        }

        public override bool DoProcessInput() {
            return Enabled|| IconEnabled;
        }

        public override int GetHeight() {
            return UI == null ? 0 : UI.GetHeight(0);
        }

        public override int GetWidth() {
            return UI == null ? 0 : UI.GetWidth(0);
        }

        public override GaugeVisualType GetVisualType() {
            return GaugeVisualType.Bar;
        }

        public override void DrawGauge(string _ID, JobIds job) {
            foreach (var sg in SubGauges) {
                sg.DrawSubGauge(_ID, job);
            }
        }
    }
}
