using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.Atk;
using System;

namespace JobBars.Jobs {
    public static class WAR {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.InnerRelease), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.InnerRelease)
                },
                Color = AtkColor.Orange
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.SurgingTempest), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 60,
                DefaultDuration = 30,
                Color = AtkColor.Red,
                Triggers = new []{
                    new Item(BuffIds.SurgingTempest)
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(AtkHelper.Localize(BuffIds.InnerRelease), new BuffProps {
                Duration = 15,
                CD = 60,
                Icon = ActionIds.InnerRelease,
                Color = AtkColor.Orange,
                Triggers = new []{ new Item(BuffIds.InnerRelease) }
            })
        };

        public static Cursor Cursors => new(JobIds.WAR, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig(AtkHelper.Localize(ActionIds.Holmgang), new CooldownProps {
                Icon = ActionIds.Holmgang,
                Duration = 10,
                CD = 240,
                Triggers = new []{ new Item(ActionIds.Holmgang) }
            }),
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Reprisal)} ({AtkHelper.Localize(JobIds.WAR)})", new CooldownProps {
                Icon = ActionIds.Reprisal,
                Duration = 10,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Reprisal) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.ShakeItOff), new CooldownProps {
                Icon = ActionIds.ShakeItOff,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.ShakeItOff) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.Bloodwhetting), new CooldownProps {
                Icon = ActionIds.Bloodwhetting,
                Duration = 6,
                CD = 25,
                Triggers = new []{
                    new Item(ActionIds.NascentFlash),
                    new Item(ActionIds.RawIntuition),
                    new Item(ActionIds.Bloodwhetting)
                }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.SurgingTempest), new IconBuffProps {
                IsTimer = true,
                Icons = new [] { ActionIds.StormsEye },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.SurgingTempest), Duration = 60 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
