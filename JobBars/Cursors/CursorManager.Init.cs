using JobBars.Data;

namespace JobBars.Cursors {
    public partial class CursorManager {
        private void Init() {
            JobToValue.Add(JobIds.OTHER, null);
            // ============ GNB ==================
            JobToValue.Add(JobIds.GNB, new Cursor(JobIds.GNB, CursorType.None, CursorType.GCD));
            // ============ PLD ==================
            JobToValue.Add(JobIds.PLD, new Cursor(JobIds.PLD, CursorType.None, CursorType.GCD));
            // ============ WAR ==================
            JobToValue.Add(JobIds.WAR, new Cursor(JobIds.WAR, CursorType.None, CursorType.GCD));
            // ============ DRK ==================
            JobToValue.Add(JobIds.DRK, new Cursor(JobIds.DRK, CursorType.None, CursorType.GCD));
            // ============ AST ==================
            JobToValue.Add(JobIds.AST, new Cursor(JobIds.AST, CursorType.None, CursorType.CastTime));
            // ============ SCH ==================
            JobToValue.Add(JobIds.SCH, new Cursor(JobIds.SCH, CursorType.None, CursorType.CastTime));
            // ============ WHM ==================
            JobToValue.Add(JobIds.WHM, new Cursor(JobIds.WHM, CursorType.None, CursorType.CastTime));
            // ============ BRD ==================
            JobToValue.Add(JobIds.BRD, new Cursor(JobIds.BRD, CursorType.None, CursorType.GCD));
            // ============ DRG ==================
            JobToValue.Add(JobIds.DRG, new Cursor(JobIds.DRG, CursorType.None, CursorType.GCD));
            // ============ SMN ==================
            JobToValue.Add(JobIds.SMN, new Cursor(JobIds.SMN, CursorType.None, CursorType.CastTime));
            // ============ SAM ==================
            JobToValue.Add(JobIds.SAM, new Cursor(JobIds.SAM, CursorType.None, CursorType.GCD));
            // ============ BLM ==================
            JobToValue.Add(JobIds.BLM, new Cursor(JobIds.BLM, CursorType.MpTick, CursorType.CastTime));
            // ============ RDM ==================
            JobToValue.Add(JobIds.RDM, new Cursor(JobIds.RDM, CursorType.None, CursorType.CastTime));
            // ============ MCH ==================
            JobToValue.Add(JobIds.MCH, new Cursor(JobIds.MCH, CursorType.None, CursorType.GCD));
            // ============ DNC ==================
            JobToValue.Add(JobIds.DNC, new Cursor(JobIds.DNC, CursorType.None, CursorType.GCD));
            // ============ NIN ==================
            JobToValue.Add(JobIds.NIN, new Cursor(JobIds.NIN, CursorType.None, CursorType.GCD));
            // ============ MNK ==================
            JobToValue.Add(JobIds.MNK, new Cursor(JobIds.MNK, CursorType.None, CursorType.GCD));
            // ============ BLU ==================
            JobToValue.Add(JobIds.BLU, new Cursor(JobIds.BLU, CursorType.None, CursorType.CastTime));
        }
    }
}