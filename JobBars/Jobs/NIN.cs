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
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class NIN {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.Bunshin), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 5,
                Triggers = new []{
                    new Item(BuffIds.Bunshin)
                },
                Color = UIColor.Red
            }),
            new GaugeChargesConfig($"{UIHelper.Localize(ActionIds.TrueNorth)} ({UIHelper.Localize(JobIds.NIN)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = UIColor.NoColor,
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
            new BuffConfig(UIHelper.Localize(ActionIds.TrickAttack), new BuffProps {
                CD = 60,
                Duration = 15,
                Icon = ActionIds.TrickAttack,
                Color = UIColor.Yellow,
                Triggers = new []{ new Item(ActionIds.TrickAttack) }
            }),
            new BuffConfig(UIHelper.Localize(BuffIds.Bunshin), new BuffProps {
                CD = 90,
                Duration = 30,
                Icon = ActionIds.Bunshin,
                Color = UIColor.Orange,
                Triggers = new []{ new Item(BuffIds.Bunshin) }
            })
        };

        public static Cursor Cursors => new(JobIds.NIN, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.SecondWind)} ({UIHelper.Localize(JobIds.NIN)})", new CooldownProps {
                Icon = ActionIds.SecondWind,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.SecondWind) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.LegSweep)} ({UIHelper.Localize(JobIds.NIN)})", new CooldownProps {
                Icon = ActionIds.LegSweep,
                Duration = 3,
                CD = 40,
                Triggers = new []{ new Item(ActionIds.LegSweep) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Bloodbath)} ({UIHelper.Localize(JobIds.NIN)})", new CooldownProps {
                Icon = ActionIds.Bloodbath,
                Duration = 20,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Bloodbath) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.NIN)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Feint) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.ArmsLength)} ({UIHelper.Localize(JobIds.NIN)})", new CooldownProps {
                Icon = ActionIds.ArmsLength,
                Duration = 6,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.ArmsLength) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.TrickAttack), new IconProps {
                Icons = new [] { ActionIds.TrickAttack },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.TrickAttack), Duration = 15 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;

        public static bool GCD_ROLL => true;
    }
}
