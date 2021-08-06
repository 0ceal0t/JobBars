using Dalamud.Plugin;
using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Gauges {
    public class GaugeGCD : Gauge {
        public static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Arrow, GaugeVisualType.Bar, GaugeVisualType.Diamond };

        private readonly int MaxWidth;
        private GaugeVisualType Type;
        private readonly SubGaugeGCD[] SubGauges;
        public SubGaugeGCD ActiveSubGauge;

        public GaugeGCD(string name, GaugeVisualType type, SubGaugeGCDProps props) : this(name, type, new[] { props }) { }
        public GaugeGCD(string name, GaugeVisualType type, SubGaugeGCDProps[] props) : base(name) {
            Type = Configuration.Config.GaugeTypeOverride.TryGetValue(Name, out var newType) ? newType : type;
            SubGauges = new SubGaugeGCD[props.Length];
            for (int i = 0; i < props.Length; i++) {
                if (props[i].MaxCounter > MaxWidth) {
                    MaxWidth = props[i].MaxCounter;
                }
                string id = string.IsNullOrEmpty(props[i].SubName) ? Name : Name + "/" + props[i].SubName;
                SubGauges[i] = new SubGaugeGCD(id, this, props[i]);
            }
        }

        protected override void Setup() {
            foreach (var sg in SubGauges) {
                sg.Reset();
            }
            ActiveSubGauge = SubGauges[0];
            ActiveSubGauge.UseSubGauge();
            ActiveSubGauge.CheckInactive();
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

        protected override int GetHeight() {
            return UI == null ? 0 : UI.GetHeight(0);
        }

        protected override int GetWidth() {
            return UI == null ? 0 : UI.GetWidth(MaxWidth);
        }

        public override GaugeVisualType GetVisualType() {
            return Type;
        }

        protected override void DrawGauge(string _ID, JobIds job) {
            foreach (var sg in SubGauges) {
                sg.DrawSubGauge(_ID, job);
            }
            // ======== TYPE =============
            if (DrawTypeOptions(_ID, Name, ValidGaugeVisualType, Type, out var newType)) {
                Type = newType;
                GaugeManager.Manager.ResetJob(job);
            }
        }
    }
}
