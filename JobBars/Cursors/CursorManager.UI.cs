using ImGuiNET;
using JobBars.Data;

namespace JobBars.Cursors {
    public partial class CursorManager {
        protected override void DrawHeader() {
            if (ImGui.Checkbox("Cursor Enabled" + _ID, ref JobBars.Config.CursorsEnabled)) {
                JobBars.Config.Save();
                if (JobBars.Config.CursorsEnabled) JobBars.Builder.ShowCursor();
                else JobBars.Builder.HideCursor();
            }

            if (ImGui.Checkbox("Hide Cursor when Mouse Held" + _ID, ref JobBars.Config.CursorHideWhenHeld)) {
                JobBars.Config.Save();
            }

            ImGui.SameLine(250);
            if (ImGui.Checkbox("Hide Cursor When Out Of Combat", ref JobBars.Config.CursorHideOutOfCombat)) {
                if (!JobBars.Config.CursorHideOutOfCombat && JobBars.Config.CursorsEnabled) { // since they might be hidden
                    JobBars.Builder.HideCursor();
                }
                JobBars.Config.Save();
            }

            if (ImGui.Checkbox("Keep Cursor in Center", ref JobBars.Config.CursorKeepInMiddle)) {
                JobBars.Config.Save();
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

        protected override void DrawItem(Cursor item) {
            item.Draw(_ID);
        }
    }
}
