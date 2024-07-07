using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Stacks;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class PLD {
        public static GaugeConfig[] Gauges => [
            new GaugeProcsConfig(UiHelper.Localize(BuffIds.DivineMight), GaugeVisualType.Diamond, new GaugeProcProps{
                ShowText = true,
                Procs = [
                    new ProcConfig(UiHelper.Localize(BuffIds.DivineMight), BuffIds.DivineMight, ColorConstants.DarkBlue)
                ]
            }),
            new GaugeStacksConfig(UiHelper.Localize(BuffIds.Requiescat), GaugeVisualType.Diamond, new GaugeStacksProps {
                MaxStacks = 4,
                Triggers = [
                    new Item(BuffIds.Requiescat)
                ],
                Color = ColorConstants.LightBlue
            }),
            new GaugeProcsConfig(UiHelper.Localize(BuffIds.AtonementReady), GaugeVisualType.Diamond, new GaugeProcProps{
                Procs = [
                    new ProcConfig(UiHelper.Localize(BuffIds.AtonementReady), BuffIds.VerfireReady, ColorConstants.LightBlue)
                ]
            }),
            new GaugeGCDConfig(UiHelper.Localize(BuffIds.FightOrFlight), GaugeVisualType.Bar, new GaugeSubGCDProps {
                MaxCounter = 8,
                MaxDuration = 20,
                Color = ColorConstants.Red,
                Triggers = [
                    new Item(BuffIds.FightOrFlight)
                ]
            })
        ];

        public static BuffConfig[] Buffs => [
            new BuffConfig(UiHelper.Localize(BuffIds.Requiescat), new BuffProps {
                CD = 60,
                Duration = 30,
                Icon = ActionIds.Requiescat,
                Color = ColorConstants.LightBlue,
                Triggers = [new Item(BuffIds.Requiescat)]
            })
        ];

        public static Cursor Cursors => new( JobIds.PLD, CursorType.None, CursorType.GCD );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig(UiHelper.Localize(ActionIds.HallowedGround), new CooldownProps {
                Icon = ActionIds.HallowedGround,
                Duration = 10,
                CD = 420,
                Triggers = [new Item(ActionIds.HallowedGround)]
            }),
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Reprisal)} ({UiHelper.Localize(JobIds.PLD)})", new CooldownProps {
                Icon = ActionIds.Reprisal,
                Duration = 15,
                CD = 60,
                Triggers = [new Item(ActionIds.Reprisal)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.DivineVeil), new CooldownProps {
                Icon = ActionIds.DivineVeil,
                Duration = 30,
                CD = 90,
                Triggers = [new Item(ActionIds.DivineVeil)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.PassageOfArms), new CooldownProps {
                Icon = ActionIds.PassageOfArms,
                Duration = 18,
                CD = 120,
                Triggers = [new Item(BuffIds.PassageOfArms)]
            })
        ];

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(UiHelper.Localize(BuffIds.FightOrFlight), new IconBuffProps {
                Icons = [
                    ActionIds.FightOrFlight,
                    ActionIds.GoringBlade
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.FightOrFlight), Duration = 20 }
                ]
            }),
            new IconBuffReplacer($"{UiHelper.Localize(ActionIds.Rampart)} ({UiHelper.Localize(JobIds.PLD)})", new IconBuffProps {
                Icons = [ActionIds.Rampart],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Rampart), Duration = 20 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.Guardian), new IconBuffProps {
                Icons = [
                    ActionIds.Sentinel,
                    ActionIds.Guardian
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Sentinel), Duration = 15 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Guardian), Duration = 15 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.Bulwark), new IconBuffProps {
                Icons = [ActionIds.Bulwark],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Bulwark), Duration = 10 }
                ]
            }),
            new IconBuffReplacer($"{UiHelper.Localize(ActionIds.ArmsLength)} ({UiHelper.Localize(JobIds.PLD)})", new IconBuffProps {
                Icons = [ActionIds.ArmsLength],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ArmsLength), Duration = 6 }
                ]
            }),
            new IconBuffReplacer(UiHelper.Localize(BuffIds.HallowedGround), new IconBuffProps {
                Icons = [ActionIds.HallowedGround],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.HallowedGround), Duration = 10 }
                ]
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => [0.1f, 0.2f, 0.3f, 0.4f, 0.5f];
    }
}
