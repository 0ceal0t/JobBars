using ImGuiNET;
using JobBars.Cooldowns.Manager;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Numerics;

namespace JobBars.Cooldowns {
    public class CustomCooldownDialog : GenericDialog {
        private enum CustomCooldownType {
            Buff,
            Action
        }

        private static readonly JobIds[] JobOptions = ( JobIds[] )Enum.GetValues( typeof( JobIds ) );
        private JobIds SelectedJob = JobIds.OTHER;

        private CustomCooldownType CustomTriggerType = CustomCooldownType.Action;
        private float CustomCD = 30;
        private float CustomDuration = 0;

        private readonly ItemSelector CustomTriggerAction = new( "Trigger", "##CustomCD_Action", AtkHelper.ActionList );
        private readonly ItemSelector CustomTriggerBuff = new( "Trigger", "##CustomCD_Buff", AtkHelper.StatusList );
        private readonly ItemSelector CustomIcon = new( "Icon", "##CustomCD_Icon", AtkHelper.ActionList );

        private static CooldownManager Manager => JobBars.CooldownManager;


        public CustomCooldownDialog() : base( "Custom Cooldown" ) { }

        public override void DrawBody() {
            var id = "##CustomCooldown";
            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );

            if( JobBars.DrawCombo( JobOptions, SelectedJob, "Job", id, out var newSelectedJob ) ) {
                SelectedJob = newSelectedJob;
            }

            if( ImGui.BeginCombo( "##CustomCD_Type", $"{CustomTriggerType}", ImGuiComboFlags.HeightLargest ) ) {
                if( ImGui.Selectable( "Action", CustomTriggerType == CustomCooldownType.Action ) ) CustomTriggerType = CustomCooldownType.Action;
                if( ImGui.Selectable( "Buff", CustomTriggerType == CustomCooldownType.Buff ) ) CustomTriggerType = CustomCooldownType.Buff;
                ImGui.EndCombo();
            }
            ImGui.SameLine();
            ImGui.Text( "Trigger Type" );

            if( CustomTriggerType == CustomCooldownType.Action ) CustomTriggerAction.Draw();
            else CustomTriggerBuff.Draw();

            CustomIcon.Draw();

            ImGui.InputFloat( $"Cooldown", ref CustomCD );
            ImGui.InputFloat( $"Duration (0 = instant)", ref CustomDuration );

            var selected = CustomTriggerType == CustomCooldownType.Action ? CustomTriggerAction.GetSelected() : CustomTriggerBuff.GetSelected();
            var icon = CustomIcon.GetSelected();

            ImGui.EndChild();

            if( icon.Data.Id != 0 && selected.Data.Id != 0 ) {
                if( ImGui.Button( "+ Add" ) ) {
                    var newName = $"{selected.Name} - Custom ({AtkHelper.Localize( SelectedJob )})";
                    var newProps = new CooldownProps {
                        CD = CustomCD,
                        Duration = CustomDuration,
                        Icon = ( ActionIds )icon.Data.Id,
                        Triggers = [selected.Data]
                    };
                    Manager.AddCustomCooldown( SelectedJob, newName, newProps );
                    Manager.ResetUi();
                }
            }
            else {
                ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.5f );
                ImGui.Button( "+ Add" );
                ImGui.PopStyleVar();
            }
        }

        public void Show( JobIds job ) {
            SelectedJob = job;
            Show();
        }
    }
}
