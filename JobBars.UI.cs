using Dalamud.Interface;
using ImGuiNET;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

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

            var size = ImGui.GetContentRegionAvail();
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
                    // ===== ENABLED / DISABLED ======
                    var _enabled = !Configuration.Config.GaugeDisabled.Contains(gauge.Name);
                    var type = gauge is GaugeGCD ? "GCDs" : "Timer";

                    ImGui.TextColored(_enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1), $"{gauge.Name} ({type})");
                    if (ImGui.Checkbox("Enabled" + _ID + gauge.Name, ref _enabled)) {
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
                    var isOverride_COLOR = Configuration.Config.GetColorOverride(gauge.Name, out var colorOverride);
                    if(ImGui.BeginCombo("Color" + _ID + gauge.Name, isOverride_COLOR ? gauge.Visual.Color.Name : $"DEFAULT ({gauge.Visual.Color.Name})")) {
                        if(ImGui.Selectable($"DEFAULT ({gauge.DefaultVisual.Color.Name}){_ID}{gauge.Name}", !isOverride_COLOR)) { // DEFAULT
                            Configuration.Config.GaugeColorOverride.Remove(gauge.Name);
                            Configuration.Config.Save();
                            SetColor(gauge, gauge.DefaultVisual.Color);
                        }
                        foreach(var entry in UIColor.AllColors) {
                            if(ImGui.Selectable($"{entry.Key}{_ID}{gauge.Name}", (gauge.Visual.Color.Name == entry.Key) && isOverride_COLOR)) { // OTHER
                                Configuration.Config.GaugeColorOverride[gauge.Name] = entry.Key;
                                Configuration.Config.Save();
                                SetColor(gauge, entry.Value);
                            }
                        }
                        ImGui.EndCombo();
                    }
                    // ====== TYPE (only for GCDs) ======
                    if(gauge is GaugeGCD) {
                        var isOverride_TYPE = Configuration.Config.GaugeTypeOverride.TryGetValue(gauge.Name, out var typeOverride);
                        if(ImGui.BeginCombo("Type" + _ID + gauge.Name, isOverride_TYPE ? $"{gauge.Visual.Type}" : $"DEFAULT ({gauge.Visual.Type})")) {
                            if(ImGui.Selectable($"DEFAULT ({gauge.DefaultVisual.Type}){_ID}{gauge.Name}", !isOverride_TYPE)) { // DEFAULT
                                Configuration.Config.GaugeTypeOverride.Remove(gauge.Name);
                                Configuration.Config.Save();
                                gauge.Visual.Type = gauge.DefaultVisual.Type;
                                GManager.ResetJob(G_SelectedJob);
                            }
                            foreach (GaugeVisualType gType in (GaugeVisualType[])Enum.GetValues(typeof(GaugeVisualType))) {
                                if (ImGui.Selectable($"{gType}{_ID}{gauge.Name}", (gauge.Visual.Type == gType) && isOverride_TYPE)) { // OTHER
                                    Configuration.Config.GaugeTypeOverride[gauge.Name] = gType;
                                    Configuration.Config.Save();
                                    gauge.Visual.Type = gType;
                                    GManager.ResetJob(G_SelectedJob);
                                }
                            }
                            ImGui.EndCombo();
                        }
                    }

                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
                }
                ImGui.EndChild();
            }

            ImGui.Columns(1);
            ImGui.EndChild();
        }
        public void SetColor(Gauge gauge, ElementColor color) {
            gauge.Visual.Color = color;
            gauge.SetColor();
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

            var size = ImGui.GetContentRegionAvail();
            ImGui.BeginChild(_ID + "/Child", size, true);
            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, 150);

            ImGui.BeginChild(_ID + "Tree");
            foreach (var job in BManager.JobToBuffs.Keys) {
                if (job == JobIds.OTHER) continue;
                if (ImGui.Selectable(job + _ID + "/Job", B_SelectedJob == job)) {
                    B_SelectedJob = job;
                }
            }
            ImGui.EndChild();

            ImGui.NextColumn();

            if (B_SelectedJob == JobIds.OTHER) {
                ImGui.Text("Select a job...");
            }
            else {
                ImGui.BeginChild(_ID + "Selected");
                foreach (var buff in BManager.JobToBuffs[B_SelectedJob]) {
                    ImGui.Text(buff.Name);

                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
                }
                ImGui.EndChild();
            }

            ImGui.Columns(1);
            ImGui.EndChild();
        }
    }
}
