﻿using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System.Numerics;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        private AtkResNode* CursorRoot = null;
        private AtkImageNode* CursorInner = null;
        private AtkImageNode* CursorOuter = null;

        private bool StaticCircleInner = true;
        private bool StaticCircleOuter = true;

        private void InitCursor() {
            CursorRoot = CreateResNode();
            CursorRoot->Width = 44;
            CursorRoot->Height = 46;
            CursorRoot->Flags = 9395;

            CursorOuter = CreateImageNode();
            CursorOuter->AtkResNode.Width = 44;
            CursorOuter->AtkResNode.Height = 46;
            CursorOuter->AtkResNode.ParentNode = CursorRoot;
            SetPartId(CursorOuter, 79, ref StaticCircleOuter);
            CursorOuter->Flags = 0;
            CursorOuter->WrapMode = 1;

            CursorInner = CreateImageNode();
            CursorInner->AtkResNode.Width = 44;
            CursorInner->AtkResNode.Height = 46;
            CursorInner->AtkResNode.ParentNode = CursorRoot;
            SetPartId(CursorInner, 79, ref StaticCircleInner);
            CursorInner->Flags = 0;
            CursorInner->WrapMode = 1;

            UIHelper.Link((AtkResNode*)CursorInner, (AtkResNode*)CursorOuter);
            CursorRoot->ChildCount = 2;
            CursorRoot->ChildNode = (AtkResNode*)CursorInner;
        }

        private void SetPartId(AtkImageNode* node, int partId, ref bool staticCircle) {
            if (partId == 80) { // Placeholder for static circle
                if (!staticCircle) UIHelper.SetupTexture(node, "ui/uld/CursorLocation.tex");
                staticCircle = true;

                UIHelper.UpdatePart(node, 0, 0, 128, 128);

            }
            else {
                if (staticCircle) UIHelper.SetupTexture(node, "ui/uld/IconA_Recast2.tex");
                staticCircle = false;

                var row = partId % 9;
                var column = (partId - row) / 9;

                var u = (ushort)(44 * row);
                var v = (ushort)(48 * column);

                UIHelper.UpdatePart(node, u, v, 44, 46);
            }
        }

        private void DisposeCursor() {
            if (CursorInner != null) {
                UIHelper.UnloadTexture(CursorInner);
                CursorInner->AtkResNode.Destroy(true);
                CursorInner = null;
            }

            if (CursorOuter != null) {
                UIHelper.UnloadTexture(CursorOuter);
                CursorOuter->AtkResNode.Destroy(true);
                CursorOuter = null;
            }

            if (CursorRoot != null) {
                CursorRoot->Destroy(true);
                CursorRoot = null;
            }
        }

        public void SetCursorPosition(Vector2 pos) => SetPosition(CursorRoot, pos.X, pos.Y);
        public void ShowCursor() => UIHelper.Show(CursorRoot);
        public void HideCursor() => UIHelper.Hide(CursorRoot);
        public void ShowCursorInner() => UIHelper.Show(CursorInner);
        public void HideCursorInner() => UIHelper.Hide(CursorInner);
        public void ShowCursorOuter() => UIHelper.Show(CursorOuter);
        public void HideCursorOuter() => UIHelper.Hide(CursorOuter);

        public void SetCursorInnerPercent(float percent, float scale) => SetCursorPercent(CursorInner, percent, scale, ref StaticCircleInner);
        public void SetCursorOuterPercent(float percent, float scale) => SetCursorPercent(CursorOuter, percent, scale, ref StaticCircleOuter);

        private void SetCursorPercent(AtkImageNode* node, float percent, float scale, ref bool staticCircle) {
            if (percent == 2) { // whatever, just use this for the solid circle
                node->AtkResNode.Width = 128;
                node->AtkResNode.Height = 128;
                node->AtkResNode.X = -(128f * scale) / 2f;
                node->AtkResNode.Y = -(128f * scale) / 2f + 2;
                SetPartId(node, 80, ref staticCircle);

                UIHelper.SetScale((AtkResNode*)node, scale, scale);
            }
            else {
                node->AtkResNode.Width = 44;
                node->AtkResNode.Height = 46;
                node->AtkResNode.X = -22f * scale;
                node->AtkResNode.Y = -20f * scale;
                SetPartId(node, (ushort)(percent * 79), ref staticCircle);

                UIHelper.SetScale((AtkResNode*)node, scale, scale);
            }
        }

        public void SetCursorInnerColor(ElementColor color) {
            var newColor = color;
            newColor.AddRed -= 40;
            newColor.AddBlue += 40;
            UIColor.SetColor(CursorInner, newColor);
        }

        public void SetCursorOuterColor(ElementColor color) {
            var newColor = color;
            newColor.AddRed -= 40;
            newColor.AddBlue += 40;
            UIColor.SetColor(CursorOuter, newColor);
        }
    }
}
