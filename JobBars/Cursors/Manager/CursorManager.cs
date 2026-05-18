using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
using JobBars.Data;
using JobBars.Helper;
using JobBars.Nodes.Cursor;
using KamiToolKit.Overlay.UiOverlay;
using System;

namespace JobBars.Cursors.Manager {
    public unsafe partial class CursorManager : PerJobManager<Cursor> {
        private Cursor CurrentCursor = null;
        private ElementColor InnerColor;
        private ElementColor OuterColor;

        private OverlayController? Controller;
        private CursorRoot? Root;

        public CursorManager() : base( "##JobBars_Cursor" ) {
            InnerColor = ColorConstants.GetColor( JobBars.Configuration.CursorInnerColor, ColorConstants.MpPink );
            OuterColor = ColorConstants.GetColor( JobBars.Configuration.CursorOuterColor, ColorConstants.HealthGreen );

            Controller = new();
            Controller.CreateNode( () => {
                Root = new( this );
                return Root;
            } );
        }

        public void Hide() {
            Root?.IsVisible = false;
        }

        public void Dispose() {
            Controller?.Dispose();
            Controller = null;
            Root = null;
        }

        public void SetJob( JobIds job ) {
            CurrentCursor = JobToValue.TryGetValue( job, out var cursor ) ? cursor : null;
        }

        public void Tick() {
            if( Root == null ) return;

            if( UiHelper.CalcDoHide( JobBars.Configuration.CursorsEnabled, JobBars.Configuration.CursorHideOutOfCombat, JobBars.Configuration.CursorHideWeaponSheathed ) ) {
                Root!.IsVisible = false;
                return;
            }
            else {
                Root!.IsVisible = true;
            }

            // ============================

            if( CurrentCursor == null ) {
                Root!.SetInner( 0, 1f );
                Root!.SetOuter( 0, 1f );
                return;
            }

            var viewport = ImGuiHelpers.MainViewport;

            if( JobBars.Configuration.CursorPosition == CursorPositionType.MouseCursor ) {
                var pos = ImGui.GetMousePos();
                var atkStage = AtkStage.Instance();

                var dragging = *( ( byte* )new IntPtr( atkStage ) + 0x137 );
                if( JobBars.Configuration.CursorHideWhenHeld && dragging != 1 ) {
                    Root!.IsVisible = false;
                    return;
                }
                Root!.IsVisible = true;

                if( pos.X > 0 && pos.Y > 0 && pos.X < viewport.Size.X && pos.Y < viewport.Size.Y && dragging == 1 ) {
                    Root.Position = pos;
                }
            }
            else {
                Root!.IsVisible = true;
                Root.Position = JobBars.Configuration.CursorPosition == CursorPositionType.Middle ? viewport.Size / 2 : JobBars.Configuration.CursorCustomPosition;
            }

            Root!.SetInnerColor( InnerColor );
            Root!.SetOuterColor( OuterColor );

            Root!.SetInner( CurrentCursor.GetInner(), JobBars.Configuration.CursorInnerScale );
            Root!.SetOuter( CurrentCursor.GetOuter(), JobBars.Configuration.CursorOuterScale );
        }
    }
}