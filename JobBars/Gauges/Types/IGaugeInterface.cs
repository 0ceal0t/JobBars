using System;
using System.Collections.Generic;
using System.Text;

namespace JobBars.Gauges.Types {
    public interface IGaugeInterface {
        public GaugeConfig GetConfig();

        public bool GetActive();
    }
}
