using ImGuiNET;
using System.Collections.Generic;

namespace JobBars.Data {
    public abstract class PerJobManagerGeneric<T> {
        protected Dictionary<JobIds, T> JobToValue = new();
        protected JobIds SelectedJob = JobIds.OTHER;
        protected string _ID;

        public PerJobManagerGeneric(string id) {
            _ID = id;
            Init();
        }

        private void Init() {
            JobToValue.Add(JobIds.OTHER, GetOTHER());

            JobToValue.Add(JobIds.GNB, GetGNB());
            JobToValue.Add(JobIds.PLD, GetPLD());
            JobToValue.Add(JobIds.WAR, GetWAR());
            JobToValue.Add(JobIds.DRK, GetDRK());

            JobToValue.Add(JobIds.AST, GetAST());
            JobToValue.Add(JobIds.SCH, GetSCH());
            JobToValue.Add(JobIds.WHM, GetWHM());
            JobToValue.Add(JobIds.SGE, GetSGE());

            JobToValue.Add(JobIds.BRD, GetBRD());
            JobToValue.Add(JobIds.MCH, GetMCH());
            JobToValue.Add(JobIds.DNC, GetDNC());

            JobToValue.Add(JobIds.DRG, GetDRG());
            JobToValue.Add(JobIds.SAM, GetSAM());
            JobToValue.Add(JobIds.NIN, GetNIN());
            JobToValue.Add(JobIds.MNK, GetMNK());
            JobToValue.Add(JobIds.RPR, GetRPR());

            JobToValue.Add(JobIds.SMN, GetSMN());
            JobToValue.Add(JobIds.BLM, GetBLM());
            JobToValue.Add(JobIds.RDM, GetRDM());
            JobToValue.Add(JobIds.BLU, GetBLU());
        }

        protected abstract T GetOTHER();
        protected abstract T GetGNB();
        protected abstract T GetPLD();
        protected abstract T GetWAR();
        protected abstract T GetDRK();
        protected abstract T GetAST();
        protected abstract T GetSCH();
        protected abstract T GetWHM();
        protected abstract T GetSGE();
        protected abstract T GetBRD();
        protected abstract T GetMCH();
        protected abstract T GetDNC();
        protected abstract T GetDRG();
        protected abstract T GetSAM();
        protected abstract T GetNIN();
        protected abstract T GetMNK();
        protected abstract T GetRPR();
        protected abstract T GetSMN();
        protected abstract T GetBLM();
        protected abstract T GetRDM();
        protected abstract T GetBLU();

        protected abstract void DrawHeader();
        protected abstract void DrawLeftColumn();
        protected abstract void DrawRightColumn();

        public void Draw() {
            DrawHeader();

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);

            ImGui.BeginChild(_ID + "/Child", ImGui.GetContentRegionAvail(), true);
            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, 200);

            ImGui.BeginChild(_ID + "Tree");

            DrawLeftColumn();

            ImGui.EndChild();
            ImGui.NextColumn();

            DrawRightColumn();

            ImGui.Columns(1);
            ImGui.EndChild();
        }
    }
}