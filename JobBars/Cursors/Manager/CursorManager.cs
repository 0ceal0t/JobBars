using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using JobBars.Atk;
using System;

namespace JobBars.Cursors.Manager {
    public unsafe partial class CursorManager : PerJobManager<Cursor> {
        private Cursor CurrentCursor = null;
        private ElementColor InnerColor;
        private ElementColor OuterColor;

        public CursorManager() : base("##JobBars_Cursor") {
            InnerColor = AtkColor.GetColor(JobBars.Configuration.CursorInnerColor, AtkColor.MpPink);
            OuterColor = AtkColor.GetColor(JobBars.Configuration.CursorOuterColor, AtkColor.HealthGreen);

            JobBars.Builder.SetCursorInnerColor(InnerColor);
            JobBars.Builder.SetCursorOuterColor(OuterColor);
        }

        public void SetJob(JobIds job) {
            CurrentCursor = JobToValue.TryGetValue(job, out var cursor) ? cursor : null;
        }

        public void Tick() {
            if (AtkHelper.CalcDoHide(JobBars.Configuration.CursorsEnabled, JobBars.Configuration.CursorHideOutOfCombat, JobBars.Configuration.CursorHideWeaponSheathed)) {
                JobBars.Builder.HideCursor();
                return;
            }
            else {
                JobBars.Builder.ShowCursor();
            }

            // ============================

            if (CurrentCursor == null) {
                JobBars.Builder.SetCursorInnerPercent(0, 1f);
                JobBars.Builder.SetCursorOuterPercent(0, 1f);
                return;
            }

            var viewport = ImGuiHelpers.MainViewport;

            if (JobBars.Configuration.CursorPosition == CursorPositionType.MouseCursor) {
                var pos = ImGui.GetMousePos() - viewport.Pos;
                var atkStage = AtkStage.GetSingleton();

                var dragging = *((byte*)new IntPtr(atkStage) + 0x137);
                if (JobBars.Configuration.CursorHideWhenHeld && dragging != 1) {
                    JobBars.Builder.HideCursor();
                    return;
                }
                JobBars.Builder.ShowCursor();

                if (pos.X > 0 && pos.Y > 0 && pos.X < viewport.Size.X && pos.Y < viewport.Size.Y && dragging == 1) {
                    JobBars.Builder.SetCursorPosition(pos);
                }
            }
            else {
                JobBars.Builder.ShowCursor();
                JobBars.Builder.SetCursorPosition(JobBars.Configuration.CursorPosition == CursorPositionType.Middle ? viewport.Size / 2 : JobBars.Configuration.CursorCustomPosition);
            }

            var inner = CurrentCursor.GetInner();
            var outer = CurrentCursor.GetOuter();
            JobBars.Builder.SetCursorInnerPercent(inner, JobBars.Configuration.CursorInnerScale);
            JobBars.Builder.SetCursorOuterPercent(outer, JobBars.Configuration.CursorOuterScale);
        }
    }
}