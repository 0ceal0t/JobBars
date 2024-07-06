using ImGuiNET;
using JobBars.Data;
using System.Numerics;

namespace JobBars.Buffs.Manager {
    public partial class BuffManager {
        public bool LOCKED = true;

        private readonly InfoBox<BuffManager> PositionInfoBox = new() {
            Label = "Position",
            ContentsAction = ( BuffManager manager ) => {
                ImGui.Checkbox( "Position Locked" + manager.Id, ref manager.LOCKED );

                ImGui.SetNextItemWidth( 25f );
                if( ImGui.InputInt( "Buffs per line" + manager.Id, ref JobBars.Configuration.BuffHorizontal, 0 ) ) {
                    JobBars.Configuration.Save();
                    JobBars.Builder.BuffRoot.Update();
                }

                if( ImGui.Checkbox( "Right-to-left" + manager.Id, ref JobBars.Configuration.BuffRightToLeft ) ) {
                    JobBars.Configuration.Save();
                    JobBars.Builder.BuffRoot.Update();
                }

                if( ImGui.Checkbox( "Bottom-to-top" + manager.Id, ref JobBars.Configuration.BuffBottomToTop ) ) {
                    JobBars.Configuration.Save();
                    JobBars.Builder.BuffRoot.Update();
                }

                if( ImGui.Checkbox( "Square buffs" + manager.Id, ref JobBars.Configuration.BuffSquare ) ) {
                    JobBars.Configuration.Save();
                    JobBars.Builder.BuffRoot.Update();
                }

                if( ImGui.InputFloat( "Scale" + manager.Id, ref JobBars.Configuration.BuffScale ) ) {
                    UpdatePositionScale();
                    JobBars.Configuration.Save();
                }

                var pos = JobBars.Configuration.BuffPosition;
                if( ImGui.InputFloat2( "Position" + manager.Id, ref pos ) ) {
                    SetBuffPosition( pos );
                }
            }
        };

        private readonly InfoBox<BuffManager> HideWhenInfoBox = new() {
            Label = "Hide When",
            ContentsAction = ( BuffManager manager ) => {
                if( ImGui.Checkbox( "Out of combat", ref JobBars.Configuration.BuffHideOutOfCombat ) ) JobBars.Configuration.Save();
                if( ImGui.Checkbox( "Weapon is sheathed", ref JobBars.Configuration.BuffHideWeaponSheathed ) ) JobBars.Configuration.Save();
            }
        };

        protected override void DrawHeader() {
            if( ImGui.Checkbox( "Buff bar enabled" + Id, ref JobBars.Configuration.BuffBarEnabled ) ) {
                JobBars.Configuration.Save();
                ResetUI();
            }
        }

        protected override void DrawSettings() {
            PositionInfoBox.Draw( this );
            HideWhenInfoBox.Draw( this );

            ImGui.SetNextItemWidth( 50f );
            if( ImGui.InputFloat( "Hide buffs with cooldown above" + Id, ref JobBars.Configuration.BuffDisplayTimer ) ) JobBars.Configuration.Save();

            if( ImGui.Checkbox( "Show party members' buffs", ref JobBars.Configuration.BuffIncludeParty ) ) {
                JobBars.Configuration.Save();
                ResetUI();
            }

            if( ImGui.Checkbox( "Thin buff border", ref JobBars.Configuration.BuffThinBorder ) ) {
                JobBars.Configuration.Save();
                JobBars.Builder.BuffRoot.Update();
            }

            ImGui.SetNextItemWidth( 50f );
            if( ImGui.InputFloat( "Opacity when on cooldown" + Id, ref JobBars.Configuration.BuffOnCDOpacity ) ) JobBars.Configuration.Save();

            ImGui.SetNextItemWidth( 100f );
            if( ImGui.InputInt( "Buff text size", ref JobBars.Configuration.BuffTextSize_v2 ) ) {
                if( JobBars.Configuration.BuffTextSize_v2 <= 0 ) JobBars.Configuration.BuffTextSize_v2 = 1;
                if( JobBars.Configuration.BuffTextSize_v2 > 255 ) JobBars.Configuration.BuffTextSize_v2 = 255;
                JobBars.Configuration.Save();
                JobBars.Builder.BuffRoot.Update();
            }
        }

        protected override void DrawItem( BuffConfig[] item, JobIds _ ) {
            var reset = false;
            foreach( var buff in item ) buff.Draw( Id, ref reset );
            if( reset ) ResetUI();
        }

        public void DrawPositionBox() {
            if( LOCKED ) return;

            if( JobBars.DrawPositionView( "Buff Bar##BuffPosition", JobBars.Configuration.BuffPosition, out var pos ) ) {
                SetBuffPosition( pos );
            }
        }

        private static void SetBuffPosition( Vector2 pos ) {
            JobBars.SetWindowPosition( "Buff Bar##BuffPosition", pos );
            JobBars.Configuration.BuffPosition = pos;
            JobBars.Configuration.Save();
            JobBars.Builder.BuffRoot.SetPosition( pos );
        }
    }
}
