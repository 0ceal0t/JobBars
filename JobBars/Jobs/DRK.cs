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
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class DRK {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeDrkMPConfig($"MP ({UIHelper.Localize(JobIds.DRK)})", GaugeVisualType.BarDiamondCombo, new GaugeDrkMpProps {
                Color = UIColor.Purple,
                DarkArtsColor = UIColor.LightBlue,
                Segments = new[] { 0.3f, 0.6f, 0.9f }
            }),
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.Delirium), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.Delirium)
                },
                Color = UIColor.Red
            }),
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.BloodWeapon), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 5,
                Triggers = new []{
                    new Item(BuffIds.BloodWeapon)
                },
                Color = UIColor.DarkBlue
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(BuffIds.Delirium), new BuffProps {
                CD = 60,
                Duration = 15,
                Icon = ActionIds.Delirium,
                Color = UIColor.Red,
                Triggers = new []{ new Item(BuffIds.Delirium) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.LivingShadow), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.LivingShadow,
                Color = UIColor.Purple,
                Triggers = new []{ new Item(ActionIds.LivingShadow) }
            })
        };

        public static Cursor Cursors => new(JobIds.DRK, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[]{
            new CooldownConfig(UIHelper.Localize(ActionIds.LivingDead), new CooldownProps {
                Icon = ActionIds.LivingDead,
                Duration = 10,
                CD = 300,
                Triggers = new []{ new Item(BuffIds.LivingDead) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Reprisal)} ({UIHelper.Localize(JobIds.DRK)})", new CooldownProps {
                Icon = ActionIds.Reprisal,
                Duration = 10,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Reprisal) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.DarkMissionary), new CooldownProps {
                Icon = ActionIds.DarkMissionary,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.DarkMissionary) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.TheBlackestNight), new CooldownProps {
                Icon = ActionIds.TheBlackestNight,
                Duration = 7,
                CD = 15,
                Triggers = new []{ new Item(ActionIds.TheBlackestNight) }
            })
        };

        public static IconReplacer[] Icons => Array.Empty<IconReplacer>();

        // DRK HAS A CUSTOM MP BAR, SO DON'T WORRY ABOUT THIS
        public static bool MP => false;
        public static float[] MP_SEGMENTS => null;
    }
}
