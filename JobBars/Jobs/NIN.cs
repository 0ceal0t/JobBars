using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.Stacks;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class NIN {
        public static GaugeConfig[] Gauges => [
            new GaugeStacksConfig(UiHelper.Localize(BuffIds.RaijuReady), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = [
                    new Item(BuffIds.RaijuReady)
                ],
                Color = ColorConstants.PurplePink
            }),
            new GaugeStacksConfig(UiHelper.Localize(BuffIds.Bunshin), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 5,
                Triggers = [
                    new Item(BuffIds.Bunshin)
                ],
                Color = ColorConstants.Red
            }),
            new GaugeChargesConfig($"{UiHelper.Localize(ActionIds.TrueNorth)} ({UiHelper.Localize(JobIds.NIN)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
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
            new BuffConfig(UiHelper.Localize(ActionIds.Dokumori), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.Dokumori,
                Color = ColorConstants.LightBlue,
                Triggers = [
                    new Item(ActionIds.Mug),
                    new Item(ActionIds.Dokumori)
                ]
            }),
            new BuffConfig(UiHelper.Localize(ActionIds.KunaisBane), new BuffProps {
                CD = 60,
                Duration = 15,
                Icon = ActionIds.KunaisBane,
                Color = ColorConstants.Yellow,
                Triggers = [
                    new Item(ActionIds.TrickAttack),
                    new Item(ActionIds.KunaisBane),
                ]
            }),
            new BuffConfig(UiHelper.Localize(BuffIds.Bunshin), new BuffProps {
                CD = 90,
                Duration = 30,
                Icon = ActionIds.Bunshin,
                Color = ColorConstants.Orange,
                Triggers = [new Item(BuffIds.Bunshin)]
            })
        ];

        public static Cursor Cursors => new( JobIds.NIN, CursorType.None, CursorType.GCD );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Feint)} ({UiHelper.Localize(JobIds.NIN)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 15,
                CD = 90,
                Triggers = [new Item(ActionIds.Feint)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UiHelper.Localize(BuffIds.KunaisBane), new IconBuffProps {
                Icons = [
                    ActionIds.TrickAttack,
                    ActionIds.KunaisBane,
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.TrickAttack), Duration = 15 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.KunaisBane), Duration = 15 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.Dokumori), new IconBuffProps {
                Icons = [
                    ActionIds.Mug,
                    ActionIds.Dokumori
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Mug), Duration = 20 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Dokumori), Duration = 20 }
                ]
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
