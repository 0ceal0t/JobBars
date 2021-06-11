using ImGuiNET;
using JobBars.Data;
using JobBars.Buffs;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars.Views {
    public class BuffSettingsView {
        private Buff Buff;
        private BuffManager Manager;
        private JobIds Job;

        public BuffSettingsView(JobIds job, Buff buff, BuffManager manager) {
            Job = job;
            Buff = buff;
            Manager = manager;
        }

        public void Draw(string _ID) {
            var _enabled = !Configuration.Config.BuffDisabled.Contains(Buff.Name);

            ImGui.TextColored(_enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1), $"{Buff.Name}");
            if (ImGui.Checkbox("Enabled" + _ID + Buff.Name, ref _enabled)) {
                if (_enabled) {
                    Configuration.Config.BuffDisabled.Remove(Buff.Name);
                }
                else {
                    Configuration.Config.BuffDisabled.Add(Buff.Name);
                }
                Configuration.Config.Save();
                Manager.Reset();
            }

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
        }
    }
}
