using Dalamud.Logging;
using Dalamud.Plugin;
using ImGuiNET;
using JobBars.Data;
using JobBars.UI;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;

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
                    DrawCooldown(cooldown, _ID);
                }
                ImGui.EndChild();
            }
            ImGui.Columns(1);
            ImGui.EndChild();
        }

        private void DrawCooldown(CooldownProps cooldown, string _ID) {
            var enabled = !Configuration.Config.CooldownDisabled.Contains(cooldown.Name);
            ImGui.TextColored(enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1), $"{cooldown.Name}");
            if (ImGui.Checkbox("Enabled" + _ID, ref enabled)) {
                if (enabled) Configuration.Config.CooldownDisabled.Remove(cooldown.Name);
                else Configuration.Config.CooldownDisabled.Add(cooldown.Name);
                Configuration.Config.Save();

                ResetUI();
            }

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
        }
    }
}