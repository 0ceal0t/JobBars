using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class AST {
        public static GaugeConfig[] Gauges => [
            new GaugeProcsConfig($"{AtkHelper.Localize(JobIds.AST)} {AtkHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps {
                Procs = [
                    new ProcConfig(AtkHelper.Localize(BuffIds.GiantDominance), BuffIds.GiantDominance, AtkColor.LightBlue)
                ]
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.Combust3), GaugeVisualType.Bar, new GaugeTimerProps {
                SubTimers = [
                    new GaugeSubTimerProps {
                        MaxDuration = 30,
                        Color = AtkColor.LightBlue,
                        SubName = AtkHelper.Localize(BuffIds.Combust3),
                        Triggers = [
                            new Item(BuffIds.Combust2),
                            new Item(BuffIds.Combust3),
                            new Item(BuffIds.Combust)
                        ]
                    }
                ]
            })
        ];

        public static BuffConfig[] Buffs => [
            new BuffConfig(AtkHelper.Localize(BuffIds.TheBalance), new BuffProps {
                Duration = 15,
                Icon = ActionIds.TheBalance,
                Color = AtkColor.Orange,
                Triggers = [new Item(BuffIds.TheBalance)],
                ApplyToTarget = true,
                ShowPartyText = true
            }),
            new BuffConfig(AtkHelper.Localize(BuffIds.TheSpear), new BuffProps {
                Duration = 15,
                Icon = ActionIds.TheSpear,
                Color = AtkColor.DarkBlue,
                Triggers = [new Item(BuffIds.TheSpear)],
                ApplyToTarget = true,
                ShowPartyText = true
            }),
            new BuffConfig(AtkHelper.Localize(ActionIds.Divination), new BuffProps {
                CD = 120,
                Duration = 15,
                Icon = ActionIds.Divination,
                Color = AtkColor.Yellow,
                Triggers = [new Item(ActionIds.Divination)]
            })
        ];

        public static Cursor Cursors => new( JobIds.AST, CursorType.None, CursorType.CastTime );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig(AtkHelper.Localize(ActionIds.NeutralSect), new CooldownProps {
                Icon = ActionIds.NeutralSect,
                Duration = 20,
                CD = 120,
                Triggers = [new Item(ActionIds.NeutralSect)]
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.Macrocosmos), new CooldownProps {
                Icon = ActionIds.Macrocosmos,
                CD = 180,
                Duration = 15,
                Triggers = [new Item(ActionIds.Macrocosmos)]
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.EarthlyStar), new CooldownProps {
                Icon = ActionIds.EarthlyStar,
                Duration = 20,
                CD = 60,
                Triggers = [new Item(ActionIds.EarthlyStar)]
            }),
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Swiftcast)} ({AtkHelper.Localize(JobIds.AST)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 40,
                Triggers = [new Item(ActionIds.Swiftcast)]
            })
        ];

        public static IconReplacer[] Icons => [
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Combust3), new IconBuffProps {
                IsTimer = true,
                Icons = [
                    ActionIds.Combust1,
                    ActionIds.Combust2,
                    ActionIds.Combust3
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Combust), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Combust2), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Combust3), Duration = 30 }
                ]
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Divination), new IconBuffProps {
                Icons = [ActionIds.Divination],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Divination), Duration = 15 }
                ]
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Lightspeed), new IconBuffProps {
                Icons = [ActionIds.Lightspeed],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Lightspeed), Duration = 15 }
                ]
            }),
            new IconBuffReplacer(AtkHelper.Localize(ActionIds.EarthlyStar), new IconBuffProps {
                Icons = [
                    ActionIds.EarthlyStar,
                    ActionIds.StellarDetonation
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.GiantDominance), Duration = 10 }
                ]
            })
        ];

        public static bool MP => true;

        public static float[] MP_SEGMENTS => [0.24f];
    }
}
