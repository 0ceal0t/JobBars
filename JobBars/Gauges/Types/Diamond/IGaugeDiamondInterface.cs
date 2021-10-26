using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Gauges.Types.Diamond {
    public interface IGaugeDiamondInterface {
        // ===== SETUP ======

        public int GetTotalMaxTicks();

        public int GetCurrentMaxTicks();

        public ElementColor[] GetDiamondColors();

        public bool GetDiamondTextVisible();

        // ===== TICK =======

        public bool[] GetDiamondValue();

        public string[] GetDiamondText();
    }
}
