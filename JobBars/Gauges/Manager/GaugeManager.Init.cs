using System;
using JobBars.Jobs;

namespace JobBars.Gauges.Manager {
    public unsafe partial class GaugeManager {
        protected override GaugeConfig[] GetOTHER() => Array.Empty<GaugeConfig>();

        protected override GaugeConfig[] GetGNB() => GNB.Gauges;
        protected override GaugeConfig[] GetPLD() => PLD.Gauges;
        protected override GaugeConfig[] GetWAR() => WAR.Gauges;
        protected override GaugeConfig[] GetDRK() => DRK.Gauges;

        protected override GaugeConfig[] GetAST() => AST.Gauges;
        protected override GaugeConfig[] GetSCH() => SCH.Gauges;
        protected override GaugeConfig[] GetWHM() => WHM.Gauges;

        protected override GaugeConfig[] GetBRD() => BRD.Gauges;
        protected override GaugeConfig[] GetMCH() => MCH.Gauges;
        protected override GaugeConfig[] GetDNC() => DNC.Gauges;

        protected override GaugeConfig[] GetDRG() => DRG.Gauges;
        protected override GaugeConfig[] GetSAM() => SAM.Gauges;
        protected override GaugeConfig[] GetNIN() => NIN.Gauges;
        protected override GaugeConfig[] GetMNK() => MNK.Gauges;

        protected override GaugeConfig[] GetSMN() => SMN.Gauges;
        protected override GaugeConfig[] GetBLM() => BLM.Gauges;
        protected override GaugeConfig[] GetRDM() => RDM.Gauges;
        protected override GaugeConfig[] GetBLU() => BLU.Gauges;
    }
}
