using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using System.Numerics;

namespace JobBars.Cooldowns.Manager {
    public unsafe partial class CooldownManager {
        private readonly InfoBox<CooldownManager> PositionInfoBox = new() {
            Label = "Position",
            ContentsAction = (CooldownManager manager) => {
                if (ImGui.Checkbox("Left-aligned" + manager.Id, ref JobBars.Config.CooldownsLeftAligned)) {
                    JobBars.Config.Save();
                    manager.ResetUI();
                }

                if (ImGui.InputFloat("Scale" + manager.Id, ref JobBars.Config.CooldownScale)) {
                    manager.UpdatePositionScale();
                    JobBars.Config.Save();
                }

                if (ImGui.InputFloat2("Position" + manager.Id, ref JobBars.Config.CooldownPosition)) {
                    manager.UpdatePositionScale();
                    JobBars.Config.Save();
                }

                if (ImGui.InputFloat("Line height" + manager.Id, ref JobBars.Config.CooldownsSpacing)) {
                    manager.UpdatePositionScale();
                    JobBars.Config.Save();
                }
            }
        };

        private readonly InfoBox<CooldownManager> ShowIconInfoBox = new() {
            Label = "Show Icons When",
            ContentsAction = (CooldownManager manager) => {
                if (ImGui.Checkbox("Default" + manager.Id, ref JobBars.Config.CooldownsStateShowDefault)) JobBars.Config.Save();
                if (ImGui.Checkbox("Active" + manager.Id, ref JobBars.Config.CooldownsStateShowRunning)) JobBars.Config.Save();
                if (ImGui.Checkbox("On cooldown" + manager.Id, ref JobBars.Config.CooldownsStateShowOnCD)) JobBars.Config.Save();
                if (ImGui.Checkbox("Off cooldown" + manager.Id, ref JobBars.Config.CooldownsStateShowOffCD)) JobBars.Config.Save();
            }
        };

        private readonly InfoBox<CooldownManager> HideWhenInfoBox = new() {
            Label = "Hide When",
            ContentsAction = (CooldownManager manager) => {
                if (ImGui.Checkbox("Out of combat", ref JobBars.Config.CooldownsHideOutOfCombat)) JobBars.Config.Save();
                if (ImGui.Checkbox("Weapon sheathed", ref JobBars.Config.CooldownsHideWeaponSheathed)) JobBars.Config.Save();
            }
        };

        private readonly CustomCooldownDialog CustomCooldownDialog = new();

        protected override void DrawHeader() {
            CustomCooldownDialog.Draw();

            if (ImGui.Checkbox("Cooldowns enabled" + Id, ref JobBars.Config.CooldownsEnabled)) {
                JobBars.Config.Save();
                ResetUI();
            }
        }

        protected override void DrawSettings() {
            PositionInfoBox.Draw(this);
            ShowIconInfoBox.Draw(this);
            HideWhenInfoBox.Draw(this);

            if (ImGui.Checkbox("Hide active buff text" + Id, ref JobBars.Config.CooldownsHideActiveBuffDuration)) JobBars.Config.Save();

            if (ImGui.Checkbox("Show party members' cooldowns" + Id, ref JobBars.Config.CooldownsShowPartyMembers)) {
                JobBars.Config.Save();
                ResetUI();
            }

            ImGui.SetNextItemWidth(50f);
            if (ImGui.InputFloat("Opacity when on cooldown" + Id, ref JobBars.Config.CooldownsOnCDOpacity)) JobBars.Config.Save();
        }

        protected override void DrawItem(CooldownConfig[] item, JobIds job) {
            var reset = false;
            foreach (var cooldown in item) cooldown.Draw(Id, false, ref reset);

            // Delete custom
            if (CustomCooldowns.TryGetValue(job, out var customCooldowns)) {
                foreach (var custom in customCooldowns) {
                    if (custom.Draw(Id, true, ref reset)) {
                        DeleteCustomCooldown(job, custom);
                        reset = true;
                        break;
                    }
                }
            }

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);
            if (ImGui.Button($"+ Add Custom Cooldown{Id}")) CustomCooldownDialog.Show(job);

            if (reset) ResetUI();
        }
    }
}