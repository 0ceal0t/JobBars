using Dalamud.Logging;
using Dalamud.Plugin;
using ImGuiNET;
using JobBars.Data;
using JobBars.UI;
using System.Collections.Generic;
using System.Net.Sockets;

namespace JobBars.Cooldowns {
    public unsafe partial class CooldownManager {
        private JobIds SettingsJobSelected = JobIds.OTHER;

        public void Draw() {
            string _ID = "##JobBars_Cooldowns";

            if (ImGui.Checkbox("Cooldowns Enabled" + _ID, ref Configuration.Config.CooldownsEnabled)) {
                Configuration.Config.Save();
                if (Configuration.Config.CooldownsEnabled) UIBuilder.Builder.ShowCooldowns();
                else UIBuilder.Builder.HideCooldowns();
            }

            var size = ImGui.GetContentRegionAvail();
            ImGui.BeginChild(_ID + "/Child", size, true);
            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, 150);

            ImGui.BeginChild(_ID + "Tree");
            foreach (var job in JobToCooldowns.Keys) {
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
                foreach (var cooldown in JobToCooldowns[SettingsJobSelected]) {
                    // cooldown
                }
                ImGui.EndChild();
            }
            ImGui.Columns(1);
            ImGui.EndChild();
        }
    }
}