using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Procs;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class SMN {
        public static GaugeConfig[] Gauges => [
            new GaugeProcsConfig(UiHelper.Localize(BuffIds.FurtherRuin), GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = [
                    new ProcConfig(UiHelper.Localize(BuffIds.FurtherRuin), BuffIds.FurtherRuin, ColorConstants.DarkBlue)
                ]
            }),
            new GaugeGCDConfig(UiHelper.Localize(ActionIds.SummonBahamut), GaugeVisualType.Arrow, new GaugeGCDProps {
                SubGCDs = [
                    new GaugeSubGCDProps {
                        MaxCounter = 6,
                        MaxDuration = 15,
                        Color = ColorConstants.LightBlue,
                        SubName = UiHelper.Localize(ActionIds.SummonBahamut),
                        Increment = [
                            new Item(ActionIds.AstralImpulse),
                            new Item(ActionIds.AstralFlare)
                        ],
                        Triggers = [
                            new Item(ActionIds.SummonBahamut)
                        ]
                    },
                    new GaugeSubGCDProps {
                        MaxCounter = 6,
                        MaxDuration = 15,
                        Color = ColorConstants.Orange,
                        SubName = UiHelper.Localize(ActionIds.SummonPhoenix),
                        Increment = [
                            new Item(ActionIds.FountainOfFire),
                            new Item(ActionIds.BrandOfPurgatory)
                        ],
                        Triggers = [
                            new Item(ActionIds.SummonPhoenix)
                        ]
                    },
                    new GaugeSubGCDProps {
                        MaxCounter = 6,
                        MaxDuration = 15,
                        Color = ColorConstants.White,
                        SubName = UiHelper.Localize(ActionIds.SummonSolarBahamut),
                        Increment = [
                            new Item(ActionIds.UmbralImpulse),
                            new Item(ActionIds.UmbralFlare)
                        ],
                        Triggers = [
                            new Item(ActionIds.SummonSolarBahamut)
                        ]
                    },
                ]
            })
        ];

        public static BuffConfig[] Buffs => [
            new BuffConfig(UiHelper.Localize(ActionIds.SearingLight), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.SearingLight,
                Color = ColorConstants.Purple,
                Triggers = [new Item(ActionIds.SearingLight)]
            }),
            new BuffConfig(UiHelper.Localize(ActionIds.SummonBahamut), new BuffProps {
                CD = 120,
                Duration = 15,
                Icon = ActionIds.SummonBahamut,
                Color = ColorConstants.LightBlue,
                Triggers = [new Item(ActionIds.SummonBahamut)]
            }),
            new BuffConfig(UiHelper.Localize(ActionIds.SummonPhoenix), new BuffProps {
                CD = 120,
                Duration = 15,
                Icon = ActionIds.SummonPhoenix,
                Color = ColorConstants.Orange,
                Triggers = [new Item(ActionIds.SummonPhoenix)]
            }),
            new BuffConfig(UiHelper.Localize(ActionIds.SummonSolarBahamut), new BuffProps {
                CD = 60,
                Duration = 15,
                Icon = ActionIds.SummonSolarBahamut,
                Color = ColorConstants.White,
                Triggers = [new Item(ActionIds.SummonSolarBahamut )]
            })
        ];

        public static Cursor Cursors => new( JobIds.SMN, CursorType.None, CursorType.CastTime );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Addle)} ({UiHelper.Localize(JobIds.SMN)})", new CooldownProps {
                Icon = ActionIds.Addle,
                Duration = 15,
                CD = 90,
                Triggers = [new Item(ActionIds.Addle)]
            }),
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Swiftcast)} ({UiHelper.Localize(JobIds.SMN)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 40,
                Triggers = [new Item(ActionIds.Swiftcast)]
            })
        ];

        public static IconReplacer[] Icons => [];

        public static bool MP => true;

        public static float[] MP_SEGMENTS => [0.24f];
    }
}
