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
    public static class BLU {
        public static GaugeConfig[] Gauges => [
            new GaugeProcsConfig($"{UiHelper.Localize(JobIds.BLU)} {UiHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = [
                    new ProcConfig(UiHelper.Localize(BuffIds.AstralAttenuation), BuffIds.AstralAttenuation, ColorConstants.NoColor),
                    new ProcConfig(UiHelper.Localize(BuffIds.UmbralAttenuation), BuffIds.UmbralAttenuation, ColorConstants.DarkBlue),
                    new ProcConfig(UiHelper.Localize(BuffIds.PhysicalAttenuation), BuffIds.PhysicalAttenuation, ColorConstants.Orange)
                ]
            }),
            new GaugeTimerConfig(UiHelper.Localize(BuffIds.BluBleed), GaugeVisualType.Bar, new GaugeSubTimerProps
            {
                MaxDuration = 60,
                Color = ColorConstants.Red,
                Triggers = [
                    new Item(BuffIds.BluBleed)
                ]
            }),
            new GaugeTimerConfig(UiHelper.Localize(BuffIds.Poison), GaugeVisualType.Bar, new GaugeSubTimerProps
            {
                MaxDuration = 15,
                Color = ColorConstants.HealthGreen,
                Triggers = [
                    new Item(BuffIds.Poison)
                ]
            })
        ];

        public static BuffConfig[] Buffs => [
            new BuffConfig(UiHelper.Localize(ActionIds.OffGuard), new BuffProps {
                CD = 60,
                Duration = 15,
                Icon = ActionIds.OffGuard,
                Color = ColorConstants.BrightGreen,
                Triggers = [new Item(ActionIds.OffGuard)]
            }),
            new BuffConfig(UiHelper.Localize(ActionIds.PeculiarLight), new BuffProps {
                CD = 60,
                Duration = 15,
                Icon = ActionIds.PeculiarLight,
                Color = ColorConstants.Red,
                Triggers = [new Item(ActionIds.PeculiarLight)]
            })
        ];

        public static Cursor Cursors => new( JobIds.BLU, CursorType.None, CursorType.CastTime );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Addle)} ({UiHelper.Localize(JobIds.BLU)})", new CooldownProps {
                Icon = ActionIds.Addle,
                Duration = 10,
                CD = 90,
                Triggers = [new Item(ActionIds.Addle)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.AngelWhisper), new CooldownProps {
                Icon = ActionIds.AngelWhisper,
                CD = 300,
                Triggers = [new Item(ActionIds.AngelWhisper)]
            }),
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Swiftcast)} ({UiHelper.Localize(JobIds.BLU)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 60,
                Triggers = [new Item(ActionIds.Swiftcast)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UiHelper.Localize(BuffIds.BluBleed), new IconBuffProps {
                IconType = IconActionType.Timer,
                Icons = [ActionIds.SongOfTorment],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.BluBleed), Duration = 60 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.Poison), new IconBuffProps {
                IconType = IconActionType.Timer,
                Icons = [ActionIds.BadBreath],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Poison), Duration = 15 }
                ]
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => [0.24f];
    }
}
