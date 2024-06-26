using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class SCH {
        public static GaugeConfig[] Gauges => [
            new GaugeProcsConfig(AtkHelper.Localize(BuffIds.Excog), GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = [
                    new ProcConfig(AtkHelper.Localize(BuffIds.Excog), BuffIds.Excog, AtkColor.BrightGreen)
                ],
                ProcSound = GaugeCompleteSoundType.When_Empty
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.Biolysis), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 30,
                Color = AtkColor.BlueGreen,
                Triggers = [
                    new Item(BuffIds.ArcBio),
                    new Item(BuffIds.ArcBio2),
                    new Item(BuffIds.Biolysis)
                ]
            })
        ];

        public static BuffConfig[] Buffs => [
            new BuffConfig(AtkHelper.Localize(ActionIds.ChainStratagem), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.ChainStratagem,
                Color = AtkColor.White,
                Triggers = [new Item(ActionIds.ChainStratagem)]
            })
        ];

        public static Cursor Cursors => new( JobIds.SCH, CursorType.None, CursorType.CastTime );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig(AtkHelper.Localize(ActionIds.Seraphism), new CooldownProps {
                Icon = ActionIds.Seraphism,
                Duration = 20,
                CD = 180,
                Triggers = [new Item(ActionIds.Seraphism )]
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.SummonSeraph), new CooldownProps {
                Icon = ActionIds.SummonSeraph,
                Duration = 22,
                CD = 120,
                Triggers = [new Item(ActionIds.SummonSeraph)]
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.Expedient), new CooldownProps {
                Icon = ActionIds.Expedient,
                Duration = 20,
                CD = 120,
                Triggers = [new Item(ActionIds.Expedient)]
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.Protraction), new CooldownProps {
                Icon = ActionIds.Protraction,
                Duration = 10,
                CD = 60,
                Triggers = [new Item(ActionIds.Protraction)]
            }),
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Swiftcast)} ({AtkHelper.Localize(JobIds.SCH)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 40,
                Triggers = [new Item(ActionIds.Swiftcast)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Seraphism), new IconBuffProps {
                Icons = [
                    ActionIds.Seraphism,
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Seraphism), Duration = 20 }
                ]
            }),
            new IconBuffReplacer(AtkHelper.Localize(ActionIds.ChainStratagem), new IconBuffProps {
                Icons = [ActionIds.ChainStratagem],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ChainStratagem), Duration = 20 }
                ]
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Biolysis), new IconBuffProps {
                IsTimer = true,
                Icons = [
                    ActionIds.SchBio,
                    ActionIds.SchBio2,
                    ActionIds.Biolysis
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ArcBio), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ArcBio2), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Biolysis), Duration = 30 }
                ]
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => [0.24f];
    }
}
