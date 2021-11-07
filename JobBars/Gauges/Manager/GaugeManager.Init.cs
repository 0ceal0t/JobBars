using System;
using System.Collections.Generic;
using JobBars.Data;
using JobBars.Gauges.MP;
using JobBars.Helper;
using JobBars.Jobs;

namespace JobBars.Gauges.Manager {
    public unsafe partial class GaugeManager {
        protected override GaugeConfig[] GetOTHER() => Array.Empty<GaugeConfig>();

        protected override GaugeConfig[] GetGNB() => GNB.Gauges;
        protected override GaugeConfig[] GetPLD() => AddMiscGauges(PLD.Gauges, JobIds.PLD, true, true);
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

        private GaugeConfig[] AddMiscGauges(GaugeConfig[] configs, JobIds job, bool mp, bool gcdRoll) {
            var configList = new List<GaugeConfig>(configs);
            var jobName = UIHelper.Localize(job);
            if (mp) configList.Add(new GaugeMPConfig($"MP ({jobName})", GaugeVisualType.Bar, null, defaultDisabled:true));
            //if (gcdRoll) configList.Add(new GaugeGCDRoll($"GCD ({jobName})", GaugeVisualType.Bar));
            return configList.ToArray();

        }
    }
}
