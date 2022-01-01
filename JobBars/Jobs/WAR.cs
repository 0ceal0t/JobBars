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
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class WAR {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.InnerRelease), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.InnerRelease)
                },
                Color = UIColor.Orange
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.SurgingTempest), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 60,
                DefaultDuration = 30,
                Color = UIColor.Red,
                Triggers = new []{
                    new Item(BuffIds.SurgingTempest)
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(BuffIds.InnerRelease), new BuffProps {
                Duration = 15,
                CD = 60,
                Icon = ActionIds.InnerRelease,
                Color = UIColor.Orange,
                Triggers = new []{ new Item(BuffIds.InnerRelease) }
            })
        };

        public static Cursor Cursors => new(JobIds.WAR, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Rampart)} ({UIHelper.Localize(JobIds.WAR)})", new CooldownProps {
                Icon = ActionIds.Rampart,
                Duration = 20,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Rampart) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.LowBlow)} ({UIHelper.Localize(JobIds.WAR)})", new CooldownProps {
                Icon = ActionIds.LowBlow,
                CD = 25,
                Triggers = new []{ new Item(ActionIds.LowBlow) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Provoke)} ({UIHelper.Localize(JobIds.WAR)})", new CooldownProps {
                Icon = ActionIds.Provoke,
                CD = 30,
                Triggers = new []{ new Item(ActionIds.Provoke) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Interject)} ({UIHelper.Localize(JobIds.WAR)})", new CooldownProps {
                Icon = ActionIds.Interject,
                CD = 30,
                Triggers = new []{ new Item(ActionIds.Interject) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Reprisal)} ({UIHelper.Localize(JobIds.WAR)})", new CooldownProps {
                Icon = ActionIds.Reprisal,
                Duration = 10,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Reprisal) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.ArmsLength)} ({UIHelper.Localize(JobIds.WAR)})", new CooldownProps {
                Icon = ActionIds.ArmsLength,
                Duration = 6,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.ArmsLength) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Shirk)} ({UIHelper.Localize(JobIds.WAR)})", new CooldownProps {
                Icon = ActionIds.Shirk,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Shirk) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.ThrillOfBattle), new CooldownProps {
                Icon = ActionIds.ThrillOfBattle,
                Duration = 15,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.ThrillOfBattle) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Vengeance), new CooldownProps {
                Icon = ActionIds.Vengeance,
                Duration = 15,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Vengeance) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Holmgang), new CooldownProps {
                Icon = ActionIds.Holmgang,
                Duration = 10,
                CD = 240,
                Triggers = new []{ new Item(ActionIds.Holmgang) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.ShakeItOff), new CooldownProps {
                Icon = ActionIds.ShakeItOff,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.ShakeItOff) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Bloodwhetting), new CooldownProps {
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
            new IconReplacer(UIHelper.Localize(BuffIds.SurgingTempest), new IconProps {
                IsTimer = true,
                Icons = new [] { ActionIds.StormsEye },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.SurgingTempest), Duration = 60 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;

        public static bool GCD_ROLL => true;
    }
}
