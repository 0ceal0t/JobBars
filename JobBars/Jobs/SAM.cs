using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class SAM {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.Meikyo), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.Meikyo)
                },
                Color = UIColor.BlueGreen
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Fugetsu), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 40,
                Color = UIColor.DarkBlue,
                Triggers = new []{
                    new Item(BuffIds.Fugetsu)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Fuka), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 40,
                Color = UIColor.Red,
                Triggers = new []{
                    new Item(BuffIds.Fuka)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Higanbana), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 60,
                Color = UIColor.Orange,
                Triggers = new []{
                    new Item(BuffIds.Higanbana)
                }
            }),
            new GaugeChargesConfig($"{UIHelper.Localize(ActionIds.TrueNorth)} ({UIHelper.Localize(JobIds.SAM)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = UIColor.NoColor,
                SameColor = true,
                Parts = new []{
                    new GaugesChargesPartProps {
                        Diamond = true,
                        MaxCharges = 2,
                        CD = 45,
                        Triggers = new []{
                            new Item(ActionIds.TrueNorth)
                        }
                    },
                    new GaugesChargesPartProps {
                        Bar = true,
                        Duration = 10,
                        Triggers = new []{
                            new Item(BuffIds.TrueNorth)
                        }
                    }
                },
                CompletionSound = GaugeCompleteSoundType.Never
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(ActionIds.OgiNamikiri), new BuffProps {
                CD = 120,
                Duration = 30,
                Icon = ActionIds.OgiNamikiri,
                Color = UIColor.Red,
                Triggers = new []{ new Item(BuffIds.OgiNamikiri) }
            })
        };

        public static Cursor Cursors => new(JobIds.SAM, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.SecondWind)} ({UIHelper.Localize(JobIds.SAM)})", new CooldownProps {
                Icon = ActionIds.SecondWind,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.SecondWind) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.LegSweep)} ({UIHelper.Localize(JobIds.SAM)})", new CooldownProps {
                Icon = ActionIds.LegSweep,
                Duration = 3,
                CD = 40,
                Triggers = new []{ new Item(ActionIds.LegSweep) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Bloodbath)} ({UIHelper.Localize(JobIds.SAM)})", new CooldownProps {
                Icon = ActionIds.Bloodbath,
                Duration = 20,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Bloodbath) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.SAM)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Feint) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.ArmsLength)} ({UIHelper.Localize(JobIds.SAM)})", new CooldownProps {
                Icon = ActionIds.ArmsLength,
                Duration = 6,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.ArmsLength) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.Fugetsu), new IconProps {
                IsTimer = true,
                Icons = new [] { ActionIds.Jinpu },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Fugetsu), Duration = 40 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.Fuka), new IconProps {
                IsTimer = true,
                Icons = new [] { ActionIds.Shifu },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Fuka), Duration = 40 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;

        public static bool GCD_ROLL => true;
    }
}
