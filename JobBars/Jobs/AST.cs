using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.Atk;
using System;

namespace JobBars.Jobs {
    public static class AST {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig($"{AtkHelper.Localize(JobIds.AST)} {AtkHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps {
                Procs = new []{
                    new ProcConfig(AtkHelper.Localize(BuffIds.GiantDominance), BuffIds.GiantDominance, AtkColor.LightBlue)
                }
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.Combust3), GaugeVisualType.Bar, new GaugeTimerProps {
                SubTimers = new [] {
                    new GaugeSubTimerProps {
                        MaxDuration = 30,
                        Color = AtkColor.LightBlue,
                        SubName = AtkHelper.Localize(BuffIds.Combust3),
                        Triggers = new []{
                            new Item(BuffIds.Combust2),
                            new Item(BuffIds.Combust3),
                            new Item(BuffIds.Combust)
                        }
                    }
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(AtkHelper.Localize(BuffIds.TheBalance), new BuffProps {
                Duration = 15,
                Icon = ActionIds.TheBalance,
                Color = AtkColor.Orange,
                Triggers = new []{ new Item(BuffIds.TheBalance) },
                ApplyToTarget = true,
                ShowPartyText = true
            }),
            new BuffConfig(AtkHelper.Localize(BuffIds.TheBole), new BuffProps {
                Duration = 15,
                Icon = ActionIds.TheBole,
                Color = AtkColor.BrightGreen,
                Triggers = new []{ new Item(BuffIds.TheBole) },
                ApplyToTarget = true,
                ShowPartyText = true
            }),
            new BuffConfig(AtkHelper.Localize(BuffIds.TheSpear), new BuffProps {
                Duration = 15,
                Icon = ActionIds.TheSpear,
                Color = AtkColor.DarkBlue,
                Triggers = new []{ new Item(BuffIds.TheSpear) },
                ApplyToTarget = true,
                ShowPartyText = true
            }),
            new BuffConfig(AtkHelper.Localize(BuffIds.TheSpire), new BuffProps {
                Duration = 15,
                Icon = ActionIds.TheSpire,
                Color = AtkColor.Yellow,
                Triggers = new []{ new Item(BuffIds.TheSpire) },
                ApplyToTarget = true,
                ShowPartyText = true
            }),
            new BuffConfig(AtkHelper.Localize(BuffIds.TheArrow), new BuffProps {
                Duration = 15,
                Icon = ActionIds.TheArrow,
                Color = AtkColor.LightBlue,
                Triggers = new []{ new Item(BuffIds.TheArrow) },
                ApplyToTarget = true,
                ShowPartyText = true
            }),
            new BuffConfig(AtkHelper.Localize(BuffIds.TheEwer), new BuffProps {
                Duration = 15,
                Icon = ActionIds.TheEwer,
                Color = AtkColor.LightBlue,
                Triggers = new []{ new Item(BuffIds.TheEwer) },
                ApplyToTarget = true,
                ShowPartyText = true
            }),
            new BuffConfig(AtkHelper.Localize(ActionIds.Divination), new BuffProps {
                CD = 120,
                Duration = 15,
                Icon = ActionIds.Divination,
                Color = AtkColor.Yellow,
                Triggers = new []{ new Item(ActionIds.Divination) }
            })
        };

        public static Cursor Cursors => new(JobIds.AST, CursorType.None, CursorType.CastTime);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig(AtkHelper.Localize(ActionIds.NeutralSect), new CooldownProps {
                Icon = ActionIds.NeutralSect,
                Duration = 20,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.NeutralSect) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.Macrocosmos), new CooldownProps {
                Icon = ActionIds.Macrocosmos,
                CD = 180,
                Duration = 15,
                Triggers = new []{ new Item(ActionIds.Macrocosmos) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.EarthlyStar), new CooldownProps {
                Icon = ActionIds.EarthlyStar,
                Duration = 20,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.EarthlyStar) }
            }),
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Swiftcast)} ({AtkHelper.Localize(JobIds.AST)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Swiftcast) }
            })
        };

        public static IconReplacer[] Icons => new IconReplacer[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Combust3), new IconBuffProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.Combust1,
                    ActionIds.Combust2,
                    ActionIds.Combust3
                },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Combust), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Combust2), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Combust3), Duration = 30 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Lightspeed), new IconBuffProps {
                Icons = new [] { ActionIds.Lightspeed },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Lightspeed), Duration = 15 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(ActionIds.EarthlyStar), new IconBuffProps {
                Icons = new [] {
                    ActionIds.EarthlyStar,
                    ActionIds.StellarDetonation
                },
                Triggers = new [] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.GiantDominance), Duration = 10 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(ActionIds.Astrodyne), new IconBuffProps {
                Icons = new [] {
                    ActionIds.Astrodyne
                },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.HarmonyOfSpirit), Duration = 15 }
                }
            }),
            new IconCooldownReplacer(AtkHelper.Localize(ActionIds.MinorArcana), new IconCooldownProps {
                Icons = new[] {
                    ActionIds.LordOfCrowns,
                    ActionIds.LadyOfCrowns
                },
                Triggers = new[] {
                    new Item(ActionIds.MinorArcana)
                },
                Cooldown = 60
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.24f };
    }
}
