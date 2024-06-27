using JobBars.Atk;
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

namespace JobBars.Jobs {
    public static class SAM {
        public static GaugeConfig[] Gauges => [
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.Meikyo), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = [
                    new Item(BuffIds.Meikyo)
                ],
                Color = AtkColor.BlueGreen
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.Fugetsu), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 40,
                Color = AtkColor.DarkBlue,
                Triggers = [
                    new Item(BuffIds.Fugetsu)
                ]
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.Fuka), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 40,
                Color = AtkColor.Red,
                Triggers = [
                    new Item(BuffIds.Fuka)
                ]
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.Higanbana), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 60,
                Color = AtkColor.Orange,
                Triggers = [
                    new Item(BuffIds.Higanbana)
                ]
            }),
            new GaugeChargesConfig($"{AtkHelper.Localize(ActionIds.TrueNorth)} ({AtkHelper.Localize(JobIds.SAM)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = AtkColor.NoColor,
                SameColor = true,
                Parts = [
                    new GaugesChargesPartProps {
                        Diamond = true,
                        MaxCharges = 2,
                        CD = 45,
                        Triggers = [
                            new Item(ActionIds.TrueNorth)
                        ]
                    },
                    new GaugesChargesPartProps {
                        Bar = true,
                        Duration = 10,
                        Triggers = [
                            new Item(BuffIds.TrueNorth)
                        ]
                    }
                ],
                CompletionSound = GaugeCompleteSoundType.Never
            })
        ];

        public static BuffConfig[] Buffs => [
            new BuffConfig(AtkHelper.Localize(ActionIds.OgiNamikiri), new BuffProps {
                CD = 120,
                Duration = 30,
                Icon = ActionIds.OgiNamikiri,
                Color = AtkColor.Red,
                Triggers = [new Item(BuffIds.OgiNamikiri)]
            })
        ];

        public static Cursor Cursors => new( JobIds.SAM, CursorType.None, CursorType.GCD );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Feint)} ({AtkHelper.Localize(JobIds.SAM)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 15,
                CD = 90,
                Triggers = [new Item(ActionIds.Feint)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Fugetsu), new IconBuffProps {
                IsTimer = true,
                Icons = [ActionIds.Jinpu],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Fugetsu), Duration = 40 }
                ]
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Fuka), new IconBuffProps {
                IsTimer = true,
                Icons = [ActionIds.Shifu],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Fuka), Duration = 40 }
                ]
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
