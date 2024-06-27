using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class VPR {
        public static GaugeConfig[] Gauges => [
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.HuntersInstinct), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 40,
                DefaultDuration = 40,
                Color = AtkColor.PurplePink,
                Triggers = [
                    new Item(BuffIds.HuntersInstinct)
                ]
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.NoxiousGash), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 40,
                DefaultDuration = 40,
                Color = AtkColor.Red,
                Triggers = [
                    new Item(BuffIds.NoxiousGash)
                ]
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.Swiftscaled), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 40,
                DefaultDuration = 40,
                Color = AtkColor.Yellow,
                Triggers = [
                    new Item(BuffIds.Swiftscaled)
                ]
            }),
        ];

        public static BuffConfig[] Buffs => [];

        public static Cursor Cursors => new( JobIds.VPR, CursorType.None, CursorType.GCD );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Feint)} ({AtkHelper.Localize(JobIds.NIN)})", new CooldownProps {
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
