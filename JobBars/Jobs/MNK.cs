using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class MNK {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeGCDConfig(UIHelper.Localize(BuffIds.PerfectBalance), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 6,
                MaxDuration = 15,
                Color = UIColor.Orange,
                Triggers = new []{
                    new Item(BuffIds.PerfectBalance)
                }
            }),
            new GaugeGCDConfig(UIHelper.Localize(BuffIds.RiddleOfFire), GaugeVisualType.Bar, new GaugeSubGCDProps {
                MaxCounter = 11,
                MaxDuration = 20,
                Color = UIColor.Red,
                Triggers = new []{
                    new Item(BuffIds.RiddleOfFire)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.TwinSnakes), GaugeVisualType.Bar, new GaugeSubTimerProps
            {
                MaxDuration = 15,
                Color = UIColor.PurplePink,
                Triggers = new []{
                    new Item(BuffIds.TwinSnakes)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Demolish), GaugeVisualType.Bar, new GaugeSubTimerProps
            {
                MaxDuration = 18,
                Color = UIColor.Yellow,
                Triggers = new [] {
                    new Item(BuffIds.Demolish)
                }
            }),
            new GaugeChargesConfig($"{UIHelper.Localize(ActionIds.TrueNorth)}/{UIHelper.Localize(ActionIds.RiddleOfEarth)}", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = UIColor.LightBlue,
                Parts = new []{
                    new GaugesChargesPartProps {
                        Diamond = true,
                        MaxCharges = 3,
                        Color = UIColor.Yellow,
                        CD = 30,
                        Triggers = new []{
                            new Item(ActionIds.RiddleOfEarth)
                        }
                    },
                    new GaugesChargesPartProps {
                        Diamond = true,
                        MaxCharges = 2,
                        Color = UIColor.NoColor,
                        CD = 45,
                        Triggers = new []{
                            new Item(ActionIds.TrueNorth)
                        }
                    },
                    new GaugesChargesPartProps {
                        Bar = true,
                        Duration = 10,
                        Triggers = new []{
                            new Item(BuffIds.RiddleOfEarth)
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
            }),
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(ActionIds.Brotherhood), new BuffProps {
                CD = 90,
                Duration = 15,
                Icon = ActionIds.Brotherhood,
                Color = UIColor.Orange,
                Triggers = new []{ new Item(ActionIds.Brotherhood) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.RiddleOfFire), new BuffProps {
                CD = 90,
                Duration = 20,
                Icon = ActionIds.RiddleOfFire,
                Color = UIColor.Red,
                Triggers = new []{ new Item(ActionIds.RiddleOfFire) }
            }),
            new BuffConfig(UIHelper.Localize(BuffIds.PerfectBalance), new BuffProps {
                CD = 90,
                Duration = 15,
                Icon = ActionIds.PerfectBalance,
                Color = UIColor.Orange,
                Triggers = new []{ new Item(BuffIds.PerfectBalance) }
            })
        };

        public static Cursor Cursors => new(JobIds.MNK, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.MNK)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Feint) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Mantra), new CooldownProps {
                Icon = ActionIds.Mantra,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Mantra) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.RiddleOfFire), new IconProps {
                Icons = new [] { ActionIds.RiddleOfFire },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.RiddleOfFire), Duration = 20 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.TwinSnakes), new IconProps {
                IsTimer = true,
                Icons = new [] { ActionIds.TwinSnakes },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.TwinSnakes), Duration = 15 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.Demolish), new IconProps {
                IsTimer = true,
                Icons = new [] { ActionIds.Demolish },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Demolish), Duration = 18 }
                }
            })
        };
    }
}
