using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System.Numerics;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        private AtkResNode* CursorRoot = null;
        private AtkImageNode* CursorInner = null;
        private AtkImageNode* CursorOuter = null;

        private void InitCursor(AtkUldPartsList* partsList) {
            CursorRoot = CreateResNode();
            CursorRoot->Width = 44;
            CursorRoot->Height = 46;
            CursorRoot->Flags = 9395;

            CursorOuter = CreateImageNode();
            CursorOuter->AtkResNode.Width = 44;
            CursorOuter->AtkResNode.Height = 46;

            CursorOuter->AtkResNode.ParentNode = CursorRoot;
            CursorOuter->PartId = 79;
            CursorOuter->PartsList = partsList;
            CursorOuter->Flags = 0;
            CursorOuter->WrapMode = 1;

            CursorInner = CreateImageNode();
            CursorInner->AtkResNode.Width = 44;
            CursorInner->AtkResNode.Height = 46;

            CursorInner->AtkResNode.ParentNode = CursorRoot;
            CursorInner->PartId = 79;
            CursorInner->PartsList = partsList;
            CursorInner->Flags = 0;
            CursorInner->WrapMode = 1;

            UIHelper.Link((AtkResNode*)CursorInner, (AtkResNode*)CursorOuter);
            CursorRoot->ChildCount = 2;
            CursorRoot->ChildNode = (AtkResNode*)CursorInner;
        }

        private void DisposeCursor() {
            if (CursorInner != null) {
                CursorInner->UnloadTexture();
                CursorInner->AtkResNode.Destroy(true);
                CursorInner = null;
            }

            if (CursorOuter != null) {
                CursorOuter->UnloadTexture();
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

        public void SetCursorInnerPercent(float percent, float scale) => SetCursorPercent(CursorInner, percent, scale);
        public void SetCursorOuterPercent(float percent, float scale) => SetCursorPercent(CursorOuter, percent, scale);

        private void SetCursorPercent(AtkImageNode* node, float percent, float scale) {
            if (percent == 2) { // whatever, just use this for the solid circle
                node->AtkResNode.Width = 128;
                node->AtkResNode.Height = 128;
                node->AtkResNode.X = -(128f * scale) / 2f;
                node->AtkResNode.Y = -(128f * scale) / 2f;
                node->PartId = 80;
                UIHelper.SetScale((AtkResNode*)node, scale, scale);
            }
            else {
                node->AtkResNode.Width = 44;
                node->AtkResNode.Height = 46;
                node->AtkResNode.X = -(44f * scale) / 2f;
                node->AtkResNode.Y = -(46f * scale) / 2f;
                node->PartId = (ushort)(percent * 79);
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
