using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class AST {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig($"{UIHelper.Localize(JobIds.AST)} {UIHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps {
                Procs = new []{
                    new ProcConfig(UIHelper.Localize(BuffIds.GiantDominance), BuffIds.GiantDominance, UIColor.LightBlue)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Combust3), GaugeVisualType.Bar, new GaugeTimerProps {
                SubTimers = new [] {
                    new GaugeSubTimerProps {
                        MaxDuration = 30,
                        Color = UIColor.LightBlue,
                        SubName = UIHelper.Localize(BuffIds.Combust3),
                        Triggers = new []{
                            new Item(BuffIds.Combust2),
                            new Item(BuffIds.Combust3)
                        }
                    },
                    new GaugeSubTimerProps {
                        MaxDuration = 18,
                        Color = UIColor.LightBlue,
                        SubName = UIHelper.Localize(BuffIds.Combust),
                        Triggers = new []{
                            new Item(BuffIds.Combust)
                        }
                    }
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(BuffIds.TheBalance), new BuffProps {
                Duration = 15,
                Icon = ActionIds.TheBalance,
                Color = UIColor.Orange,
                Triggers = new []{ new Item(BuffIds.TheBalance) },
                IsPlayerOnly = true
            }),
            new BuffConfig(UIHelper.Localize(BuffIds.TheBole), new BuffProps {
                Duration = 15,
                Icon = ActionIds.TheBole,
                Color = UIColor.BrightGreen,
                Triggers = new []{ new Item(BuffIds.TheBole) },
                IsPlayerOnly = true
            }),
            new BuffConfig(UIHelper.Localize(BuffIds.TheSpear), new BuffProps {
                Duration = 15,
                Icon = ActionIds.TheSpear,
                Color = UIColor.DarkBlue,
                Triggers = new []{ new Item(BuffIds.TheSpear) },
                IsPlayerOnly = true
            }),
            new BuffConfig(UIHelper.Localize(BuffIds.TheSpire), new BuffProps {
                Duration = 15,
                Icon = ActionIds.TheSpire,
                Color = UIColor.Yellow,
                Triggers = new []{ new Item(BuffIds.TheSpire) },
                IsPlayerOnly = true
            }),
            new BuffConfig(UIHelper.Localize(BuffIds.TheArrow), new BuffProps {
                Duration = 15,
                Icon = ActionIds.TheArrow,
                Color = UIColor.LightBlue,
                Triggers = new []{ new Item(BuffIds.TheArrow) },
                IsPlayerOnly = true
            }),
            new BuffConfig(UIHelper.Localize(BuffIds.TheEwer), new BuffProps {
                Duration = 15,
                Icon = ActionIds.TheEwer,
                Color = UIColor.LightBlue,
                Triggers = new []{ new Item(BuffIds.TheEwer) },
                IsPlayerOnly = true
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.Divination), new BuffProps {
                CD = 120,
                Duration = 15,
                Icon = ActionIds.Divination,
                Color = UIColor.Yellow,
                Triggers = new []{ new Item(ActionIds.Divination) }
            })
        };

        public static Cursor Cursors => new(JobIds.AST, CursorType.None, CursorType.CastTime);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Swiftcast)} ({UIHelper.Localize(JobIds.AST)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Swiftcast) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.LucidDreaming)} ({UIHelper.Localize(JobIds.AST)})", new CooldownProps {
                Icon = ActionIds.LucidDreaming,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.LucidDreaming) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Surecast)} ({UIHelper.Localize(JobIds.AST)})", new CooldownProps {
                Icon = ActionIds.Surecast,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Surecast) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Rescue)} ({UIHelper.Localize(JobIds.AST)})", new CooldownProps {
                Icon = ActionIds.Rescue,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Rescue) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Synastry), new CooldownProps {
                Icon = ActionIds.Synastry,
                Duration = 20,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Synastry) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.CollectiveUnconscious), new CooldownProps {
                Icon = ActionIds.CollectiveUnconscious,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.CollectiveUnconscious) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.CelestialOpposition), new CooldownProps {
                Icon = ActionIds.CelestialOpposition,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.CelestialOpposition) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.EarthlyStar), new CooldownProps {
                Icon = ActionIds.EarthlyStar,
                Duration = 20,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.EarthlyStar) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.NeutralSect), new CooldownProps {
                Icon = ActionIds.NeutralSect,
                Duration = 20,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.NeutralSect) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Exaltation), new CooldownProps {
                Icon = ActionIds.Exaltation,
                Duration = 8,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Exaltation) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Macrocosmos), new CooldownProps {
                Icon = ActionIds.Macrocosmos,
                CD = 180,
                Duration = 15,
                Triggers = new []{ new Item(ActionIds.Macrocosmos) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.Combust3), new IconProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.Combust1,
                    ActionIds.Combust2,
                    ActionIds.Combust3
                },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Combust), Duration = 18 },
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Combust2), Duration = 30 },
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Combust3), Duration = 30 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.Lightspeed), new IconProps {
                Icons = new [] { ActionIds.Lightspeed },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Lightspeed), Duration = 15 }
                }
            }),
            new IconReplacer(UIHelper.Localize(ActionIds.EarthlyStar), new IconProps {
                Icons = new [] {
                    ActionIds.EarthlyStar,
                    ActionIds.StellarDetonation
                },
                Triggers = new [] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.GiantDominance), Duration = 10 }
                }
            }),
            new IconReplacer(UIHelper.Localize(ActionIds.Astrodyne), new IconProps {
                Icons = new [] {
                    ActionIds.Astrodyne
                },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.HarmonyOfSpirit), Duration = 15 }
                }
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.24f };

        public static bool GCD_ROLL => false;
    }
}
