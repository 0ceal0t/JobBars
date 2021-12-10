using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class SGE {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
        };

        public static BuffConfig[] Buffs => new BuffConfig[] {
        };

        public static Cursor Cursors => new(JobIds.SGE, CursorType.None, CursorType.CastTime);

        public static CooldownConfig[] Cooldowns => new CooldownConfig[] {
        };

        public static IconReplacer[] Icons => new IconReplacer[] {
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.24f };

        public static bool GCD_ROLL => false;
    }
}
