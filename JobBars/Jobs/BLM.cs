using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class BLM {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig($"{UIHelper.Localize(JobIds.BLM)} {UIHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps{
                ShowText = true,
                Procs = new []{
                    new ProcConfig(UIHelper.Localize(BuffIds.Firestarter), BuffIds.Firestarter, UIColor.Orange),
                    new ProcConfig(UIHelper.Localize(BuffIds.Thundercloud), BuffIds.Thundercloud, UIColor.LightBlue)
                }
            }),
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.Triplecast), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.Triplecast)
                },
                Color = UIColor.MpPink
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Thunder3), GaugeVisualType.Bar, new GaugeTimerProps {
                SubTimers = new[] {
                    new GaugeSubTimerProps {
                        MaxDuration = 30,
                        Color = UIColor.DarkBlue,
                        SubName = UIHelper.Localize(BuffIds.Thunder3),
                        Triggers = new []{
                            new Item(BuffIds.Thunder3),
                            new Item(BuffIds.Thunder)
                        }
                    },
                    new GaugeSubTimerProps {
                        MaxDuration = 18,
                        Color = UIColor.Purple,
                        SubName = UIHelper.Localize(BuffIds.Thunder4),
                        Triggers = new []{
                            new Item(BuffIds.Thunder4),
                            new Item(BuffIds.Thunder2)
                        }
                    }
                }
            })
        };

        public static BuffConfig[] Buffs => Array.Empty<BuffConfig>();

        public static Cursor Cursors => new(JobIds.BLM, CursorType.MpTick, CursorType.CastTime);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Addle)} ({UIHelper.Localize(JobIds.BLM)})", new CooldownProps {
                Icon = ActionIds.Addle,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Addle) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.Thunder3), new IconProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.Thunder,
                    ActionIds.Thunder3
                },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Thunder), Duration = 21 },
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Thunder3), Duration = 30 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.Thunder4), new IconProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.Thunder2,
                    ActionIds.Thunder4
                },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Thunder2), Duration = 18 },
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Thunder4), Duration = 18 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.LeyLines), new IconProps {
                Icons = new [] { ActionIds.LeyLines },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.LeyLines), Duration = 30 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.Sharpcast), new IconProps {
                Icons = new [] { ActionIds.Sharpcast },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Sharpcast), Duration = 30 }
                }
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.88f }; // 3f4 (with umbral hearts) + 1f4 + 3f4

        public static bool GCD_ROLL => false;
    }
}
