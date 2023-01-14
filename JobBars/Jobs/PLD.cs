using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class PLD {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig(UIHelper.Localize(BuffIds.DivineMight), GaugeVisualType.Diamond, new GaugeProcProps{
                ShowText = true,
                Procs = new []{
                    new ProcConfig(UIHelper.Localize(BuffIds.DivineMight), BuffIds.DivineMight, UIColor.DarkBlue)
                }
            }),
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.Requiescat), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 4,
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
                MaxCounter = 9,
                MaxDuration = 20,
                Color = UIColor.Red,
                Triggers = new[] {
                    new Item(BuffIds.FightOrFlight)
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(BuffIds.Requiescat), new BuffProps {
                CD = 60,
                Duration = 30,
                Icon = ActionIds.Requiescat,
                Color = UIColor.LightBlue,
                Triggers = new []{ new Item(BuffIds.Requiescat) }
            })
        };

        public static Cursor Cursors => new(JobIds.PLD, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig(UIHelper.Localize(ActionIds.HallowedGround), new CooldownProps {
                Icon = ActionIds.HallowedGround,
                Duration = 10,
                CD = 420,
                Triggers = new []{ new Item(ActionIds.HallowedGround) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Reprisal)} ({UIHelper.Localize(JobIds.PLD)})", new CooldownProps {
                Icon = ActionIds.Reprisal,
                Duration = 10,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Reprisal) }
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
            new IconBuffReplacer(UIHelper.Localize(BuffIds.FightOrFlight), new IconBuffProps {
                Icons = new [] { ActionIds.FightOrFlight },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.FightOrFlight), Duration = 20 }
                }
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };
    }
}
