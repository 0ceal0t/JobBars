using ImGuiNET;
using JobBars.Data;
using JobBars.Gauges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Views {
    public class JobGaugeSettingsView {
        public JobIds Job;
        private List<GaugeSettingsView> Gauges;

        public JobGaugeSettingsView(JobIds job, Gauge[] gauges, GaugeManager manager) {
            Job = job;
            Gauges = gauges.Select(g => new GaugeSettingsView(job, g, manager)).ToList();
        }

        public void Draw(string _ID) {
            ImGui.BeginChild(_ID + "Selected");
            foreach(var gauge in Gauges) {
                gauge.Draw(_ID);
            }
            ImGui.EndChild();
        }
    }
}
