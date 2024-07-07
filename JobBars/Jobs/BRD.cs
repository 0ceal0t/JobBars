using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class BRD {
        public static GaugeConfig[] Gauges => [
            new GaugeProcsConfig(UiHelper.Localize(BuffIds.HawksEye), GaugeVisualType.Diamond, new GaugeProcProps {
                Procs = [
                    new ProcConfig(UiHelper.Localize(BuffIds.HawksEye), BuffIds.HawksEye, ColorConstants.Yellow)
                ]
            }),
            new GaugeChargesConfig(UiHelper.Localize(ActionIds.HeartbreakShot), GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = ColorConstants.Red,
                SameColor = true,
                Parts = [
                    new GaugesChargesPartProps {
                        Bar = true,
                        Diamond = true,
                        CD = 15,
                        MaxCharges = 3,
                        Triggers = [
                            new Item(ActionIds.BloodLetter),
                            new Item(ActionIds.HeartbreakShot),
                        ]
                    }
                ]
            }),
            new GaugeTimerConfig(UiHelper.Localize(BuffIds.VenomousBite), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 45,
                Color = ColorConstants.Purple,
                Triggers = [
                    new Item(BuffIds.CausticBite),
                    new Item(BuffIds.VenomousBite)
                ]
            }),
            new GaugeTimerConfig(UiHelper.Localize(BuffIds.Stormbite), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 45,
                Color = ColorConstants.LightBlue,
                Triggers = [
                    new Item(BuffIds.Windbite),
                    new Item(BuffIds.Stormbite),
                ]
            }),
            new GaugeGCDConfig(UiHelper.Localize(BuffIds.RagingStrikes), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 8,
                MaxDuration = 20,
                Color = ColorConstants.Orange,
                Triggers = [
                    new Item(BuffIds.RagingStrikes)
                ],
            })
        ];

        public static BuffConfig[] Buffs => [
            new BuffConfig(UiHelper.Localize(ActionIds.BattleVoice), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.BattleVoice,
                Color = ColorConstants.Orange,
                Triggers = [new Item(ActionIds.BattleVoice)]
            }),
            new BuffConfig(UiHelper.Localize(ActionIds.RadiantFinale), new BuffProps {
                CD = 110,
                Duration = 20,
                Icon = ActionIds.RadiantFinale,
                Color = ColorConstants.DarkBlue,
                Triggers = [new Item(ActionIds.RadiantFinale)]
            }),
            new BuffConfig(UiHelper.Localize(BuffIds.Barrage), new BuffProps {
                CD = 120,
                Duration = 10,
                Icon = ActionIds.Barrage,
                Color = ColorConstants.Yellow,
                Triggers = [new Item(BuffIds.Barrage)]
            }),
            new BuffConfig(UiHelper.Localize(ActionIds.RagingStrikes), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.RagingStrikes,
                Color = ColorConstants.Yellow,
                Triggers = [new Item(ActionIds.RagingStrikes)]
            })
        ];

        public static Cursor Cursors => new( JobIds.BRD, CursorType.None, CursorType.GCD );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig(UiHelper.Localize(ActionIds.Troubadour), new CooldownProps {
                Icon = ActionIds.Troubadour,
                Duration = 15,
                CD = 90,
                Triggers = [new Item(ActionIds.Troubadour)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.NaturesMinne), new CooldownProps {
                Icon = ActionIds.NaturesMinne,
                Duration = 15,
                CD = 120,
                Triggers = [new Item(ActionIds.NaturesMinne)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UiHelper.Localize(BuffIds.RagingStrikes), new IconBuffProps {
                Icons = [ActionIds.RagingStrikes],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.RagingStrikes), Duration = 15 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.BattleVoice), new IconBuffProps {
                Icons = [ActionIds.BattleVoice],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.BattleVoice), Duration = 20 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.RadiantFinale), new IconBuffProps {
                Icons = [ActionIds.RadiantFinale],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.RadiantFinale), Duration = 20 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.VenomousBite), new IconBuffProps {
                IsTimer = true,
                Icons = [
                    ActionIds.CausticBite,
                    ActionIds.VenomousBite,
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.CausticBite), Duration = 45 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.VenomousBite), Duration = 45 },
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.Stormbite), new IconBuffProps {
                IsTimer = true,
                Icons = [
                    ActionIds.Windbite,
                    ActionIds.Stormbite,
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Windbite), Duration = 45 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Stormbite), Duration = 45 },
                ]
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
