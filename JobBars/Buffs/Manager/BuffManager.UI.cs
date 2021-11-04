using ImGuiNET;
using System.Numerics;

namespace JobBars.Buffs.Manager {
    public partial class BuffManager {
        private bool LOCKED = true;

        protected override void DrawHeader() {
            if (ImGui.Checkbox("Buff Bar Enabled" + _ID, ref JobBars.Config.BuffBarEnabled)) {
                JobBars.Config.Save();
                if (JobBars.Config.BuffBarEnabled) JobBars.Builder.ShowBuffs();
                else JobBars.Builder.HideBuffs();
                ResetUI();
            }

            if (ImGui.CollapsingHeader("Position" + _ID + "/Row")) DrawPositionRow();

            if (ImGui.CollapsingHeader("Settings" + _ID + "/Row")) DrawSettingsRow();
        }

        private void DrawPositionRow() {
            ImGui.Indent();

            ImGui.Checkbox("Position Locked" + _ID, ref LOCKED);

            ImGui.SetNextItemWidth(25f);
            if (ImGui.InputInt("Buffs Per Line" + _ID, ref JobBars.Config.BuffHorizontal, 0)) {
                JobBars.Config.Save();
                JobBars.Builder.RefreshBuffLayout();
            }

            if (ImGui.Checkbox("Right-to-Left" + _ID, ref JobBars.Config.BuffRightToLeft)) {
                JobBars.Config.Save();
                JobBars.Builder.RefreshBuffLayout();
            }

            if (ImGui.Checkbox("Bottom-to-Top" + _ID, ref JobBars.Config.BuffBottomToTop)) {
                JobBars.Config.Save();
                JobBars.Builder.RefreshBuffLayout();
            }

            if (ImGui.InputFloat("Scale" + _ID, ref JobBars.Config.BuffScale)) {
                UpdatePositionScale();
                JobBars.Config.Save();
            }

            var pos = JobBars.Config.BuffPosition;
            if (ImGui.InputFloat2("Position" + _ID, ref pos)) {
                SetBuffPosition(pos);
            }

            ImGui.Unindent();
        }

        private void DrawSettingsRow() {
            ImGui.Indent();

            if (ImGui.Checkbox("Highlight Buffed Party Members" + _ID, ref JobBars.Config.BuffPartyListEnabled)) {
                JobBars.Config.Save();
            }

            if (ImGui.InputFloat("Hide Buffs With Cooldown Above" + _ID, ref JobBars.Config.BuffDisplayTimer)) {
                JobBars.Config.Save();
            }

            if (ImGui.Checkbox("Hide Buffs When Out Of Combat", ref JobBars.Config.BuffHideOutOfCombat)) {
                if (!JobBars.Config.BuffHideOutOfCombat && JobBars.Config.BuffBarEnabled) { // since they might be hidden
                    JobBars.Builder.ShowBuffs();
                }
                JobBars.Config.Save();
            }

            if (ImGui.Checkbox("Show Party Members' CDs And Buffs", ref JobBars.Config.BuffIncludeParty)) {
                JobBars.Config.Save();
                ResetUI();
            }

            ImGui.Unindent();
        }

        // ==========================================

        protected override void DrawItem(BuffConfig[] item) {
            var reset = false;
            foreach (var buff in item) buff.Draw(_ID, ref reset);
            if (reset) ResetUI();
        }

        public void DrawPositionBox() {
            if (LOCKED) return;

            if (JobBars.DrawPositionView("Buff Bar##BuffPosition", JobBars.Config.BuffPosition, out var pos)) {
                SetBuffPosition(pos);
            }
        }

        private static void SetBuffPosition(Vector2 pos) {
            JobBars.SetWindowPosition("Buff Bar##BuffPosition", pos);
            JobBars.Config.BuffPosition = pos;
            JobBars.Config.Save();
            JobBars.Builder.SetBuffPosition(pos);
        }
    }
}
