using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.Custom.VrpCoil;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class VPR {
        public static GaugeConfig[] Gauges => [
            new GaugeVprCoilConfig("Rattling Coil", GaugeVisualType.Diamond ),

            new GaugeTimerConfig(UiHelper.Localize(BuffIds.HuntersInstinct), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 40,
                DefaultDuration = 40,
                Color = ColorConstants.PurplePink,
                Triggers = [
                    new Item(BuffIds.HuntersInstinct)
                ]
            }),
            new GaugeTimerConfig(UiHelper.Localize(BuffIds.Swiftscaled), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 40,
                DefaultDuration = 40,
                Color = ColorConstants.Yellow,
                Triggers = [
                    new Item(BuffIds.Swiftscaled)
                ]
            }),
            new GaugeChargesConfig($"{UiHelper.Localize(ActionIds.TrueNorth)} ({UiHelper.Localize(JobIds.VPR)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = ColorConstants.NoColor,
                SameColor = true,
                Parts = [
                    new GaugesChargesPartProps {
                        Diamond = true,
                        MaxCharges = 2,
                        CD = 45,
                        Triggers = [
                            new Item(ActionIds.TrueNorth)
                        ]
                    },
                    new GaugesChargesPartProps {
                        Bar = true,
                        Duration = 10,
                        Triggers = [
                            new Item(BuffIds.TrueNorth)
                        ]
                    }
                ],
                CompletionSound = GaugeCompleteSoundType.Never
            })
        ];

        public static BuffConfig[] Buffs => [];

        public static Cursor Cursors => new( JobIds.VPR, CursorType.None, CursorType.GCD );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Feint)} ({UiHelper.Localize(JobIds.NIN)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 15,
                CD = 90,
                Triggers = [new Item(ActionIds.Feint)]
            })
        ];

        public static IconReplacer[] Icons => [];

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
