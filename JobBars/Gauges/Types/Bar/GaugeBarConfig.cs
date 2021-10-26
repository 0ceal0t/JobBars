namespace JobBars.Gauges.Types.Bar {
    public class GaugeBarConfig : GaugeTypeConfig {
        public bool ShowText { get; private set; }
        public bool SwapText { get; private set; }

        public GaugeBarConfig(string name) : base(name) {
            ShowText = JobBars.Config.GaugeShowText.Get(Name);
            SwapText = JobBars.Config.GaugeSwapText.Get(Name);
        }

        public override void Draw(string id, out bool newPos, out bool newVisual, out bool reset) {
            newPos = newVisual = reset = false;

            if (JobBars.Config.GaugeShowText.Draw($"Show Text{id}", Name, ShowText, out var newShowText)) {
                ShowText = newShowText;
                newVisual = true;
            }

            if (JobBars.Config.GaugeSwapText.Draw($"Swap Text Position{id}", Name, SwapText, out var newSwapText)) {
                SwapText = newSwapText;
                newVisual = true;
            }
        }
    }
}
