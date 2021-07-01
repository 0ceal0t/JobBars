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

        private void BuildSettingsUI() {
            if (!Ready || !Init) return;

            string _ID = "##JobBars_Settings";
            ImGuiHelpers.ForceNextWindowMainViewport();
            ImGui.SetNextWindowSize(new Vector2(500, 800), ImGuiCond.FirstUseEver);
            if (Visible && ImGui.Begin("JobBars Settings", ref Visible)) {
                ImGui.BeginTabBar("Tabs" + _ID);
                if (ImGui.BeginTabItem("Gauges" + _ID)) {
                    GManager?.Draw();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Buffs" + _ID)) {
                    BManager?.Draw();
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
                ImGui.End();
            }
            GManager?.DrawPositionBox();
            BManager?.DrawPositionBox();
        }

        public static bool DrawPositionView(string _ID, string text, Vector2 position, out Vector2 newPosition) {
            ImGuiHelpers.ForceNextWindowMainViewport();
            ImGui.SetNextWindowPos(position, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSize(new Vector2(200, 200));
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.7f);
            ImGui.Begin(text + _ID, ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoResize);
            newPosition = ImGui.GetWindowPos();
            if (ImGui.InputFloat2("Position" + _ID, ref newPosition)) {
                ImGui.SetWindowPos(text + _ID, newPosition);
            }
            ImGui.PopStyleVar(1);
            ImGui.End();
            return newPosition != position;
        }
    }
}
