using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars.UI {
    public unsafe class UIBuff : UIElement {
        public static ushort WIDTH = 34;
        public static ushort HEIGHT = 30;

        private ushort PART_ID;
        private AtkTextNode* TextNode;
        private AtkImageNode* Overlay;
        string CurrentText = "";

        public UIBuff(UIBuilder _ui, ushort partId, AtkResNode* node = null) : base(_ui, HEIGHT) {
            PART_ID = partId;
            Setup(node);
        }

        public override void LoadExisting(AtkResNode* node) {
            RootRes = node;
            TextNode = (AtkTextNode*)RootRes->ChildNode;
            Overlay = (AtkImageNode*)RootRes->ChildNode->PrevSiblingNode;
        }

        public override void Init() {
            var nameplateAddon = _UI._ADDON;

            // ======= CONTAINERS =========
            RootRes = _UI.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = WIDTH;
            RootRes->Height = HEIGHT;
            RootRes->ChildCount = 3;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = RootRes;

            TextNode = _UI.CreateTextNode();
            TextNode->FontSize = 15;
            TextNode->LineSpacing = 15;
            TextNode->AlignmentFontType = 20;
            TextNode->AtkResNode.Width = WIDTH;
            TextNode->AtkResNode.Height = HEIGHT;
            TextNode->AtkResNode.X = 0;
            TextNode->AtkResNode.Y = 0;
            TextNode->AtkResNode.Flags |= 0x10;
            TextNode->AtkResNode.Flags_2 = 1;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)TextNode;

            Overlay = _UI.CreateImageNode();
            Overlay->AtkResNode.Width = WIDTH;
            Overlay->AtkResNode.Height = 1;
            Overlay->AtkResNode.X = 0;
            Overlay->AtkResNode.Y = 0;
            Overlay->PartId = UIBuilder.BUFF_OVERLAY;
            Overlay->PartsList = nameplateAddon->UldManager.PartsList;
            Overlay->Flags = 0;
            Overlay->WrapMode = 1;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)Overlay;

            var icon = _UI.CreateImageNode();
            icon->AtkResNode.Width = WIDTH;
            icon->AtkResNode.Height = HEIGHT;
            icon->AtkResNode.X = 0;
            icon->AtkResNode.Y = 0;
            icon->PartId = PART_ID;
            icon->PartsList = nameplateAddon->UldManager.PartsList;
            icon->Flags = 0;
            icon->WrapMode = 1;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)icon;
            //
            RootRes->ChildNode = (AtkResNode*)TextNode;

            icon->AtkResNode.ParentNode = RootRes;
            Overlay->AtkResNode.ParentNode = RootRes;
            TextNode->AtkResNode.ParentNode = RootRes;

            TextNode->AtkResNode.PrevSiblingNode = (AtkResNode*)Overlay;
            Overlay->AtkResNode.PrevSiblingNode = (AtkResNode*)icon;

            UiHelper.SetText(TextNode, "");
        }

        public void SetText(string text) {
            if (text != CurrentText) {
                UiHelper.SetText(TextNode, text);
                CurrentText = text;
            }
        }

        public void SetPercent(float percent) {
            int h = (int)(HEIGHT * percent);
            int yOffset = HEIGHT - h;
            UiHelper.SetSize( (AtkResNode*) Overlay, null, h);
            UiHelper.SetPosition((AtkResNode*)Overlay, 0, yOffset);
        }

        public override void SetColor(ElementColor color) {
        }
    }
}
