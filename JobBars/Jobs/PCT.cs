using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.Stacks;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class PCT {
        public static GaugeConfig[] Gauges => [
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.Hyperphantasia), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 5,
                Triggers = [
                    new Item(BuffIds.Hyperphantasia)
                ],
                Color = AtkColor.PurplePink
            }),
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.SubtractivePaletee), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = [
                    new Item(BuffIds.SubtractivePaletee)
                ],
                Color = AtkColor.BlueGreen
            }),
        ];

        public static BuffConfig[] Buffs => [
            new BuffConfig(AtkHelper.Localize(ActionIds.StarryMuse), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.StarryMuse,
                Color = AtkColor.Purple,
                Triggers = [new Item(ActionIds.StarryMuse )]
            })
        ];

        public static Cursor Cursors => new( JobIds.PCT, CursorType.None, CursorType.CastTime );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Addle)} ({AtkHelper.Localize(JobIds.PCT)})", new CooldownProps {
                Icon = ActionIds.Addle,
                Duration = 15,
                CD = 90,
                Triggers = [new Item(ActionIds.Addle)]
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.TemperaCoat), new CooldownProps {
                Icon = ActionIds.TemperaCoat,
                CD = 120,
                Triggers = [new Item(ActionIds.TemperaCoat )]
            })
        ];

        public static IconReplacer[] Icons => [
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.StarryMuse), new IconBuffProps {
                Icons = [
                    ActionIds.StarryMuse
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.StarryMuse), Duration = 20 }
                ]
            }),
        ];

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
