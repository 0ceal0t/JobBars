using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Stacks;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class RDM {
        public static GaugeConfig[] Gauges => [
            new GaugeProcsConfig($"{UiHelper.Localize(JobIds.RDM)} {UiHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = [
                    new ProcConfig(UiHelper.Localize(BuffIds.VerstoneReady), BuffIds.VerstoneReady, ColorConstants.NoColor),
                    new ProcConfig(UiHelper.Localize(BuffIds.VerfireReady), BuffIds.VerfireReady, ColorConstants.Red)
                ]
            }),
            new GaugeStacksConfig(UiHelper.Localize(BuffIds.Manafication), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 6,
                Triggers = [
                    new Item(BuffIds.Manafication)
                ],
                Color = ColorConstants.DarkBlue
            })
        ];

        public static BuffConfig[] Buffs => [
            new BuffConfig(UiHelper.Localize(ActionIds.Manafication), new BuffProps {
                CD = 110,
                Duration = 15,
                Icon = ActionIds.Manafication,
                Color = ColorConstants.DarkBlue,
                Triggers = [new Item(ActionIds.Manafication)]
            }),
            new BuffConfig(UiHelper.Localize(ActionIds.Embolden), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.Embolden,
                Color = ColorConstants.White,
                Triggers = [new Item(ActionIds.Embolden)]
            })
        ];

        public static Cursor Cursors => new( JobIds.RDM, CursorType.None, CursorType.CastTime );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Addle)} ({UiHelper.Localize(JobIds.RDM)})", new CooldownProps {
                Icon = ActionIds.Addle,
                Duration = 15,
                CD = 90,
                Triggers = [new Item(ActionIds.Addle)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.MagickBarrier), new CooldownProps {
                Icon = ActionIds.MagickBarrier,
                Duration = 10,
                CD = 120,
                Triggers = [new Item(ActionIds.MagickBarrier)]
            })
        ];

        public static IconReplacer[] Icons => [];

        public static bool MP => true;

        public static float[] MP_SEGMENTS => [0.24f];
    }
}
