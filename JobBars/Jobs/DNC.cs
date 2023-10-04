using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Procs;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.Atk;
using System;

namespace JobBars.Jobs {
    public static class DNC {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig($"{AtkHelper.Localize(JobIds.DNC)} {AtkHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = new []{
                    new ProcConfig(AtkHelper.Localize(BuffIds.FlourishingSymmetry),
                        new[] { new Item(BuffIds.FlourishingSymmetry), new Item(BuffIds.SilkenSymmetry) },
                        AtkColor.BrightGreen),
                    new ProcConfig(AtkHelper.Localize(BuffIds.FlourishingFlow),
                        new[] { new Item(BuffIds.FlourishingFlow), new Item(BuffIds.SilkenFlow) },
                        AtkColor.DarkBlue),
                    new ProcConfig(AtkHelper.Localize(BuffIds.ThreefoldFanDance), BuffIds.ThreefoldFanDance, AtkColor.HealthGreen),
                    new ProcConfig(AtkHelper.Localize(BuffIds.FourfoldFanDance), BuffIds.FourfoldFanDance, AtkColor.LightBlue),
                    new ProcConfig(AtkHelper.Localize(BuffIds.FlourishingStarfall), BuffIds.FlourishingStarfall, AtkColor.Red)
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(AtkHelper.Localize(ActionIds.QuadTechFinish), new BuffProps {
                CD = 115, // -5 seconds for the dance to actually be cast
                Duration = 20,
                Icon = ActionIds.QuadTechFinish,
                Color = AtkColor.Orange,
                Triggers = new []{ new Item(ActionIds.QuadTechFinish) }
            }),
            new BuffConfig(AtkHelper.Localize(ActionIds.Devilment), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.Devilment,
                Color = AtkColor.BrightGreen,
                Triggers = new []{ new Item(ActionIds.Devilment) }
            })
        };

        public static Cursor Cursors => new(JobIds.DNC, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig(AtkHelper.Localize(ActionIds.ShieldSamba), new CooldownProps {
                Icon = ActionIds.ShieldSamba,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.ShieldSamba) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.Improvisation), new CooldownProps {
                Icon = ActionIds.Improvisation,
                Duration = 15,
                CD = 120,
                Triggers = new []{ new Item(BuffIds.Improvisation) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.CuringWaltz), new CooldownProps {
                Icon = ActionIds.CuringWaltz,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.CuringWaltz) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Devilment), new IconBuffProps {
                Icons = new [] { ActionIds.Devilment },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Devilment), Duration = 20 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
