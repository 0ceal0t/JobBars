using JobBars.UI;

namespace JobBars.Gauges.Charges {
    public struct GaugeChargesProps {
        public GaugesChargesPartProps[] Parts;
        public ElementColor BarColor;
        public bool SameColor;
        public GaugeCompleteSoundType CompletionSound;
        public bool ProgressSound;
    }

    public struct GaugesChargesPartProps {
        public Item[] Triggers;
        public float Duration;
        public float CD;
        public bool Bar;
        public bool Diamond;
        public int MaxCharges;
        public ElementColor Color;
    }

    public class GaugeChargesConfig : GaugeConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.BarDiamondCombo, GaugeVisualType.Bar, GaugeVisualType.Diamond };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public GaugesChargesPartProps[] Parts { get; private set; }
        public bool SameColor { get; private set; }
        public ElementColor BarColor { get; private set; }
        public GaugeCompleteSoundType CompletionSound { get; private set; }
        public bool ProgressSound { get; private set; }
        public bool ReverseFill { get; private set; }

        public GaugeChargesConfig(string name, GaugeVisualType type, GaugeChargesProps props) : base(name, type) {
            Parts = props.Parts;
            SameColor = props.SameColor;
            BarColor = JobBars.Config.GaugeColor.Get(Name, props.BarColor);
            CompletionSound = JobBars.Config.GaugeCompletionSound.Get(Name, props.CompletionSound);
            ProgressSound = JobBars.Config.GaugeProgressSound.Get(Name, props.ProgressSound);
            ReverseFill = JobBars.Config.GaugeReverseFill.Get(Name, false);
        }

        protected override void DrawConfig(string id, out bool newPos, out bool newVisual, out bool reset) {
            newPos = newVisual = reset = false;

            if (JobBars.Config.GaugeColor.Draw($"Color{id}", Name, BarColor, out var newColor)) {
                BarColor = newColor;
                newVisual = true;
            }

            if (JobBars.Config.GaugeCompletionSound.Draw($"Completion Sound{id}", Name, ValidSoundType, CompletionSound, out var newCompletionSound)) {
                CompletionSound = newCompletionSound;
            }

            if (JobBars.Config.GaugeProgressSound.Draw($"Play Sound On Change{id}", Name, ProgressSound, out var newProgressSound)) {
                ProgressSound = newProgressSound;
            }

            if (JobBars.Config.GaugeReverseFill.Draw($"Reverse Tick Fill Order{id}", Name, ReverseFill, out var newReverseFill)) {
                ReverseFill = newReverseFill;
            }
        }
    }
}
