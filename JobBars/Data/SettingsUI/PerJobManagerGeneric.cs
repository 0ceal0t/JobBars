using ImGuiNET;
using System.Collections.Generic;

namespace JobBars.Data {
    public abstract class PerJobManagerGeneric<T> {
        protected Dictionary<JobIds, T> JobToValue = new();
        protected JobIds SelectedJob = JobIds.OTHER;
        protected string _ID;

        public PerJobManagerGeneric(string id) {
            _ID = id;
        }

        protected abstract void DrawHeader();
        protected abstract void DrawLeftColumn();
        protected abstract void DrawRightColumn();

        public void Draw() {
            DrawHeader();

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