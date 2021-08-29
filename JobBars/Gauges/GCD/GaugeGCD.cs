using JobBars.Data;

namespace JobBars.Gauges {
    public class GaugeGCD : Gauge {
        public static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Arrow, GaugeVisualType.Bar, GaugeVisualType.Diamond };

        private readonly int MaxWidth;
        private GaugeVisualType Type;

        public SubGaugeGCD ActiveSubGauge;
        private readonly SubGaugeGCD[] SubGauges;

        private bool IconEnabled = false;

        public GaugeGCD(string name, GaugeVisualType type, SubGaugeGCDProps props) : this(name, type, new[] { props }) { }
        public GaugeGCD(string name, GaugeVisualType type, SubGaugeGCDProps[] props) : base(name) {
            Type = JobBars.Config.GaugeType.Get(Name, type);

            SubGauges = new SubGaugeGCD[props.Length];
            for (int i = 0; i < props.Length; i++) {
                if (props[i].MaxCounter > MaxWidth) {
                    MaxWidth = props[i].MaxCounter;
                }
                string id = string.IsNullOrEmpty(props[i].SubName) ? Name : Name + "/" + props[i].SubName;
                SubGauges[i] = new SubGaugeGCD(id, this, props[i]);
            }
            RefreshIconEnabled();
        }

        public void RefreshIconEnabled() {
            foreach (var sg in SubGauges) {
                if (!sg.NoIcon) {
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
            foreach (var sg in SubGauges) sg.Tick();
        }

        public override void ProcessAction(Item action) {
            foreach (var sg in SubGauges) sg.ProcessAction(action);
        }

        public override bool CanProcessInput() => Enabled || IconEnabled;

        protected override int GetHeight() => UI.GetHeight(0);
        protected override int GetWidth() => UI.GetWidth(MaxWidth);
        public override GaugeVisualType GetVisualType() => Type;

        protected override void DrawGauge(string _ID, JobIds job) {
            if (JobBars.Config.GaugeType.Draw($"Type{_ID}", Name, ValidGaugeVisualType, Type, out var value)) {
                Type = value;
                JobBars.GaugeManager.ResetJob(job);
            }

            foreach (var sg in SubGauges) sg.Draw(_ID, job);
        }
    }
}
