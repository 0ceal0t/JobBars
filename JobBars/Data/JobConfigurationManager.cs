using ImGuiNET;
using System.Collections.Generic;
using JobBars.Helper;

namespace JobBars.Data {
    public abstract class JobConfigurationManager<T> {
        protected Dictionary<JobIds, T> JobToValue = new();
        protected string _ID;
        protected JobIds SettingsJobSelected = JobIds.OTHER;

        public JobConfigurationManager(string id) {
            _ID = id;
        }

        protected abstract void DrawHeader();
        protected abstract void DrawItem(T item);

        public void Draw() {
            DrawHeader();

            ImGui.BeginChild(_ID + "/Child", ImGui.GetContentRegionAvail(), true);
            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, 150);

            ImGui.BeginChild(_ID + "Tree");
            foreach (var job in JobToValue.Keys) {
                if (job == JobIds.OTHER) continue;
                if (ImGui.Selectable(UIHelper.JobToString(job) + _ID + "/Job", SettingsJobSelected == job)) {
                    SettingsJobSelected = job;
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();

            if (SettingsJobSelected == JobIds.OTHER) {
                ImGui.Text("Select a job...");
            }
            else {
                ImGui.BeginChild(_ID + "Selected");
                DrawItem(JobToValue[SettingsJobSelected]);
                ImGui.EndChild();
            }
            ImGui.Columns(1);
            ImGui.EndChild();
        }
    }
}
