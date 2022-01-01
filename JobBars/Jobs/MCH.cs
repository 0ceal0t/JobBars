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
    public static class MCH {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeGCDConfig(UIHelper.Localize(ActionIds.Hypercharge), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 5,
                MaxDuration = 9,
                Color = UIColor.Orange,
                Increment = new []{
                    new Item(ActionIds.AutoCrossbow),
                    new Item(ActionIds.HeatBlast)
                },
                Triggers = new []{
                    new Item(ActionIds.Hypercharge)
                }
            }),
            new GaugeGCDConfig(UIHelper.Localize(BuffIds.Wildfire), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 6,
                MaxDuration = 10,
                Color = UIColor.Red,
                Triggers = new []{
                    new Item(BuffIds.Wildfire)
                }
            }),
            new GaugeChargesConfig(UIHelper.Localize(ActionIds.GaussRound), GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = UIColor.Red,
                SameColor = true,
                Parts = new []{
                    new GaugesChargesPartProps {
                        Bar = true,
                        Diamond = true,
                        CD = 30,
                        MaxCharges = 3,
                        Triggers = new[] { new Item(ActionIds.GaussRound) }
                    }
                }
            }),
            new GaugeChargesConfig(UIHelper.Localize(ActionIds.Ricochet), GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = UIColor.LightBlue,
                SameColor = true,
                Parts = new []{
                    new GaugesChargesPartProps {
                        Bar = true,
                        Diamond = true,
                        CD = 30,
                        MaxCharges = 3,
                        Triggers = new[] { new Item(ActionIds.Ricochet) }
                    }
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(ActionIds.Wildfire), new BuffProps {
                CD = 120,
                Duration = 10,
                Icon = ActionIds.Wildfire,
                Color = UIColor.Orange,
                Triggers = new []{ new Item(ActionIds.Wildfire) }
            })
        };

        public static Cursor Cursors => new(JobIds.MCH, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.LegGraze)} ({UIHelper.Localize(JobIds.MCH)})", new CooldownProps {
                Icon = ActionIds.LegGraze,
                Duration = 10,
                CD = 30,
                Triggers = new []{ new Item(ActionIds.LegGraze) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.SecondWind)} ({UIHelper.Localize(JobIds.MCH)})", new CooldownProps {
                Icon = ActionIds.SecondWind,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.SecondWind) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.FootGraze)} ({UIHelper.Localize(JobIds.MCH)})", new CooldownProps {
                Icon = ActionIds.FootGraze,
                Duration = 10,
                CD = 30,
                Triggers = new []{ new Item(ActionIds.FootGraze) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.HeadGraze)} ({UIHelper.Localize(JobIds.MCH)})", new CooldownProps {
                Icon = ActionIds.HeadGraze,
                CD = 30,
                Triggers = new []{ new Item(ActionIds.HeadGraze) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.ArmsLength)} ({UIHelper.Localize(JobIds.MCH)})", new CooldownProps {
                Icon = ActionIds.ArmsLength,
                Duration = 6,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.ArmsLength) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Tactician), new CooldownProps {
                Icon = ActionIds.Tactician,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Tactician) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.Wildfire), new IconProps {
                Icons = new [] { ActionIds.Wildfire },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Wildfire), Duration = 10 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;

        public static bool GCD_ROLL => true;
    }
}
