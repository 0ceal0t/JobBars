using Dalamud.Interface;
using Dalamud.Interface.Utility;
using ImGuiNET;
using System;
using System.Numerics;

namespace JobBars {
    // Adapted from DailyDuty: https://github.com/MidoriKami/DailyDuty/blob/master/DailyDuty/Graphical/InfoBox.cs
    public unsafe class InfoBox<T> {
        public Vector4 TextColor { get; set; } = new Vector4(1f);
        public Vector4 BorderColor { get; set; } = new Vector4(0.25686274510f, 0.26078431373f, 0.28039215686f, 1f);

        public Action<T> ContentsAction { get; set; }
        public float CurveRadius { get; set; } = 5.0f;
        public Vector2 Size { get; set; } = Vector2.Zero;
        public float BorderThickness { get; set; } = 1.0f;
        public int SegmentResolution { get; set; } = 10;
        public Vector2 Offset { get; set; } = new Vector2(0, 10);
        public string Label { get; set; } = "Label Not Set";

        public bool AutoResizeX { get; set; } = true;
        public bool AutoResizeY { get; set; } = true;
        public Vector2 PaddingBefore { get; set; } = new(10f, 10f);
        public Vector2 PaddingAfter { get; set; } = new(10f, 5f);

        private ImDrawListPtr DrawList => ImGui.GetWindowDrawList();

        private uint TextColorU32 => ImGui.GetColorU32(TextColor);
        private uint BorderColorU32 => ImGui.GetColorU32(BorderColor);

        private Vector2 StartPosition { get; set; }

        public void Draw(T item) {
            StartPosition = ImGui.GetCursorScreenPos();
            StartPosition += Offset;

            DrawCorners();
            DrawBorders();

            DrawContents(item);

            if (Size == Vector2.Zero) {
                Size = new Vector2() {
                    X = ImGui.GetContentRegionAvail().X - 5f,
                    Y = ImGui.GetItemRectMax().Y - ImGui.GetItemRectMin().Y + CurveRadius * 2.0f + PaddingBefore.Y + PaddingAfter.Y
                };

            }

            if (AutoResizeY) {
                Size = Size with { Y = ImGui.GetItemRectMax().Y - ImGui.GetItemRectMin().Y + CurveRadius * 2.0f + PaddingBefore.Y + PaddingAfter.Y };
            }
            if (AutoResizeX) {
                Size = Size with { X = ImGui.GetItemRectMax().X - ImGui.GetItemRectMin().X + CurveRadius * 2.0f + PaddingBefore.X + PaddingAfter.X };
            }

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 15f);
        }

        public void DrawCentered(T item, float percentSize = 0.80f) {
            var region = ImGui.GetContentRegionAvail();
            var currentPosition = ImGui.GetCursorPos();
            var width = new Vector2(region.X * percentSize);
            ImGui.SetCursorPos(currentPosition with { X = region.X / 2.0f - width.X / 2.0f });

            Size = width;
            Draw(item);
        }

        private void DrawContents(T item) {
            var topLeftCurveCenter = new Vector2(StartPosition.X + CurveRadius, StartPosition.Y + CurveRadius);

            ImGui.SetCursorScreenPos(topLeftCurveCenter + PaddingBefore);
            ImGui.PushTextWrapPos(Size.X);

            ImGui.BeginGroup();
            ImGui.PushID(Label);
            ContentsAction(item);
            ImGui.PopID();
            ImGui.EndGroup();

            ImGui.PopTextWrapPos();
        }

        private void DrawCorners() {
            var topLeftCurveCenter = new Vector2(StartPosition.X + CurveRadius, StartPosition.Y + CurveRadius);
            var topRightCurveCenter = new Vector2(StartPosition.X + Size.X - CurveRadius, StartPosition.Y + CurveRadius);
            var bottomLeftCurveCenter = new Vector2(StartPosition.X + CurveRadius, StartPosition.Y + Size.Y - CurveRadius);
            var bottomRightCurveCenter = new Vector2(StartPosition.X + Size.X - CurveRadius, StartPosition.Y + Size.Y - CurveRadius);

            DrawList.PathArcTo(topLeftCurveCenter, CurveRadius, DegreesToRadians(180), DegreesToRadians(270), SegmentResolution);
            DrawList.PathStroke(BorderColorU32, ImDrawFlags.None, BorderThickness);

            DrawList.PathArcTo(topRightCurveCenter, CurveRadius, DegreesToRadians(360), DegreesToRadians(270), SegmentResolution);
            DrawList.PathStroke(BorderColorU32, ImDrawFlags.None, BorderThickness);

            DrawList.PathArcTo(bottomLeftCurveCenter, CurveRadius, DegreesToRadians(90), DegreesToRadians(180), SegmentResolution);
            DrawList.PathStroke(BorderColorU32, ImDrawFlags.None, BorderThickness);

            DrawList.PathArcTo(bottomRightCurveCenter, CurveRadius, DegreesToRadians(0), DegreesToRadians(90), SegmentResolution);
            DrawList.PathStroke(BorderColorU32, ImDrawFlags.None, BorderThickness);
        }

        private void DrawBorders() {
            var color = BorderColorU32;

            DrawList.AddLine(new Vector2(StartPosition.X - 0.5f, StartPosition.Y + CurveRadius - 0.5f), new Vector2(StartPosition.X - 0.5f, StartPosition.Y + Size.Y - CurveRadius + 0.5f), color, BorderThickness);
            DrawList.AddLine(new Vector2(StartPosition.X + Size.X - 0.5f, StartPosition.Y + CurveRadius - 0.5f), new Vector2(StartPosition.X + Size.X - 0.5f, StartPosition.Y + Size.Y - CurveRadius + 0.5f), color, BorderThickness);
            DrawList.AddLine(new Vector2(StartPosition.X + CurveRadius - 0.5f, StartPosition.Y + Size.Y - 0.5f), new Vector2(StartPosition.X + Size.X - CurveRadius + 0.5f, StartPosition.Y + Size.Y - 0.5f), color, BorderThickness);

            var textSize = ImGui.CalcTextSize(Label);
            var textStartPadding = 7.0f * ImGuiHelpers.GlobalScale;
            var textEndPadding = 7.0f * ImGuiHelpers.GlobalScale;
            var textVerticalOffset = textSize.Y / 2.0f;

            DrawList.AddText(new Vector2(StartPosition.X + CurveRadius + textStartPadding, StartPosition.Y - textVerticalOffset), TextColorU32, Label);
            DrawList.AddLine(new Vector2(StartPosition.X + CurveRadius + textStartPadding + textSize.X + textEndPadding, StartPosition.Y - 0.5f), new Vector2(StartPosition.X + Size.X - CurveRadius + 0.5f, StartPosition.Y - 0.5f), color, BorderThickness);
        }

        private float DegreesToRadians(float degrees) => MathF.PI / 180 * degrees;
    }
}