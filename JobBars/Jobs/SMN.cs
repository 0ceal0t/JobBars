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
    public static class SMN {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.Ruin4), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 4,
                Triggers = new []{
                    new Item(BuffIds.Ruin4)
                },
                Color = UIColor.DarkBlue
            }),
            new GaugeGCDConfig(UIHelper.Localize(ActionIds.SummonBahamut), GaugeVisualType.Arrow, new GaugeGCDProps {
                SubGCDs = new [] {
                    new GaugeSubGCDProps {
                        MaxCounter = 8,
                        MaxDuration = 21,
                        Color = UIColor.LightBlue,
                        SubName = UIHelper.Localize(ActionIds.SummonBahamut),
                        Increment = new []{
                            new Item(ActionIds.Wyrmwave)
                        },
                        Triggers = new []{
                            new Item(ActionIds.SummonBahamut),
                            new Item(ActionIds.Wyrmwave) // in case this registers first for some reason
                        }
                    },
                    new GaugeSubGCDProps {
                        MaxCounter = 8,
                        MaxDuration = 21,
                        Color = UIColor.Orange,
                        SubName = UIHelper.Localize(ActionIds.FirebirdTrance),
                        Increment = new []{
                            new Item(ActionIds.ScarletFlame)
                        },
                        Triggers = new []{
                            new Item(ActionIds.FirebirdTrance),
                            new Item(ActionIds.ScarletFlame) // in case this registers first for some reason
                        }
                    }
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Bio3), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 30,
                Color = UIColor.HealthGreen,
                Triggers = new []{
                    new Item(BuffIds.ArcBio),
                    new Item(BuffIds.ArcBio2),
                    new Item(BuffIds.Bio3)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Miasma3), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 30,
                Color = UIColor.BlueGreen,
                Triggers = new []{
                    new Item(BuffIds.Miasma),
                    new Item(BuffIds.Miasma3)
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(ActionIds.Devotion), new BuffProps {
                CD = 180,
                Duration = 15,
                Icon = ActionIds.Devotion,
                Color = UIColor.Yellow,
                Triggers = new []{ new Item(ActionIds.Devotion) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.SummonBahamut), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.SummonBahamut,
                Color = UIColor.LightBlue,
                Triggers = new []{ new Item(ActionIds.SummonBahamut) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.FirebirdTrance), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.FirebirdTrance,
                Color = UIColor.Orange,
                Triggers = new []{ new Item(ActionIds.FirebirdTrance) }
            })
        };

        public static Cursor Cursors => new(JobIds.SMN, CursorType.None, CursorType.CastTime);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Addle)} ({UIHelper.Localize(JobIds.SMN)})", new CooldownProps {
                Icon = ActionIds.Addle,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Addle) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Swiftcast)} ({UIHelper.Localize(JobIds.SMN)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Swiftcast) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.Bio3), new IconProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.ArcBio,
                    ActionIds.ArcBio2,
                    ActionIds.Bio3
                },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.ArcBio), Duration = 30 },
                    new IconTriggerStruct { Trigger = new Item(BuffIds.ArcBio2), Duration = 30 },
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Bio3), Duration = 30 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.Miasma), new IconProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.Miasma,
                    ActionIds.Miasma3
                },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Miasma), Duration = 30 },
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Miasma3), Duration = 30 }
                }
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.24f };

        public static bool GCD_ROLL => false;
    }
}
