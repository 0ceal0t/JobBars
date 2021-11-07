using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Stacks;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class RDM {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig($"{UIHelper.Localize(JobIds.RDM)} {UIHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = new []{
                    new ProcConfig(UIHelper.Localize(BuffIds.VerstoneReady), BuffIds.VerstoneReady, UIColor.NoColor),
                    new ProcConfig(UIHelper.Localize(BuffIds.VerfireReady), BuffIds.VerfireReady, UIColor.Red)
                }
            }),
            new GaugeGCDConfig(UIHelper.Localize(BuffIds.Manafication), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 5,
                MaxDuration = 10,
                Color = UIColor.DarkBlue,
                Triggers = new[] {
                    new Item(BuffIds.Manafication)
                }
            }),
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.Acceleration), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.Acceleration)
                },
                Color = UIColor.PurplePink
            }),
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(ActionIds.Manafication), new BuffProps {
                CD = 110,
                Duration = 10,
                Icon = ActionIds.Manafication,
                Color = UIColor.DarkBlue,
                Triggers = new []{ new Item(ActionIds.Manafication) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.Embolden), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.Embolden,
                Color = UIColor.White,
                Triggers = new []{ new Item(ActionIds.Embolden) }
            })
        };

        public static Cursor Cursors => new(JobIds.RDM, CursorType.None, CursorType.CastTime);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Addle)} ({UIHelper.Localize(JobIds.RDM)})", new CooldownProps {
                Icon = ActionIds.Addle,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Addle) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.Manafication), new IconProps {
                Icons = new [] { ActionIds.Manafication },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Manafication), Duration = 10 }
                }
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.24f };

        public static bool GCD_ROLL => false;
    }
}
