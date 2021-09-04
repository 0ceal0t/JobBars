using ImGuiNET;

namespace JobBars.Icons {
    public partial class IconManager {
        protected override void DrawHeader() {
            if (ImGui.Checkbox("Icon Replacement Enabled", ref JobBars.Config.IconsEnabled)) {
                JobBars.Config.Save();
                JobBars.IconBuilder.Reset();
            }

            if (ImGui.Button("RESET")) {
                JobBars.IconBuilder.Reset();
            }
        }

        protected override void DrawItem(IconReplacer[] item) {
            foreach(var icon in item) {
                icon.Draw(_ID, SettingsJobSelected);
            }
        }
    }
}
