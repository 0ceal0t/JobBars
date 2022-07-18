using ImGuiNET;
using JobBars.Data;
using System.Numerics;

namespace JobBars.Buffs.Manager {
    public partial class BuffManager {
        private bool LOCKED = true;

        protected override void DrawHeader() {
            if (ImGui.Checkbox("Buff bar enabled" + _ID, ref JobBars.Config.BuffBarEnabled)) {
                JobBars.Config.Save();
                ResetUI();
            }

            if (ImGui.CollapsingHeader("Position" + _ID + "/Row")) DrawPositionRow();
            if (ImGui.CollapsingHeader("Settings" + _ID + "/Row")) DrawSettingsRow();
            if (ImGui.CollapsingHeader("Party list" + _ID + "/Row")) DrawPartyListSettings();
        }

        private void DrawPositionRow() {
            ImGui.Indent();

            ImGui.Checkbox("Position Locked" + _ID, ref LOCKED);

            ImGui.SetNextItemWidth(25f);
            if (ImGui.InputInt("Buffs per line" + _ID, ref JobBars.Config.BuffHorizontal, 0)) {
                JobBars.Config.Save();
                JobBars.Builder.RefreshBuffLayout();
            }

            if (ImGui.Checkbox("Right-to-left" + _ID, ref JobBars.Config.BuffRightToLeft)) {
                JobBars.Config.Save();
                JobBars.Builder.RefreshBuffLayout();
            }

            if (ImGui.Checkbox("Bottom-to-top" + _ID, ref JobBars.Config.BuffBottomToTop)) {
                JobBars.Config.Save();
                JobBars.Builder.RefreshBuffLayout();
            }

            if (ImGui.Checkbox("Square buffs" + _ID, ref JobBars.Config.BuffSquare)) {
                JobBars.Config.Save();
                JobBars.Builder.UpdateBuffsSize();
            }

            if (ImGui.DragFloat("Scale" + _ID, ref JobBars.Config.BuffScale,0.05f)) {
                UpdatePositionScale();
                JobBars.Config.Save();
            }

            var pos = JobBars.Config.BuffPosition;
            if (ImGui.DragFloat2("Position" + _ID, ref pos)) {
                SetBuffPosition(pos);
            }

            ImGui.Unindent();
        }

        private void DrawSettingsRow() {
            ImGui.Indent();

            if (ImGui.InputFloat("Hide buffs with cooldown above" + _ID, ref JobBars.Config.BuffDisplayTimer)) JobBars.Config.Save();
            if (ImGui.Checkbox("Hide buffs when out of combat", ref JobBars.Config.BuffHideOutOfCombat)) JobBars.Config.Save();
            if (ImGui.Checkbox("Hide buffs when weapon is sheathed", ref JobBars.Config.BuffHideWeaponSheathed)) JobBars.Config.Save();

            if (ImGui.Checkbox("Show party members' buffs", ref JobBars.Config.BuffIncludeParty)) {
                JobBars.Config.Save();
                ResetUI();
            }

            if (ImGui.InputInt("Buff text size", ref JobBars.Config.BuffTextSize)) {
                if (JobBars.Config.BuffTextSize <= 0) JobBars.Config.BuffTextSize = 1;
                if (JobBars.Config.BuffTextSize > 255) JobBars.Config.BuffTextSize = 255;
                JobBars.Config.Save();
                JobBars.Builder.UpdateBuffsTextSize();
            }

            if (ImGui.Checkbox("Thin buff border", ref JobBars.Config.BuffThinBorder)) {
                JobBars.Config.Save();
                JobBars.Builder.UpdateBorderThin();
            }

            if (ImGui.InputFloat("Opacity when on cooldown" + _ID, ref JobBars.Config.BuffOnCDOpacity)) JobBars.Config.Save();

            ImGui.Unindent();
        }

        private void DrawPartyListSettings() {
            ImGui.Indent();

            if (ImGui.Checkbox("Show card duration when on AST" + _ID, ref JobBars.Config.BuffPartyListASTText)) JobBars.Config.Save();

            ImGui.Unindent();
        }

        protected override void DrawItem(BuffConfig[] item, JobIds _) {
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
