using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class MNK {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.PerfectBalance), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.PerfectBalance)
                },
                Color = UIColor.Orange
            }),
            new GaugeProcsConfig(UIHelper.Localize(BuffIds.LeadenFist), GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = new []{
                    new ProcConfig(UIHelper.Localize(BuffIds.LeadenFist), BuffIds.LeadenFist, UIColor.Yellow)
                },
                NoSoundOnProc = true
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.DisciplinedFist), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 15,
                Color = UIColor.PurplePink,
                Triggers = new []{
                    new Item(BuffIds.DisciplinedFist)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Demolish), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 18,
                Color = UIColor.Yellow,
                Triggers = new [] {
                    new Item(BuffIds.Demolish)
                }
            }),
            new GaugeGCDConfig(UIHelper.Localize(BuffIds.RiddleOfFire), GaugeVisualType.Bar, new GaugeSubGCDProps {
                MaxCounter = 11,
                MaxDuration = 20,
                Color = UIColor.Red,
                Triggers = new []{
                    new Item(BuffIds.RiddleOfFire)
                }
            }),
            new GaugeChargesConfig($"{UIHelper.Localize(ActionIds.TrueNorth)} ({UIHelper.Localize(JobIds.MNK)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = UIColor.NoColor,
                SameColor = true,
                Parts = new []{
                    new GaugesChargesPartProps {
                        Diamond = true,
                        MaxCharges = 2,
                        CD = 45,
                        Triggers = new []{  new Item(ActionIds.TrueNorth) }
                    },
                    new GaugesChargesPartProps {
                        Bar = true,
                        Duration = 10,
                        Triggers = new []{ new Item(BuffIds.TrueNorth) }
                    }
                },
                CompletionSound = GaugeCompleteSoundType.Never
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(ActionIds.Brotherhood), new BuffProps {
                CD = 120,
                Duration = 15,
                Icon = ActionIds.Brotherhood,
                Color = UIColor.Orange,
                Triggers = new []{ new Item(ActionIds.Brotherhood) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.RiddleOfFire), new BuffProps {
                CD = 60,
                Duration = 20,
                Icon = ActionIds.RiddleOfFire,
                Color = UIColor.Red,
                Triggers = new []{ new Item(ActionIds.RiddleOfFire) }
            })
        };

        public static Cursor Cursors => new(JobIds.MNK, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.SecondWind)} ({UIHelper.Localize(JobIds.MNK)})", new CooldownProps {
                Icon = ActionIds.SecondWind,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.SecondWind) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.LegSweep)} ({UIHelper.Localize(JobIds.MNK)})", new CooldownProps {
                Icon = ActionIds.LegSweep,
                Duration = 3,
                CD = 40,
                Triggers = new []{ new Item(ActionIds.LegSweep) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Bloodbath)} ({UIHelper.Localize(JobIds.MNK)})", new CooldownProps {
                Icon = ActionIds.Bloodbath,
                Duration = 20,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Bloodbath) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.MNK)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Feint) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.ArmsLength)} ({UIHelper.Localize(JobIds.MNK)})", new CooldownProps {
                Icon = ActionIds.ArmsLength,
                Duration = 6,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.ArmsLength) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Mantra), new CooldownProps {
                Icon = ActionIds.Mantra,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Mantra) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.RiddleOfFire), new IconProps {
                Icons = new [] { ActionIds.RiddleOfFire },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.RiddleOfFire), Duration = 20 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.DisciplinedFist), new IconProps {
                IsTimer = true,
                Icons = new [] { ActionIds.TwinSnakes },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.DisciplinedFist), Duration = 15 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.Demolish), new IconProps {
                IsTimer = true,
                Icons = new [] { ActionIds.Demolish },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Demolish), Duration = 18 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;

        public static bool GCD_ROLL => true;
    }
}
