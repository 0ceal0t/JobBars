namespace JobBars.Gauges.Types.Bar {
    public class GaugeBarConfig : GaugeTypeConfig {
        public bool ShowText { get; private set; }
        public bool SwapText { get; private set; }
        public bool Vertical { get; private set; }

        public GaugeBarConfig( string name ) : base( name ) {
            ShowText = JobBars.Configuration.GaugeShowText.Get( Name );
            SwapText = JobBars.Configuration.GaugeSwapText.Get( Name );
            Vertical = JobBars.Configuration.GaugeVertical.Get( Name );
        }

        public override void Draw( string id, ref bool newVisual, ref bool reset ) {
            if( JobBars.Configuration.GaugeShowText.Draw( $"Show text{id}", Name, ShowText, out var newShowText ) ) {
                ShowText = newShowText;
                newVisual = true;
            }

            if( JobBars.Configuration.GaugeSwapText.Draw( $"Swap text position{id}", Name, SwapText, out var newSwapText ) ) {
                SwapText = newSwapText;
                newVisual = true;
            }

            if( JobBars.Configuration.GaugeVertical.Draw( $"Vertical{id}", Name, Vertical, out var newVertical ) ) {
                Vertical = newVertical;
                newVisual = true;
            }
        }
    }
}
