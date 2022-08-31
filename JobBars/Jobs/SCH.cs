using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class SCH {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig(UIHelper.Localize(BuffIds.Excog), GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = new []{
                    new ProcConfig(UIHelper.Localize(BuffIds.Excog), BuffIds.Excog, UIColor.BrightGreen)
                },
                ProcSound = GaugeCompleteSoundType.When_Empty
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Biolysis), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 30,
                Color = UIColor.BlueGreen,
                Triggers = new []{
                    new Item(BuffIds.ArcBio),
                    new Item(BuffIds.ArcBio2),
                    new Item(BuffIds.Biolysis)
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(ActionIds.ChainStratagem), new BuffProps {
                CD = 120,
                Duration = 15,
                Icon = ActionIds.ChainStratagem,
                Color = UIColor.White,
                Triggers = new []{ new Item(ActionIds.ChainStratagem) }
            })
        };

        public static Cursor Cursors => new(JobIds.SCH, CursorType.None, CursorType.CastTime);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig(UIHelper.Localize(ActionIds.SummonSeraph), new CooldownProps {
                Icon = ActionIds.SummonSeraph,
                Duration = 22,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.SummonSeraph) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Expedient), new CooldownProps {
                Icon = ActionIds.Expedient,
                Duration = 20,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Expedient) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Protraction), new CooldownProps {
                Icon = ActionIds.Protraction,
                Duration = 10,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Protraction) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Swiftcast)} ({UIHelper.Localize(JobIds.SCH)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Swiftcast) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UIHelper.Localize(ActionIds.ChainStratagem), new IconBuffProps {
                Icons = new [] { ActionIds.ChainStratagem },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ChainStratagem), Duration = 15 }
                }
            }),
            new IconBuffReplacer(UIHelper.Localize(BuffIds.Biolysis), new IconBuffProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.SchBio,
                    ActionIds.SchBio2,
                    ActionIds.Biolysis
                },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ArcBio), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ArcBio2), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Biolysis), Duration = 30 }
                }
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.24f };
    }
}
