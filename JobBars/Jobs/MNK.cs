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
using JobBars.Atk;
using System;

namespace JobBars.Jobs {
    public static class MNK {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.PerfectBalance), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.PerfectBalance)
                },
                Color = AtkColor.Orange
            }),
            new GaugeProcsConfig(AtkHelper.Localize(BuffIds.LeadenFist), GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = new []{
                    new ProcConfig(AtkHelper.Localize(BuffIds.LeadenFist), BuffIds.LeadenFist, AtkColor.Yellow)
                },
                ProcSound = GaugeCompleteSoundType.Never
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.DisciplinedFist), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 15,
                Color = AtkColor.PurplePink,
                Triggers = new []{
                    new Item(BuffIds.DisciplinedFist)
                }
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.Demolish), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 18,
                Color = AtkColor.Yellow,
                Triggers = new [] {
                    new Item(BuffIds.Demolish)
                }
            }),
            new GaugeGCDConfig(AtkHelper.Localize(BuffIds.RiddleOfFire), GaugeVisualType.Bar, new GaugeSubGCDProps {
                MaxCounter = 11,
                MaxDuration = 20,
                Color = AtkColor.Red,
                Triggers = new []{
                    new Item(BuffIds.RiddleOfFire)
                }
            }),
            new GaugeChargesConfig($"{AtkHelper.Localize(ActionIds.TrueNorth)} ({AtkHelper.Localize(JobIds.MNK)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = AtkColor.NoColor,
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
            new BuffConfig(AtkHelper.Localize(ActionIds.Brotherhood), new BuffProps {
                CD = 120,
                Duration = 15,
                Icon = ActionIds.Brotherhood,
                Color = AtkColor.Orange,
                Triggers = new []{ new Item(ActionIds.Brotherhood) }
            }),
            new BuffConfig(AtkHelper.Localize(ActionIds.RiddleOfFire), new BuffProps {
                CD = 60,
                Duration = 20,
                Icon = ActionIds.RiddleOfFire,
                Color = AtkColor.Red,
                Triggers = new []{ new Item(ActionIds.RiddleOfFire) }
            })
        };

        public static Cursor Cursors => new(JobIds.MNK, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Feint)} ({AtkHelper.Localize(JobIds.MNK)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Feint) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.Mantra), new CooldownProps {
                Icon = ActionIds.Mantra,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Mantra) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.RiddleOfFire), new IconBuffProps {
                Icons = new [] { ActionIds.RiddleOfFire },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.RiddleOfFire), Duration = 20 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.DisciplinedFist), new IconBuffProps {
                IsTimer = true,
                Icons = new [] { ActionIds.TwinSnakes },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.DisciplinedFist), Duration = 15 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Demolish), new IconBuffProps {
                IsTimer = true,
                Icons = new [] { ActionIds.Demolish },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Demolish), Duration = 18 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
