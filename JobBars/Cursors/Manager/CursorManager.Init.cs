using JobBars.Data;
using JobBars.Jobs;

namespace JobBars.Cursors.Manager {
    public partial class CursorManager {
        protected override Cursor GetOTHER() => null;

        protected override Cursor GetGNB() => GNB.Cursors;
        protected override Cursor GetPLD() => PLD.Cursors;
        protected override Cursor GetWAR() => WAR.Cursors;
        protected override Cursor GetDRK() => DRK.Cursors;

        protected override Cursor GetAST() => AST.Cursors;
        protected override Cursor GetSCH() => SCH.Cursors;
        protected override Cursor GetWHM() => WHM.Cursors;
        protected override Cursor GetSGE() => SGE.Cursors;

        protected override Cursor GetBRD() => BRD.Cursors;
        protected override Cursor GetMCH() => MCH.Cursors;
        protected override Cursor GetDNC() => DNC.Cursors;

        protected override Cursor GetDRG() => DRG.Cursors;
        protected override Cursor GetSAM() => SAM.Cursors;
        protected override Cursor GetNIN() => NIN.Cursors;
        protected override Cursor GetMNK() => MNK.Cursors;
        protected override Cursor GetRPR() => RPR.Cursors;

        protected override Cursor GetSMN() => SMN.Cursors;
        protected override Cursor GetBLM() => BLM.Cursors;
        protected override Cursor GetRDM() => RDM.Cursors;
        protected override Cursor GetBLU() => BLU.Cursors;
    }
}