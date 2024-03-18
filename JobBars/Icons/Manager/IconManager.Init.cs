using JobBars.Jobs;

namespace JobBars.Icons.Manager {
    public partial class IconManager {
        protected override IconReplacer[] GetOTHER() => [];

        protected override IconReplacer[] GetGNB() => GNB.Icons;
        protected override IconReplacer[] GetPLD() => PLD.Icons;
        protected override IconReplacer[] GetWAR() => WAR.Icons;
        protected override IconReplacer[] GetDRK() => DRK.Icons;

        protected override IconReplacer[] GetAST() => AST.Icons;
        protected override IconReplacer[] GetSCH() => SCH.Icons;
        protected override IconReplacer[] GetWHM() => WHM.Icons;
        protected override IconReplacer[] GetSGE() => SGE.Icons;

        protected override IconReplacer[] GetBRD() => BRD.Icons;
        protected override IconReplacer[] GetMCH() => MCH.Icons;
        protected override IconReplacer[] GetDNC() => DNC.Icons;

        protected override IconReplacer[] GetDRG() => DRG.Icons;
        protected override IconReplacer[] GetSAM() => SAM.Icons;
        protected override IconReplacer[] GetNIN() => NIN.Icons;
        protected override IconReplacer[] GetMNK() => MNK.Icons;
        protected override IconReplacer[] GetRPR() => RPR.Icons;

        protected override IconReplacer[] GetSMN() => SMN.Icons;
        protected override IconReplacer[] GetBLM() => BLM.Icons;
        protected override IconReplacer[] GetRDM() => RDM.Icons;
        protected override IconReplacer[] GetBLU() => BLU.Icons;
    }
}
