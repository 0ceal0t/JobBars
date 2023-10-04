using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Stacks;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.Atk;
using System;

namespace JobBars.Jobs {
    public static class NIN {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.RaijuReady), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.RaijuReady)
                },
                Color = AtkColor.PurplePink
            }),
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.Bunshin), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 5,
                Triggers = new []{
                    new Item(BuffIds.Bunshin)
                },
                Color = AtkColor.Red
            }),
            new GaugeChargesConfig($"{AtkHelper.Localize(ActionIds.TrueNorth)} ({AtkHelper.Localize(JobIds.NIN)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = AtkColor.NoColor,
                SameColor = true,
                Parts = new []{
                    new GaugesChargesPartProps {
                        Diamond = true,
                        MaxCharges = 2,
                        CD = 45,
                        Triggers = new []{
                            new Item(ActionIds.TrueNorth)
                        }
                    },
                    new GaugesChargesPartProps {
                        Bar = true,
                        Duration = 10,
                        Triggers = new []{
                            new Item(BuffIds.TrueNorth)
                        }
                    }
                },
                CompletionSound = GaugeCompleteSoundType.Never
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(AtkHelper.Localize(ActionIds.Mug), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.Mug,
                Color = AtkColor.LightBlue,
                Triggers = new []{ new Item(ActionIds.Mug) }
            }),
            new BuffConfig(AtkHelper.Localize(ActionIds.TrickAttack), new BuffProps {
                CD = 60,
                Duration = 15,
                Icon = ActionIds.TrickAttack,
                Color = AtkColor.Yellow,
                Triggers = new []{ new Item(ActionIds.TrickAttack) }
            }),
            new BuffConfig(AtkHelper.Localize(BuffIds.Bunshin), new BuffProps {
                CD = 90,
                Duration = 30,
                Icon = ActionIds.Bunshin,
                Color = AtkColor.Orange,
                Triggers = new []{ new Item(BuffIds.Bunshin) }
            })
        };

        public static Cursor Cursors => new(JobIds.NIN, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Feint)} ({AtkHelper.Localize(JobIds.NIN)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Feint) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.TrickAttack), new IconBuffProps {
                Icons = new [] { ActionIds.TrickAttack },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.TrickAttack), Duration = 15 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Mug), new IconBuffProps {
                Icons = new [] { ActionIds.Mug },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Mug), Duration = 20 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
