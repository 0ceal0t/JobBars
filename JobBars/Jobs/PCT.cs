using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.Stacks;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.Gauges.Timer;

namespace JobBars.Jobs {
    public static class PCT {
        public static GaugeConfig[] Gauges => [
            new GaugeStacksConfig(UiHelper.Localize(BuffIds.Hyperphantasia), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 5,
                Triggers = [
                    new Item(BuffIds.Hyperphantasia)
                ],
                Color = ColorConstants.PurplePink
            }),
            new GaugeStacksConfig(UiHelper.Localize(BuffIds.SubtractivePaletee), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = [
                    new Item(BuffIds.SubtractivePaletee)
                ],
                Color = ColorConstants.BlueGreen
            }),
            new GaugeTimerConfig(UiHelper.Localize(BuffIds.HammerTime), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 30,
                Color = ColorConstants.Orange,
                Triggers = [
                    new Item(BuffIds.HammerTime)
                ],
            }),
        ];

        public static BuffConfig[] Buffs => [
            new BuffConfig(UiHelper.Localize(ActionIds.StarryMuse), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.StarryMuse,
                Color = ColorConstants.Purple,
                Triggers = [new Item(ActionIds.StarryMuse )]
            })
        ];

        public static Cursor Cursors => new( JobIds.PCT, CursorType.None, CursorType.CastTime );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Addle)} ({UiHelper.Localize(JobIds.PCT)})", new CooldownProps {
                Icon = ActionIds.Addle,
                Duration = 15,
                CD = 90,
                Triggers = [new Item(ActionIds.Addle)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.TemperaCoat), new CooldownProps {
                Icon = ActionIds.TemperaCoat,
                CD = 120,
                Triggers = [new Item(ActionIds.TemperaCoat )]
            })
        ];

        public static IconReplacer[] Icons => [
            new IconBuffReplacer(UiHelper.Localize(BuffIds.StarryMuse), new IconBuffProps {
                Icons = [
                    ActionIds.StarryMuse,
                    ActionIds.ScenicMuse,
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.StarryMuse), Duration = 20 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.HammerTime), new IconBuffProps {
               Icons = [
                   ActionIds.StrikingMuse,
                   ActionIds.SteelMuse,
                   ActionIds.HammerStamp,
                   ActionIds.HammerBrush,
                   ActionIds.PolishingHammer,
                   ActionIds.HammerMotif,
                   ActionIds.WeaponMotif,
               ],
               Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.HammerTime), Duration = 30 }
               ]
            }),
        ];

        public static bool MP => true;

        public static float[] MP_SEGMENTS => null;
    }
}
