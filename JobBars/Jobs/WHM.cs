﻿using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class WHM {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeTimerConfig(UIHelper.Localize(BuffIds.Dia), GaugeVisualType.Bar, new GaugeTimerProps {
                SubTimers = new [] {
                    new GaugeSubTimerProps {
                        MaxDuration = 30,
                        Color = UIColor.LightBlue,
                        SubName = UIHelper.Localize(BuffIds.Dia),
                        Triggers = new []{
                            new Item(BuffIds.Dia),
                            new Item(BuffIds.Aero),
                            new Item(BuffIds.Aero2)
                        }
                    }
                }
            }),
            new GaugeStacksConfig(UIHelper.Localize(BuffIds.LilyBell), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 5,
                Triggers = new []{
                    new Item(BuffIds.LilyBell)
                },
                Color = UIColor.BlueGreen
            }),
        };

        public static BuffConfig[] Buffs => Array.Empty<BuffConfig>();

        public static Cursor Cursors => new(JobIds.WHM, CursorType.None, CursorType.CastTime);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig(UIHelper.Localize(ActionIds.Temperance), new CooldownProps {
                Icon = ActionIds.Temperance,
                Duration = 20,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Temperance) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.Benediction), new CooldownProps {
                Icon = ActionIds.Benediction,
                CD = 180,
                Triggers = new []{ new Item(ActionIds.Benediction) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.LilyBell), new CooldownProps {
                Icon = ActionIds.LilyBell,
                Duration = 15,
                CD = 180,
                Triggers = new []{ new Item(ActionIds.LilyBell) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Swiftcast)} ({UIHelper.Localize(JobIds.WHM)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Swiftcast) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UIHelper.Localize(BuffIds.Dia), new IconBuffProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.Aero,
                    ActionIds.Aero2,
                    ActionIds.Dia
                },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Aero), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Aero2), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Dia), Duration = 30 }
                }
            }),
            new IconBuffReplacer(UIHelper.Localize(BuffIds.PresenceOfMind), new IconBuffProps {
                Icons = new [] { ActionIds.PresenceOfMind },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.PresenceOfMind), Duration = 15 }
                }
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.24f };
    }
}
