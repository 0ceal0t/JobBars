using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.UI;
using JobBars.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars {
    public unsafe partial class JobBars {
        public bool Visible = false;
        private bool GAUGE_LOCK = true;
        private bool BUFF_LOCK = true;

        private JobGaugeSettingsView Gauge_SelectedJob;
        private Dictionary<JobIds, JobGaugeSettingsView> JobGaugeSettings;

        private JobBuffSettingsView Buff_SelectedJob;
        private Dictionary<JobIds, JobBuffSettingsView> JobBuffSettings;

        private void SetupUI() {
            Gauge_SelectedJob = null;
            JobGaugeSettings = new();
            foreach(var entry in GManager.JobToGauges) {
                if (entry.Key == JobIds.OTHER) continue;
                JobGaugeSettings[entry.Key] = new JobGaugeSettingsView(entry.Key, entry.Value, GManager);
            }

            Buff_SelectedJob = null;
            JobBuffSettings = new();
            foreach (var entry in BManager.JobToBuffs) {
                if (entry.Key == JobIds.OTHER) continue;
                JobBuffSettings[entry.Key] = new JobBuffSettingsView(entry.Key, entry.Value, BManager);
            }
        }

        private void BuildUI() {
            if (!Ready || !Init) return;
            // ====== SETTINGS =======
            string _ID = "##JobBars_Settings";
            ImGuiHelpers.ForceNextWindowMainViewport();
            ImGui.SetNextWindowSize(new Vector2(500, 800), ImGuiCond.FirstUseEver);
            if (Visible && ImGui.Begin("JobBars Settings", ref Visible)) {
                ImGui.BeginTabBar("Tabs" + _ID);
                if (ImGui.BeginTabItem("Gauges" + _ID)) {
                    DrawGaugeSettings();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Party Member Buffs" + _ID)) {
                    DrawBuffSettings();
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
                ImGui.End();
            }
            // ====== GAUGE POSITION =======
            if (!GAUGE_LOCK) {
                if(Configuration.Config.GaugeSplit) {
                    foreach(var gauge in GManager.CurrentGauges) {
                        if (gauge.StartHidden) continue;
                        if (DrawPositionView("##GaugePosition" + gauge.Name, gauge.Name, Configuration.Config.GetGaugeSplitPosition(gauge.Name), out var pos)) {
                            Configuration.Config.GaugeSplitPosition[gauge.Name] = pos;
                            Configuration.Config.Save();
                            gauge.UI?.SetSplitPosition(pos);
                        }
                    }
                    
                }
                else {
                    if(DrawPositionView("##GaugePosition", "Gauge Bar Position", Configuration.Config.GaugePosition, out var pos)) {
                        Configuration.Config.GaugePosition = pos;
                        Configuration.Config.Save();
                        UI?.SetGaugePosition(pos);
                    }
                }
            }
            // ====== BUFF POSITION =======
            if (!BUFF_LOCK) {
                if(DrawPositionView("##BuffPosition", "Buff Bar Position", Configuration.Config.BuffPosition, out var pos)) {
                    Configuration.Config.BuffPosition = pos;
                    Configuration.Config.Save();
                    UI?.SetBuffPosition(pos);
                }
            }
        }

        private bool DrawPositionView(string _ID, string text, Vector2 position, out Vector2 newPosition) {
            ImGuiHelpers.ForceNextWindowMainViewport();
            ImGui.SetNextWindowPos(position, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSize(new Vector2(200, 200));
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.7f);
            ImGui.Begin(_ID, ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize);
            ImGui.Text(text);
            newPosition = ImGui.GetWindowPos();
            ImGui.PopStyleVar(1);
            ImGui.End();
            return newPosition != position;
        }

        private void DrawGaugeSettings() {
            if (GManager == null) return;
            string _ID = "##JobBars_Gauges";

            // ===== GENERAL GAUGE =======
            ImGui.Checkbox("Locked" + _ID, ref GAUGE_LOCK);
            if (ImGui.Checkbox("Split Gauges" + _ID, ref Configuration.Config.GaugeSplit)) {
                GManager.Reset();
                Configuration.Config.Save();
            }
            if (ImGui.InputFloat("Scale" + _ID, ref Configuration.Config.GaugeScale)) {
                UI.SetGaugeScale(Configuration.Config.GaugeScale);
                Configuration.Config.Save();
            }
            if(ImGui.Checkbox("DoT Icon Replacement", ref Configuration.Config.GaugeIconReplacement)) {
                GManager.Reset();
                Configuration.Config.Save();
            }
            
            ImGui.SetNextItemWidth(25f);
            if (ImGui.InputInt("Sound effect # when DoTs are low (0 = off)",ref Configuration.Config.SeNumber,0)) {
                if (Configuration.Config.SeNumber < 0) Configuration.Config.SeNumber = 0;
                if (Configuration.Config.SeNumber >16) Configuration.Config.SeNumber = 16;
                Configuration.Config.Save();
            }

            if (ImGui.Checkbox("Horizontal Gauges", ref Configuration.Config.GaugeHorizontal)) {
                GManager.Reset();
                Configuration.Config.Save();
            }
            
            if (ImGui.Checkbox("Align Right", ref Configuration.Config.GaugeAlignRight)) {
                GManager.Reset();
                Configuration.Config.Save();
            }

            ImGui.BeginChild(_ID + "/Child", ImGui.GetContentRegionAvail(), true);
            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, 150);

            ImGui.BeginChild(_ID + "Tree");
            foreach(var job in JobGaugeSettings) {
                if (ImGui.Selectable(job.Key + _ID + "/Job", Gauge_SelectedJob == job.Value)) {
                    Gauge_SelectedJob = job.Value;
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();

            if (Gauge_SelectedJob == null) {
                ImGui.Text("Select a job...");
            }
            else {
                Gauge_SelectedJob.Draw(_ID);
            }
            ImGui.Columns(1);
            ImGui.EndChild();
        }

        private void DrawBuffSettings() {
            if (GManager == null) return;
            string _ID = "##JobBars_Buffs";

            // ===== GENERAL BUFFS =======
            ImGui.Checkbox("Locked" + _ID, ref BUFF_LOCK);
            if(ImGui.Checkbox("Buff Bar Enabled" + _ID, ref Configuration.Config.BuffBarEnabled)) {
                BManager.Reset();
                Configuration.Config.Save();
            }
            if (ImGui.InputFloat("Scale" + _ID, ref Configuration.Config.BuffScale)) {
                UI.SetBuffScale(Configuration.Config.BuffScale);
                Configuration.Config.Save();
            }

            var size = ImGui.GetContentRegionAvail();
            ImGui.BeginChild(_ID + "/Child", size, true);
            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, 150);

            ImGui.BeginChild(_ID + "Tree");
            foreach (var job in JobBuffSettings) {
                if (ImGui.Selectable(job.Key + _ID + "/Job", Buff_SelectedJob == job.Value)) {
                    Buff_SelectedJob = job.Value;
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();

            if (Buff_SelectedJob == null) {
                ImGui.Text("Select a job...");
            }
            else {
                Buff_SelectedJob.Draw(_ID);
            }
            ImGui.Columns(1);
            ImGui.EndChild();
        }
    }
}
