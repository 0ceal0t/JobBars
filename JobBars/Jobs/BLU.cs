using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.Atk;
using System;

namespace JobBars.Jobs {
    public static class BLU {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig($"{AtkHelper.Localize(JobIds.BLU)} {AtkHelper.ProcText}", GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = new []{
                    new ProcConfig(AtkHelper.Localize(BuffIds.AstralAttenuation), BuffIds.AstralAttenuation, AtkColor.NoColor),
                    new ProcConfig(AtkHelper.Localize(BuffIds.UmbralAttenuation), BuffIds.UmbralAttenuation, AtkColor.DarkBlue),
                    new ProcConfig(AtkHelper.Localize(BuffIds.PhysicalAttenuation), BuffIds.PhysicalAttenuation, AtkColor.Orange)
                }
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.BluBleed), GaugeVisualType.Bar, new GaugeSubTimerProps
            {
                MaxDuration = 60,
                Color = AtkColor.Red,
                Triggers = new []{
                    new Item(BuffIds.BluBleed)
                }
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.Poison), GaugeVisualType.Bar, new GaugeSubTimerProps
            {
                MaxDuration = 15,
                Color = AtkColor.HealthGreen,
                Triggers = new []{
                    new Item(BuffIds.Poison)
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(AtkHelper.Localize(ActionIds.OffGuard), new BuffProps {
                CD = 60,
                Duration = 15,
                Icon = ActionIds.OffGuard,
                Color = AtkColor.BrightGreen,
                Triggers = new []{ new Item(ActionIds.OffGuard) }
            }),
            new BuffConfig(AtkHelper.Localize(ActionIds.PeculiarLight), new BuffProps {
                CD = 60,
                Duration = 15,
                Icon = ActionIds.PeculiarLight,
                Color = AtkColor.Red,
                Triggers = new []{ new Item(ActionIds.PeculiarLight) }
            })
        };

        public static Cursor Cursors => new(JobIds.BLU, CursorType.None, CursorType.CastTime);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Addle)} ({AtkHelper.Localize(JobIds.BLU)})", new CooldownProps {
                Icon = ActionIds.Addle,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Addle) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.AngelWhisper), new CooldownProps {
                Icon = ActionIds.AngelWhisper,
                CD = 300,
                Triggers = new []{ new Item(ActionIds.AngelWhisper) }
            }),
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Swiftcast)} ({AtkHelper.Localize(JobIds.BLU)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Swiftcast) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.BluBleed), new IconBuffProps {
                IsTimer = true,
                Icons = new [] { ActionIds.SongOfTorment },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.BluBleed), Duration = 60 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Poison), new IconBuffProps {
                IsTimer = true,
                Icons = new [] { ActionIds.BadBreath },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Poison), Duration = 15 }
                }
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.24f };
    }
}
