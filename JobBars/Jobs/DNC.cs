using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Procs;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class DNC {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig($"{UIHelper.Localize(JobIds.DNC)} {UIHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = new []{
                    new ProcConfig(UIHelper.Localize(BuffIds.FlourishingCascade), BuffIds.FlourishingCascade, UIColor.BrightGreen),
                    new ProcConfig(UIHelper.Localize(BuffIds.FlourishingFountain), BuffIds.FlourishingFountain, UIColor.Yellow),
                    new ProcConfig(UIHelper.Localize(BuffIds.FlourishingWindmill), BuffIds.FlourishingWindmill, UIColor.DarkBlue),
                    new ProcConfig(UIHelper.Localize(BuffIds.FlourishingShower), BuffIds.FlourishingShower, UIColor.Red),
                    new ProcConfig(UIHelper.Localize(BuffIds.FlourishingFanDance), BuffIds.FlourishingFanDance, UIColor.HealthGreen)
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(ActionIds.QuadTechFinish), new BuffProps {
                CD = 115, // -5 seconds for the dance to actually be cast
                Duration = 20,
                Icon = ActionIds.QuadTechFinish,
                Color = UIColor.Orange,
                Triggers = new []{ new Item(ActionIds.QuadTechFinish) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.Devilment), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.Devilment,
                Color = UIColor.BrightGreen,
                Triggers = new []{ new Item(ActionIds.Devilment) }
            })
        };

        public static Cursor Cursors => new(JobIds.DNC, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig(UIHelper.Localize(ActionIds.ShieldSamba), new CooldownProps {
                Icon = ActionIds.ShieldSamba,
                Duration = 15,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.ShieldSamba) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Improvisation), new CooldownProps {
                Icon = ActionIds.Improvisation,
                Duration = 15,
                CD = 120,
                Triggers = new []{ new Item(BuffIds.Improvisation) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.Devilment), new IconProps {
                Icons = new [] { ActionIds.Devilment },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Devilment), Duration = 20 }
                }
            })
        };
    }
}
