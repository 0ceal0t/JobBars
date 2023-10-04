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
using JobBars.Atk;
using System;

namespace JobBars.Jobs {
    public static class RDM {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig($"{AtkHelper.Localize(JobIds.RDM)} {AtkHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = new []{
                    new ProcConfig(AtkHelper.Localize(BuffIds.VerstoneReady), BuffIds.VerstoneReady, AtkColor.NoColor),
                    new ProcConfig(AtkHelper.Localize(BuffIds.VerfireReady), BuffIds.VerfireReady, AtkColor.Red)
                }
            }),
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.Manafication), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 6,
                Triggers = new []{
                    new Item(BuffIds.Manafication)
                },
                Color = AtkColor.DarkBlue
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(AtkHelper.Localize(ActionIds.Manafication), new BuffProps {
                CD = 110,
                Duration = 15,
                Icon = ActionIds.Manafication,
                Color = AtkColor.DarkBlue,
                Triggers = new []{ new Item(ActionIds.Manafication) }
            }),
            new BuffConfig(AtkHelper.Localize(ActionIds.Embolden), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.Embolden,
                Color = AtkColor.White,
                Triggers = new []{ new Item(ActionIds.Embolden) }
            })
        };

        public static Cursor Cursors => new(JobIds.RDM, CursorType.None, CursorType.CastTime);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Addle)} ({AtkHelper.Localize(JobIds.RDM)})", new CooldownProps {
                Icon = ActionIds.Addle,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Addle) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.MagickBarrier), new CooldownProps {
                Icon = ActionIds.MagickBarrier,
                Duration = 10,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.MagickBarrier) }
            })
        };

        public static IconReplacer[] Icons => Array.Empty<IconReplacer>();

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.24f };
    }
}
