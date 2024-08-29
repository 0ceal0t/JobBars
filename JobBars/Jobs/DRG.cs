using JobBars.Atk;
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

namespace JobBars.Jobs {
    public static class DRG {
        public static GaugeConfig[] Gauges => [
            new GaugeGCDConfig(UiHelper.Localize(BuffIds.LanceCharge), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 8,
                MaxDuration = 20,
                Color = ColorConstants.Red,
                Triggers = [
                    new Item(BuffIds.LanceCharge)
                ]
            }),
            new GaugeTimerConfig(UiHelper.Localize(BuffIds.ChaoticSpring), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 24,
                Color = ColorConstants.Purple,
                Triggers = [
                    new Item(BuffIds.ChaosThrust),
                    new Item(BuffIds.ChaoticSpring)
                ]
            }),
            new GaugeTimerConfig(UiHelper.Localize(BuffIds.PowerSurge), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 30,
                Color = ColorConstants.BlueGreen,
                Triggers = [
                    new Item(BuffIds.PowerSurge),
                ]
            }),
            new GaugeChargesConfig($"{UiHelper.Localize(ActionIds.TrueNorth)} ({UiHelper.Localize(JobIds.DRG)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = ColorConstants.NoColor,
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
            new BuffConfig(UiHelper.Localize(ActionIds.BattleLitany), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.BattleLitany,
                Color = ColorConstants.LightBlue,
                Triggers = [new Item(ActionIds.BattleLitany)]
            })
        ];

        public static Cursor Cursors => new( JobIds.DRG, CursorType.None, CursorType.GCD );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Feint)} ({UiHelper.Localize(JobIds.DRG)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 15,
                CD = 90,
                Triggers = [new Item(ActionIds.Feint)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UiHelper.Localize(BuffIds.LanceCharge), new IconBuffProps {
                IconType = IconActionType.Buff,
                Icons = [ActionIds.LanceCharge],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.LanceCharge), Duration = 20 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.PowerSurge), new IconBuffProps {
                IconType = IconActionType.Timer,
                Icons = [
                    ActionIds.Disembowel,
                    ActionIds.SpiralBlow,
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.PowerSurge), Duration = 30 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.ChaosThrust), new IconBuffProps {
                IconType = IconActionType.Timer,
                Icons = [
                    ActionIds.ChaosThrust,
                    ActionIds.ChaoticSpring
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ChaosThrust), Duration = 24 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ChaoticSpring), Duration = 24 }
                ]
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
