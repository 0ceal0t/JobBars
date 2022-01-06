using Dalamud.Interface;
using ImGuiNET;
using JobBars.Data;
using System;
using System.Numerics;

namespace JobBars {
    public unsafe partial class JobBars {
        public bool Visible = false;

        private readonly AttachAddon[] ValidAttachTypes = (AttachAddon[])Enum.GetValues(typeof(AttachAddon));

        public static readonly Vector4 RED_COLOR = new(0.85098039216f, 0.32549019608f, 0.30980392157f, 1.0f);
        public static readonly Vector4 GREEN_COLOR = new(0.36078431373f, 0.72156862745f, 0.36078431373f, 1.0f);

        private void BuildSettingsUI() {
            if (!PlayerExists) return;
            if (!Visible) return;

            string _ID = "##JobBars_Settings";
            ImGui.SetNextWindowSize(new Vector2(600, 1000), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("JobBars Settings", ref Visible)) {
                if (ImGui.Checkbox("Use 4K Textures (Requires Restart)" + _ID, ref Config.Use4K)) {
                    Config.Save();
                }

                ImGui.SetNextItemWidth(200f);
                if (ImGui.BeginCombo("Attached UI element (Requires Restart)" + _ID, $"{Config.AttachAddon}")) {
                    foreach (var attachType in ValidAttachTypes) {
                        if (ImGui.Selectable($"{attachType}" + _ID, attachType == Config.AttachAddon)) {
                            Config.AttachAddon = attachType;
                            Config.Save();
                        }
                    }
                    ImGui.EndCombo();
                }

                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);

                // ==========================

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

        public static bool RemoveButton(string label, bool small = false) => ColorButton(label, RED_COLOR, small);

        public static bool OkButton(string label, bool small = false) => ColorButton(label, GREEN_COLOR, small);

        public static bool ColorButton(string label, Vector4 color, bool small) {
            var ret = false;
            ImGui.PushStyleColor(ImGuiCol.Button, color);
            if (small) {
                if (ImGui.SmallButton(label)) {
                    ret = true;
                }
            }
            else {
                if (ImGui.Button(label)) {
                    ret = true;
                }
            }
            ImGui.PopStyleColor();
            return ret;
        }
    }
}
