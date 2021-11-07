using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class BRD {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig($"{UIHelper.Localize(JobIds.BRD)} {UIHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps {
                Procs = new []{
                    new ProcConfig(UIHelper.Localize(BuffIds.StraightShotReady), BuffIds.StraightShotReady, UIColor.Yellow),
                    new ProcConfig(UIHelper.Localize(ActionIds.BloodLetter), ActionIds.BloodLetter, UIColor.Red)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.VenomousBite), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 30,
                Color = UIColor.Purple,
                Triggers = new []{
                    new Item(BuffIds.CausticBite),
                    new Item(BuffIds.VenomousBite)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Stormbite), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 30,
                Color = UIColor.LightBlue,
                Triggers = new []{
                    new Item(BuffIds.Windbite),
                    new Item(BuffIds.Stormbite),
                }
            }),
            new GaugeGCDConfig(UIHelper.Localize(BuffIds.RagingStrikes), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 9,
                MaxDuration = 20,
                Color = UIColor.Orange,
                Triggers = new []{
                    new Item(BuffIds.RagingStrikes)
                },
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(ActionIds.BattleVoice), new BuffProps {
                CD = 180,
                Duration = 20,
                Icon = ActionIds.BattleVoice,
                Color = UIColor.Orange,
                Triggers = new []{ new Item(ActionIds.BattleVoice) }
            }),
            new BuffConfig(UIHelper.Localize(BuffIds.Barrage), new BuffProps {
                CD = 80,
                Duration = 10,
                Icon = ActionIds.Barrage,
                Color = UIColor.Yellow,
                Triggers = new []{ new Item(BuffIds.Barrage) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.RagingStrikes), new BuffProps {
                CD = 80,
                Duration = 20,
                Icon = ActionIds.RagingStrikes,
                Color = UIColor.Yellow,
                Triggers = new []{ new Item(ActionIds.RagingStrikes) }
            })
        };

        public static Cursor Cursors => new(JobIds.BRD, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig(UIHelper.Localize(ActionIds.Troubadour), new CooldownProps {
                Icon = ActionIds.Troubadour,
                Duration = 15,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Troubadour) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.NaturesMinne), new CooldownProps {
                Icon = ActionIds.NaturesMinne,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.NaturesMinne) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.RagingStrikes), new IconProps {
                Icons = new [] { ActionIds.RagingStrikes },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.RagingStrikes), Duration = 20 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.VenomousBite), new IconProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.CausticBite,
                    ActionIds.VenomousBite,
                },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.CausticBite), Duration = 30 },
                    new IconTriggerStruct { Trigger = new Item(BuffIds.VenomousBite), Duration = 30 },
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.Stormbite), new IconProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.Windbite,
                    ActionIds.Stormbite,
                },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Windbite), Duration = 30 },
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Stormbite), Duration = 30 },
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;

        public static bool GCD_ROLL => true;
    }
}
