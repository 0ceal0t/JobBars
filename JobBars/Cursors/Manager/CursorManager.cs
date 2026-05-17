using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
using JobBars.Data;
using JobBars.Helper;
using JobBars.Nodes.Builder;
using JobBars.Nodes.Cursor;
using KamiToolKit.Controllers;
using System;

namespace JobBars.Cursors.Manager {
    public unsafe partial class CursorManager : PerJobManager<Cursor> {
        private Cursor CurrentCursor = null;
        private ElementColor InnerColor;
        private ElementColor OuterColor;

        private AddonController? Controller;
        private CursorRoot? Root;

        public CursorManager() : base( "##JobBars_Cursor" ) {
            InnerColor = ColorConstants.GetColor( JobBars.Configuration.CursorInnerColor, ColorConstants.MpPink );
            OuterColor = ColorConstants.GetColor( JobBars.Configuration.CursorOuterColor, ColorConstants.HealthGreen );

            Controller = new AddonController {
                AddonName = UiHelper.BuffGaugeAttachAddonName,
                OnSetup = SetupAddon,
                OnFinalize = ResetAddon,
                OnUpdate = UpdateAddon
            };
            Controller.Enable();
        }

        public void Hide() {
            Root?.IsVisible = false;
        }

        private void SetupAddon( AtkUnitBase* addon ) {
            Root = new();
            Root.AttachNode( addon );
        }

        private void ResetAddon( AtkUnitBase* addon ) {
            Root?.Dispose();
            Root = null;
        }

        private void UpdateAddon( AtkUnitBase* addon ) {
            Tick();
        }

        public void Dispose() {
            Controller?.Dispose();
            Controller = null;
        }

        public void SetJob( JobIds job ) {
            CurrentCursor = JobToValue.TryGetValue( job, out var cursor ) ? cursor : null;
        }

        private void Tick() {
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
                    NodeBuilder.SetPositionGlobal( Root!, pos );
                }
            }
            else {
                Root!.IsVisible = true;
                NodeBuilder.SetPositionGlobal(
                    Root!,
                    JobBars.Configuration.CursorPosition == CursorPositionType.Middle ? viewport.Size / 2 : JobBars.Configuration.CursorCustomPosition );
            }

            Root!.SetInnerColor( InnerColor );
            Root!.SetOuterColor( OuterColor );

            Root!.SetInner( CurrentCursor.GetInner(), JobBars.Configuration.CursorInnerScale );
            Root!.SetOuter( CurrentCursor.GetOuter(), JobBars.Configuration.CursorOuterScale );
        }
    }
}