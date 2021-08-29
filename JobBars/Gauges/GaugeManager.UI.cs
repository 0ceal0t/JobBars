using ImGuiNET;
using System.Numerics;

namespace JobBars.Gauges {
    public partial class GaugeManager {
        private bool LOCKED = true;

        protected override void DrawHeader() {
            ImGui.Checkbox("Position Locked" + _ID, ref LOCKED);

            if (ImGui.Checkbox("Gauges Enabled" + _ID, ref JobBars.Config.GaugesEnabled)) {
                if (JobBars.Config.GaugesEnabled) JobBars.Builder.ShowGauges();
                else JobBars.Builder.HideGauges();
                JobBars.Config.Save();
            }

            if (ImGui.Checkbox("Split Gauges" + _ID, ref JobBars.Config.GaugeSplit)) {
                UpdatePositionScale();
                JobBars.Config.Save();
            }

            if (ImGui.InputFloat("Scale" + _ID, ref JobBars.Config.GaugeScale)) {
                UpdatePositionScale();
                JobBars.Config.Save();
            }

            var pos = JobBars.Config.GaugePosition;
            if (ImGui.InputFloat2("Position" + _ID, ref pos)) {
                SetGaugePosition(pos);
            }

            JobBars.Separator(); // =====================================

            if (ImGui.Checkbox("GCD Gauge Text Visible", ref JobBars.Config.GaugeGCDTextVisible)) {
                JobBars.Builder.SetGaugeTextVisible(JobBars.Config.GaugeGCDTextVisible);
                JobBars.Config.Save();
            }

            if (ImGui.Checkbox("Hide GCD Gauges When Inactive", ref JobBars.Config.GaugeHideGCDInactive)) {
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

            if (!JobBars.Config.GaugeSplit) {
                JobBars.Separator(); // =====================================

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
        }

        protected override void DrawItem(Gauge[] item) {
            foreach (var gauge in item) {
                gauge.Draw(_ID, SettingsJobSelected);
            }
        }

        public void DrawPositionBox() {
            if (!LOCKED) {
                if (JobBars.Config.GaugeSplit) {
                    foreach (var gauge in CurrentGauges) gauge.DrawPositionBox();
                }
                else {
                    var currentPos = JobBars.Config.GaugePosition;
                    if (JobBars.DrawPositionView("Gauge Bar##GaugePosition", currentPos, out var pos)) {
                        SetGaugePosition(pos);
                    }
                }
            }
        }

        private static void SetGaugePosition(Vector2 pos) {
            JobBars.SetWindowPosition("Gauge Bar##GaugePosition", pos);
            JobBars.Config.GaugePosition = pos;
            JobBars.Config.Save();
            JobBars.Builder.SetGaugePosition(pos);
        }
    }
}
