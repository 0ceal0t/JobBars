using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Icons {
    public partial class IconManager {
        protected override void DrawHeader() {
            if (ImGui.Checkbox("Icon Replacement Enabled", ref JobBars.Config.IconsEnabled)) {
                JobBars.Config.Save();
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
