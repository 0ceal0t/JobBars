using JobBars.Data;

namespace JobBars.Gauges {
    public class SubGauge {
        protected SubGaugeConfig Config;

        public string Name;

        public SubGauge(string name) {
            Name = name;
            Config = Configuration.Config.GetSubGaugeConfig(name);
        }
    }
}
