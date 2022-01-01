using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class PLD {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.Requiescat), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 5,
                Triggers = new []{
                    new Item(BuffIds.Requiescat)
                },
                Color = UIColor.LightBlue
            }),
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.SwordOath), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.SwordOath)
                },
                Color = UIColor.BlueGreen
            }),
            new GaugeGCDConfig(UIHelper.Localize(BuffIds.FightOrFlight), GaugeVisualType.Bar, new GaugeSubGCDProps {
                MaxCounter = 11,
                MaxDuration = 25,
                Color = UIColor.Red,
                Increment = new []{
                    new Item(ActionIds.ShieldLob),
                    new Item(ActionIds.RageOfHalone),
                    new Item(ActionIds.FastBlade),
                    new Item(ActionIds.RiotBlade),
                    new Item(ActionIds.RoyalAuthority),
                    new Item(ActionIds.Atonement),
                    new Item(ActionIds.GoringBlade),
                    new Item(ActionIds.TotalEclipse),
                    new Item(ActionIds.Prominence)
                },
                Triggers = new[] {
                    new Item(BuffIds.FightOrFlight)
                }
            }),
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.GoringBlade), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 21,
                Color = UIColor.Orange,
                Triggers = new []{
                    new Item(BuffIds.GoringBlade),
                    new Item(BuffIds.BladeOfValor)
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(ActionIds.Requiescat), new BuffProps {
                CD = 60,
                Duration = 30,
                Icon = ActionIds.Requiescat,
                Color = UIColor.LightBlue,
                Triggers = new []{ new Item(ActionIds.Requiescat) }
            })
        };

        public static Cursor Cursors => new(JobIds.PLD, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Rampart)} ({UIHelper.Localize(JobIds.PLD)})", new CooldownProps {
                Icon = ActionIds.Rampart,
                Duration = 20,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Rampart) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.LowBlow)} ({UIHelper.Localize(JobIds.PLD)})", new CooldownProps {
                Icon = ActionIds.LowBlow,
                Duration = 0,
                CD = 25,
                Triggers = new []{ new Item(ActionIds.LowBlow) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Provoke)} ({UIHelper.Localize(JobIds.PLD)})", new CooldownProps {
                Icon = ActionIds.Provoke,
                Duration = 0,
                CD = 30,
                Triggers = new []{ new Item(ActionIds.Provoke) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Interject)} ({UIHelper.Localize(JobIds.PLD)})", new CooldownProps {
                Icon = ActionIds.Interject,
                Duration = 0,
                CD = 30,
                Triggers = new []{ new Item(ActionIds.Interject) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Reprisal)} ({UIHelper.Localize(JobIds.PLD)})", new CooldownProps {
                Icon = ActionIds.Reprisal,
                Duration = 10,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Reprisal) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.ArmsLength)} ({UIHelper.Localize(JobIds.PLD)})", new CooldownProps {
                Icon = ActionIds.ArmsLength,
                Duration = 6,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.ArmsLength) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Shirk)} ({UIHelper.Localize(JobIds.PLD)})", new CooldownProps {
                Icon = ActionIds.Shirk,
                Duration = 0,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Shirk) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Sentinel), new CooldownProps {
                Icon = ActionIds.Sentinel,
                Duration = 15,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Sentinel) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.HallowedGround), new CooldownProps {
                Icon = ActionIds.HallowedGround,
                Duration = 10,
                CD = 420,
                Triggers = new []{ new Item(ActionIds.HallowedGround) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.DivineVeil), new CooldownProps {
                Icon = ActionIds.DivineVeil,
                Duration = 30,
                CD = 90,
                Triggers = new []{ new Item(BuffIds.DivineVeil) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.PassageOfArms), new CooldownProps {
                Icon = ActionIds.PassageOfArms,
                Duration = 18,
                CD = 120,
                Triggers = new []{ new Item(BuffIds.PassageOfArms) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.FightOrFlight), new IconProps {
                Icons = new [] { ActionIds.FightOrFlight },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.FightOrFlight), Duration = 25 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.GoringBlade), new IconProps {
                IsTimer = true,
                Icons = new [] { ActionIds.GoringBlade },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.GoringBlade), Duration = 21 },
                    new IconTriggerStruct { Trigger = new Item(BuffIds.BladeOfValor), Duration = 21 }
                }
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };

        public static bool GCD_ROLL => true;
    }
}
