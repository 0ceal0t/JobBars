using JobBars.Data;

namespace JobBars.Gauges {
    public class GaugeTimer : Gauge {
        public SubGaugeTimer ActiveSubGauge;

        private readonly SubGaugeTimer[] SubGauges;
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

        protected override void LoadUI_() {
            foreach (var sg in SubGauges) sg.Reset();
            ActiveSubGauge = SubGauges[0];
        }

        protected override void ApplyUIConfig_() {
            ActiveSubGauge.ApplySubGauge();
        }

        public override void Tick() {
            foreach (var sg in SubGauges) {
                sg.Tick();
            }
        }

        public override void ProcessAction(Item action) {
            foreach (var sg in SubGauges) {
                sg.ProcessAction(action);
            }
        }

        public override bool CanProcessInput() => Enabled || IconEnabled;

        protected override int GetHeight() => UI.GetHeight(0);
        protected override int GetWidth() => UI.GetWidth(0);
        public override GaugeVisualType GetVisualType() => GaugeVisualType.Bar;

        protected override void DrawGauge(string _ID, JobIds job) {
            foreach (var sg in SubGauges) {
                sg.DrawSubGauge(_ID, job);
            }
        }
    }
}
