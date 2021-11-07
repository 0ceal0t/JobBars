using System;
using System.Collections.Generic;
using JobBars.Data;
using JobBars.Gauges.MP;
using JobBars.Helper;
using JobBars.Jobs;

namespace JobBars.Gauges.Manager {
    public unsafe partial class GaugeManager {
        protected override GaugeConfig[] GetOTHER() => Array.Empty<GaugeConfig>();

        protected override GaugeConfig[] GetGNB() => AddMiscGauges(GNB.Gauges, JobIds.GNB, GNB.MP, GNB.MP_SEGMENTS, GNB.GCD_ROLL);
        protected override GaugeConfig[] GetPLD() => AddMiscGauges(PLD.Gauges, JobIds.PLD, PLD.MP, PLD.MP_SEGMENTS, PLD.GCD_ROLL);
        protected override GaugeConfig[] GetWAR() => AddMiscGauges(WAR.Gauges, JobIds.WAR, WAR.MP, WAR.MP_SEGMENTS, WAR.GCD_ROLL);
        protected override GaugeConfig[] GetDRK() => AddMiscGauges(DRK.Gauges, JobIds.DRK, DRK.MP, DRK.MP_SEGMENTS, DRK.GCD_ROLL);

        protected override GaugeConfig[] GetAST() => AddMiscGauges(AST.Gauges, JobIds.AST, AST.MP, AST.MP_SEGMENTS, AST.GCD_ROLL);
        protected override GaugeConfig[] GetSCH() => AddMiscGauges(SCH.Gauges, JobIds.SCH, SCH.MP, SCH.MP_SEGMENTS, SCH.GCD_ROLL);
        protected override GaugeConfig[] GetWHM() => AddMiscGauges(WHM.Gauges, JobIds.WHM, WHM.MP, WHM.MP_SEGMENTS, WHM.GCD_ROLL);

        protected override GaugeConfig[] GetBRD() => AddMiscGauges(BRD.Gauges, JobIds.BRD, BRD.MP, BRD.MP_SEGMENTS, BRD.GCD_ROLL);
        protected override GaugeConfig[] GetMCH() => AddMiscGauges(MCH.Gauges, JobIds.MCH, MCH.MP, MCH.MP_SEGMENTS, MCH.GCD_ROLL);
        protected override GaugeConfig[] GetDNC() => AddMiscGauges(DNC.Gauges, JobIds.DNC, DNC.MP, DNC.MP_SEGMENTS, DNC.GCD_ROLL);

        protected override GaugeConfig[] GetDRG() => AddMiscGauges(DRG.Gauges, JobIds.DRG, DRG.MP, DRG.MP_SEGMENTS, DRG.GCD_ROLL);
        protected override GaugeConfig[] GetSAM() => AddMiscGauges(SAM.Gauges, JobIds.SAM, SAM.MP, SAM.MP_SEGMENTS, SAM.GCD_ROLL);
        protected override GaugeConfig[] GetNIN() => AddMiscGauges(NIN.Gauges, JobIds.NIN, NIN.MP, NIN.MP_SEGMENTS, NIN.GCD_ROLL);
        protected override GaugeConfig[] GetMNK() => AddMiscGauges(MNK.Gauges, JobIds.MNK, MNK.MP, MNK.MP_SEGMENTS, MNK.GCD_ROLL);

        protected override GaugeConfig[] GetSMN() => AddMiscGauges(SMN.Gauges, JobIds.SMN, SMN.MP, SMN.MP_SEGMENTS, SMN.GCD_ROLL);
        protected override GaugeConfig[] GetBLM() => AddMiscGauges(BLM.Gauges, JobIds.BLM, BLM.MP, BLM.MP_SEGMENTS, BLM.GCD_ROLL);
        protected override GaugeConfig[] GetRDM() => AddMiscGauges(RDM.Gauges, JobIds.RDM, RDM.MP, RDM.MP_SEGMENTS, RDM.GCD_ROLL);
        protected override GaugeConfig[] GetBLU() => AddMiscGauges(BLU.Gauges, JobIds.BLU, BLU.MP, BLU.MP_SEGMENTS, BLU.GCD_ROLL);

        private GaugeConfig[] AddMiscGauges(GaugeConfig[] configs, JobIds job, bool mp, float[] mpSegments, bool gcdRoll) {
            var configList = new List<GaugeConfig>(configs);
            var jobName = UIHelper.Localize(job);
            if (mp) configList.Add(new GaugeMPConfig($"MP ({jobName})", GaugeVisualType.Bar, mpSegments, defaultDisabled:true));
            //if (gcdRoll) configList.Add(new GaugeGCDRoll($"GCD ({jobName})", GaugeVisualType.Bar));
            return configList.ToArray();

        }
    }
}
