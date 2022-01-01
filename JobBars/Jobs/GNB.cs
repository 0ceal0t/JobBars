using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.GCD;

using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class GNB {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeGCDConfig(UIHelper.Localize(BuffIds.NoMercy), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 9,
                MaxDuration = 20,
                Color = UIColor.Orange,
                Triggers = new []{
                    new Item(BuffIds.NoMercy)
                }
            })
        };

        public static BuffConfig[] Buffs => Array.Empty<BuffConfig>();

        public static Cursor Cursors => new(JobIds.GNB, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Rampart)} ({UIHelper.Localize(JobIds.GNB)})", new CooldownProps {
                Icon = ActionIds.Rampart,
                Duration = 20,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Rampart) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.LowBlow)} ({UIHelper.Localize(JobIds.GNB)})", new CooldownProps {
                Icon = ActionIds.LowBlow,
                Duration = 0,
                CD = 25,
                Triggers = new []{ new Item(ActionIds.LowBlow) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Provoke)} ({UIHelper.Localize(JobIds.GNB)})", new CooldownProps {
                Icon = ActionIds.Provoke,
                Duration = 0,
                CD = 30,
                Triggers = new []{ new Item(ActionIds.Provoke) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Interject)} ({UIHelper.Localize(JobIds.GNB)})", new CooldownProps {
                Icon = ActionIds.Interject,
                Duration = 0,
                CD = 30,
                Triggers = new []{ new Item(ActionIds.Interject) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Reprisal)} ({UIHelper.Localize(JobIds.GNB)})", new CooldownProps {
                Icon = ActionIds.Reprisal,
                Duration = 10,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Reprisal) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.ArmsLength)} ({UIHelper.Localize(JobIds.GNB)})", new CooldownProps {
                Icon = ActionIds.ArmsLength,
                Duration = 6,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.ArmsLength) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Shirk)} ({UIHelper.Localize(JobIds.GNB)})", new CooldownProps {
                Icon = ActionIds.Shirk,
                Duration = 0,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Shirk) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Camouflage), new CooldownProps {
                Icon = ActionIds.Camouflage,
                Duration = 20,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Camouflage) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Nebula), new CooldownProps {
                Icon = ActionIds.Nebula,
                Duration = 15,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Nebula) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Superbolide), new CooldownProps {
                Icon = ActionIds.Superbolide,
                Duration = 10,
                CD = 360,
                Triggers = new []{ new Item(ActionIds.Superbolide) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.HeartOfLight), new CooldownProps {
                Icon = ActionIds.HeartOfLight,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.HeartOfLight) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.HeartofCorundum), new CooldownProps {
                Icon = ActionIds.HeartofCorundum,
                Duration = 7,
                CD = 30,
                Triggers = new []{
                    new Item(ActionIds.HeartofStone),
                    new Item(ActionIds.HeartofCorundum),
                }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.NoMercy), new IconProps {
                Icons = new [] { ActionIds.NoMercy },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.NoMercy), Duration = 20 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;

        public static bool GCD_ROLL => true;
    }
}
