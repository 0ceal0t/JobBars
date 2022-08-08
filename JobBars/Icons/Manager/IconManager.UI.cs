using ImGuiNET;
using JobBars.Data;

namespace JobBars.Icons.Manager {
    public partial class IconManager {
        protected override void DrawHeader() {
            if (ImGui.Checkbox("Icon replacement enabled", ref JobBars.Config.IconsEnabled)) {
                JobBars.Config.Save();
                Reset();
            }
        }

        protected override void DrawSettings() { }

        protected override void DrawItem(IconReplacer[] item, JobIds _) {
            foreach (var icon in item) {
                icon.Draw(Id, SelectedJob);
            }
        }
    }
}