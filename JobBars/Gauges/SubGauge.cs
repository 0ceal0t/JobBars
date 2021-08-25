using JobBars.Helper;
using JobBars.UI;
using System.Collections.Generic;

namespace JobBars.Gauges {
    public abstract class SubGauge<T> where T : Gauge {
        public readonly string Name;

        protected readonly T ParentGauge;
        protected UIGaugeElement UI => ParentGauge.UI;

        public SubGauge(string name, T parentGauge) {
            Name = name;
            ParentGauge = parentGauge;
        }

        public abstract void UseSubGauge();
        public abstract void Tick(Dictionary<Item, BuffElem> buffDict);
        public abstract void ProcessAction(Item action);
    }
}
