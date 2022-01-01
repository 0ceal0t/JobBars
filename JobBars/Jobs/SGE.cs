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
    public static class SGE {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.EukrasianDosis), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 30,
                Color = UIColor.MpPink,
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
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Swiftcast)} ({UIHelper.Localize(JobIds.SGE)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Swiftcast) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.LucidDreaming)} ({UIHelper.Localize(JobIds.SGE)})", new CooldownProps {
                Icon = ActionIds.LucidDreaming,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.LucidDreaming) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Surecast)} ({UIHelper.Localize(JobIds.SGE)})", new CooldownProps {
                Icon = ActionIds.Surecast,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Surecast) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Rescue)} ({UIHelper.Localize(JobIds.SGE)})", new CooldownProps {
                Icon = ActionIds.Rescue,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Rescue) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Physis2), new CooldownProps {
                Icon = ActionIds.Physis2,
                Duration = 15,
                CD = 60,
                Triggers = new []{
                    new Item(ActionIds.Physis),
                    new Item(ActionIds.Physis2),
                }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Soteria), new CooldownProps {
                Icon = ActionIds.Soteria,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Soteria) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Kerachole), new CooldownProps {
                Icon = ActionIds.Kerachole,
                Duration = 15,
                CD = 30,
                Triggers = new []{ new Item(ActionIds.Kerachole) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Ixochole), new CooldownProps {
                Icon = ActionIds.Ixochole,
                CD = 30,
                Triggers = new []{ new Item(ActionIds.Ixochole) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Taurochole), new CooldownProps {
                Icon = ActionIds.Taurochole,
                CD = 45,
                Triggers = new []{ new Item(ActionIds.Taurochole) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Haima), new CooldownProps {
                Icon = ActionIds.Haima,
                Duration = 15,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Haima) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Holos), new CooldownProps {
                Icon = ActionIds.Holos,
                CD = 120,
                Duration = 20,
                Triggers = new []{ new Item(ActionIds.Holos) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Panhaima), new CooldownProps {
                Icon = ActionIds.Panhaima,
                Duration = 15,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Panhaima) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Pneuma), new CooldownProps {
                Icon = ActionIds.Pneuma,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Pneuma) }
            })
        };

        public static IconReplacer[] Icons => new IconReplacer[] {
            new IconReplacer(UIHelper.Localize(BuffIds.EukrasianDosis), new IconProps {
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
                    new IconTriggerStruct { Trigger = new Item(BuffIds.EukrasianDosis), Duration = 30 },
                    new IconTriggerStruct { Trigger = new Item(BuffIds.EukrasianDosis2), Duration = 30 },
                    new IconTriggerStruct { Trigger = new Item(BuffIds.EukrasianDosis3), Duration = 30 }
                }
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.24f };

        public static bool GCD_ROLL => false;
    }
}
