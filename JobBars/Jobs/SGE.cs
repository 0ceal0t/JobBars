using JobBars.Atk;
using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;

namespace JobBars.Jobs {
    public static class SGE {
        public static GaugeConfig[] Gauges => [
            new GaugeTimerConfig(UiHelper.Localize(BuffIds.EukrasianDosis), GaugeVisualType.Bar, new GaugeSubTimerProps {
                MaxDuration = 30,
                Color = ColorConstants.MpPink,
                Triggers = [
                    new Item(BuffIds.EukrasianDosis),
                    new Item(BuffIds.EukrasianDosis2),
                    new Item(BuffIds.EukrasianDosis3),
                    new Item(BuffIds.EukrasianDyskrasia),
                ]
            })
        ];

        public static BuffConfig[] Buffs => [];

        public static Cursor Cursors => new( JobIds.SGE, CursorType.None, CursorType.CastTime );

        public static CooldownConfig[] Cooldowns => [
            new CooldownConfig(UiHelper.Localize(ActionIds.Philosophia), new CooldownProps {
                Icon = ActionIds.Philosophia,
                CD = 180,
                Duration = 20,
                Triggers = [new Item(ActionIds.Philosophia )]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.Pneuma), new CooldownProps {
                Icon = ActionIds.Pneuma,
                CD = 120,
                Triggers = [new Item(ActionIds.Pneuma)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.Holos), new CooldownProps {
                Icon = ActionIds.Holos,
                CD = 120,
                Duration = 20,
                Triggers = [new Item(ActionIds.Holos)]
            }),
            new CooldownConfig(UiHelper.Localize(ActionIds.Panhaima), new CooldownProps {
                Icon = ActionIds.Panhaima,
                Duration = 15,
                CD = 120,
                Triggers = [new Item(ActionIds.Panhaima)]
            }),
            new CooldownConfig($"{UiHelper.Localize(ActionIds.Swiftcast)} ({UiHelper.Localize(JobIds.SGE)})", new CooldownProps {
                Icon = ActionIds.Swiftcast,
                CD = 40,
                Triggers = [new Item(ActionIds.Swiftcast)]
            })
        ];

        public static IconReplacer[] Icons => [
            new IconBuffReplacer(UiHelper.Localize(BuffIds.EukrasianDosis), new IconBuffProps {
                IsTimer = true,
                Icons = [
                    ActionIds.EukrasianDosis,
                    ActionIds.EukrasianDosis2,
                    ActionIds.EukrasianDosis3,
                    ActionIds.Dosis,
                    ActionIds.Dosis2,
                    ActionIds.Dosis3,
                    ActionIds.EukrasianDyskrasia
                ],
                Triggers = [
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.EukrasianDosis), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.EukrasianDosis2), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.EukrasianDosis3), Duration = 30 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.EukrasianDyskrasia), Duration = 30 }
                ]
            })
        ];

        public static bool MP => true;

        public static float[] MP_SEGMENTS => [0.24f];
    }
}
