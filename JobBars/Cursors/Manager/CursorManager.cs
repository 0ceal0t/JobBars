using System;
using ImGuiNET;
using Dalamud.Interface;
using JobBars.Data;
using JobBars.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace JobBars.Cursors.Manager {
    public unsafe partial class CursorManager : PerJobManager<Cursor> {
        private Cursor CurrentCursor = null;
        private ElementColor InnerColor;
        private ElementColor OuterColor;

        public CursorManager() : base("##JobBars_Cursor") {
            InnerColor = UIColor.GetColor(JobBars.Config.CursorInnerColor, UIColor.MpPink);
            OuterColor = UIColor.GetColor(JobBars.Config.CursorOuterColor, UIColor.HealthGreen);

            JobBars.Builder.SetCursorInnerColor(InnerColor);
            JobBars.Builder.SetCursorOuterColor(OuterColor);
            if (JobBars.Config.CursorsEnabled) JobBars.Builder.ShowCursor();
            else JobBars.Builder.HideCursor();
        }

        public void SetJob(JobIds job) {
            CurrentCursor = JobToValue.TryGetValue(job, out var cursor) ? cursor : null;
        }

        public void Tick(bool inCombat) {
            if (!JobBars.Config.CursorsEnabled) return;
            if (JobBars.Config.CursorHideOutOfCombat) {
                if (inCombat) JobBars.Builder.ShowCursor();
                else {
                    JobBars.Builder.HideCursor();
                    return;
                }
            }

            if (CurrentCursor == null) {
                JobBars.Builder.SetCursorInnerPercent(0, 1f);
                JobBars.Builder.SetCursorOuterPercent(0, 1f);
                return;
            }

            var viewport = ImGuiHelpers.MainViewport;

            if (JobBars.Config.CursorPosition == CursorPositionType.MouseCursor) {
                var pos = ImGui.GetMousePos() - viewport.Pos;
                var atkStage = AtkStage.GetSingleton();

                var dragging = *((byte*)new IntPtr(atkStage) + 0x137);
                if (JobBars.Config.CursorHideWhenHeld && dragging != 1) {
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
                JobBars.Builder.SetCursorPosition(JobBars.Config.CursorPosition == CursorPositionType.Middle ? viewport.Size / 2 : JobBars.Config.CursorCustomPosition);
            }

            var inner = CurrentCursor.GetInner();
            var outer = CurrentCursor.GetOuter();
            JobBars.Builder.SetCursorInnerPercent(inner, JobBars.Config.CursorInnerScale);
            JobBars.Builder.SetCursorOuterPercent(outer, JobBars.Config.CursorOuterScale);
        }
    }
}