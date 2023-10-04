using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.Atk;
using System;

namespace JobBars.Jobs {
    public static class BRD {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig(AtkHelper.Localize(BuffIds.StraightShotReady), GaugeVisualType.Diamond, new GaugeProcProps {
                Procs = new []{
                    new ProcConfig(AtkHelper.Localize(BuffIds.StraightShotReady), BuffIds.StraightShotReady, AtkColor.Yellow)
                }
            }),
            new GaugeChargesConfig(AtkHelper.Localize(ActionIds.BloodLetter), GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                BarColor = AtkColor.Red,
                SameColor = true,
                Parts = new []{
                    new GaugesChargesPartProps {
                        Bar = true,
                        Diamond = true,
                        CD = 15,
                        MaxCharges = 3,
                        Triggers = new[] { new Item(ActionIds.BloodLetter) }
                    }
                }
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.VenomousBite), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 45,
                Color = AtkColor.Purple,
                Triggers = new []{
                    new Item(BuffIds.CausticBite),
                    new Item(BuffIds.VenomousBite)
                }
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.Stormbite), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 45,
                Color = AtkColor.LightBlue,
                Triggers = new []{
                    new Item(BuffIds.Windbite),
                    new Item(BuffIds.Stormbite),
                }
            }),
            new GaugeGCDConfig(AtkHelper.Localize(BuffIds.RagingStrikes), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 8,
                MaxDuration = 20,
                Color = AtkColor.Orange,
                Triggers = new []{
                    new Item(BuffIds.RagingStrikes)
                },
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(AtkHelper.Localize(ActionIds.BattleVoice), new BuffProps {
                CD = 120,
                Duration = 15,
                Icon = ActionIds.BattleVoice,
                Color = AtkColor.Orange,
                Triggers = new []{ new Item(ActionIds.BattleVoice) }
            }),
            new BuffConfig(AtkHelper.Localize(ActionIds.RadiantFinale), new BuffProps {
                CD = 110,
                Duration = 15,
                Icon = ActionIds.RadiantFinale,
                Color = AtkColor.DarkBlue,
                Triggers = new []{ new Item(ActionIds.RadiantFinale) }
            }),
            new BuffConfig(AtkHelper.Localize(BuffIds.Barrage), new BuffProps {
                CD = 120,
                Duration = 10,
                Icon = ActionIds.Barrage,
                Color = AtkColor.Yellow,
                Triggers = new []{ new Item(BuffIds.Barrage) }
            }),
            new BuffConfig(AtkHelper.Localize(ActionIds.RagingStrikes), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.RagingStrikes,
                Color = AtkColor.Yellow,
                Triggers = new []{ new Item(ActionIds.RagingStrikes) }
            })
        };

        public static Cursor Cursors => new(JobIds.BRD, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig(AtkHelper.Localize(ActionIds.Troubadour), new CooldownProps {
                Icon = ActionIds.Troubadour,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Troubadour) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.NaturesMinne), new CooldownProps {
                Icon = ActionIds.NaturesMinne,
                Duration = 15,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.NaturesMinne) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.RagingStrikes), new IconBuffProps {
                Icons = new [] { ActionIds.RagingStrikes },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.RagingStrikes), Duration = 15 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.BattleVoice), new IconBuffProps {
                Icons = new [] { ActionIds.BattleVoice },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.BattleVoice), Duration = 15 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.RadiantFinale), new IconBuffProps {
                Icons = new [] { ActionIds.RadiantFinale },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.RadiantFinale), Duration = 15 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.VenomousBite), new IconBuffProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.CausticBite,
                    ActionIds.VenomousBite,
                },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.CausticBite), Duration = 45 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.VenomousBite), Duration = 45 },
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Stormbite), new IconBuffProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.Windbite,
                    ActionIds.Stormbite,
                },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Windbite), Duration = 45 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Stormbite), Duration = 45 },
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
