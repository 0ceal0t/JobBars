using ImGuiNET;
using System.Numerics;

namespace JobBars.Cooldowns {
    public unsafe partial class CooldownManager {
        protected override void DrawHeader() {
            if (ImGui.Checkbox("Cooldowns Enabled" + _ID, ref JobBars.Config.CooldownsEnabled)) {
                JobBars.Config.Save();
                if (JobBars.Config.CooldownsEnabled) JobBars.Builder.ShowCooldowns();
                else JobBars.Builder.HideCooldowns();
                ResetUI();
            }

            if (ImGui.CollapsingHeader("Position" + _ID + "/Row")) DrawPositionRow();

            if (ImGui.CollapsingHeader("Settings" + _ID + "/Row")) DrawSettingsRow();
        }

        private void DrawPositionRow() {
            ImGui.Indent();

            if (ImGui.Checkbox("Left-Aligned" + _ID, ref JobBars.Config.CooldownsLeftAligned)) {
                JobBars.Config.Save();
                ResetUI();
            }

            if (ImGui.InputFloat2("Position" + _ID, ref JobBars.Config.CooldownPosition)) {
                JobBars.Config.Save();
                JobBars.Builder.SetCooldownPosition(JobBars.Config.CooldownPosition);
            }

            ImGui.Unindent();
        }

        private void DrawSettingsRow() {
            ImGui.Indent();

            if (ImGui.Checkbox("Hide Cooldowns When Out Of Combat" + _ID, ref JobBars.Config.CooldownsHideOutOfCombat)) {
                if (!JobBars.Config.CooldownsHideOutOfCombat && JobBars.Config.CooldownsEnabled) { // since they might be hidden
                    JobBars.Builder.ShowCooldowns();
                }
                JobBars.Config.Save();
            }

            if (ImGui.Checkbox("Show Border When Active" + _ID, ref JobBars.Config.CooldownsShowBorderWhenActive)) {
                JobBars.Config.Save();
            }

            ImGui.Unindent();
        }

        // ==========================================

        protected override void DrawItem(CooldownProps[] item) {
            foreach (var cdProp in item) DrawCooldown(cdProp);
        }

        private void DrawCooldown(CooldownProps cooldown) {
            ImGui.TextColored(cooldown.Enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1), $"{cooldown.Name}");

            if (JobBars.Config.CooldownEnabled.Draw($"Enabled{_ID}{cooldown.Name}", cooldown.Name, cooldown.Enabled)) {
                ResetUI();
            }

            if (JobBars.Config.CooldownOrder.Draw($"Order{_ID}{cooldown.Name}", cooldown.Name)) {
                ResetUI();
            }

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
        }
    }
}