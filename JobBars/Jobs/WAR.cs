using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class WAR {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeGCDConfig(UIHelper.Localize(BuffIds.InnerRelease), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 5,
                MaxDuration = 10,
                Color = UIColor.Orange,
                Increment = new []{
                    new Item(ActionIds.FellCleave),
                    new Item(ActionIds.Decimate)
                },
                Triggers = new []{
                    new Item(BuffIds.InnerRelease)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.StormsEye), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 60,
                DefaultDuration = 30,
                Color = UIColor.Red,
                Triggers = new []{
                    new Item(BuffIds.StormsEye)
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(BuffIds.InnerRelease), new BuffProps {
                Duration = 10,
                CD = 90,
                Icon = ActionIds.InnerRelease,
                Color = UIColor.Orange,
                Triggers = new []{ new Item(BuffIds.InnerRelease) }
            })
        };

        public static Cursor Cursors => new(JobIds.WAR, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig(UIHelper.Localize(ActionIds.Holmgang), new CooldownProps {
                Icon = ActionIds.Holmgang,
                Duration = 8,
                CD = 240,
                Triggers = new []{ new Item(ActionIds.Holmgang) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Reprisal)} ({UIHelper.Localize(JobIds.WAR)})", new CooldownProps {
                Icon = ActionIds.Reprisal,
                Duration = 10,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Reprisal) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.ShakeItOff), new CooldownProps {
                Icon = ActionIds.ShakeItOff,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.ShakeItOff) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.NascentFlash), new CooldownProps {
                Icon = ActionIds.NascentFlash,
                Duration = 6,
                CD = 25,
                Triggers = new []{
                    new Item(ActionIds.NascentFlash),
                    new Item(ActionIds.RawIntuition)
                }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.InnerRelease), new IconProps {
                Icons = new [] { ActionIds.InnerRelease },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.InnerRelease), Duration = 10 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.StormsEye), new IconProps {
                IsTimer = true,
                Icons = new [] { ActionIds.StormsEye },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.StormsEye), Duration = 60 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;

        public static bool GCD_ROLL => true;
    }
}
