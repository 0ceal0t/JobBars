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
            new GaugeProcsConfig(UiHelper.Localize(BuffIds.Excog), GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = [
                    new ProcConfig(UiHelper.Localize(BuffIds.Excog), BuffIds.Excog, ColorConstants.BrightGreen)
                ],
                ProcSound = GaugeCompleteSoundType.When_Empty
            }),
            new GaugeTimerConfig(UiHelper.Localize(BuffIds.Biolysis), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 30,
                Color = ColorConstants.BlueGreen,
                Triggers = [
                    new Item(BuffIds.ArcBio),
                    new Item(BuffIds.ArcBio2),
                    new Item(BuffIds.Biolysis)
                ]
            })
        ];

        public static BuffConfig[] Buffs => [
            new BuffConfig(UiHelper.Localize(ActionIds.ChainStratagem), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.ChainStratagem,
                Color = ColorConstants.White,
                Triggers = [new Item(ActionIds.ChainStratagem)]
            })
        ];

        public static Cursor Cursors => new( JobIds.SCH, CursorType.None, CursorType.CastTime );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig(UiHelper.Localize(ActionIds.Seraphism), new CooldownProps {
                Icon = ActionIds.Seraphism,
                Duration = 20,
                CD = 180,
                Triggers = [new Item(ActionIds.Seraphism )]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.SummonSeraph), new CooldownProps {
                Icon = ActionIds.SummonSeraph,
                Duration = 22,
                CD = 120,
                Triggers = [new Item(ActionIds.SummonSeraph)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.Expedient), new CooldownProps {
                Icon = ActionIds.Expedient,
                Duration = 20,
                CD = 120,
                Triggers = [new Item(ActionIds.Expedient)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.Protraction), new CooldownProps {
                Icon = ActionIds.Protraction,
                Duration = 10,
                CD = 60,
                Triggers = [new Item(ActionIds.Protraction)]
            }),
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Swiftcast)} ({UiHelper.Localize(JobIds.SCH)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 40,
                Triggers = [new Item(ActionIds.Swiftcast)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UiHelper.Localize(BuffIds.Seraphism), new IconBuffProps {
                IconType = IconActionType.Buff,
                Icons = [
                    ActionIds.Seraphism,
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Seraphism), Duration = 20 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(ActionIds.ChainStratagem), new IconBuffProps {
                IconType = IconActionType.Buff,
                Icons = [ActionIds.ChainStratagem],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ChainStratagem), Duration = 20 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.Biolysis), new IconBuffProps {
                IconType = IconActionType.Timer,
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
