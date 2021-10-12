using ImGuiNET;
using System.Numerics;

namespace JobBars.Buffs {
    public partial class BuffManager {
        private bool LOCKED = true;

        protected override void DrawHeader() {
            ImGui.Checkbox("Position Locked" + _ID, ref LOCKED);

            if (ImGui.Checkbox("Buff Bar Enabled" + _ID, ref JobBars.Config.BuffBarEnabled)) {
                JobBars.Config.Save();
                if (JobBars.Config.BuffBarEnabled) JobBars.Builder.ShowBuffs();
                else JobBars.Builder.HideBuffs();

                ResetUI();
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

            ImGui.SetNextItemWidth(25f);
            if (ImGui.InputInt("Buffs Per Line" + _ID, ref JobBars.Config.BuffHorizontal, 0)) {
                JobBars.Config.Save();
                JobBars.Builder.RefreshBuffLayout();
            }

            ImGui.SameLine(250);
            if (ImGui.Checkbox("Right-to-Left" + _ID, ref JobBars.Config.BuffRightToLeft)) {
                JobBars.Config.Save();
                JobBars.Builder.RefreshBuffLayout();
            }

            ImGui.SameLine(500);
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

            if (ImGui.InputFloat("Hide Buffs With Cooldown Above" + _ID, ref JobBars.Config.BuffDisplayTimer)) {
                JobBars.Config.Save();
            }
        }

        protected override void DrawItem(BuffProps[] item) {
            foreach (var buff in item) {
                DrawBuff(buff);
            }
        }

        private void DrawBuff(BuffProps buff) {
            ImGui.TextColored(buff.Enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1), $"{buff.Name}");
            if (JobBars.Config.BuffEnabled.Draw($"Enabled{_ID}{buff.Name}", buff.Name, buff.Enabled)) {
                ResetUI();
            }
        }

        public void DrawPositionBox() {
            if (!LOCKED) {
                if (JobBars.DrawPositionView("Buff Bar##BuffPosition", JobBars.Config.BuffPosition, out var pos)) {
                    SetBuffPosition(pos);
                }
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
