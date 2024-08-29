using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class WHM {
        public static GaugeConfig[] Gauges => [
            new GaugeTimerConfig(UiHelper.Localize(BuffIds.Dia), GaugeVisualType.Bar, new GaugeTimerProps {
                SubTimers = [
                    new GaugeSubTimerProps {
                        MaxDuration = 30,
                        Color = ColorConstants.LightBlue,
                        SubName = UiHelper.Localize(BuffIds.Dia),
                        Triggers = [
                            new Item(BuffIds.Dia),
                            new Item(BuffIds.Aero),
                            new Item(BuffIds.Aero2)
                        ]
                    }
                ]
            }),
            new GaugeStacksConfig(UiHelper.Localize(BuffIds.LilyBell), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 5,
                Triggers = [
                    new Item(BuffIds.LilyBell)
                ],
                Color = ColorConstants.BlueGreen
            }),
            new GaugeStacksConfig(UiHelper.Localize(BuffIds.SacredSight), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = [
                    new Item(BuffIds.SacredSight)
                ],
                Color = ColorConstants.White
            }),
        ];

        public static BuffConfig[] Buffs => [];

        public static Cursor Cursors => new( JobIds.WHM, CursorType.None, CursorType.CastTime );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig(UiHelper.Localize(ActionIds.Temperance), new CooldownProps {
                Icon = ActionIds.Temperance,
                Duration = 20,
                CD = 120,
                Triggers = [new Item(ActionIds.Temperance)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.Benediction), new CooldownProps {
                Icon = ActionIds.Benediction,
                CD = 180,
                Triggers = [new Item(ActionIds.Benediction)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.LilyBell), new CooldownProps {
                Icon = ActionIds.LilyBell,
                Duration = 15,
                CD = 180,
                Triggers = [new Item(ActionIds.LilyBell)]
            }),
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Swiftcast)} ({UiHelper.Localize(JobIds.WHM)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 40,
                Triggers = [new Item(ActionIds.Swiftcast)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UiHelper.Localize(BuffIds.Dia), new IconBuffProps {
                IconType = IconActionType.Timer,
                Icons = [
                    ActionIds.Aero,
                    ActionIds.Aero2,
                    ActionIds.Dia
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Aero), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Aero2), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Dia), Duration = 30 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.PresenceOfMind), new IconBuffProps {
                IconType = IconActionType.Buff,
                Icons = [ActionIds.PresenceOfMind],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.PresenceOfMind), Duration = 15 }
                ]
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => [0.24f];
    }
}
