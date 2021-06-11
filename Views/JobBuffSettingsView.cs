using ImGuiNET;
using JobBars.Buffs;
using JobBars.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Views {
    public class JobBuffSettingsView {
        public JobIds Job;
        private List<BuffSettingsView> Buffs;

        public JobBuffSettingsView(JobIds job, Buff[] buffs, BuffManager manager) {
            Job = job;
            Buffs = buffs.Select(g => new BuffSettingsView(job, g, manager)).ToList();
        }

        public void Draw(string _ID) {
            ImGui.BeginChild(_ID + "Selected");
            foreach (var buffs in Buffs) {
                buffs.Draw(_ID);
            }
            ImGui.EndChild();
        }
    }
}
