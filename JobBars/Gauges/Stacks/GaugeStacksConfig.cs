using JobBars.UI;

namespace JobBars.Gauges.Stacks {
    public struct GaugeStacksProps {
        public int MaxStacks;
        public Item[] Triggers;
        public ElementColor Color;
        public GaugeCompleteSoundType CompletionSound;
        public bool ProgressSound;
    }

    public class GaugeStacksConfig : GaugeConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Arrow, GaugeVisualType.Bar, GaugeVisualType.Diamond };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public int MaxStacks { get; private set; }
        public Item[] Triggers { get; private set; }
        public ElementColor Color { get; private set; }
        public GaugeCompleteSoundType CompletionSound { get; private set; }
        public bool ProgressSound { get; private set; }

        public GaugeStacksConfig(string name, GaugeVisualType type, GaugeStacksProps props) : base(name, type) {
            MaxStacks = props.MaxStacks;
            Triggers = props.Triggers;
            Color = JobBars.Config.GaugeColor.Get(Name, props.Color);
            CompletionSound = JobBars.Config.GaugeCompletionSound.Get(Name, props.CompletionSound);
            ProgressSound = JobBars.Config.GaugeProgressSound.Get(Name, props.ProgressSound);
        }

        protected override void DrawConfig(string id, out bool newPos, out bool newVisual, out bool reset) {
            newPos = newVisual = reset = false;

            if (JobBars.Config.GaugeColor.Draw($"Color{id}", Name, Color, out var newColor)) {
                Color = newColor;
                newVisual = true;
            }

            if (JobBars.Config.GaugeCompletionSound.Draw($"Completion Sound{id}", Name, ValidSoundType, CompletionSound, out var newCompletionSound)) {
                CompletionSound = newCompletionSound;
            }

            if (JobBars.Config.GaugeProgressSound.Draw($"Play Sound On Change{id}", Name, ProgressSound, out var newProgressSound)) {
                ProgressSound = newProgressSound;
            }
        }
    }
}
