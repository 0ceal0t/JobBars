using Dalamud.Logging;
using ImGuiNET;
using JobBars.Data;
using System;
using System.Numerics;

namespace JobBars.Gauges {
    public partial class GaugeManager {
        private bool LOCKED = true;

        public static readonly GaugePositionType[] ValidGaugePositionType = (GaugePositionType[])Enum.GetValues(typeof(GaugePositionType));

        protected override void DrawHeader() {
            ImGui.Checkbox("Position Locked" + _ID, ref LOCKED);

            if (ImGui.Checkbox("Gauges Enabled" + _ID, ref JobBars.Config.GaugesEnabled)) {
                if (JobBars.Config.GaugesEnabled) JobBars.Builder.ShowGauges();
                else JobBars.Builder.HideGauges();
                JobBars.Config.Save();
            }

            if (ImGui.Checkbox("Hide Gauges When Inactive", ref JobBars.Config.GaugesHideWhenInactive)) {
                Reset();
                JobBars.Config.Save();
            }

            ImGui.SameLine(250);
            if (ImGui.Checkbox("Hide Gauges When Out Of Combat", ref JobBars.Config.GaugesHideOutOfCombat)) {
                if (!JobBars.Config.GaugesHideOutOfCombat && JobBars.Config.GaugesEnabled) { // since they might be hidden
                    JobBars.Builder.ShowGauges();
                }
                JobBars.Config.Save();
            }

            ImGui.SetNextItemWidth(25f);
            if (ImGui.InputInt("Sound # When DoTs Low (0 = off)", ref JobBars.Config.GaugeSoundEffect, 0)) {
                if (JobBars.Config.GaugeSoundEffect < 0) JobBars.Config.GaugeSoundEffect = 0;
                if (JobBars.Config.GaugeSoundEffect > 16) JobBars.Config.GaugeSoundEffect = 16;
                JobBars.Config.Save();
            }

            ImGui.SameLine(250);
            ImGui.SetNextItemWidth(50f);
            if (ImGui.InputFloat("DoT Low Warning Time (0 = off)", ref JobBars.Config.GaugeLowTimerWarning)) {
                JobBars.Config.Save();
            }

            if (JobBars.Config.GaugePositionType != GaugePositionType.Split) {
                if (ImGui.Checkbox("Horizontal Gauges", ref JobBars.Config.GaugeHorizontal)) {
                    UpdatePositionScale();
                    JobBars.Config.Save();
                }

                ImGui.SameLine(250);
                if (ImGui.Checkbox("Bottom-to-Top", ref JobBars.Config.GaugeBottomToTop)) {
                    UpdatePositionScale();
                    JobBars.Config.Save();
                }

                ImGui.SameLine(500);
                if (ImGui.Checkbox("Align Right", ref JobBars.Config.GaugeAlignRight)) {
                    UpdatePositionScale();
                    JobBars.Config.Save();
                }
            }

            if (ImGui.BeginCombo("Gauge Positioning" + _ID, $"{JobBars.Config.GaugePositionType}")) {
                foreach (var positionType in ValidGaugePositionType) {
                    if (ImGui.Selectable($"{positionType}" + _ID, positionType == JobBars.Config.GaugePositionType)) {
                        JobBars.Config.GaugePositionType = positionType;
                        JobBars.Config.Save();

                        UpdatePositionScale();
                    }
                }
                ImGui.EndCombo();
            }

            if (JobBars.Config.GaugePositionType == GaugePositionType.Global) { // GLOBAL POSITIONING
                var pos = JobBars.Config.GaugePositionGlobal;
                if (ImGui.InputFloat2("Position" + _ID, ref pos)) {
                    SetGaugePositionGlobal(pos);
                }
            }
            else if (JobBars.Config.GaugePositionType == GaugePositionType.PerJob) { // PER-JOB POSITIONING
                var pos = GetPerJobPosition();
                if (ImGui.InputFloat2($"Position ({CurrentJob})" + _ID, ref pos)) {
                    SetGaugePositionPerJob(CurrentJob, pos);
                }
            }

            if (ImGui.InputFloat("Scale" + _ID, ref JobBars.Config.GaugeScale)) {
                UpdatePositionScale();
                JobBars.Config.Save();
            }
        }

        protected override void DrawItem(Gauge item) {
            item.Draw(_ID, SelectedJob);
        }

        protected override string ItemToString(Gauge item) {
            return item.Name;
        }

        public void DrawPositionBox() {
            if (!LOCKED) {
                if (JobBars.Config.GaugePositionType == GaugePositionType.Split) {
                    foreach (var gauge in CurrentGauges) gauge.DrawPositionBox();
                }
                else if(JobBars.Config.GaugePositionType == GaugePositionType.PerJob) {
                    var currentPos = GetPerJobPosition();
                    if (JobBars.DrawPositionView($"Gauge Bar ({CurrentJob})##GaugePosition", currentPos, out var pos)) {
                        SetGaugePositionPerJob(CurrentJob, pos);
                    }
                }
                else { // GLOBAL
                    var currentPos = JobBars.Config.GaugePositionGlobal;
                    if (JobBars.DrawPositionView("Gauge Bar##GaugePosition", currentPos, out var pos)) {
                        SetGaugePositionGlobal(pos);
                    }
                }
            }
        }

        private static void SetGaugePositionGlobal(Vector2 pos) {
            JobBars.SetWindowPosition("Gauge Bar##GaugePosition", pos);
            JobBars.Config.GaugePositionGlobal = pos;
            JobBars.Config.Save();
            JobBars.Builder.SetGaugePosition(pos);
        }

        private static void SetGaugePositionPerJob(JobIds job, Vector2 pos) {
            JobBars.SetWindowPosition($"Gauge Bar ({job})##GaugePosition", pos);
            JobBars.Config.GaugePerJobPosition.Set($"{job}", pos);
            JobBars.Config.Save();
            JobBars.Builder.SetGaugePosition(pos);
        }
    }
}
