using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.UI {
    public unsafe class UIArrow : UIElement {
        private static int MAX = 9;
        private AtkImageNode*[] Selected;
        private AtkResNode*[] Ticks;

        public UIArrow(UIBuilder _ui, AtkResNode* node = null) : base(_ui, node) {
        }

        public override void Init() {
            /*
             * resnode container (160 x 32)
             *  resnode single (32 x 32)
             *      bg part 0
             *      resnode (32 x 32)
             *          selected part 1
             */

            Selected = new AtkImageNode*[MAX];
            Ticks = new AtkResNode*[MAX];

            var nameplateAddon = _UI._ADDON;

            for (int idx = 0; idx < MAX; idx++) {
                var bg = _UI.CreateImageNode();
                bg->AtkResNode.Width = 32;
                bg->AtkResNode.Height = 32;
                bg->AtkResNode.X = 0;
                bg->AtkResNode.Y = 0;
                bg->PartId = UIBuilder.ARROW_BG;
                bg->PartsList = nameplateAddon->UldManager.PartsList;
                bg->Flags = 0;
                bg->WrapMode = 1;
                nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)bg;

                Selected[idx] = _UI.CreateImageNode();
                Selected[idx]->AtkResNode.Width = 32;
                Selected[idx]->AtkResNode.Height = 32;
                Selected[idx]->AtkResNode.X = 0;
                Selected[idx]->AtkResNode.Y = 0;
                Selected[idx]->AtkResNode.OriginX = 16;
                Selected[idx]->AtkResNode.OriginY = 16;
                Selected[idx]->PartId = UIBuilder.ARROW_FG;
                Selected[idx]->PartsList = nameplateAddon->UldManager.PartsList;
                Selected[idx]->Flags = 0;
                Selected[idx]->WrapMode = 1;
                nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)Selected[idx];

                var selectedContainer = _UI.CreateResNode();
                selectedContainer->X = 0;
                selectedContainer->Y = 0;
                selectedContainer->Width = 32;
                selectedContainer->Height = 32;
                selectedContainer->OriginX = 16;
                selectedContainer->OriginY = 16;
                nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = selectedContainer;
                bg->AtkResNode.PrevSiblingNode = selectedContainer;
                //
                selectedContainer->ChildCount = 1;
                selectedContainer->ChildNode = (AtkResNode*)Selected[idx];
                Selected[idx]->AtkResNode.ParentNode = selectedContainer;

                Ticks[idx] = _UI.CreateResNode();
                Ticks[idx]->X = 18 * idx;
                Ticks[idx]->Y = 0;
                Ticks[idx]->Width = 32;
                Ticks[idx]->Height = 32;
                nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = Ticks[idx];
                //
                Ticks[idx]->ChildCount = 3;
                Ticks[idx]->ChildNode = (AtkResNode*)bg;
                bg->AtkResNode.ParentNode = Ticks[idx];
                selectedContainer->ParentNode = Ticks[idx];
            }

            RootRes = _UI.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = 160;
            RootRes->Height = 46;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = RootRes;
            //
            RootRes->ChildNode = Ticks[0];

            //Ticks[0]->ParentNode = RootRes; // temp
            //Ticks[1]->ParentNode = RootRes;
            //Ticks[0]->PrevSiblingNode = Ticks[1];

            for(int idx = 0; idx < MAX; idx++) {
                Ticks[idx]->ParentNode = RootRes;
                if(idx < (MAX - 1)) {
                    Ticks[idx]->PrevSiblingNode = Ticks[idx + 1];
                }
            }
        }

        public override unsafe void Pickup(AtkResNode* node) {
            Selected = new AtkImageNode*[MAX];
            Ticks = new AtkResNode*[MAX];

            RootRes = node;
            var n = RootRes->ChildNode;
            for(int i = 0; i < MAX; i++) {
                if (n == null) continue;
                Ticks[i] = n;
                Selected[i] = (AtkImageNode*)n->ChildNode->PrevSiblingNode->ChildNode;
                n = n->PrevSiblingNode;
            }
        }

        public override void SetColor(UIColor.ElementColor color) {
            foreach(var item in Selected) {
                UIColor.SetColor((AtkResNode*)item, color);
            }
        }

        public void SetMaxValue(int value) {
            for(int idx = 0; idx < MAX; idx++) {
                if(idx <= (value-1)) {
                    UIBuilder.RecurseHide(Ticks[idx], false); // show
                }
                else {
                    UIBuilder.RecurseHide(Ticks[idx], true);
                }
            }
        }

        public void SetValue(int value) {
            for (int idx = 0; idx < MAX; idx++) {
                if (idx <= (value - 1)) {
                    UIBuilder.RecurseHide((AtkResNode*)Selected[idx], false); // show
                }
                else {
                    UIBuilder.RecurseHide((AtkResNode*)Selected[idx], true);
                }
            }
        }
    }
}
