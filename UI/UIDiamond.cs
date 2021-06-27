using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars.UI {
    public unsafe class UIDiamond : UIElement {
        private static int MAX = 12;
        private AtkResNode*[] Selected;
        private AtkResNode*[] Ticks;
        private AtkTextNode*[] Text;

        public UIDiamond(UIBuilder _ui, AtkResNode* node = null) : base(_ui) {
            Setup(node);
        }

        public override void Init() {
            Selected = new AtkResNode*[MAX];
            Ticks = new AtkResNode*[MAX];
            Text = new AtkTextNode*[MAX];

            var nameplateAddon = UI._ADDON;

            RootRes = UI.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = 160;
            RootRes->Height = 46;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = RootRes;

            for (int idx = 0; idx < MAX; idx++) {
                // ======= TICKS =========
                Ticks[idx] = UI.CreateResNode();
                Ticks[idx]->X = 20 * idx;
                Ticks[idx]->Y = 0;
                Ticks[idx]->Width = 32;
                Ticks[idx]->Height = 32;
                nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = Ticks[idx];

                var bg = UI.CreateImageNode();
                bg->AtkResNode.Width = 32;
                bg->AtkResNode.Height = 32;
                bg->AtkResNode.X = 0;
                bg->AtkResNode.Y = 0;
                bg->PartId = UIBuilder.DIAMOND_BG;
                bg->PartsList = nameplateAddon->UldManager.PartsList;
                bg->Flags = 0;
                bg->WrapMode = 1;
                nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)bg;

                // ======== SELECTED ========
                Selected[idx] = UI.CreateResNode();
                Selected[idx]->X = 0;
                Selected[idx]->Y = 0;
                Selected[idx]->Width = 32;
                Selected[idx]->Height = 32;
                Selected[idx]->OriginX = 16;
                Selected[idx]->OriginY = 16;
                nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = Selected[idx];

                Text[idx] = UI.CreateTextNode();
                Text[idx]->FontSize = 12;
                Text[idx]->LineSpacing = 12;
                Text[idx]->AlignmentFontType = 4;
                Text[idx]->TextId = 0;
                Text[idx]->SheetType = 0;
                Text[idx]->TextFlags = 8;
                Text[idx]->TextFlags2 = 0;
                Text[idx]->AtkResNode.Width = 32;
                Text[idx]->AtkResNode.Height = 32;
                Text[idx]->AtkResNode.Y = 16;
                Text[idx]->AtkResNode.X = -1;
                Text[idx]->AtkResNode.Flags |= 0x10;
                Text[idx]->AtkResNode.Flags_2 = 1;
                nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)Text[idx];

                var selectedImage = UI.CreateImageNode();
                selectedImage->AtkResNode.Width = 32;
                selectedImage->AtkResNode.Height = 32;
                selectedImage->AtkResNode.X = 0;
                selectedImage->AtkResNode.Y = 0;
                selectedImage->AtkResNode.OriginX = 16;
                selectedImage->AtkResNode.OriginY = 16;
                selectedImage->PartId = UIBuilder.DIAMOND_FG;
                selectedImage->PartsList = nameplateAddon->UldManager.PartsList;
                selectedImage->Flags = 0;
                selectedImage->WrapMode = 1;
                nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)selectedImage;

                // ======== SELECTED SETUP ========
                Selected[idx]->ChildCount = 2;
                Selected[idx]->ChildNode = (AtkResNode*)selectedImage;
                selectedImage->AtkResNode.PrevSiblingNode = (AtkResNode*)Text[idx];
                selectedImage->AtkResNode.ParentNode = Selected[idx];
                Text[idx]->AtkResNode.ParentNode = Selected[idx];

                // ======= SETUP TICKS =====
                Ticks[idx]->ChildCount = 4;
                Ticks[idx]->ChildNode = (AtkResNode*)bg;
                bg->AtkResNode.PrevSiblingNode = Selected[idx];
                bg->AtkResNode.ParentNode = Ticks[idx];
                Selected[idx]->ParentNode = Ticks[idx];
            }

            // ====== SETUP ROOT =======
            RootRes->ChildNode = Ticks[0];
            RootRes->ChildCount = (ushort)(5 * MAX);
            for (int idx = 0; idx < MAX; idx++) {
                Ticks[idx]->ParentNode = RootRes;
                if (idx < (MAX - 1)) {
                    Ticks[idx]->PrevSiblingNode = Ticks[idx + 1];
                }
            }
        }

        public override void LoadExisting(AtkResNode* node) {
            Selected = new AtkResNode*[MAX];
            Ticks = new AtkResNode*[MAX];
            Text = new AtkTextNode*[MAX];

            RootRes = node;
            var n = RootRes->ChildNode;
            for (int i = 0; i < MAX; i++) {
                if (n == null) continue;
                Ticks[i] = n;
                Selected[i] = n->ChildNode->PrevSiblingNode;
                Text[i] = (AtkTextNode*)Selected[i]->ChildNode->PrevSiblingNode;
                n = n->PrevSiblingNode;
            }
        }

        public override void SetColor(ElementColor color) {
            for(int idx = 0; idx < MAX; idx++) {
                SetColor(color, idx);
            }
        }

        public void SetColor(ElementColor color, int idx) {
            UIColor.SetColor(Selected[idx]->ChildNode, color);
        }

        public void SetMaxValue(int value, bool showText = false) {
            for (int idx = 0; idx < MAX; idx++) {
                if (idx < value) {
                    UiHelper.Show(Ticks[idx]);
                    if(showText) {
                        UiHelper.Show(Text[idx]);
                    }
                    else {
                        UiHelper.Hide(Text[idx]);
                    }
                }
                else {
                    UiHelper.Hide(Ticks[idx]);
                }
            }
        }

        public void SetValue(int value) {
            for (int idx = 0; idx < MAX; idx++) {
                if (idx < value) {
                    UiHelper.Show(Selected[idx]);
                }
                else {
                    UiHelper.Hide(Selected[idx]);
                }
            }
        }

        public void SelectPart(int idx) {
            UiHelper.Show(Selected[idx]);
        }

        public void UnselectPart(int idx) {
            UiHelper.Hide(Selected[idx]);
        }

        public void SetText(int idx, string text) {
            UiHelper.SetText(Text[idx], text);
        }

        public void ShowText(int idx) {
            UiHelper.Show(Text[idx]);
        }

        public void HideText(int idx) {
            UiHelper.Hide(Text[idx]);
        }

        public override int GetHeight(int param) {
            return 35;
        }

        public override int GetWidth(int param) {
            return 32 + 20 * (param - 1);
        }

        public override int GetHorizontalYOffset() {
            return -3;
        }
    }
}
