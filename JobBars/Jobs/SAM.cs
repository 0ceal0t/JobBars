using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class SAM {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Jinpu), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 40,
                Color = UIColor.DarkBlue,
                Triggers = new []{
                    new Item(BuffIds.Jinpu)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Shifu), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 40,
                Color = UIColor.Red,
                Triggers = new []{
                    new Item(BuffIds.Shifu)
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
            new BuffConfig(UIHelper.Localize(ActionIds.DoubleMidare), new BuffProps {
                CD = 60,
                Duration = 5,
                Icon = ActionIds.DoubleMidare,
                Color = UIColor.DarkBlue,
                Triggers = new []{ new Item(ActionIds.DoubleMidare) }
            })
        };

        public static Cursor Cursors => new(JobIds.SAM, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.SAM)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Feint) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.Jinpu), new IconProps {
                IsTimer = true,
                Icons = new [] { ActionIds.Jinpu },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Jinpu), Duration = 40 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.Shifu), new IconProps {
                IsTimer = true,
                Icons = new [] { ActionIds.Shifu },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Shifu), Duration = 40 }
                }
            })
        };
    }
}
