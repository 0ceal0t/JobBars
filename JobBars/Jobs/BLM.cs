using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class BLM {
        public static GaugeConfig[] Gauges => [
            new GaugeProcsConfig($"{UiHelper.Localize(JobIds.BLM)} {UiHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps{
                ShowText = true,
                Procs = [
                    new ProcConfig(UiHelper.Localize(BuffIds.Firestarter), BuffIds.Firestarter, ColorConstants.Orange),
                    new ProcConfig(UiHelper.Localize(BuffIds.Thunderhead), BuffIds.Thunderhead, ColorConstants.LightBlue)
                ]
            }),
            new GaugeStacksConfig(UiHelper.Localize(BuffIds.Triplecast), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = [
                    new Item(BuffIds.Triplecast)
                ],
                Color = ColorConstants.MpPink
            }),
            new GaugeTimerConfig(UiHelper.Localize(BuffIds.HighThunder), GaugeVisualType.Bar, new GaugeTimerProps {
                SubTimers = [
                    new GaugeSubTimerProps {
                        MaxDuration = 30,
                        Color = ColorConstants.DarkBlue,
                        SubName = UiHelper.Localize(BuffIds.HighThunder),
                        Triggers = [
                            new Item(BuffIds.Thunder3),
                            new Item(BuffIds.Thunder),
                            new Item(BuffIds.HighThunder),
                        ]
                    },
                    new GaugeSubTimerProps {
                        MaxDuration = 24,
                        Color = ColorConstants.Purple,
                        SubName = UiHelper.Localize(BuffIds.HighThunder2),
                        Triggers = [
                            new Item(BuffIds.Thunder4),
                            new Item(BuffIds.Thunder2),
                            new Item(BuffIds.HighThunder2),
                        ]
                    }
                ]
            })
        ];

        public static BuffConfig[] Buffs => [];

        public static Cursor Cursors => new( JobIds.BLM, CursorType.MpTick, CursorType.CastTime );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Addle)} ({UiHelper.Localize(JobIds.BLM)})", new CooldownProps {
                Icon = ActionIds.Addle,
                Duration = 15,
                CD = 90,
                Triggers = [new Item(ActionIds.Addle)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UiHelper.Localize(BuffIds.HighThunder), new IconBuffProps {
                IconType = IconActionType.Timer,
                Icons = [
                    ActionIds.Thunder,
                    ActionIds.Thunder3,
                    ActionIds.HighThunder,
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Thunder), Duration = 21 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Thunder3), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.HighThunder), Duration = 30 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.HighThunder2), new IconBuffProps {
                IconType = IconActionType.Timer,
                Icons = [
                    ActionIds.Thunder2,
                    ActionIds.Thunder4,
                    ActionIds.HighThunder2,
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Thunder2), Duration = 18 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Thunder4), Duration = 18 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.HighThunder2), Duration = 24 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.LeyLines), new IconBuffProps {
                IconType = IconActionType.Buff,
                Icons = [ActionIds.LeyLines],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.LeyLines), Duration = 30 }
                ]
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => [0.88f]; // 3f4 (with umbral hearts) + 1f4 + 3f4
    }
}
