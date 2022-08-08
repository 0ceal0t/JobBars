using ImGuiNET;
using JobBars.Data;
using System.Numerics;

namespace JobBars.Buffs.Manager {
    public partial class BuffManager {
        public bool LOCKED = true;

        private readonly InfoBox<BuffManager> PositionInfoBox = new() {
            Label = "Position",
            ContentsAction = (BuffManager manager) => {
                ImGui.Checkbox("Position Locked" + manager.Id, ref manager.LOCKED);

                ImGui.SetNextItemWidth(25f);
                if (ImGui.InputInt("Buffs per line" + manager.Id, ref JobBars.Config.BuffHorizontal, 0)) {
                    JobBars.Config.Save();
                    JobBars.Builder.RefreshBuffLayout();
                }

                if (ImGui.Checkbox("Right-to-left" + manager.Id, ref JobBars.Config.BuffRightToLeft)) {
                    JobBars.Config.Save();
                    JobBars.Builder.RefreshBuffLayout();
                }

                if (ImGui.Checkbox("Bottom-to-top" + manager.Id, ref JobBars.Config.BuffBottomToTop)) {
                    JobBars.Config.Save();
                    JobBars.Builder.RefreshBuffLayout();
                }

                if (ImGui.Checkbox("Square buffs" + manager.Id, ref JobBars.Config.BuffSquare)) {
                    JobBars.Config.Save();
                    JobBars.Builder.UpdateBuffsSize();
                }

                if (ImGui.InputFloat("Scale" + manager.Id, ref JobBars.Config.BuffScale)) {
                    manager.UpdatePositionScale();
                    JobBars.Config.Save();
                }

                var pos = JobBars.Config.BuffPosition;
                if (ImGui.InputFloat2("Position" + manager.Id, ref pos)) {
                    SetBuffPosition(pos);
                }
            }
        };

        private readonly InfoBox<BuffManager> PartyListInfoBox = new() {
            Label = "Party List",
            ContentsAction = (BuffManager manager) => {
                if (ImGui.Checkbox("Show card duration when on AST" + manager.Id, ref JobBars.Config.BuffPartyListASTText)) JobBars.Config.Save();
            }
        };

        protected override void DrawHeader() {
            if (ImGui.Checkbox("Buff bar enabled" + Id, ref JobBars.Config.BuffBarEnabled)) {
                JobBars.Config.Save();
                ResetUI();
            }
        }

        protected override void DrawSettings() {
            PositionInfoBox.Draw(this);
            PartyListInfoBox.Draw(this);

            if (ImGui.InputFloat("Hide buffs with cooldown above" + Id, ref JobBars.Config.BuffDisplayTimer)) JobBars.Config.Save();
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

            if (ImGui.InputFloat("Opacity when on cooldown" + Id, ref JobBars.Config.BuffOnCDOpacity)) JobBars.Config.Save();
        }

        protected override void DrawItem(BuffConfig[] item, JobIds _) {
            var reset = false;
            foreach (var buff in item) buff.Draw(Id, ref reset);
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
