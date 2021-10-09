﻿using Dalamud.Interface;
using ImGuiNET;
using System.Numerics;

namespace JobBars {
    public unsafe partial class JobBars {
        public bool Visible = false;

        private void BuildSettingsUI() {
            if (!PlayerExists) return;
            if (!Visible) return;

            string _ID = "##JobBars_Settings";
            ImGui.SetNextWindowSize(new Vector2(600, 1000), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("JobBars Settings", ref Visible)) {
                if (ImGui.Checkbox("Use 4K Textures (Requires Restart)" + _ID, ref Config.Use4K)) {
                    Config.Save();
                }

                ImGui.BeginTabBar("Tabs" + _ID);
                if (ImGui.BeginTabItem("Gauges" + _ID)) {
                    GaugeManager?.Draw();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Icons" + _ID)) {
                    IconManager?.Draw();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Buffs" + _ID)) {
                    BuffManager?.Draw();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Cooldowns" + _ID)) {
                    CooldownManager?.Draw();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Cursor" + _ID)) {
                    CursorManager?.Draw();
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
            ImGui.End();

            GaugeManager?.DrawPositionBox();
            BuffManager?.DrawPositionBox();
        }

        public static void SetWindowPosition(string Id, Vector2 position) {
            var minPosition = ImGuiHelpers.MainViewport.Pos;
            ImGui.SetWindowPos(Id, position + minPosition);
        }

        public static bool DrawPositionView(string Id, Vector2 position, out Vector2 newPosition) {
            ImGuiHelpers.ForceNextWindowMainViewport();
            var minPosition = ImGuiHelpers.MainViewport.Pos;
            ImGui.SetNextWindowPos(position + minPosition, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSize(new Vector2(200, 200));
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.7f);
            ImGui.Begin(Id, ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoResize);
            newPosition = ImGui.GetWindowPos() - minPosition;
            ImGui.PopStyleVar(1);
            ImGui.End();
            return newPosition != position;
        }

        public static void Separator() {
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2);
            ImGui.Separator();
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2);
        }
    }
}
