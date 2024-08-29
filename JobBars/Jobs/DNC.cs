using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.Procs;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class DNC {
        public static GaugeConfig[] Gauges => [
            new GaugeProcsConfig($"{UiHelper.Localize(JobIds.DNC)} {UiHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = [
                    new ProcConfig(UiHelper.Localize(BuffIds.FlourishingSymmetry),
                        [
                            new Item(BuffIds.FlourishingSymmetry),
                            new Item(BuffIds.SilkenSymmetry)
                        ], ColorConstants.BrightGreen),
                    new ProcConfig(UiHelper.Localize(BuffIds.FlourishingFlow),
                        [
                            new Item(BuffIds.FlourishingFlow),
                            new Item(BuffIds.SilkenFlow)
                        ], ColorConstants.DarkBlue),
                    new ProcConfig(UiHelper.Localize(BuffIds.ThreefoldFanDance), BuffIds.ThreefoldFanDance, ColorConstants.HealthGreen),
                    new ProcConfig(UiHelper.Localize(BuffIds.FourfoldFanDance), BuffIds.FourfoldFanDance, ColorConstants.LightBlue),
                    new ProcConfig(UiHelper.Localize(BuffIds.FlourishingStarfall), BuffIds.FlourishingStarfall, ColorConstants.Red)
                ]
            }),
            new GaugeProcsConfig(UiHelper.Localize(BuffIds.FinishingMoveReady), GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = [
                    new ProcConfig(UiHelper.Localize(BuffIds.FinishingMoveReady), BuffIds.FinishingMoveReady, ColorConstants.White)
                ]
            })
        ];

        public static BuffConfig[] Buffs => [
            new BuffConfig(UiHelper.Localize(ActionIds.QuadTechFinish), new BuffProps {
                CD = 115, // -5 seconds for the dance to actually be cast
                Duration = 20,
                Icon = ActionIds.QuadTechFinish,
                Color = ColorConstants.Orange,
                Triggers = [new Item(ActionIds.QuadTechFinish)]
            }),
            new BuffConfig(UiHelper.Localize(ActionIds.Devilment), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.Devilment,
                Color = ColorConstants.BrightGreen,
                Triggers = [new Item(ActionIds.Devilment)]
            })
        ];

        public static Cursor Cursors => new( JobIds.DNC, CursorType.None, CursorType.GCD );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig(UiHelper.Localize(ActionIds.ShieldSamba), new CooldownProps {
                Icon = ActionIds.ShieldSamba,
                Duration = 15,
                CD = 90,
                Triggers = [new Item(ActionIds.ShieldSamba)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.Improvisation), new CooldownProps {
                Icon = ActionIds.Improvisation,
                Duration = 15,
                CD = 120,
                Triggers = [new Item(BuffIds.Improvisation)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.CuringWaltz), new CooldownProps {
                Icon = ActionIds.CuringWaltz,
                CD = 60,
                Triggers = [new Item(ActionIds.CuringWaltz)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UiHelper.Localize(BuffIds.Devilment), new IconBuffProps {
                IconType = IconActionType.Buff,
                Icons = [ActionIds.Devilment],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Devilment), Duration = 20 }
                ]
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
