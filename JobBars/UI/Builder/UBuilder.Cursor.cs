using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System.Collections.Generic;
using System.Numerics;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        private AtkResNode* CursorRoot = null;
        private AtkImageNode* CursorInner = null;
        private AtkImageNode* CursorOuter = null;

        private void InitCursor(AtkUldPartsList* partsList) {
            ushort WIDTH = 44;
            ushort HEIGHT = 46;
            float INNER_SCALE = 1.2f;
            float OUTER_SCALE = 1.7f;

            CursorRoot = CreateResNode();
            CursorRoot->Width = WIDTH;
            CursorRoot->Height = HEIGHT;
            CursorRoot->OriginX = 0;
            CursorRoot->OriginY = 0;
            CursorRoot->Flags = 9395;

            CursorOuter = CreateImageNode();
            CursorOuter->AtkResNode.Width = WIDTH;
            CursorOuter->AtkResNode.Height = HEIGHT;

            CursorOuter->AtkResNode.X = -(OUTER_SCALE * WIDTH) / 2.0f;
            CursorOuter->AtkResNode.Y = -(OUTER_SCALE * HEIGHT) / 2.0f;

            CursorOuter->AtkResNode.ParentNode = CursorRoot;
            CursorOuter->PartId = 79;
            CursorOuter->PartsList = partsList;
            CursorOuter->Flags = 0;
            CursorOuter->WrapMode = 1;
            UIHelper.SetScale((AtkResNode*)CursorOuter, OUTER_SCALE, OUTER_SCALE);

            CursorInner = CreateImageNode();
            CursorInner->AtkResNode.Width = WIDTH;
            CursorInner->AtkResNode.Height = HEIGHT;

            CursorInner->AtkResNode.X = -(INNER_SCALE * WIDTH) / 2.0f;
            CursorInner->AtkResNode.Y = -(INNER_SCALE * HEIGHT) / 2.0f;

            CursorInner->AtkResNode.ParentNode = CursorRoot;
            CursorInner->PartId = 79;
            CursorInner->PartsList = partsList;
            CursorInner->Flags = 0;
            CursorInner->WrapMode = 1;
            UIHelper.SetScale((AtkResNode*)CursorInner, INNER_SCALE, INNER_SCALE);

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

        public void SetInnerPercent(float percent) => CursorInner->PartId = (ushort)(percent * 79);
        public void SetOuterPercent(float percent) => CursorOuter->PartId = (ushort)(percent * 79);
    }
}
