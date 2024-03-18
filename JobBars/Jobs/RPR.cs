using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class RPR {
        public static GaugeConfig[] Gauges => [
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.SoulReaver), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 2,
                Triggers = [
                    new Item(BuffIds.SoulReaver),
                    new Item(BuffIds.SoulReaver2)
                ],
                Color = AtkColor.Red
            }),
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.ImmortalSacrifice), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 8,
                Triggers = [
                    new Item(BuffIds.ImmortalSacrifice)
                ],
                Color = AtkColor.PurplePink
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.BloodsownCircle), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 6,
                DefaultDuration = 30,
                Color = AtkColor.BlueGreen,
                HideLowWarning = true,
                Triggers = [
                    new Item(BuffIds.BloodsownCircle)
                ]
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.DeathsDesign), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 60,
                DefaultDuration = 30,
                Color = AtkColor.Purple,
                Triggers = [
                    new Item(BuffIds.DeathsDesign)
                ]
            }),
            new GaugeChargesConfig($"{AtkHelper.Localize(ActionIds.TrueNorth)} ({AtkHelper.Localize(JobIds.RPR)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = AtkColor.NoColor,
                SameColor = true,
                Parts = [
                    new GaugesChargesPartProps {
                        Diamond = true,
                        MaxCharges = 2,
                        CD = 45,
                        Triggers = [new Item(ActionIds.TrueNorth)]
                    },
                    new GaugesChargesPartProps {
                        Bar = true,
                        Duration = 10,
                        Triggers = [new Item(BuffIds.TrueNorth)]
                    }
                ],
                CompletionSound = GaugeCompleteSoundType.Never
            })
        ];

        public static BuffConfig[] Buffs => [
            new BuffConfig(AtkHelper.Localize(ActionIds.ArcaneCircle), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.ArcaneCircle,
                Color = AtkColor.Red,
                Triggers = [new Item(ActionIds.ArcaneCircle)]
            })
        ];

        public static Cursor Cursors => new( JobIds.RPR, CursorType.None, CursorType.GCD );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Feint)} ({AtkHelper.Localize(JobIds.RPR)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 10,
                CD = 90,
                Triggers = [new Item(ActionIds.Feint)]
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.ArcaneCrest), new CooldownProps {
                Icon = ActionIds.ArcaneCrest,
                CD = 30,
                Triggers = [new Item(ActionIds.ArcaneCrest)]
            })
        ];

        public static IconReplacer[] Icons => [
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.DeathsDesign), new IconBuffProps {
                IsTimer = true,
                Icons = [ActionIds.ShadowOfDeath],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.DeathsDesign), Duration = 60 }
                ]
            })
        ];

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
