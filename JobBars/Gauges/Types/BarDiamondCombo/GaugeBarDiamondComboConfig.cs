namespace JobBars.Gauges.Types.BarDiamondCombo {
    public class GaugeBarDiamondComboConfig : GaugeTypeConfig {
        public bool ShowText { get; private set; }

        public GaugeBarDiamondComboConfig(string name) : base(name) {
            ShowText = JobBars.Config.GaugeShowText.Get(Name);
        }

        public override void Draw(string id, out bool newPos, out bool newVisual, out bool reset) {
            newPos = newVisual = reset = false;

            if (JobBars.Config.GaugeShowText.Draw($"Show Text{id}", Name, ShowText, out var newShowText)) {
                ShowText = newShowText;
                newVisual = true;
            }
        }
    }
}
