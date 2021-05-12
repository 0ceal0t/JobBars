using Dalamud.Interface;
using ImGuiNET;
using JobBars.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JobBars {
    public unsafe partial class JobBars {

        public bool Visible = true;
        private bool GAUGE_LOCK = true;
        private bool BUFF_LOCK = true;

        private void BuildUI() {
            if (STEP != STEPS.READY) return;
            // ====== SETTINGS =======
            string _ID = "##JobBars_Settings";
            ImGuiHelpers.ForceNextWindowMainViewport();
            ImGui.SetNextWindowSize(new Vector2(500, 800), ImGuiCond.FirstUseEver);
            if (Visible && ImGui.Begin("Settings", ref Visible)) {
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
                ImGuiHelpers.ForceNextWindowMainViewport();
                ImGui.SetNextWindowPos(Configuration.Config.GaugePosition, ImGuiCond.FirstUseEver);
                ImGui.SetNextWindowSize(new Vector2(200, 200));
                ImGui.Begin("##GaugePosition", ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize);
                ImGui.Text("Gauge Bar Position");

                var pos = ImGui.GetWindowPos();
                if (pos != Configuration.Config.GaugePosition) {
                    Configuration.Config.GaugePosition = pos;
                    Configuration.Config.Save();
                    UI?.SetGaugePosition(pos);
                }

                ImGui.End();
            }
            // ====== BUFF POSITION =======
            if (!BUFF_LOCK) {
                ImGuiHelpers.ForceNextWindowMainViewport();
                ImGui.SetNextWindowPos(Configuration.Config.BuffPosition, ImGuiCond.FirstUseEver);
                ImGui.SetNextWindowSize(new Vector2(200, 200));
                ImGui.Begin("##BuffPosition", ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize);
                ImGui.Text("Buff Bar Position");

                var pos = ImGui.GetWindowPos();
                if (pos != Configuration.Config.BuffPosition) {
                    Configuration.Config.BuffPosition = pos;
                    Configuration.Config.Save();
                    UI?.SetBuffPosition(pos);
                }

                ImGui.End();
            }
        }

        JobIds G_SelectedJob = JobIds.OTHER;
        private void DrawGaugeSettings() {
            if (GManager == null) return;

            string _ID = "##JobBars_Gauges";
            if (ImGui.Checkbox("Locked" + _ID, ref GAUGE_LOCK)) {
            }
            if (ImGui.InputFloat("Scale" + _ID, ref Configuration.Config.GaugeScale)) {
                UI?.SetGaugeScale(Configuration.Config.GaugeScale);
                Configuration.Config.Save();
            }

            var size = ImGui.GetContentRegionAvail() - new Vector2(0, ImGui.GetTextLineHeight() + 10);
            ImGui.BeginChild(_ID + "/Child", size, true);
            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, 150);

            ImGui.BeginChild(_ID + "Tree");
            foreach (var job in GManager.JobToGauges.Keys) {
                if (job == JobIds.OTHER) continue;
                if (ImGui.Selectable(job + _ID + "/Job", G_SelectedJob == job)) {
                    G_SelectedJob = job;
                }
            }
            ImGui.EndChild();

            ImGui.NextColumn();

            if (G_SelectedJob == JobIds.OTHER) {
                ImGui.Text("Select a job...");
            }
            else {
                ImGui.BeginChild(_ID + "Selected");
                foreach (var gauge in GManager.JobToGauges[G_SelectedJob]) {
                    ImGui.TextColored(new Vector4(0, 1, 0, 1), gauge.Name);

                    // ===== ENABLED / DISABLED ======
                    var _enabled = !Configuration.Config.GaugeDisabled.Contains(gauge.Name);
                    if(ImGui.Checkbox("Enabled" + _ID + gauge.Name, ref _enabled)) {
                        if(_enabled) {
                            Configuration.Config.GaugeDisabled.Remove(gauge.Name);
                        }
                        else {
                            Configuration.Config.GaugeDisabled.Add(gauge.Name);
                        }
                        Configuration.Config.Save();
                        GManager.ResetJob(G_SelectedJob);
                    }
                    // ===== COLOR =======
                    ImGui.BeginCombo("Color" + _ID + gauge.Name, gauge.Visual.Color.Name);

                    ImGui.EndCombo();
                }
                ImGui.EndChild();
            }

            ImGui.Columns(1);
            ImGui.EndChild();
            if (ImGui.SmallButton("SAVE" + _ID)) {

            }
        }

        JobIds B_SelectedJob = JobIds.OTHER;
        private void DrawBuffSettings() {
            if (GManager == null) return;

            string _ID = "##JobBars_Buffs";
            if (ImGui.Checkbox("Locked" + _ID, ref BUFF_LOCK)) {
            }
            if (ImGui.InputFloat("Scale" + _ID, ref Configuration.Config.BuffScale)) {
                UI?.SetBuffScale(Configuration.Config.BuffScale);
                Configuration.Config.Save();
            }

            var size = ImGui.GetContentRegionAvail() - new Vector2(0, ImGui.GetTextLineHeight() + 10);
            ImGui.BeginChild(_ID + "/Child", size, true);
            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, 150);

            // ....

            var spaceLeft = ImGui.GetContentRegionAvail().Y;
            if (spaceLeft > 0) {
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + spaceLeft);
            }

            ImGui.NextColumn();

            if (B_SelectedJob == JobIds.OTHER) {
                ImGui.Text("Select a job...");
            }
            else {
                ImGui.BeginChild(_ID + "Selected");

                //....

                ImGui.EndChild();
            }

            ImGui.Columns(1);
            ImGui.EndChild();
            if (ImGui.SmallButton("SAVE" + _ID)) {

            }
        }
    }
}
