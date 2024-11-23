using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Stacks;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class MCH {
        public static GaugeConfig[] Gauges => [
            new GaugeStacksConfig(UiHelper.Localize(BuffIds.Overheated), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 5,
                Triggers = [
                    new Item(BuffIds.Overheated)
                ],
                Color = ColorConstants.Orange,
            }),
            new GaugeGCDConfig(UiHelper.Localize(BuffIds.Wildfire), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 6,
                MaxDuration = 10,
                Color = ColorConstants.Red,
                Triggers = [
                    new Item(BuffIds.Wildfire)
                ]
            }),
            new GaugeChargesConfig(UiHelper.Localize(ActionIds.DoubleCheck), GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = ColorConstants.Red,
                SameColor = true,
                Parts = [
                    new GaugesChargesPartProps {
                        Bar = true,
                        Diamond = true,
                        CD = 30,
                        MaxCharges = 3,
                        Triggers = [
                            new Item(ActionIds.GaussRound),
                            new Item(ActionIds.DoubleCheck)
                        ]
                    }
                ]
            }),
            new GaugeChargesConfig(UiHelper.Localize(ActionIds.Checkmate), GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = ColorConstants.LightBlue,
                SameColor = true,
                Parts = [
                    new GaugesChargesPartProps {
                        Bar = true,
                        Diamond = true,
                        CD = 30,
                        MaxCharges = 3,
                        Triggers = [
                            new Item(ActionIds.Ricochet),
                            new Item(ActionIds.Checkmate),
                        ]
                    }
                ]
            })
        ];

        public static BuffConfig[] Buffs => [
            new BuffConfig(UiHelper.Localize(ActionIds.Wildfire), new BuffProps {
                CD = 120,
                Duration = 10,
                Icon = ActionIds.Wildfire,
                Color = ColorConstants.Orange,
                Triggers = [new Item(ActionIds.Wildfire)]
            })
        ];

        public static Cursor Cursors => new( JobIds.MCH, CursorType.None, CursorType.GCD );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig(UiHelper.Localize(ActionIds.Tactician), new CooldownProps {
                Icon = ActionIds.Tactician,
                Duration = 15,
                CD = 90,
                Triggers = [new Item(ActionIds.Tactician)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.Dismantle), new CooldownProps {
                Icon = ActionIds.Dismantle,
                Duration = 10,
                CD = 120,
                Triggers = [new Item(ActionIds.Dismantle)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UiHelper.Localize(BuffIds.Wildfire), new IconBuffProps {
                IconType = IconActionType.Buff,
                Icons = [ActionIds.Wildfire],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Wildfire), Duration = 10 }
                ]
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
