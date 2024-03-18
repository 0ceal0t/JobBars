namespace JobBars.Gauges.Types.BarDiamondCombo {
    public class GaugeBarDiamondComboConfig : GaugeTypeConfig {
        public bool ShowText { get; private set; }

        public GaugeBarDiamondComboConfig( string name ) : base( name ) {
            ShowText = JobBars.Configuration.GaugeShowText.Get( Name );
        }

        public override void Draw( string id, ref bool newVisual, ref bool reset ) {
            if( JobBars.Configuration.GaugeShowText.Draw( $"Show text{id}", Name, ShowText, out var newShowText ) ) {
                ShowText = newShowText;
                newVisual = true;
            }
        }
    }
}
