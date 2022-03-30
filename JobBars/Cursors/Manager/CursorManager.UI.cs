using ImGuiNET;
using JobBars.Data;
using System;

namespace JobBars.Cursors.Manager {
    public partial class CursorManager {
        private static readonly CursorPositionType[] ValidCursorPositionType = (CursorPositionType[])Enum.GetValues(typeof(CursorPositionType));

        protected override void DrawHeader() {
            if (ImGui.Checkbox("Cursor Enabled" + _ID, ref JobBars.Config.CursorsEnabled)) {
                JobBars.Config.Save();
            }

            if (ImGui.Checkbox("Hide Cursor when Mouse Held" + _ID, ref JobBars.Config.CursorHideWhenHeld)) {
                JobBars.Config.Save();
            }

            if (ImGui.Checkbox("Hide Cursor When Out Of Combat", ref JobBars.Config.CursorHideOutOfCombat)) {
                JobBars.Config.Save();
            }

            if (ImGui.Checkbox("Hide Cursor When Weapon Sheathed", ref JobBars.Config.CursorHideWeaponSheathed)) {
                JobBars.Config.Save();
            }

            if (JobBars.DrawCombo(ValidCursorPositionType, JobBars.Config.CursorPosition, "Cursor Positioning", _ID, out var newPosition)) {
                JobBars.Config.CursorPosition = newPosition;
                JobBars.Config.Save();
            }

            if (JobBars.Config.CursorPosition == CursorPositionType.CustomPosition) {
                if (ImGui.InputFloat2("Custom Cursor Position", ref JobBars.Config.CursorCustomPosition)) {
                    JobBars.Config.Save();
                }
            }

            if (ImGui.InputFloat("Inner Scale" + _ID, ref JobBars.Config.CursorInnerScale)) {
                JobBars.Config.Save();
            }

            if (ImGui.InputFloat("Outer Scale" + _ID, ref JobBars.Config.CursorOuterScale)) {
                JobBars.Config.Save();
            }

            if (Configuration.DrawColor("Inner Color", InnerColor, out var newColorInner)) {
                InnerColor = newColorInner;
                JobBars.Config.CursorInnerColor = newColorInner.Name;
                JobBars.Config.Save();

                JobBars.Builder.SetCursorInnerColor(InnerColor);
            }

            if (Configuration.DrawColor("Outer Color", OuterColor, out var newColorOuter)) {
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
