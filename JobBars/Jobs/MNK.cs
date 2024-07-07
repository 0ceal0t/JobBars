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
    public static class MNK {
        public static GaugeConfig[] Gauges => [
            new GaugeStacksConfig(UiHelper.Localize(BuffIds.PerfectBalance), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = [
                    new Item(BuffIds.PerfectBalance)
                ],
                Color = ColorConstants.Orange
            }),
            new GaugeGCDConfig(UiHelper.Localize(BuffIds.RiddleOfFire), GaugeVisualType.Bar, new GaugeSubGCDProps {
                MaxCounter = 11,
                MaxDuration = 20,
                Color = ColorConstants.Red,
                Triggers = [
                    new Item(BuffIds.RiddleOfFire)
                ]
            }),
            new GaugeChargesConfig($"{UiHelper.Localize(ActionIds.TrueNorth)} ({UiHelper.Localize(JobIds.MNK)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = ColorConstants.NoColor,
                SameColor = true,
                Parts = [
                    new GaugesChargesPartProps {
                        Diamond = true,
                        MaxCharges = 2,
                        CD = 45,
                        Triggers = [new Item(ActionIds.TrueNorth)]
                    },
                    new GaugesChargesPartProps {
                        Bar = true,
                        Duration = 10,
                        Triggers = [new Item(BuffIds.TrueNorth)]
                    }
                ],
                CompletionSound = GaugeCompleteSoundType.Never
            })
        ];

        public static BuffConfig[] Buffs => [
            new BuffConfig(UiHelper.Localize(ActionIds.Brotherhood), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.Brotherhood,
                Color = ColorConstants.Orange,
                Triggers = [new Item(ActionIds.Brotherhood)]
            }),
            new BuffConfig(UiHelper.Localize(ActionIds.RiddleOfFire), new BuffProps {
                CD = 60,
                Duration = 20,
                Icon = ActionIds.RiddleOfFire,
                Color = ColorConstants.Red,
                Triggers = [new Item(ActionIds.RiddleOfFire)]
            })
        ];

        public static Cursor Cursors => new( JobIds.MNK, CursorType.None, CursorType.GCD );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Feint)} ({UiHelper.Localize(JobIds.MNK)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 10,
                CD = 90,
                Triggers = [new Item(ActionIds.Feint)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.Mantra), new CooldownProps {
                Icon = ActionIds.Mantra,
                Duration = 15,
                CD = 90,
                Triggers = [new Item(ActionIds.Mantra)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UiHelper.Localize(BuffIds.RiddleOfFire), new IconBuffProps {
                Icons = [
                    ActionIds.RiddleOfFire,
                    ActionIds.FiresReply,
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.RiddleOfFire), Duration = 20 }
                ]
            }),
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
