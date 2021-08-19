using Dalamud.Logging;
using ImGuiNET;
using JobBars.Data;
using JobBars.UI;
using System.Numerics;

namespace JobBars.Cooldowns {
    public unsafe partial class CooldownManager {
        private JobIds SettingsJobSelected = JobIds.OTHER;

        public void Draw() {
            string _ID = "##JobBars_Cooldowns";

            if (ImGui.Checkbox("Cooldowns Enabled" + _ID, ref Configuration.Config.CooldownsEnabled)) {
                Configuration.Config.Save();
                if (Configuration.Config.CooldownsEnabled) UIBuilder.Builder.ShowCooldowns();
                else UIBuilder.Builder.HideCooldowns();

                ResetUI();
            }

            if (ImGui.Checkbox("Hide Cooldowns When Out Of Combat", ref Configuration.Config.CooldownsHideOutOfCombat)) {
                if (!Configuration.Config.CooldownsHideOutOfCombat && Configuration.Config.CooldownsEnabled) { // since they might be hidden
                    UIBuilder.Builder.ShowCooldowns();
                }
                Configuration.Config.Save();
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
                    DrawCooldown(cooldown, _ID + cooldown.Name);
                }
                ImGui.EndChild();
            }
            ImGui.Columns(1);
            ImGui.EndChild();
        }

        private void DrawCooldown(CooldownProps cooldown, string _ID) {
            var enabled = cooldown.Enabled;
            ImGui.TextColored(enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1), $"{cooldown.Name}");
            if (ImGui.Checkbox("Enabled" + _ID, ref enabled)) {
                cooldown.Enabled = enabled;
                ResetUI();
            }

            var order = cooldown.Order;
            if(ImGui.InputInt("Order" + _ID, ref order)) {
                cooldown.Order = order;
                ResetUI();
            }

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
        }
    }
}