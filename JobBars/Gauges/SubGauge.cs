using JobBars.Data;
using JobBars.UI;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Gauges {
    public abstract class SubGauge<T> where T : Gauge {
        public readonly string Name;

        protected readonly T ParentGauge;
        protected UIGaugeElement UI => ParentGauge.UI;

        public SubGauge(string name, T parentGauge) {
            Name = name;
            ParentGauge = parentGauge;
        }

        public abstract void Reset();

        public abstract void ApplySubGauge();
        public abstract void Tick();
        public abstract void ProcessAction(Item action);
    }
}
