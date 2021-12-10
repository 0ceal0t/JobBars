using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.GCD;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class RPR {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
        };

        public static BuffConfig[] Buffs => new BuffConfig[] {
        };

        public static Cursor Cursors => new(JobIds.RPR, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new CooldownConfig[] {
        };

        public static IconReplacer[] Icons => new IconReplacer[] {
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;

        public static bool GCD_ROLL => true;
    }
}
