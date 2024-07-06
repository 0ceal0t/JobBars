using ImGuiNET;
using JobBars.Data;
using System;

namespace JobBars.Cursors.Manager {
    public partial class CursorManager {
        private static readonly CursorPositionType[] ValidCursorPositionType = ( CursorPositionType[] )Enum.GetValues( typeof( CursorPositionType ) );

        private readonly InfoBox<CursorManager> HideWhenInfoBox = new() {
            Label = "Hide When",
            ContentsAction = ( CursorManager manager ) => {
                if( ImGui.Checkbox( "Mouse held", ref JobBars.Configuration.CursorHideWhenHeld ) ) JobBars.Configuration.Save();
                if( ImGui.Checkbox( "Out of combat", ref JobBars.Configuration.CursorHideOutOfCombat ) ) JobBars.Configuration.Save();
                if( ImGui.Checkbox( "Weapon sheathed", ref JobBars.Configuration.CursorHideWeaponSheathed ) ) JobBars.Configuration.Save();
            }
        };

        protected override void DrawHeader() {
            if( ImGui.Checkbox( "Cursor enabled" + Id, ref JobBars.Configuration.CursorsEnabled ) ) JobBars.Configuration.Save();
        }

        protected override void DrawSettings() {
            HideWhenInfoBox.Draw( this );

            if( JobBars.DrawCombo( ValidCursorPositionType, JobBars.Configuration.CursorPosition, "Cursor positioning", Id, out var newPosition ) ) {
                JobBars.Configuration.CursorPosition = newPosition;
                JobBars.Configuration.Save();
            }

            if( JobBars.Configuration.CursorPosition == CursorPositionType.CustomPosition ) {
                if( ImGui.InputFloat2( "Custom cursor position", ref JobBars.Configuration.CursorCustomPosition ) ) {
                    JobBars.Configuration.Save();
                }
            }

            if( ImGui.InputFloat( "Inner scale" + Id, ref JobBars.Configuration.CursorInnerScale ) ) JobBars.Configuration.Save();
            if( ImGui.InputFloat( "Outer scale" + Id, ref JobBars.Configuration.CursorOuterScale ) ) JobBars.Configuration.Save();

            if( Configuration.DrawColor( "Inner color", InnerColor, out var newColorInner ) ) {
                InnerColor = newColorInner;
                JobBars.Configuration.CursorInnerColor = newColorInner.Name;
                JobBars.Configuration.Save();
                JobBars.Builder.CursorRoot.SetInnerColor( InnerColor );
            }

            if( Configuration.DrawColor( "Outer color", OuterColor, out var newColorOuter ) ) {
                OuterColor = newColorOuter;
                JobBars.Configuration.CursorOuterColor = newColorOuter.Name;
                JobBars.Configuration.Save();
                JobBars.Builder.CursorRoot.SetOuterColor( OuterColor );
            }
        }

        protected override void DrawItem( Cursor item, JobIds _ ) {
            item.Draw( Id );
        }
    }
}
