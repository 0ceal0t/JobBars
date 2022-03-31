using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using System.Numerics;

namespace JobBars.Cooldowns.Manager {
    public unsafe partial class CooldownManager {
        private enum CustomCooldownType {
            Buff,
            Action
        }

        private bool ShowNewCustom = false;
        private CustomCooldownType CustomTriggerType = CustomCooldownType.Action;
        private float CustomCD = 30;
        private float CustomDuration = 0;

        private readonly ItemSelector CustomTriggerAction = new("Trigger", "##CustomCD_1", UIHelper.ActionList);
        private readonly ItemSelector CustomTriggerBuff = new("Trigger", "##CustomCD_2", UIHelper.StatusList);
        private readonly ItemSelector CustomIcon = new("Icon", "##CustomCD_3", UIHelper.ActionList);

        protected override void DrawHeader() {
            if (ImGui.Checkbox("Cooldowns enabled" + _ID, ref JobBars.Config.CooldownsEnabled)) {
                JobBars.Config.Save();
                ResetUI();
            }

            if (ImGui.CollapsingHeader("Position" + _ID + "/Row")) DrawPositionRow();
            if (ImGui.CollapsingHeader("Settings" + _ID + "/Row")) DrawSettingsRow();
        }

        private void DrawPositionRow() {
            ImGui.Indent();

            if (ImGui.Checkbox("Left-aligned" + _ID, ref JobBars.Config.CooldownsLeftAligned)) {
                JobBars.Config.Save();
                ResetUI();
            }

            if (ImGui.InputFloat("Scale" + _ID, ref JobBars.Config.CooldownScale)) {
                UpdatePositionScale();
                JobBars.Config.Save();
            }

            if (ImGui.InputFloat2("Position" + _ID, ref JobBars.Config.CooldownPosition)) {
                UpdatePositionScale();
                JobBars.Config.Save();
            }

            if (ImGui.InputFloat("Line height" + _ID, ref JobBars.Config.CooldownsSpacing)) {
                UpdatePositionScale();
                JobBars.Config.Save();
            }

            ImGui.Unindent();
        }

        private void DrawSettingsRow() {
            ImGui.Indent();

            if (ImGui.Checkbox("Hide cooldowns when out of combat" + _ID, ref JobBars.Config.CooldownsHideOutOfCombat)) JobBars.Config.Save();
            if (ImGui.Checkbox("Hide cooldowns when weapon sheathed", ref JobBars.Config.CooldownsHideWeaponSheathed)) JobBars.Config.Save();
            if (ImGui.Checkbox("Hide active buff duration (Show only cooldowns)" + _ID, ref JobBars.Config.CooldownsHideActiveBuffDuration)) JobBars.Config.Save();

            if (ImGui.Checkbox("Show party members' cooldowns" + _ID, ref JobBars.Config.CooldownsShowPartyMembers)) {
                JobBars.Config.Save();
                ResetUI();
            }

            if (ImGui.InputFloat("Opacity when on cooldown" + _ID, ref JobBars.Config.CooldownsOnCDOpacity)) JobBars.Config.Save();

            ImGui.Unindent();
        }

        // ==========================================

        protected override void DrawItem(CooldownConfig[] item, JobIds job) {
            var reset = false;
            foreach (var cooldown in item) cooldown.Draw(_ID, ref reset);

            // Delete custom
            if (CustomCooldowns.TryGetValue(job, out var customCooldowns)) {
                foreach (var custom in customCooldowns) {
                    if (JobBars.RemoveButton($"Delete {custom.Name}", true)) {
                        CustomCooldowns[job].Remove(custom);
                        JobBars.Config.RemoveCustomCooldown(custom.Name);
                        reset = true;
                        break;
                    }
                    else {
                        custom.Draw(_ID, ref reset); // Draw custom
                    }
                }
            }

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);

            if (!ShowNewCustom) { // New custom
                if (ImGui.Button("New custom cooldown")) ShowNewCustom = true;
            }
            else {
                var style = ImGui.GetStyle();
                var itemHeight = ImGui.GetTextLineHeight() + style.ItemSpacing.Y;
                ImGui.BeginChild("##CustomCooldownChild", new Vector2(ImGui.GetContentRegionAvail().X, itemHeight * 9), true);

                if (ImGui.BeginCombo("##CustomCD_4", $"{CustomTriggerType}", ImGuiComboFlags.HeightLargest)) {
                    if (ImGui.Selectable("Action", CustomTriggerType == CustomCooldownType.Action)) CustomTriggerType = CustomCooldownType.Action;
                    if (ImGui.Selectable("Buff", CustomTriggerType == CustomCooldownType.Buff)) CustomTriggerType = CustomCooldownType.Buff;
                    ImGui.EndCombo();
                }
                ImGui.SameLine();
                ImGui.Text("Trigger Type");

                if (CustomTriggerType == CustomCooldownType.Action) CustomTriggerAction.Draw();
                else CustomTriggerBuff.Draw();

                CustomIcon.Draw();

                ImGui.InputFloat($"Cooldown", ref CustomCD);
                ImGui.InputFloat($"Duration (0 = instant)", ref CustomDuration);

                var selected = CustomTriggerType == CustomCooldownType.Action ? CustomTriggerAction.GetSelected() : CustomTriggerBuff.GetSelected();
                var icon = CustomIcon.GetSelected();

                if (icon.Data.Id != 0 && selected.Data.Id != 0) {
                    if (ImGui.Button("+ Add custom cooldown")) {
                        var newName = $"{selected.Name} - Custom ({UIHelper.Localize(job)})";
                        var newProps = new CooldownProps {
                            CD = CustomCD,
                            Duration = CustomDuration,
                            Icon = (ActionIds)icon.Data.Id,
                            Triggers = new[] { selected.Data }
                        };

                        if (!CustomCooldowns.ContainsKey(job)) CustomCooldowns[job] = new();
                        CustomCooldowns[job].Add(new CooldownConfig(newName, newProps));
                        JobBars.Config.AddCustomCooldown(newName, job, newProps);

                        ShowNewCustom = false;
                        reset = true;
                    }
                }
                else {
                    ImGui.TextColored(new Vector4(1, 0, 0, 1), "Select trigger and icon to continue");
                }


                ImGui.EndChild();
            }

            if (reset) ResetUI();
        }
    }
}