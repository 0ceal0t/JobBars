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
    public static class SGE {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.EukrasianDosis), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 30,
                Color = AtkColor.MpPink,
                Triggers = new []{
                    new Item(BuffIds.EukrasianDosis),
                    new Item(BuffIds.EukrasianDosis2),
                    new Item(BuffIds.EukrasianDosis3)
                }
            })
        };

        public static BuffConfig[] Buffs => new BuffConfig[] {
        };

        public static Cursor Cursors => new(JobIds.SGE, CursorType.None, CursorType.CastTime);

        public static CooldownConfig[] Cooldowns => new CooldownConfig[] {
            new CooldownConfig(AtkHelper.Localize(ActionIds.Pneuma), new CooldownProps {
                Icon = ActionIds.Pneuma,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Pneuma) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.Holos), new CooldownProps {
                Icon = ActionIds.Holos,
                CD = 120,
                Duration = 20,
                Triggers = new []{ new Item(ActionIds.Holos) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.Panhaima), new CooldownProps {
                Icon = ActionIds.Panhaima,
                Duration = 15,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Panhaima) }
            }),
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Swiftcast)} ({AtkHelper.Localize(JobIds.SGE)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Swiftcast) }
            })
        };

        public static IconReplacer[] Icons => new IconReplacer[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.EukrasianDosis), new IconBuffProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.EukrasianDosis,
                    ActionIds.EukrasianDosis2,
                    ActionIds.EukrasianDosis3,
                    ActionIds.Dosis,
                    ActionIds.Dosis2,
                    ActionIds.Dosis3,
                },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.EukrasianDosis), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.EukrasianDosis2), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.EukrasianDosis3), Duration = 30 }
                }
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.24f };
    }
}
