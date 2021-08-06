using ImGuiNET;
using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Gauges {
    public partial class GaugeManager {
        private bool LOCKED = true;
        private JobIds SettingsJobSelected = JobIds.OTHER;

        public void Draw() {
            string _ID = "##JobBars_Gauges";

            ImGui.Checkbox("Locked" + _ID, ref LOCKED);

            ImGui.SameLine();
            if (ImGui.Checkbox("Gauges Enabled" + _ID, ref Configuration.Config.GaugesEnabled)) {
                if (Configuration.Config.GaugesEnabled) UIBuilder.Builder.ShowGauges();
                else UIBuilder.Builder.HideGauges();
                Configuration.Config.Save();
            }

            ImGui.SameLine();
            if (ImGui.Checkbox("Split Gauges" + _ID, ref Configuration.Config.GaugeSplit)) {
                SetPositionScale();
                Configuration.Config.Save();
            }

            if (ImGui.InputFloat("Scale" + _ID, ref Configuration.Config.GaugeScale)) {
                SetPositionScale();
                Configuration.Config.Save();
            }

            JobBars.Separator(); // =====================================

            if (ImGui.Checkbox("DoT Icon Replacement (Global)", ref Configuration.Config.GaugeIconReplacement)) {
                UIIconManager.Manager.Reset();
                Configuration.Config.Save();
            }

            if (ImGui.Checkbox("Hide GCD Gauges When Inactive", ref Configuration.Config.GaugeHideGCDInactive)) {
                Reset();
                Configuration.Config.Save();
            }

            ImGui.SameLine();
            if (ImGui.Checkbox("Hide Gauges When Out Of Combat", ref Configuration.Config.GaugesHideOutOfCombat)) {
                if (!Configuration.Config.GaugesHideOutOfCombat && Configuration.Config.GaugesEnabled) { // since they might be hidden
                    UIBuilder.Builder.ShowGauges();
                }
                Configuration.Config.Save();
            }

            JobBars.Separator(); // =====================================

            ImGui.SetNextItemWidth(50f);
            if (ImGui.InputFloat("DoT Low Warning Time (0 = off)", ref Configuration.Config.GaugeLowTimerWarning)) {
                Configuration.Config.Save();
            }

            ImGui.SameLine();
            ImGui.SetNextItemWidth(25f);
            if (ImGui.InputInt("Sound Effect # When DoTs Are Low (0 = off)", ref Configuration.Config.SeNumber, 0)) {
                if (Configuration.Config.SeNumber < 0) Configuration.Config.SeNumber = 0;
                if (Configuration.Config.SeNumber > 16) Configuration.Config.SeNumber = 16;
                Configuration.Config.Save();
            }

            if (!Configuration.Config.GaugeSplit) {
                JobBars.Separator(); // =====================================

                if (ImGui.Checkbox("Horizontal Gauges", ref Configuration.Config.GaugeHorizontal)) {
                    SetPositionScale();
                    Configuration.Config.Save();
                }

                ImGui.SameLine();
                if (ImGui.Checkbox("Align Right", ref Configuration.Config.GaugeAlignRight)) {
                    SetPositionScale();
                    Configuration.Config.Save();
                }
            }

            ImGui.BeginChild(_ID + "/Child", ImGui.GetContentRegionAvail(), true);
            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, 150);

            ImGui.BeginChild(_ID + "Tree");
            foreach (var job in JobToGauges.Keys) {
                if (job == JobIds.OTHER) continue;
                if (ImGui.Selectable(job + _ID + "/Job", SettingsJobSelected == job)) {
                    SettingsJobSelected = job;
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();

            if (SettingsJobSelected == JobIds.OTHER) {
                ImGui.Text("Select a job...");
            }
            else {
                ImGui.BeginChild(_ID + "Selected");
                foreach (var gauge in JobToGauges[SettingsJobSelected]) {
                    gauge.Draw(_ID, SettingsJobSelected);
                }
                ImGui.EndChild();
            }
            ImGui.Columns(1);
            ImGui.EndChild();
        }

        public void DrawPositionBox() {
            if (!LOCKED) {
                if (Configuration.Config.GaugeSplit) {
                    foreach (var gauge in CurrentGauges) {
                        if (JobBars.DrawPositionView("##GaugePosition" + gauge.Name, gauge.Name, Configuration.Config.GetGaugeSplitPosition(gauge.Name), out var pos)) {
                            gauge.SetSplitPosition(pos);
                        }
                    }
                }
                else {
                    var currentPos = Configuration.Config.GaugePosition;
                    if (JobBars.DrawPositionView("##GaugePosition", "Gauge Bar", currentPos, out var pos)) {
                        SetGaugePosition(pos);
                    }
                }
            }
        }

        private void SetGaugePosition(Vector2 pos) {
            Configuration.Config.GaugePosition = pos;
            Configuration.Config.Save();
            UIBuilder.Builder.SetGaugePosition(pos);
        }
    }
}
