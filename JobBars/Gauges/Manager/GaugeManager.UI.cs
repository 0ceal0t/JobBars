using Dalamud.Logging;
using ImGuiNET;
using JobBars.Data;
using System;
using System.Numerics;

namespace JobBars.Gauges.Manager {
    public partial class GaugeManager {
        public bool LOCKED = true;

        private static readonly GaugePositionType[] ValidGaugePositionType = (GaugePositionType[])Enum.GetValues(typeof(GaugePositionType));

        private readonly InfoBox<GaugeManager> PositionInfoBox = new() {
            Label = "Position",
            ContentsAction = (GaugeManager manager) => {
                ImGui.Checkbox("Position locked" + manager.Id, ref manager.LOCKED);

                if (JobBars.Config.GaugePositionType != GaugePositionType.Split) {
                    if (ImGui.Checkbox("Horizontal gauges", ref JobBars.Config.GaugeHorizontal)) {
                        manager.UpdatePositionScale();
                        JobBars.Config.Save();
                    }

                    if (ImGui.Checkbox("Bottom-to-top", ref JobBars.Config.GaugeBottomToTop)) {
                        manager.UpdatePositionScale();
                        JobBars.Config.Save();
                    }

                    if (ImGui.Checkbox("Align right", ref JobBars.Config.GaugeAlignRight)) {
                        manager.UpdatePositionScale();
                        JobBars.Config.Save();
                    }
                }

                if (JobBars.DrawCombo(ValidGaugePositionType, JobBars.Config.GaugePositionType, "Gauge positioning", manager.Id, out var newPosition)) {
                    JobBars.Config.GaugePositionType = newPosition;
                    JobBars.Config.Save();

                    manager.UpdatePositionScale();
                }

                if (JobBars.Config.GaugePositionType == GaugePositionType.Global) { // GLOBAL POSITIONING
                    var pos = JobBars.Config.GaugePositionGlobal;
                    if (ImGui.InputFloat2("Position" + manager.Id, ref pos)) {
                        SetGaugePositionGlobal(pos);
                    }
                }
                else if (JobBars.Config.GaugePositionType == GaugePositionType.PerJob) { // PER-JOB POSITIONING
                    var pos = manager.GetPerJobPosition();
                    if (ImGui.InputFloat2($"Position ({manager.CurrentJob})" + manager.Id, ref pos)) {
                        SetGaugePositionPerJob(manager.CurrentJob, pos);
                    }
                }

                if (ImGui.InputFloat("Scale" + manager.Id, ref JobBars.Config.GaugeScale)) {
                    manager.UpdatePositionScale();
                    JobBars.Config.Save();
                }
            }
        };

        private readonly InfoBox<GaugeManager> HideWhenInfoBox = new() {
            Label = "Hide When",
            ContentsAction = (GaugeManager manager) => {
                if (ImGui.Checkbox("Out of combat", ref JobBars.Config.GaugesHideOutOfCombat)) JobBars.Config.Save();
                if (ImGui.Checkbox("Weapon sheathed", ref JobBars.Config.GaugesHideWeaponSheathed)) JobBars.Config.Save();
            }
        };

        protected override void DrawHeader() {
            if (ImGui.Checkbox("Gauges Enabled" + Id, ref JobBars.Config.GaugesEnabled)) {
                JobBars.Config.Save();
            }
        }

        protected override void DrawSettings() {
            PositionInfoBox.Draw(this);
            HideWhenInfoBox.Draw(this);

            if (ImGui.Checkbox("Pulse diamond and arrow color", ref JobBars.Config.GaugePulse)) JobBars.Config.Save();

            ImGui.SetNextItemWidth(50f);
            if (ImGui.InputFloat("Slidecast seconds (0 = off)", ref JobBars.Config.GaugeSlidecastTime)) JobBars.Config.Save();
        }

        public void DrawPositionBox() {
            if (LOCKED) return;
            if (JobBars.Config.GaugePositionType == GaugePositionType.Split) {
                foreach (var config in CurrentConfigs) config.DrawPositionBox();
            }
            else if (JobBars.Config.GaugePositionType == GaugePositionType.PerJob) {
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

        // ==========================================

        protected override void DrawItem(GaugeConfig item) {
            ImGui.Indent(5);
            item.Draw(Id, out bool newVisual, out bool reset);
            ImGui.Unindent();

            if (SelectedJob != CurrentJob) return;
            if (newVisual) {
                UpdateVisuals();
                UpdatePositionScale();
            }
            if (reset) Reset();
        }

        protected override string ItemToString(GaugeConfig item) => item.Name;

        protected override bool IsEnabled(GaugeConfig item) => item.Enabled;
    }
}
