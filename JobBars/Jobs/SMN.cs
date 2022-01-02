using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class SMN {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig(UIHelper.Localize(BuffIds.FurtherRuin), GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = new []{
                    new ProcConfig(UIHelper.Localize(BuffIds.FurtherRuin), BuffIds.FurtherRuin, UIColor.DarkBlue)
                }
            }),
            new GaugeGCDConfig(UIHelper.Localize(ActionIds.SummonBahamut), GaugeVisualType.Arrow, new GaugeGCDProps {
                SubGCDs = new [] {
                    new GaugeSubGCDProps {
                        MaxCounter = 6,
                        MaxDuration = 15,
                        Color = UIColor.LightBlue,
                        SubName = UIHelper.Localize(ActionIds.SummonBahamut),
                        Increment = new []{
                            new Item(ActionIds.AstralImpulse),
                            new Item(ActionIds.AstralFlare)
                        },
                        Triggers = new []{
                            new Item(ActionIds.SummonBahamut)
                        }
                    },
                    new GaugeSubGCDProps {
                        MaxCounter = 6,
                        MaxDuration = 15,
                        Color = UIColor.Orange,
                        SubName = UIHelper.Localize(ActionIds.SummonPhoenix),
                        Increment = new []{
                            new Item(ActionIds.FountainOfFire),
                            new Item(ActionIds.BrandOfPurgatory)
                        },
                        Triggers = new []{
                            new Item(ActionIds.SummonPhoenix)
                        }
                    }
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(ActionIds.SearingLight), new BuffProps {
                CD = 120,
                Duration = 30,
                Icon = ActionIds.SearingLight,
                Color = UIColor.Purple,
                Triggers = new []{ new Item(ActionIds.SearingLight) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.SummonBahamut), new BuffProps {
                CD = 120,
                Duration = 15,
                Icon = ActionIds.SummonBahamut,
                Color = UIColor.LightBlue,
                Triggers = new []{ new Item(ActionIds.SummonBahamut) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.SummonPhoenix), new BuffProps {
                CD = 120,
                Duration = 15,
                Icon = ActionIds.SummonPhoenix,
                Color = UIColor.Orange,
                Triggers = new []{ new Item(ActionIds.SummonPhoenix) }
            })
        };

        public static Cursor Cursors => new(JobIds.SMN, CursorType.None, CursorType.CastTime);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Addle)} ({UIHelper.Localize(JobIds.SMN)})", new CooldownProps {
                Icon = ActionIds.Addle,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Addle) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Swiftcast)} ({UIHelper.Localize(JobIds.SMN)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Swiftcast) }
            })
        };

        public static IconReplacer[] Icons => Array.Empty<IconReplacer>();

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.24f };

        public static bool GCD_ROLL => false;
    }
}
