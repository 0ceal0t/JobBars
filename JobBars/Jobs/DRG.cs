using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.GCD;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class DRG {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeGCDConfig(UIHelper.Localize(BuffIds.LanceCharge), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 8,
                MaxDuration = 20,
                Color = UIColor.Red,
                Triggers = new []{
                    new Item(BuffIds.LanceCharge)
                },
            }),
            new GaugeGCDConfig(UIHelper.Localize(BuffIds.RightEye), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 8,
                MaxDuration = 20,
                Color = UIColor.Orange,
                Triggers = new []{
                    new Item(BuffIds.RightEye),
                    new Item(BuffIds.RightEye2)
                }
            }),
            new GaugeChargesConfig($"{UIHelper.Localize(ActionIds.TrueNorth)} ({UIHelper.Localize(JobIds.DRG)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
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
            new BuffConfig(UIHelper.Localize(ActionIds.DragonSight), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.DragonSight,
                Color = UIColor.Orange,
                Triggers = new []{ new Item(ActionIds.DragonSight) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.BattleLitany), new BuffProps {
                CD = 120,
                Duration = 15,
                Icon = ActionIds.BattleLitany,
                Color = UIColor.LightBlue,
                Triggers = new []{ new Item(ActionIds.BattleLitany) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.LanceCharge), new BuffProps {
                CD = 60,
                Duration = 20,
                Icon = ActionIds.LanceCharge,
                Color = UIColor.Red,
                Triggers = new []{ new Item(ActionIds.LanceCharge) }
            })
        };

        public static Cursor Cursors => new(JobIds.DRG, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.DRG)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Feint) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.LanceCharge), new IconProps {
                Icons = new [] { ActionIds.LanceCharge },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.LanceCharge), Duration = 20 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.RightEye), new IconProps {
                Icons = new [] { ActionIds.DragonSight },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.RightEye), Duration = 20 },
                    new IconTriggerStruct { Trigger = new Item(BuffIds.RightEye2), Duration = 20 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.PowerSurge), new IconProps {
                IsTimer = true,
                Icons = new [] { ActionIds.Disembowel },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.PowerSurge), Duration = 24 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.ChaosThrust), new IconProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.ChaosThrust,
                    ActionIds.ChaoticSpring
                },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.ChaosThrust), Duration = 24 },
                    new IconTriggerStruct { Trigger = new Item(BuffIds.ChaoticSpring), Duration = 24 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;

        public static bool GCD_ROLL => true;
    }
}
