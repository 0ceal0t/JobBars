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
            new GaugeProcsConfig($"{UiHelper.Localize(JobIds.AST)} {UiHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps {
                Procs = [
                    new ProcConfig(UiHelper.Localize(BuffIds.GiantDominance), BuffIds.GiantDominance, ColorConstants.LightBlue)
                ]
            }),
            new GaugeTimerConfig(UiHelper.Localize(BuffIds.Combust3), GaugeVisualType.Bar, new GaugeTimerProps {
                SubTimers = [
                    new GaugeSubTimerProps {
                        MaxDuration = 30,
                        Color = ColorConstants.LightBlue,
                        SubName = UiHelper.Localize(BuffIds.Combust3),
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
            new BuffConfig(UiHelper.Localize(BuffIds.TheBalance), new BuffProps {
                Duration = 15,
                Icon = ActionIds.TheBalance,
                Color = ColorConstants.Orange,
                Triggers = [new Item(BuffIds.TheBalance)],
                ApplyToTarget = true,
                ShowPartyText = true
            }),
            new BuffConfig(UiHelper.Localize(BuffIds.TheSpear), new BuffProps {
                Duration = 15,
                Icon = ActionIds.TheSpear,
                Color = ColorConstants.DarkBlue,
                Triggers = [new Item(BuffIds.TheSpear)],
                ApplyToTarget = true,
                ShowPartyText = true
            }),
            new BuffConfig(UiHelper.Localize(ActionIds.Divination), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.Divination,
                Color = ColorConstants.Yellow,
                Triggers = [new Item(ActionIds.Divination)]
            })
        ];

        public static Cursor Cursors => new( JobIds.AST, CursorType.None, CursorType.CastTime );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig(UiHelper.Localize(ActionIds.NeutralSect), new CooldownProps {
                Icon = ActionIds.NeutralSect,
                Duration = 20,
                CD = 120,
                Triggers = [new Item(ActionIds.NeutralSect)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.Macrocosmos), new CooldownProps {
                Icon = ActionIds.Macrocosmos,
                CD = 180,
                Duration = 15,
                Triggers = [new Item(ActionIds.Macrocosmos)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.EarthlyStar), new CooldownProps {
                Icon = ActionIds.EarthlyStar,
                Duration = 20,
                CD = 60,
                Triggers = [new Item(ActionIds.EarthlyStar)]
            }),
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Swiftcast)} ({UiHelper.Localize(JobIds.AST)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 40,
                Triggers = [new Item(ActionIds.Swiftcast)]
            })
        ];

        public static IconReplacer[] Icons => [
            new IconBuffReplacer(UiHelper.Localize(BuffIds.Combust3), new IconBuffProps {
                IconType = IconActionType.Timer,
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
            new IconBuffReplacer(UiHelper.Localize(BuffIds.Divination), new IconBuffProps {
                IconType = IconActionType.Buff,
                Icons = [ActionIds.Divination],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Divination), Duration = 20 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.Lightspeed), new IconBuffProps {
                IconType = IconActionType.Buff,
                Icons = [ActionIds.Lightspeed],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Lightspeed), Duration = 15 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(ActionIds.EarthlyStar), new IconBuffProps {
                IconType = IconActionType.Buff,
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
