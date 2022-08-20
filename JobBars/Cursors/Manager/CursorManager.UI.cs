using ImGuiNET;
using JobBars.Data;
using System;

namespace JobBars.Cursors.Manager {
    public partial class CursorManager {
        private static readonly CursorPositionType[] ValidCursorPositionType = (CursorPositionType[])Enum.GetValues(typeof(CursorPositionType));

        private readonly InfoBox<CursorManager> HideWhenInfoBox = new() {
            Label = "Hide When",
            ContentsAction = (CursorManager manager) => {
                if (ImGui.Checkbox("Mouse held", ref JobBars.Config.CursorHideWhenHeld)) JobBars.Config.Save();
                if (ImGui.Checkbox("Out of combat", ref JobBars.Config.CursorHideOutOfCombat)) JobBars.Config.Save();
                if (ImGui.Checkbox("Weapon sheathed", ref JobBars.Config.CursorHideWeaponSheathed)) JobBars.Config.Save();
            }
        };

        protected override void DrawHeader() {
            if (ImGui.Checkbox("Cursor enabled" + Id, ref JobBars.Config.CursorsEnabled)) JobBars.Config.Save();
        }

        protected override void DrawSettings() {
            HideWhenInfoBox.Draw(this);

            if (JobBars.DrawCombo(ValidCursorPositionType, JobBars.Config.CursorPosition, "Cursor positioning", Id, out var newPosition)) {
                JobBars.Config.CursorPosition = newPosition;
                JobBars.Config.Save();
            }

            if (JobBars.Config.CursorPosition == CursorPositionType.CustomPosition) {
                if (ImGui.InputFloat2("Custom cursor position", ref JobBars.Config.CursorCustomPosition)) {
                    JobBars.Config.Save();
                }
            }

            if (ImGui.InputFloat("Inner scale" + Id, ref JobBars.Config.CursorInnerScale)) JobBars.Config.Save();
            if (ImGui.InputFloat("Outer scale" + Id, ref JobBars.Config.CursorOuterScale)) JobBars.Config.Save();

            if (Configuration.DrawColor("Inner color", InnerColor, out var newColorInner)) {
                InnerColor = newColorInner;
                JobBars.Config.CursorInnerColor = newColorInner.Name;
                JobBars.Config.Save();

                JobBars.Builder.SetCursorInnerColor(InnerColor);
            }

            if (Configuration.DrawColor("Outer color", OuterColor, out var newColorOuter)) {
                OuterColor = newColorOuter;
                JobBars.Config.CursorOuterColor = newColorOuter.Name;
                JobBars.Config.Save();

                JobBars.Builder.SetCursorOuterColor(OuterColor);
            }
        }

        protected override void DrawItem(Cursor item, JobIds _) {
            item.Draw(Id);
        }
    }
}
