using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.Custom;
using JobBars.Gauges.Stacks;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class DRK {
        public static GaugeConfig[] Gauges => [
            new GaugeDrkMPConfig($"MP ({UiHelper.Localize(JobIds.DRK)})", GaugeVisualType.BarDiamondCombo, new GaugeDrkMpProps {
                Color = ColorConstants.Purple,
                DarkArtsColor = ColorConstants.LightBlue,
                Segments = [0.3f, 0.6f, 0.9f]
            }),
            new GaugeStacksConfig(UiHelper.Localize(BuffIds.Delirium), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = [
                    new Item(BuffIds.Delirium),
                    new Item(BuffIds.EnhancedDelirium)
                ],
                Color = ColorConstants.Red
            })
        ];

        public static BuffConfig[] Buffs => [
            new BuffConfig(UiHelper.Localize(BuffIds.Delirium), new BuffProps {
                CD = 60,
                Duration = 15,
                Icon = ActionIds.Delirium,
                Color = ColorConstants.Red,
                Triggers = [
                    new Item(BuffIds.Delirium),
                    new Item(BuffIds.EnhancedDelirium)
                ]
            }),
            new BuffConfig(UiHelper.Localize(ActionIds.LivingShadow), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.LivingShadow,
                Color = ColorConstants.Purple,
                Triggers = [new Item(ActionIds.LivingShadow)]
            })
        ];

        public static Cursor Cursors => new( JobIds.DRK, CursorType.None, CursorType.GCD );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig(UiHelper.Localize(ActionIds.LivingDead), new CooldownProps {
                Icon = ActionIds.LivingDead,
                Duration = 10,
                CD = 300,
                Triggers = [new Item(BuffIds.LivingDead)]
            }),
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Reprisal)} ({UiHelper.Localize(JobIds.DRK)})", new CooldownProps {
                Icon = ActionIds.Reprisal,
                Duration = 15,
                CD = 60,
                Triggers = [new Item(ActionIds.Reprisal)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.DarkMissionary), new CooldownProps {
                Icon = ActionIds.DarkMissionary,
                Duration = 15,
                CD = 90,
                Triggers = [new Item(ActionIds.DarkMissionary)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.TheBlackestNight), new CooldownProps {
                Icon = ActionIds.TheBlackestNight,
                Duration = 7,
                CD = 15,
                Triggers = [new Item(ActionIds.TheBlackestNight)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer($"{UiHelper.Localize(ActionIds.Rampart)} ({UiHelper.Localize(JobIds.DRK)})", new IconBuffProps {
                Icons = [ActionIds.Rampart],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Rampart), Duration = 20 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.ShadowedVigil), new IconBuffProps {
                Icons = [
                    ActionIds.ShadowWall,
                    ActionIds.ShadowedVigil
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ShadowWall), Duration = 15 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ShadowedVigil), Duration = 15 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.DarkMind), new IconBuffProps {
                Icons = [ActionIds.DarkMind],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.DarkMind), Duration = 10 }
                ]
            }),
            new IconBuffReplacer($"{UiHelper.Localize(ActionIds.ArmsLength)} ({UiHelper.Localize(JobIds.DRK)})", new IconBuffProps {
                Icons = [ActionIds.ArmsLength],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ArmsLength), Duration = 6 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.LivingDead), new IconBuffProps {
                Icons = [ActionIds.LivingDead],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.LivingDead), Duration = 10 }
                ]
            })
        };

        // DRK HAS A CUSTOM MP BAR, SO DON'T WORRY ABOUT THIS
        public static bool MP => false;
        public static float[] MP_SEGMENTS => null;
    }
}
