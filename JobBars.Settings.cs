using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace JobBars {
    public unsafe partial class JobBars {
        public bool Visible = false;
        private bool GAUGE_LOCK = true;
        private bool BUFF_LOCK = true;

        private JobIds Buff_JobSelected = JobIds.OTHER;
        private JobIds Gauge_JobSelected = JobIds.OTHER;

        private void BuildSettingsUI() {
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

                if (ImGui.BeginTabItem("Buffs" + _ID)) {
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
                        if (DrawPositionView("##GaugePosition" + gauge.Name, gauge.Name, Configuration.Config.GetGaugeSplitPosition(gauge.Name), out var pos)) {
                            gauge.SetSplitPosition(pos);
                        }
                    }
                    
                }
                else {
                    if(DrawPositionView("##GaugePosition", "Gauge Bar", Configuration.Config.GaugePosition, out var pos)) {
                        SetGaugePosition(pos);
                    }
                }
            }
            // ====== BUFF POSITION =======
            if (!BUFF_LOCK) {
                if(DrawPositionView("##BuffPosition", "Buff Bar", Configuration.Config.BuffPosition, out var pos)) {
                    SetBuffPosition(pos);
                }
            }
        }

        private static bool DrawPositionView(string _ID, string text, Vector2 position, out Vector2 newPosition) {
            ImGuiHelpers.ForceNextWindowMainViewport();
            ImGui.SetNextWindowPos(position, ImGuiCond.Once);
            ImGui.SetNextWindowSize(new Vector2(200, 200));
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.7f);
            ImGui.Begin(_ID, ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize);
            ImGui.Text(text);
            newPosition = ImGui.GetWindowPos();
            ImGui.PopStyleVar(1);
            ImGui.End();
            return newPosition != position;
        }

        private void SetGaugePosition(Vector2 pos) {
            Configuration.Config.GaugePosition = pos;
            Configuration.Config.Save();
            UI?.SetGaugePosition(pos);
        }

        private void SetBuffPosition(Vector2 pos) {
            Configuration.Config.BuffPosition = pos;
            Configuration.Config.Save();
            UI?.SetBuffPosition(pos);
        }

        private void DrawGaugeSettings() {
            if (GManager == null) return;
            string _ID = "##JobBars_Gauges";

            // ===== GENERAL GAUGE =======
            ImGui.Checkbox("Locked" + _ID, ref GAUGE_LOCK);
            ImGui.SameLine();

            if (ImGui.Checkbox("Gauges Enabled" + _ID, ref Configuration.Config.GaugesEnabled)) {
                Configuration.Config.Save();
                if (Configuration.Config.GaugesEnabled) {
                    UI.ShowGauges();
                }
                else {
                    UI.HideGauges();
                }
            }
            ImGui.SameLine();

            if (ImGui.Checkbox("Split Gauges" + _ID, ref Configuration.Config.GaugeSplit)) {
                GManager.Reset();
                Configuration.Config.Save();
            }

            if (ImGui.InputFloat("Scale" + _ID, ref Configuration.Config.GaugeScale)) {
                UI.SetGaugeScale(Configuration.Config.GaugeScale);
                Configuration.Config.Save();
            }

            if(ImGui.Checkbox("DoT Icon Replacement (Global)", ref Configuration.Config.GaugeIconReplacement)) {
                UIIconManager.Manager.Reset();
                Configuration.Config.Save();
            }
            
            ImGui.SetNextItemWidth(25f);
            if (ImGui.InputInt("Sound Effect # When DoTs Are Low (0 = off)", ref Configuration.Config.SeNumber,0)) {
                if (Configuration.Config.SeNumber < 0) Configuration.Config.SeNumber = 0;
                if (Configuration.Config.SeNumber >16) Configuration.Config.SeNumber = 16;
                Configuration.Config.Save();
            }

            if (ImGui.Checkbox("Horizontal Gauges", ref Configuration.Config.GaugeHorizontal)) {
                GManager.SetPositionScale();
                Configuration.Config.Save();
            }
            ImGui.SameLine();

            if (ImGui.Checkbox("Align Right", ref Configuration.Config.GaugeAlignRight)) {
                GManager.SetPositionScale();
                Configuration.Config.Save();
            }

            if (ImGui.Checkbox("Hide GCD Gauges When Inactive", ref Configuration.Config.GaugeHideGCDInactive)) {
                GManager.Reset();
                Configuration.Config.Save();
            }

            ImGui.BeginChild(_ID + "/Child", ImGui.GetContentRegionAvail(), true);
            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, 150);

            ImGui.BeginChild(_ID + "Tree");
            foreach(var job in GManager.JobToGauges.Keys) {
                if (job == JobIds.OTHER) continue;
                if (ImGui.Selectable(job + _ID + "/Job", Gauge_JobSelected == job)) {
                    Gauge_JobSelected = job;
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();

            if (Gauge_JobSelected == JobIds.OTHER) {
                ImGui.Text("Select a job...");
            }
            else {
                ImGui.BeginChild(_ID + "Selected");
                foreach (var gauge in GManager.JobToGauges[Gauge_JobSelected]) {
                    gauge.Draw(_ID, Gauge_JobSelected);
                }
                ImGui.EndChild();
            }
            ImGui.Columns(1);
            ImGui.EndChild();
        }

        private void DrawBuffSettings() {
            if (BManager == null) return;
            string _ID = "##JobBars_Buffs";

            // ===== GENERAL BUFFS =======
            ImGui.Checkbox("Locked" + _ID, ref BUFF_LOCK);
            ImGui.SameLine();

            if (ImGui.Checkbox("Buff Bar Enabled" + _ID, ref Configuration.Config.BuffBarEnabled)) {
                Configuration.Config.Save();
                if(Configuration.Config.BuffBarEnabled) {
                    UI.ShowBuffs();
                }
                else {
                    UI.HideBuffs();
                }
            }

            if (ImGui.InputFloat("Scale" + _ID, ref Configuration.Config.BuffScale)) {
                UI.SetBuffScale(Configuration.Config.BuffScale);
                Configuration.Config.Save();
            }

            if (ImGui.InputInt("Buffs Per Line" + _ID, ref Configuration.Config.BuffHorizontal)) {
                Configuration.Config.Save();
            }

            if (ImGui.Checkbox("Right-to-Left" + _ID, ref Configuration.Config.BuffRightToLeft)) {
                Configuration.Config.Save();
            }
            ImGui.SameLine();

            if (ImGui.Checkbox("Bottom-to-Top" + _ID, ref Configuration.Config.BuffBottomToTop)) {
                Configuration.Config.Save();
            }

            var size = ImGui.GetContentRegionAvail();
            ImGui.BeginChild(_ID + "/Child", size, true);
            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, 150);

            ImGui.BeginChild(_ID + "Tree");
            foreach (var job in BManager.JobToBuffs.Keys) {
                if (job == JobIds.OTHER) continue;
                if (ImGui.Selectable(job + _ID + "/Job", Buff_JobSelected == job)) {
                    Buff_JobSelected = job;
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();

            if (Buff_JobSelected == JobIds.OTHER) {
                ImGui.Text("Select a job...");
            }
            else {
                ImGui.BeginChild(_ID + "Selected");
                foreach (var buff in BManager.JobToBuffs[Buff_JobSelected]) {
                    buff.Draw(_ID, Buff_JobSelected);
                }
                ImGui.EndChild();
            }
            ImGui.Columns(1);
            ImGui.EndChild();
        }
    }
}
