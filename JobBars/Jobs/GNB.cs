using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.GCD;

using JobBars.Helper;
using JobBars.Icons;
using JobBars.Atk;
using System;

namespace JobBars.Jobs {
    public static class GNB {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeGCDConfig(AtkHelper.Localize(BuffIds.NoMercy), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 9,
                MaxDuration = 20,
                Color = AtkColor.Orange,
                Triggers = new []{
                    new Item(BuffIds.NoMercy)
                }
            })
        };

        public static BuffConfig[] Buffs => Array.Empty<BuffConfig>();

        public static Cursor Cursors => new(JobIds.GNB, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig(AtkHelper.Localize(ActionIds.Superbolide), new CooldownProps {
                Icon = ActionIds.Superbolide,
                Duration = 10,
                CD = 360,
                Triggers = new []{ new Item(ActionIds.Superbolide) }
            }),
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Reprisal)} ({AtkHelper.Localize(JobIds.GNB)})", new CooldownProps {
                Icon = ActionIds.Reprisal,
                Duration = 10,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Reprisal) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.HeartOfLight), new CooldownProps {
                Icon = ActionIds.HeartOfLight,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.HeartOfLight) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.NoMercy), new IconBuffProps {
                Icons = new [] { ActionIds.NoMercy },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.NoMercy), Duration = 20 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
