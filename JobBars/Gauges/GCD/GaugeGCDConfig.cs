using JobBars.UI;

namespace JobBars.Gauges.GCD {
    public struct GaugeSubGCDProps {
        public int MaxCounter;
        public float MaxDuration;
        public Item[] Triggers;
        public ElementColor Color;
        public bool Invert;
        public string SubName;
        public Item[] Increment;
        public GaugeCompleteSoundType CompletionSound;
        public bool ProgressSound;
    }

    public struct GaugeGCDProps {
        public GaugeSubGCDProps[] SubGCDs;
    }

    public class GaugeGCDConfig : GaugeConfig {
        public class GaugeSubGCDConfig {
            public readonly string Name;

            public readonly string SubName;
            public readonly int MaxCounter;
            public readonly float MaxDuration;
            public readonly Item[] Triggers;
            public readonly Item[] Increment;
            public ElementColor Color;
            public bool Invert;
            public GaugeCompleteSoundType CompletionSound;
            public bool ProgressSound;

            public GaugeSubGCDConfig(string name, GaugeSubGCDProps props) {
                Name = name;

                SubName = props.SubName;
                MaxCounter = props.MaxCounter;
                MaxDuration = props.MaxDuration;
                Triggers = props.Triggers;
                Color = JobBars.Config.GaugeColor.Get(Name, props.Color);
                Invert = JobBars.Config.GaugeInvert.Get(Name, props.Invert);
                CompletionSound = JobBars.Config.GaugeCompletionSound.Get(Name, props.CompletionSound);
                ProgressSound = JobBars.Config.GaugeProgressSound.Get(Name, props.ProgressSound);
                Increment = props.Increment;
            }
        }

        // ===========================

        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Bar, GaugeVisualType.Arrow, GaugeVisualType.Diamond };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public GaugeSubGCDConfig[] SubGCDs { get; private set; }

        public GaugeGCDConfig(string name, GaugeVisualType type, GaugeSubGCDProps subConfig) : this(name, type, new GaugeGCDProps {
            SubGCDs = new[] { subConfig }
        }) { }

        public GaugeGCDConfig(string name, GaugeVisualType type, GaugeGCDProps props) : base(name, type) {
            SubGCDs = new GaugeSubGCDConfig[props.SubGCDs.Length];
            for (int i = 0; i < SubGCDs.Length; i++) {
                var id = string.IsNullOrEmpty(props.SubGCDs[i].SubName) ? Name : Name + "/" + props.SubGCDs[i].SubName;
                SubGCDs[i] = new GaugeSubGCDConfig(id, props.SubGCDs[i]);
            }
        }

        protected override void DrawConfig(string id, out bool newPos, out bool newVisual, out bool reset) {
            newPos = newVisual = reset = false;

            foreach (var subGCD in SubGCDs) {
                var suffix = string.IsNullOrEmpty(subGCD.SubName) ? "" : $" ({subGCD.SubName})";

                if (JobBars.Config.GaugeColor.Draw($"Color{suffix}{id}", subGCD.Name, subGCD.Color, out var newColor)) {
                    subGCD.Color = newColor;
                    newVisual = true;
                }

                if (JobBars.Config.GaugeInvert.Draw($"Invert{suffix}{id}", subGCD.Name, subGCD.Invert, out var newInvert)) {
                    subGCD.Invert = newInvert;
                }

                if (JobBars.Config.GaugeCompletionSound.Draw($"Completion Sound{suffix}{id}", subGCD.Name, ValidSoundType, subGCD.CompletionSound, out var newCompletionSound)) {
                    subGCD.CompletionSound = newCompletionSound;
                }

                if (JobBars.Config.GaugeProgressSound.Draw($"Play Sound On Progress{suffix}{id}", subGCD.Name, subGCD.ProgressSound, out var newProgressSound)) {
                    subGCD.ProgressSound = newProgressSound;
                }
            }
        }
    }
}
