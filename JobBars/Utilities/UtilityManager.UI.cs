using ImGuiNET;
using JobBars.Data;
using JobBars.UI;
using System.Numerics;

namespace JobBars.Utilities {
    public partial class UtilityManager {
        private JobIds SettingsJobSelected = JobIds.OTHER;

        public void Draw() {
            string _ID = "##JobBars_Utilities";

            /*
            if (ImGui.Checkbox("Gauges Enabled" + _ID, ref Configuration.Config.GaugesEnabled)) {
                if (Configuration.Config.GaugesEnabled) UIBuilder.Builder.ShowGauges();
                else UIBuilder.Builder.HideGauges();
                Configuration.Config.Save();
            }
            */

            /*ImGui.BeginChild(_ID + "/Child", ImGui.GetContentRegionAvail(), true);
            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, 150);

            ImGui.BeginChild(_ID + "Tree");
            foreach (var job in JobToGauges.Keys) {
                if (job == JobIds.OTHER) continue;
                if (ImGui.Selectable(job + _ID + "/Job", SettingsJobSelected == job)) {
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
                foreach (var gauge in JobToGauges[SettingsJobSelected]) {
                    gauge.Draw(_ID, SettingsJobSelected);
                }
                ImGui.EndChild();
            }
            ImGui.Columns(1);
            ImGui.EndChild();*/
        }
    }
}
