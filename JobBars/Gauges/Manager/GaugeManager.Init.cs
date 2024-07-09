using JobBars.Data;
using JobBars.Gauges.MP;
using JobBars.Gauges.Rolling;
using JobBars.Helper;
using JobBars.Jobs;
using System.Collections.Generic;

namespace JobBars.Gauges.Manager {
    public unsafe partial class GaugeManager {
        protected override GaugeConfig[] GetOTHER() => [];

        protected override GaugeConfig[] GetGNB() => AddMiscGauges( GNB.Gauges, JobIds.GNB, GNB.MP, GNB.MP_SEGMENTS );
        protected override GaugeConfig[] GetPLD() => AddMiscGauges( PLD.Gauges, JobIds.PLD, PLD.MP, PLD.MP_SEGMENTS );
        protected override GaugeConfig[] GetWAR() => AddMiscGauges( WAR.Gauges, JobIds.WAR, WAR.MP, WAR.MP_SEGMENTS );
        protected override GaugeConfig[] GetDRK() => AddMiscGauges( DRK.Gauges, JobIds.DRK, DRK.MP, DRK.MP_SEGMENTS );

        protected override GaugeConfig[] GetAST() => AddMiscGauges( AST.Gauges, JobIds.AST, AST.MP, AST.MP_SEGMENTS );
        protected override GaugeConfig[] GetSCH() => AddMiscGauges( SCH.Gauges, JobIds.SCH, SCH.MP, SCH.MP_SEGMENTS );
        protected override GaugeConfig[] GetWHM() => AddMiscGauges( WHM.Gauges, JobIds.WHM, WHM.MP, WHM.MP_SEGMENTS );
        protected override GaugeConfig[] GetSGE() => AddMiscGauges( SGE.Gauges, JobIds.SGE, SGE.MP, SGE.MP_SEGMENTS );

        protected override GaugeConfig[] GetBRD() => AddMiscGauges( BRD.Gauges, JobIds.BRD, BRD.MP, BRD.MP_SEGMENTS );
        protected override GaugeConfig[] GetMCH() => AddMiscGauges( MCH.Gauges, JobIds.MCH, MCH.MP, MCH.MP_SEGMENTS );
        protected override GaugeConfig[] GetDNC() => AddMiscGauges( DNC.Gauges, JobIds.DNC, DNC.MP, DNC.MP_SEGMENTS );

        protected override GaugeConfig[] GetDRG() => AddMiscGauges( DRG.Gauges, JobIds.DRG, DRG.MP, DRG.MP_SEGMENTS );
        protected override GaugeConfig[] GetSAM() => AddMiscGauges( SAM.Gauges, JobIds.SAM, SAM.MP, SAM.MP_SEGMENTS );
        protected override GaugeConfig[] GetNIN() => AddMiscGauges( NIN.Gauges, JobIds.NIN, NIN.MP, NIN.MP_SEGMENTS );
        protected override GaugeConfig[] GetVPR() => AddMiscGauges( VPR.Gauges, JobIds.VPR, VPR.MP, VPR.MP_SEGMENTS );
        protected override GaugeConfig[] GetMNK() => AddMiscGauges( MNK.Gauges, JobIds.MNK, MNK.MP, MNK.MP_SEGMENTS );
        protected override GaugeConfig[] GetRPR() => AddMiscGauges( RPR.Gauges, JobIds.RPR, RPR.MP, RPR.MP_SEGMENTS );

        protected override GaugeConfig[] GetSMN() => AddMiscGauges( SMN.Gauges, JobIds.SMN, SMN.MP, SMN.MP_SEGMENTS );
        protected override GaugeConfig[] GetBLM() => AddMiscGauges( BLM.Gauges, JobIds.BLM, BLM.MP, BLM.MP_SEGMENTS );
        protected override GaugeConfig[] GetRDM() => AddMiscGauges( RDM.Gauges, JobIds.RDM, RDM.MP, RDM.MP_SEGMENTS );
        protected override GaugeConfig[] GetPCT() => AddMiscGauges( PCT.Gauges, JobIds.PCT, PCT.MP, PCT.MP_SEGMENTS );
        protected override GaugeConfig[] GetBLU() => AddMiscGauges( BLU.Gauges, JobIds.BLU, BLU.MP, BLU.MP_SEGMENTS );

        private static GaugeConfig[] AddMiscGauges( GaugeConfig[] configs, JobIds job, bool mp, float[] mpSegments ) {
            var configList = new List<GaugeConfig>( configs );
            var jobName = UiHelper.Localize( job );
            if( mp ) configList.Add( new GaugeMpConfig( $"MP ({jobName})", GaugeVisualType.Bar, mpSegments, defaultDisabled: true ) );
            configList.Add( new GaugeRollingConfig( $"GCD ({jobName})", GaugeVisualType.Bar ) );
            return [.. configList];

        }
    }
}
