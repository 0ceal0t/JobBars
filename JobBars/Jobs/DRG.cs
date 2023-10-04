using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.Atk;
using System;

namespace JobBars.Jobs {
    public static class DRG {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeGCDConfig(AtkHelper.Localize(BuffIds.LanceCharge), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 8,
                MaxDuration = 20,
                Color = AtkColor.Red,
                Triggers = new []{
                    new Item(BuffIds.LanceCharge)
                }
            }),
            new GaugeGCDConfig(AtkHelper.Localize(BuffIds.RightEye), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 8,
                MaxDuration = 20,
                Color = AtkColor.Orange,
                Triggers = new []{
                    new Item(BuffIds.RightEye),
                    new Item(BuffIds.RightEye2)
                }
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.ChaoticSpring), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 24,
                Color = AtkColor.Purple,
                Triggers = new [] {
                    new Item(BuffIds.ChaosThrust),
                    new Item(BuffIds.ChaoticSpring)
                }
            }),
            new GaugeChargesConfig($"{AtkHelper.Localize(ActionIds.TrueNorth)} ({AtkHelper.Localize(JobIds.DRG)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
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
            new BuffConfig(AtkHelper.Localize(ActionIds.DragonSight), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.DragonSight,
                Color = AtkColor.Orange,
                Triggers = new []{ new Item(ActionIds.DragonSight) }
            }),
            new BuffConfig(AtkHelper.Localize(ActionIds.BattleLitany), new BuffProps {
                CD = 120,
                Duration = 15,
                Icon = ActionIds.BattleLitany,
                Color = AtkColor.LightBlue,
                Triggers = new []{ new Item(ActionIds.BattleLitany) }
            }),
            new BuffConfig(AtkHelper.Localize(ActionIds.LanceCharge), new BuffProps {
                CD = 60,
                Duration = 20,
                Icon = ActionIds.LanceCharge,
                Color = AtkColor.Red,
                Triggers = new []{ new Item(ActionIds.LanceCharge) }
            })
        };

        public static Cursor Cursors => new(JobIds.DRG, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Feint)} ({AtkHelper.Localize(JobIds.DRG)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Feint) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.LanceCharge), new IconBuffProps {
                Icons = new [] { ActionIds.LanceCharge },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.LanceCharge), Duration = 20 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.RightEye), new IconBuffProps {
                Icons = new [] { ActionIds.DragonSight },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.RightEye), Duration = 20 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.RightEye2), Duration = 20 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.PowerSurge), new IconBuffProps {
                IsTimer = true,
                Icons = new [] { ActionIds.Disembowel },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.PowerSurge), Duration = 24 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.ChaosThrust), new IconBuffProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.ChaosThrust,
                    ActionIds.ChaoticSpring
                },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ChaosThrust), Duration = 24 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ChaoticSpring), Duration = 24 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
