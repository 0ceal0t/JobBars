using JobBars.Jobs;

namespace JobBars.Cooldowns.Manager {
    public partial class CooldownManager {
        protected override CooldownConfig[] GetOTHER() => [];

        protected override CooldownConfig[] GetGNB() => GNB.Cooldowns;
        protected override CooldownConfig[] GetPLD() => PLD.Cooldowns;
        protected override CooldownConfig[] GetWAR() => WAR.Cooldowns;
        protected override CooldownConfig[] GetDRK() => DRK.Cooldowns;

        protected override CooldownConfig[] GetAST() => AST.Cooldowns;
        protected override CooldownConfig[] GetSCH() => SCH.Cooldowns;
        protected override CooldownConfig[] GetWHM() => WHM.Cooldowns;
        protected override CooldownConfig[] GetSGE() => SGE.Cooldowns;

        protected override CooldownConfig[] GetBRD() => BRD.Cooldowns;
        protected override CooldownConfig[] GetMCH() => MCH.Cooldowns;
        protected override CooldownConfig[] GetDNC() => DNC.Cooldowns;

        protected override CooldownConfig[] GetDRG() => DRG.Cooldowns;
        protected override CooldownConfig[] GetSAM() => SAM.Cooldowns;
        protected override CooldownConfig[] GetNIN() => NIN.Cooldowns;
        protected override CooldownConfig[] GetMNK() => MNK.Cooldowns;
        protected override CooldownConfig[] GetRPR() => RPR.Cooldowns;

        protected override CooldownConfig[] GetSMN() => SMN.Cooldowns;
        protected override CooldownConfig[] GetBLM() => BLM.Cooldowns;
        protected override CooldownConfig[] GetRDM() => RDM.Cooldowns;
        protected override CooldownConfig[] GetBLU() => BLU.Cooldowns;
    }
}