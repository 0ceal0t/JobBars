using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.UI {
    public unsafe class UIDiamond : UIElement {
        private static int MAX = 6;
        private AtkImageNode*[] Selected;
        private AtkResNode*[] Ticks;

        public UIDiamond(UIBuilder _ui, AtkResNode* node = null) : base(_ui) {
            Setup(node);
        }

        public override void Init() {
            Selected = new AtkImageNode*[MAX];
            Ticks = new AtkResNode*[MAX];

            var nameplateAddon = _UI._ADDON;

            RootRes = _UI.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = 160;
            RootRes->Height = 46;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = RootRes;

            for (int idx = 0; idx < MAX; idx++) {
                // ======= TICKS =========
                Ticks[idx] = _UI.CreateResNode();
                Ticks[idx]->X = 20 * idx;
                Ticks[idx]->Y = 0;
                Ticks[idx]->Width = 32;
                Ticks[idx]->Height = 32;
                nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = Ticks[idx];

                var bg = _UI.CreateImageNode();
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
                var selectedContainer = _UI.CreateResNode();
                selectedContainer->X = 0;
                selectedContainer->Y = 0;
                selectedContainer->Width = 32;
                selectedContainer->Height = 32;
                selectedContainer->OriginX = 0;
                selectedContainer->OriginY = 0;
                nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = selectedContainer;

                Selected[idx] = _UI.CreateImageNode();
                Selected[idx]->AtkResNode.Width = 32;
                Selected[idx]->AtkResNode.Height = 32;
                Selected[idx]->AtkResNode.X = 0;
                Selected[idx]->AtkResNode.Y = 0;
                Selected[idx]->AtkResNode.OriginX = 0;
                Selected[idx]->AtkResNode.OriginY = 0;
                Selected[idx]->PartId = UIBuilder.DIAMOND_FG;
                Selected[idx]->PartsList = nameplateAddon->UldManager.PartsList;
                Selected[idx]->Flags = 0;
                Selected[idx]->WrapMode = 1;
                nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)Selected[idx];
                // ======== SELECTED SETUP ========
                selectedContainer->ChildCount = 1;
                selectedContainer->ChildNode = (AtkResNode*)Selected[idx];
                Selected[idx]->AtkResNode.ParentNode = selectedContainer;

                // ======= SETUP TICKS =====
                Ticks[idx]->ChildCount = 3;
                Ticks[idx]->ChildNode = (AtkResNode*)bg;
                bg->AtkResNode.PrevSiblingNode = selectedContainer;
                bg->AtkResNode.ParentNode = Ticks[idx];
                selectedContainer->ParentNode = Ticks[idx];
            }

            // ====== SETUP ROOT =======
            RootRes->ChildNode = Ticks[0];
            RootRes->ChildCount = (ushort)(4 * MAX);
            for (int idx = 0; idx < MAX; idx++) {
                Ticks[idx]->ParentNode = RootRes;
                if (idx < (MAX - 1)) {
                    Ticks[idx]->PrevSiblingNode = Ticks[idx + 1];
                }
            }
        }

        public override unsafe void LoadExisting(AtkResNode* node) {
            Selected = new AtkImageNode*[MAX];
            Ticks = new AtkResNode*[MAX];

            RootRes = node;
            var n = RootRes->ChildNode;
            for (int i = 0; i < MAX; i++) {
                if (n == null) continue;
                Ticks[i] = n;
                Selected[i] = (AtkImageNode*)n->ChildNode->PrevSiblingNode->ChildNode;
                n = n->PrevSiblingNode;
            }
        }

        public override void SetColor(UIColor.ElementColor color) {
        }
        public void SetColor(UIColor.ElementColor color, int idx) {
            UIColor.SetColor((AtkResNode*)Selected[idx], color);
        }

        public void SetParts(int value) {
            for (int idx = 0; idx < MAX; idx++) {
                if (idx < value) {
                    UIBuilder.RecurseHide(Ticks[idx], false, false); // show
                }
                else {
                    UIBuilder.RecurseHide(Ticks[idx], true, false);
                }
            }
        }

        public void SelectPart(int idx) {
            UIBuilder.RecurseHide((AtkResNode*)Selected[idx], false, false);
        }
        public void UnselectPart(int idx) {
            UIBuilder.RecurseHide((AtkResNode*)Selected[idx], true, false);
        }
        public override int GetHeight(int param) {
            return 32;
        }
        public override int GetWidth(int param) {
            return 32 + 20 * (param - 1);
        }
        public override int GetHorizontalYOffset() {
            return -3;
        }
    }
}
