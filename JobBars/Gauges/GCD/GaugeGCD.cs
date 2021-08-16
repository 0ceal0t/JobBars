using Dalamud.Plugin;
using JobBars.Data;
using System;
using System.Collections.Generic;

namespace JobBars.Gauges {
    public class GaugeGCD : Gauge {
        public static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Arrow, GaugeVisualType.Bar, GaugeVisualType.Diamond };

        private readonly int MaxWidth;
        private GaugeVisualType Type;
        private readonly SubGaugeGCD[] SubGauges;
        public SubGaugeGCD ActiveSubGauge;

        public GaugeGCD(string name, GaugeVisualType type, SubGaugeGCDProps props) : this(name, type, new[] { props }) { }
        public GaugeGCD(string name, GaugeVisualType type, SubGaugeGCDProps[] props) : base(name) {
            Type = Config.Type != null ? Config.Type.Value : type;

            SubGauges = new SubGaugeGCD[props.Length];
            for (int i = 0; i < props.Length; i++) {
                if (props[i].MaxCounter > MaxWidth) {
                    MaxWidth = props[i].MaxCounter;
                }
                string id = string.IsNullOrEmpty(props[i].SubName) ? Name : Name + "/" + props[i].SubName;
                SubGauges[i] = new SubGaugeGCD(id, this, props[i]);
            }
        }

        protected override void LoadUI_Impl() {
            foreach (var sg in SubGauges) sg.Reset();
            ActiveSubGauge = SubGauges[0];
            ActiveSubGauge.UseSubGauge();
        }

        protected override void RefreshUI_Impl() {
            ActiveSubGauge.UseSubGauge();
        }

        public override void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict) {
            foreach (var sg in SubGauges) sg.Tick(time, buffDict);
        }

        public override void ProcessAction(Item action) {
            foreach (var sg in SubGauges) sg.ProcessAction(action);
        }

        protected override int GetHeight() => UI == null ? 0 : UI.GetHeight(0);
        protected override int GetWidth() => UI == null ? 0 : UI.GetWidth(MaxWidth);
        public override GaugeVisualType GetVisualType() => Type;

        protected override void DrawGauge(string _ID, JobIds job) {
            foreach (var sg in SubGauges) sg.DrawSubGauge(_ID, job);

            if (DrawTypeOptions(_ID, ValidGaugeVisualType, Type, out var newType)) {
                Config.Type = Type = newType;
                Configuration.Config.Save();
                GaugeManager.Manager.ResetJob(job);
            }
        }
    }
}
