using Dalamud.Plugin;
using JobBars.Data;
using System;
using System.Collections.Generic;
namespace JobBars.Gauges {
    public class GaugeTimer : Gauge {
        private readonly SubGaugeTimer[] SubGauges;
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
            foreach (var sg in SubGauges) {
                if (!sg.NoIcon()) {
                    IconEnabled = true;
                    return;
                }
            }
            IconEnabled = false;
        }

        protected override void SetupUI() {
            foreach (var sg in SubGauges) sg.Reset();
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

        public override bool DoProcessInput() => Enabled || IconEnabled;

        protected override int GetHeight() => UI == null ? 0 : UI.GetHeight(0);

        protected override int GetWidth() => UI == null ? 0 : UI.GetWidth(0);

        public override GaugeVisualType GetVisualType() => GaugeVisualType.Bar;

        protected override void DrawGauge(string _ID, JobIds job) {
            foreach (var sg in SubGauges) {
                sg.DrawSubGauge(_ID, job);
            }
        }
    }
}
