using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Dalamud.Bindings.ImGui;
using JobBars.Atk;
using JobBars.Data;
using JobBars.Helper;
using JobBars.Nodes.Builder;
using System;

namespace JobBars.Cursors.Manager {
    public unsafe partial class CursorManager : PerJobManager<Cursor> {
        private Cursor CurrentCursor = null;
        private ElementColor InnerColor;
        private ElementColor OuterColor;

        public CursorManager() : base( "##JobBars_Cursor" ) {
            InnerColor = ColorConstants.GetColor( JobBars.Configuration.CursorInnerColor, ColorConstants.MpPink );
            OuterColor = ColorConstants.GetColor( JobBars.Configuration.CursorOuterColor, ColorConstants.HealthGreen );
        }

        public void SetJob( JobIds job ) {
            CurrentCursor = JobToValue.TryGetValue( job, out var cursor ) ? cursor : null;
        }

        public void Tick() {
            if( UiHelper.CalcDoHide( JobBars.Configuration.CursorsEnabled, JobBars.Configuration.CursorHideOutOfCombat, JobBars.Configuration.CursorHideWeaponSheathed ) ) {
                JobBars.NodeBuilder.CursorRoot.IsVisible = false;
                return;
            }
            else {
                JobBars.NodeBuilder.CursorRoot.IsVisible = true;
            }

            // ============================

            if( CurrentCursor == null ) {
                JobBars.NodeBuilder.CursorRoot.SetInner( 0, 1f );
                JobBars.NodeBuilder.CursorRoot.SetOuter( 0, 1f );
                return;
            }

            var viewport = ImGuiHelpers.MainViewport;

            if( JobBars.Configuration.CursorPosition == CursorPositionType.MouseCursor ) {
                var pos = ImGui.GetMousePos();
                var atkStage = AtkStage.Instance();

                var dragging = *( ( byte* )new IntPtr( atkStage ) + 0x137 );
                if( JobBars.Configuration.CursorHideWhenHeld && dragging != 1 ) {
                    JobBars.NodeBuilder.CursorRoot.IsVisible = false;
                    return;
                }
                JobBars.NodeBuilder.CursorRoot.IsVisible = true;

                if( pos.X > 0 && pos.Y > 0 && pos.X < viewport.Size.X && pos.Y < viewport.Size.Y && dragging == 1 ) {
                    NodeBuilder.SetPositionGlobal( JobBars.NodeBuilder.CursorRoot, pos );
                }
            }
            else {
                JobBars.NodeBuilder.CursorRoot.IsVisible = true;
                NodeBuilder.SetPositionGlobal(
                    JobBars.NodeBuilder.CursorRoot,
                    JobBars.Configuration.CursorPosition == CursorPositionType.Middle ? viewport.Size / 2 : JobBars.Configuration.CursorCustomPosition );
            }

            JobBars.NodeBuilder.CursorRoot.SetInnerColor( InnerColor );
            JobBars.NodeBuilder.CursorRoot.SetOuterColor( OuterColor );

            JobBars.NodeBuilder.CursorRoot.SetInner( CurrentCursor.GetInner(), JobBars.Configuration.CursorInnerScale );
            JobBars.NodeBuilder.CursorRoot.SetOuter( CurrentCursor.GetOuter(), JobBars.Configuration.CursorOuterScale );
        }
    }
}