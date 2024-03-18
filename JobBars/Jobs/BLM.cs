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
            new GaugeProcsConfig($"{AtkHelper.Localize(JobIds.BLM)} {AtkHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps{
                ShowText = true,
                Procs = [
                    new ProcConfig(AtkHelper.Localize(BuffIds.Firestarter), BuffIds.Firestarter, AtkColor.Orange),
                    new ProcConfig(AtkHelper.Localize(BuffIds.Thundercloud), BuffIds.Thundercloud, AtkColor.LightBlue)
                ]
            }),
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.Triplecast), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = [
                    new Item(BuffIds.Triplecast)
                ],
                Color = AtkColor.MpPink
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.Thunder3), GaugeVisualType.Bar, new GaugeTimerProps {
                SubTimers = [
                    new GaugeSubTimerProps {
                        MaxDuration = 30,
                        Color = AtkColor.DarkBlue,
                        SubName = AtkHelper.Localize(BuffIds.Thunder3),
                        Triggers = [
                            new Item(BuffIds.Thunder3),
                            new Item(BuffIds.Thunder)
                        ]
                    },
                    new GaugeSubTimerProps {
                        MaxDuration = 18,
                        Color = AtkColor.Purple,
                        SubName = AtkHelper.Localize(BuffIds.Thunder4),
                        Triggers = [
                            new Item(BuffIds.Thunder4),
                            new Item(BuffIds.Thunder2)
                        ]
                    }
                ]
            })
        ];

        public static BuffConfig[] Buffs => [];

        public static Cursor Cursors => new( JobIds.BLM, CursorType.MpTick, CursorType.CastTime );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Addle)} ({AtkHelper.Localize(JobIds.BLM)})", new CooldownProps {
                Icon = ActionIds.Addle,
                Duration = 10,
                CD = 90,
                Triggers = [new Item(ActionIds.Addle)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Thunder3), new IconBuffProps {
                IsTimer = true,
                Icons = [
                    ActionIds.Thunder,
                    ActionIds.Thunder3
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Thunder), Duration = 21 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Thunder3), Duration = 30 }
                ]
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Thunder4), new IconBuffProps {
                IsTimer = true,
                Icons = [
                    ActionIds.Thunder2,
                    ActionIds.Thunder4
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Thunder2), Duration = 18 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Thunder4), Duration = 18 }
                ]
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.LeyLines), new IconBuffProps {
                Icons = [ActionIds.LeyLines],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.LeyLines), Duration = 30 }
                ]
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Sharpcast), new IconBuffProps {
                Icons = [ActionIds.Sharpcast],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Sharpcast), Duration = 30 }
                ]
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => [0.88f]; // 3f4 (with umbral hearts) + 1f4 + 3f4
    }
}
