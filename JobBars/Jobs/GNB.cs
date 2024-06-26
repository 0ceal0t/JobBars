using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.GCD;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class GNB {
        public static GaugeConfig[] Gauges => [
            new GaugeGCDConfig(AtkHelper.Localize(BuffIds.NoMercy), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 9,
                MaxDuration = 20,
                Color = AtkColor.Orange,
                Triggers = [
                    new Item(BuffIds.NoMercy)
                ]
            })
        ];

        public static BuffConfig[] Buffs => [];

        public static Cursor Cursors => new( JobIds.GNB, CursorType.None, CursorType.GCD );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig(AtkHelper.Localize(ActionIds.Superbolide), new CooldownProps {
                Icon = ActionIds.Superbolide,
                Duration = 10,
                CD = 360,
                Triggers = [new Item(ActionIds.Superbolide)]
            }),
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Reprisal)} ({AtkHelper.Localize(JobIds.GNB)})", new CooldownProps {
                Icon = ActionIds.Reprisal,
                Duration = 15,
                CD = 60,
                Triggers = [new Item(ActionIds.Reprisal)]
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.HeartOfLight), new CooldownProps {
                Icon = ActionIds.HeartOfLight,
                Duration = 15,
                CD = 90,
                Triggers = [new Item(ActionIds.HeartOfLight)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.NoMercy), new IconBuffProps {
                Icons = [
                    ActionIds.NoMercy,
                    ActionIds.SonicBreak
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.NoMercy), Duration = 20 }
                ]
            }),
            new IconBuffReplacer($"{AtkHelper.Localize(ActionIds.Rampart)} ({AtkHelper.Localize(JobIds.GNB)})", new IconBuffProps {
                Icons = [ActionIds.Rampart],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Rampart), Duration = 20 }
                ]
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.GreatNebula), new IconBuffProps {
                Icons = [
                    ActionIds.Nebula,
                    ActionIds.GreatNebula
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Nebula), Duration = 15 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.GreatNebula), Duration = 15 }
                ]
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Camouflage), new IconBuffProps {
                Icons = [ActionIds.Camouflage],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Camouflage), Duration = 20 }
                ]
            }),
            new IconBuffReplacer($"{AtkHelper.Localize(ActionIds.ArmsLength)} ({AtkHelper.Localize(JobIds.GNB)})", new IconBuffProps {
                Icons = [ActionIds.ArmsLength],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ArmsLength), Duration = 6 }
                ]
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Superbolide), new IconBuffProps {
                Icons = [ActionIds.Superbolide],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Superbolide), Duration = 10 }
                ]
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.HeartOfCorundum), new IconBuffProps {
                Icons =
                [
                    ActionIds.HeartOfStone,
                    ActionIds.HeartOfCorundum
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.HeartOfStone), Duration = 7 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.HeartOfCorundum), Duration = 8 }
                ]
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
