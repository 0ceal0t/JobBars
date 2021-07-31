using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars.UI {
    public unsafe class UIBuff : UIElement {
        public static readonly ushort WIDTH = 37;
        public static readonly ushort HEIGHT = 28;

        private readonly ushort PART_ID;
        private AtkTextNode* TextNode;
        private AtkImageNode* Overlay;
        private AtkImageNode* Border;
        private string CurrentText = "";

        static int BUFFS_HORIZONTAL => Configuration.Config.BuffHorizontal;

        public UIBuff(UIBuilder _ui, ushort partId, AtkResNode* node = null) : base(_ui) {
            PART_ID = partId;
            Setup(node);
        }

        public override void LoadExisting(AtkResNode* node) {
            RootRes = node;
            TextNode = (AtkTextNode*)RootRes->ChildNode;
            Overlay = (AtkImageNode*)RootRes->ChildNode->PrevSiblingNode->PrevSiblingNode;
            Border = (AtkImageNode*)RootRes->ChildNode->PrevSiblingNode->PrevSiblingNode->PrevSiblingNode;
            SetOffCD();
        }

        public override void Init() {
            var nameplateAddon = UI.ADDON;

            // ======= CONTAINERS =========
            RootRes = UI.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = WIDTH;
            RootRes->Height = HEIGHT;
            RootRes->ChildCount = 4;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = RootRes;

            TextNode = UI.CreateTextNode();
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

            var icon = UI.CreateImageNode();
            icon->AtkResNode.Width = WIDTH;
            icon->AtkResNode.Height = HEIGHT;
            icon->AtkResNode.X = 0;
            icon->AtkResNode.Y = 0;
            icon->PartId = PART_ID;
            icon->PartsList = nameplateAddon->UldManager.PartsList;
            icon->Flags = 0;
            icon->WrapMode = 1;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)icon;

            Overlay = UI.CreateImageNode();
            Overlay->AtkResNode.Width = WIDTH;
            Overlay->AtkResNode.Height = 1;
            Overlay->AtkResNode.X = 0;
            Overlay->AtkResNode.Y = 0;
            Overlay->PartId = UIBuilder.BUFF_OVERLAY;
            Overlay->PartsList = nameplateAddon->UldManager.PartsList;
            Overlay->Flags = 0;
            Overlay->WrapMode = 1;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)Overlay;

            Border = UI.CreateImageNode();
            Border->AtkResNode.Width = 47;
            Border->AtkResNode.Height = 47;
            Border->AtkResNode.X = -2;
            Border->AtkResNode.Y = -2;
            Border->PartId = UIBuilder.BUFF_BORDER;
            Border->PartsList = nameplateAddon->UldManager.PartsList;
            Border->Flags = 0;
            Border->WrapMode = 1;
            UiHelper.SetScale((AtkResNode*)Border, ((float)WIDTH + 4) / 47.0f, ((float)HEIGHT + 4) / 47.0f);
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)Border;

            icon->AtkResNode.ParentNode = RootRes;
            Overlay->AtkResNode.ParentNode = RootRes;
            TextNode->AtkResNode.ParentNode = RootRes;
            Border->AtkResNode.ParentNode = RootRes;

            RootRes->ChildNode = (AtkResNode*)TextNode;
            TextNode->AtkResNode.PrevSiblingNode = (AtkResNode*)icon;
            icon->AtkResNode.PrevSiblingNode = (AtkResNode*)Overlay;
            Overlay->AtkResNode.PrevSiblingNode = (AtkResNode*)Border;

            UiHelper.SetText(TextNode, "");
        }

        public void SetPosition(int idx) {
            var position_x = idx % BUFFS_HORIZONTAL;
            var position_y = (idx - position_x) / BUFFS_HORIZONTAL;

            int xMod = Configuration.Config.BuffRightToLeft ? -1 : 1;
            int yMod = Configuration.Config.BuffBottomToTop ? -1 : 1;

            UiHelper.SetPosition(RootRes, xMod * (WIDTH + 9) * position_x, yMod * (HEIGHT + 7) * position_y);
        }

        public void SetOnCD() {
            RootRes->MultiplyBlue = 75;
            RootRes->MultiplyRed = 75;
            RootRes->MultiplyGreen = 75;
        }

        public void SetOffCD() {
            RootRes->MultiplyBlue = 100;
            RootRes->MultiplyRed = 100;
            RootRes->MultiplyGreen = 100;
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
            UiHelper.SetSize((AtkResNode*)Overlay, null, h);
            UiHelper.SetPosition((AtkResNode*)Overlay, 0, yOffset);
        }

        public override void SetColor(ElementColor color) {
            var newColor = color;
            newColor.AddBlue -= 50;
            UIColor.SetColor((AtkResNode*)Border, newColor);
        }

        public override int GetHeight(int param) {
            return HEIGHT;
        }

        public override int GetWidth(int param) {
            return WIDTH;
        }

        public override int GetHorizontalYOffset() {
            return 0;
        }
    }
}