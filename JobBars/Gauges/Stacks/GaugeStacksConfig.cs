using JobBars.Atk;

namespace JobBars.Gauges.Stacks {
    public struct GaugeStacksProps {
        public int MaxStacks;
        public Item[] Triggers;
        public ElementColor Color;
        public GaugeCompleteSoundType CompletionSound;
        public bool ProgressSound;
    }

    public class GaugeStacksConfig : GaugeConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = [GaugeVisualType.Arrow, GaugeVisualType.Bar, GaugeVisualType.Diamond];
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public int MaxStacks { get; private set; }
        public Item[] Triggers { get; private set; }
        public ElementColor Color { get; private set; }
        public GaugeCompleteSoundType CompletionSound { get; private set; }
        public bool ReverseFill { get; private set; }

        public GaugeStacksConfig( string name, GaugeVisualType type, GaugeStacksProps props ) : base( name, type ) {
            MaxStacks = props.MaxStacks;
            Triggers = props.Triggers;
            Color = JobBars.Configuration.GaugeColor.Get( Name, props.Color );
            CompletionSound = JobBars.Configuration.GaugeCompletionSound.Get( Name, props.CompletionSound );
            ReverseFill = JobBars.Configuration.GaugeReverseFill.Get( Name, false );
        }

        public override GaugeTracker GetTracker( int idx ) => new GaugeStacksTracker( this, idx );

        protected override void DrawConfig( string id, ref bool newVisual, ref bool reset ) {
            if( JobBars.Configuration.GaugeColor.Draw( $"Color{id}", Name, Color, out var newColor ) ) {
                Color = newColor;
                newVisual = true;
            }

            if( JobBars.Configuration.GaugeCompletionSound.Draw( $"Completion sound{id}", Name, ValidSoundType, CompletionSound, out var newCompletionSound ) ) {
                CompletionSound = newCompletionSound;
            }

            DrawCompletionSoundEffect();
            DrawSoundEffect( "Change sound effect" );

            if( JobBars.Configuration.GaugeReverseFill.Draw( $"Reverse tick fill order{id}", Name, ReverseFill, out var newReverseFill ) ) {
                ReverseFill = newReverseFill;
                newVisual = true;
            }
        }
    }
}
