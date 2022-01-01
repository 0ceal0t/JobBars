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
                NoSoundOnProc = true
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
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Swiftcast)} ({UIHelper.Localize(JobIds.SCH)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Swiftcast) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.LucidDreaming)} ({UIHelper.Localize(JobIds.SCH)})", new CooldownProps {
                Icon = ActionIds.LucidDreaming,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.LucidDreaming) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Surecast)} ({UIHelper.Localize(JobIds.SCH)})", new CooldownProps {
                Icon = ActionIds.Surecast,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Surecast) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Rescue)} ({UIHelper.Localize(JobIds.SCH)})", new CooldownProps {
                Icon = ActionIds.Rescue,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Rescue) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.SacredSoil), new CooldownProps {
                Icon = ActionIds.SacredSoil,
                Duration = 15,
                CD = 30,
                Triggers = new []{ new Item(ActionIds.SacredSoil) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Indomitability), new CooldownProps {
                Icon = ActionIds.Indomitability,
                CD = 30,
                Triggers = new []{ new Item(ActionIds.Indomitability) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Dissipation), new CooldownProps {
                Icon = ActionIds.Dissipation,
                Duration = 30,
                CD = 180,
                Triggers = new []{ new Item(ActionIds.Dissipation) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Excogitation), new CooldownProps {
                Icon = ActionIds.Excogitation,
                CD = 45,
                Triggers = new []{ new Item(ActionIds.Excogitation) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.SummonSeraph), new CooldownProps {
                Icon = ActionIds.SummonSeraph,
                Duration = 22,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.SummonSeraph) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Protraction), new CooldownProps {
                Icon = ActionIds.Protraction,
                Duration = 10,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Protraction) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Expedient), new CooldownProps {
                Icon = ActionIds.Expedient,
                Duration = 20,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Expedient) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(ActionIds.ChainStratagem), new IconProps {
                Icons = new [] { ActionIds.ChainStratagem },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.ChainStratagem), Duration = 15 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.Biolysis), new IconProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.SchBio,
                    ActionIds.SchBio2,
                    ActionIds.Biolysis
                },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.ArcBio), Duration = 30 },
                    new IconTriggerStruct { Trigger = new Item(BuffIds.ArcBio2), Duration = 30 },
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Biolysis), Duration = 30 }
                }
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.24f };

        public static bool GCD_ROLL => false;
    }
}
