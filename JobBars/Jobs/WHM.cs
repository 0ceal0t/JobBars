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
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.Dia), GaugeVisualType.Bar, new GaugeTimerProps {
                SubTimers = [
                    new GaugeSubTimerProps {
                        MaxDuration = 30,
                        Color = AtkColor.LightBlue,
                        SubName = AtkHelper.Localize(BuffIds.Dia),
                        Triggers = [
                            new Item(BuffIds.Dia),
                            new Item(BuffIds.Aero),
                            new Item(BuffIds.Aero2)
                        ]
                    }
                ]
            }),
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.LilyBell), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 5,
                Triggers = [
                    new Item(BuffIds.LilyBell)
                ],
                Color = AtkColor.BlueGreen
            }),
        ];

        public static BuffConfig[] Buffs => [];

        public static Cursor Cursors => new( JobIds.WHM, CursorType.None, CursorType.CastTime );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig(AtkHelper.Localize(ActionIds.Temperance), new CooldownProps {
                Icon = ActionIds.Temperance,
                Duration = 20,
                CD = 120,
                Triggers = [new Item(ActionIds.Temperance)]
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.Benediction), new CooldownProps {
                Icon = ActionIds.Benediction,
                CD = 180,
                Triggers = [new Item(ActionIds.Benediction)]
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.LilyBell), new CooldownProps {
                Icon = ActionIds.LilyBell,
                Duration = 15,
                CD = 180,
                Triggers = [new Item(ActionIds.LilyBell)]
            }),
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Swiftcast)} ({AtkHelper.Localize(JobIds.WHM)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 60,
                Triggers = [new Item(ActionIds.Swiftcast)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Dia), new IconBuffProps {
                IsTimer = true,
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
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.PresenceOfMind), new IconBuffProps {
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
