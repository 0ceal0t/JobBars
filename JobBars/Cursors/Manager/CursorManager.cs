using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using JobBars.Atk;
using JobBars.Data;
using JobBars.Helper;
using System;

namespace JobBars.Cursors.Manager {
    public unsafe partial class CursorManager : PerJobManager<Cursor> {
        private Cursor CurrentCursor = null;
        private ElementColor InnerColor;
        private ElementColor OuterColor;

        public CursorManager() : base( "##JobBars_Cursor" ) {
            InnerColor = AtkColor.GetColor( JobBars.Configuration.CursorInnerColor, AtkColor.MpPink );
            OuterColor = AtkColor.GetColor( JobBars.Configuration.CursorOuterColor, AtkColor.HealthGreen );

            JobBars.Builder.CursorRoot.SetInnerColor( InnerColor );
            JobBars.Builder.CursorRoot.SetOuterColor( OuterColor );
        }

        public void SetJob( JobIds job ) {
            CurrentCursor = JobToValue.TryGetValue( job, out var cursor ) ? cursor : null;
        }

        public void Tick() {
            if( AtkHelper.CalcDoHide( JobBars.Configuration.CursorsEnabled, JobBars.Configuration.CursorHideOutOfCombat, JobBars.Configuration.CursorHideWeaponSheathed ) ) {
                JobBars.Builder.CursorRoot.IsVisible = false;
                return;
            }
            else {
                JobBars.Builder.CursorRoot.IsVisible = true;
            }

            // ============================

            if( CurrentCursor == null ) {
                JobBars.Builder.CursorRoot.SetInner( 0, 1f );
                JobBars.Builder.CursorRoot.SetOuter( 0, 1f );
                return;
            }

            var viewport = ImGuiHelpers.MainViewport;

            if( JobBars.Configuration.CursorPosition == CursorPositionType.MouseCursor ) {
                var pos = ImGui.GetMousePos();
                var atkStage = AtkStage.Instance();

                var dragging = *( ( byte* )new IntPtr( atkStage ) + 0x137 );
                if( JobBars.Configuration.CursorHideWhenHeld && dragging != 1 ) {
                    JobBars.Builder.CursorRoot.IsVisible = false;
                    return;
                }
                JobBars.Builder.CursorRoot.IsVisible = true;

                if( pos.X > 0 && pos.Y > 0 && pos.X < viewport.Size.X && pos.Y < viewport.Size.Y && dragging == 1 ) {
                    AtkBuilder.SetPosition( JobBars.Builder.CursorRoot, pos );
                }
            }
            else {
                JobBars.Builder.CursorRoot.IsVisible = true;
                AtkBuilder.SetPosition(
                    JobBars.Builder.CursorRoot,
                    JobBars.Configuration.CursorPosition == CursorPositionType.Middle ? viewport.Size / 2 : JobBars.Configuration.CursorCustomPosition );
            }

            JobBars.Builder.CursorRoot.SetInner( CurrentCursor.GetInner(), JobBars.Configuration.CursorInnerScale );
            JobBars.Builder.CursorRoot.SetOuter( CurrentCursor.GetOuter(), JobBars.Configuration.CursorOuterScale );
        }
    }
}