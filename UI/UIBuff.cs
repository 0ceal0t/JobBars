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

        public UIBuff(UIBuilder _ui, ushort partId, AtkResNode* node = null) : base(_ui, HEIGHT) {
            PART_ID = partId;
            Setup(node);
        }

        public override void LoadExisting(AtkResNode* node) {
            RootRes = node;
        }

        public override void Init() {
            var nameplateAddon = _UI._ADDON;

            var icon = _UI.CreateImageNode();
            icon->AtkResNode.Width = 34;
            icon->AtkResNode.Height = 30;
            icon->AtkResNode.X = 0;
            icon->AtkResNode.Y = 0;
            icon->PartId = PART_ID;
            icon->PartsList = nameplateAddon->UldManager.PartsList;
            icon->Flags = 0;
            icon->WrapMode = 1;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)icon;

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

            // ======= CONTAINERS =========
            RootRes = _UI.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = WIDTH;
            RootRes->Height = HEIGHT;
            RootRes->ChildCount = 2;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = RootRes;
            //
            RootRes->ChildNode = (AtkResNode*)TextNode;

            icon->AtkResNode.ParentNode = RootRes;
            TextNode->AtkResNode.ParentNode = RootRes;

            TextNode->AtkResNode.PrevSiblingNode = (AtkResNode*)icon;
            icon->AtkResNode.NextSiblingNode = (AtkResNode*)TextNode;

            UiHelper.SetText(TextNode, "12");
        }

        public override void SetColor(ElementColor color) {
        }
    }
}
