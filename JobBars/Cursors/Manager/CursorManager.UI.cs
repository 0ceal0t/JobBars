using ImGuiNET;
using JobBars.Data;
using System;

namespace JobBars.Cursors.Manager {
    public partial class CursorManager {
        private static readonly CursorPositionType[] ValidCursorPositionType = (CursorPositionType[])Enum.GetValues(typeof(CursorPositionType));

        protected override void DrawHeader() {
            if (ImGui.Checkbox("Cursor enabled" + _ID, ref JobBars.Config.CursorsEnabled)) JobBars.Config.Save();
            if (ImGui.Checkbox("Hide cursor when mouse held" + _ID, ref JobBars.Config.CursorHideWhenHeld)) JobBars.Config.Save();
            if (ImGui.Checkbox("Hide cursor when out of combat", ref JobBars.Config.CursorHideOutOfCombat)) JobBars.Config.Save();
            if (ImGui.Checkbox("Hide cursor when weapon sheathed", ref JobBars.Config.CursorHideWeaponSheathed)) JobBars.Config.Save();

            if (JobBars.DrawCombo(ValidCursorPositionType, JobBars.Config.CursorPosition, "Cursor positioning", _ID, out var newPosition)) {
                JobBars.Config.CursorPosition = newPosition;
                JobBars.Config.Save();
            }

            if (JobBars.Config.CursorPosition == CursorPositionType.CustomPosition) {
                if (ImGui.InputFloat2("Custom cursor position", ref JobBars.Config.CursorCustomPosition)) {
                    JobBars.Config.Save();
                }
            }

            if (ImGui.InputFloat("Inner scale" + _ID, ref JobBars.Config.CursorInnerScale)) JobBars.Config.Save();
            if (ImGui.InputFloat("Outer scale" + _ID, ref JobBars.Config.CursorOuterScale)) JobBars.Config.Save();

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
            item.Draw(_ID);
        }
    }
}
