using JobBars.Jobs;

namespace JobBars.Buffs.Manager {
    public unsafe partial class BuffManager {
        protected override BuffConfig[] GetOTHER() => [];

        protected override BuffConfig[] GetGNB() => GNB.Buffs;
        protected override BuffConfig[] GetPLD() => PLD.Buffs;
        protected override BuffConfig[] GetWAR() => WAR.Buffs;
        protected override BuffConfig[] GetDRK() => DRK.Buffs;

        protected override BuffConfig[] GetAST() => AST.Buffs;
        protected override BuffConfig[] GetSCH() => SCH.Buffs;
        protected override BuffConfig[] GetWHM() => WHM.Buffs;
        protected override BuffConfig[] GetSGE() => SGE.Buffs;

        protected override BuffConfig[] GetBRD() => BRD.Buffs;
        protected override BuffConfig[] GetMCH() => MCH.Buffs;
        protected override BuffConfig[] GetDNC() => DNC.Buffs;

        protected override BuffConfig[] GetDRG() => DRG.Buffs;
        protected override BuffConfig[] GetSAM() => SAM.Buffs;
        protected override BuffConfig[] GetNIN() => NIN.Buffs;
        protected override BuffConfig[] GetVPR() => VPR.Buffs;
        protected override BuffConfig[] GetMNK() => MNK.Buffs;
        protected override BuffConfig[] GetRPR() => RPR.Buffs;

        protected override BuffConfig[] GetSMN() => SMN.Buffs;
        protected override BuffConfig[] GetBLM() => BLM.Buffs;
        protected override BuffConfig[] GetRDM() => RDM.Buffs;
        protected override BuffConfig[] GetPCT() => PCT.Buffs;
        protected override BuffConfig[] GetBLU() => BLU.Buffs;
    }
}