using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Custom;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Stacks;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.Atk;
using System;

namespace JobBars.Jobs {
    public static class DRK {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeDrkMPConfig($"MP ({AtkHelper.Localize(JobIds.DRK)})", GaugeVisualType.BarDiamondCombo, new GaugeDrkMpProps {
                Color = AtkColor.Purple,
                DarkArtsColor = AtkColor.LightBlue,
                Segments = new[] { 0.3f, 0.6f, 0.9f }
            }),
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.Delirium), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.Delirium)
                },
                Color = AtkColor.Red
            }),
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.BloodWeapon), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 5,
                Triggers = new []{
                    new Item(BuffIds.BloodWeapon)
                },
                Color = AtkColor.DarkBlue
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(AtkHelper.Localize(BuffIds.Delirium), new BuffProps {
                CD = 60,
                Duration = 15,
                Icon = ActionIds.Delirium,
                Color = AtkColor.Red,
                Triggers = new []{ new Item(BuffIds.Delirium) }
            }),
            new BuffConfig(AtkHelper.Localize(ActionIds.LivingShadow), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.LivingShadow,
                Color = AtkColor.Purple,
                Triggers = new []{ new Item(ActionIds.LivingShadow) }
            })
        };

        public static Cursor Cursors => new(JobIds.DRK, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[]{
            new CooldownConfig(AtkHelper.Localize(ActionIds.LivingDead), new CooldownProps {
                Icon = ActionIds.LivingDead,
                Duration = 10,
                CD = 300,
                Triggers = new []{ new Item(BuffIds.LivingDead) }
            }),
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Reprisal)} ({AtkHelper.Localize(JobIds.DRK)})", new CooldownProps {
                Icon = ActionIds.Reprisal,
                Duration = 10,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Reprisal) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.DarkMissionary), new CooldownProps {
                Icon = ActionIds.DarkMissionary,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.DarkMissionary) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.TheBlackestNight), new CooldownProps {
                Icon = ActionIds.TheBlackestNight,
                Duration = 7,
                CD = 15,
                Triggers = new []{ new Item(ActionIds.TheBlackestNight) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer($"{AtkHelper.Localize(ActionIds.Rampart)} ({AtkHelper.Localize(JobIds.DRK)})", new IconBuffProps {
                Icons = new [] { ActionIds.Rampart },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Rampart), Duration = 20 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.ShadowWall), new IconBuffProps {
                Icons = new [] { ActionIds.ShadowWall },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ShadowWall), Duration = 15 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.DarkMind), new IconBuffProps {
                Icons = new [] { ActionIds.DarkMind },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.DarkMind), Duration = 10 }
                }
            }),
            new IconBuffReplacer($"{AtkHelper.Localize(ActionIds.ArmsLength)} ({AtkHelper.Localize(JobIds.DRK)})", new IconBuffProps {
                Icons = new [] { ActionIds.ArmsLength },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ArmsLength), Duration = 6 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.LivingDead), new IconBuffProps {
                Icons = new [] { ActionIds.LivingDead },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.LivingDead), Duration = 10 }
                }
            })
        };

        // DRK HAS A CUSTOM MP BAR, SO DON'T WORRY ABOUT THIS
        public static bool MP => false;
        public static float[] MP_SEGMENTS => null;
    }
}
