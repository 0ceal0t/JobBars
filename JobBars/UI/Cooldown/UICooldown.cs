using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Collections.Generic;

namespace JobBars.UI {
    public unsafe class UICooldownItem : UIElement {
        private AtkImageNode* Icon;
        private AtkTextNode* TextNode;
        private AtkImageNode* Border;

        public static readonly ushort WIDTH = 30;
        public static readonly ushort HEIGHT = 30;

        public UICooldownItem(AtkUldPartsList* partsList) {
            RootRes = UIBuilder.CreateResNode();
            RootRes->Width = WIDTH;
            RootRes->Height = HEIGHT;
            RootRes->ChildCount = 3;

            TextNode = UIBuilder.CreateTextNode();
            TextNode->FontSize = 13;
            TextNode->LineSpacing = (byte)HEIGHT;
            TextNode->AlignmentFontType = 20;
            TextNode->AtkResNode.Width = WIDTH;
            TextNode->AtkResNode.Height = HEIGHT;
            TextNode->AtkResNode.X = 0;
            TextNode->AtkResNode.Y = 0;
            TextNode->AtkResNode.Flags |= 0x10;
            TextNode->AtkResNode.Flags_2 = 1;
            TextNode->EdgeColor = new ByteColor { R = 51, G = 51, B = 51, A = 255 };

            Icon = UIBuilder.CreateImageNode();
            Icon->AtkResNode.Width = WIDTH;
            Icon->AtkResNode.Height = HEIGHT;
            Icon->AtkResNode.X = 0;
            Icon->AtkResNode.Y = 0;
            Icon->PartId = 0;
            Icon->Flags |= (byte)ImageNodeFlags.AutoFit;
            Icon->WrapMode = 1;

            UIHelper.LoadIcon(Icon, 405);
            UIHelper.UpdatePart(Icon->PartsList, 0, 0, 0, 44, 46);

            Border = UIBuilder.CreateImageNode();
            Border->AtkResNode.Width = 49;
            Border->AtkResNode.Height = 47;
            Border->AtkResNode.X = -4;
            Border->AtkResNode.Y = -2;
            Border->PartId = UIBuilder.CD_BORDER;
            Border->PartsList = partsList;
            Border->Flags = 0;
            Border->WrapMode = 1;
            UIHelper.SetScale((AtkResNode*)Border, ((float)WIDTH + 8) / 49.0f, ((float)HEIGHT + 4) / 47.0f);

            TextNode->AtkResNode.ParentNode = RootRes;
            Icon->AtkResNode.ParentNode = RootRes;
            Border->AtkResNode.ParentNode = RootRes;

            RootRes->ChildNode = (AtkResNode*)Icon;

            UIHelper.Link((AtkResNode*)Icon, (AtkResNode*)Border);
            UIHelper.Link((AtkResNode*)Border, (AtkResNode*)TextNode);
            TextNode->SetText("");
        }

        private void SetTextSize(byte size) {
            TextNode->FontSize = size;
        }

        public void SetNoDash() {
            Border->PartId = UIBuilder.CD_BORDER;
        }

        public void SetDash(float percent) {
            Border->PartId = JobBars.Config.CooldownsShowBorderWhenActive ? (ushort)(UIBuilder.CD_DASH_START + percent * 7) : UIBuilder.CD_BORDER;
        }

        public void SetText(string text) {
            SetTextSize(text.Length > 2 ? (byte)9 : (byte)13);
            TextNode->SetText(text);
        }

        public void SetOnCD() {
            Icon->AtkResNode.MultiplyBlue = 75;
            Icon->AtkResNode.MultiplyRed = 75;
            Icon->AtkResNode.MultiplyGreen = 75;
        }

        public void SetOffCD() {
            Icon->AtkResNode.MultiplyBlue = 100;
            Icon->AtkResNode.MultiplyRed = 100;
            Icon->AtkResNode.MultiplyGreen = 100;
        }

        public void LoadIcon(ActionIds action) {
            var icon = UIHelper.GetIcon(action);
            Icon->LoadIconTexture(icon, 0);
        }

        public override void Dispose() {
            if (TextNode != null) {
                TextNode->AtkResNode.Destroy(true);
                TextNode = null;
            }

            if (Icon != null) {
                UIHelper.UnloadIcon(Icon);
                Icon->AtkResNode.Destroy(true);
                Icon = null;
            }

            if (Border != null) {
                Border->UnloadTexture();
                Border->AtkResNode.Destroy(true);
                Border = null;
            }

            if (RootRes != null) {
                RootRes->Destroy(true);
                RootRes = null;
            }
        }
    }

    public unsafe class UICooldown : UIElement {
        public static readonly int MAX_ITEMS = 20;

        public List<UICooldownItem> Items = new();

        public UICooldown(AtkUldPartsList* partsList) {
            RootRes = UIBuilder.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = (ushort)((3 + UICooldownItem.WIDTH) * MAX_ITEMS); // with padding
            RootRes->Height = UICooldownItem.HEIGHT;

            UICooldownItem lastItem = null;
            for (var idx = 0; idx < MAX_ITEMS; idx++) {
                var item = new UICooldownItem(partsList);
                item.RootRes->ParentNode = RootRes;
                UIHelper.SetPosition(item.RootRes, -(5 + UICooldownItem.WIDTH) * idx, 0);
                Items.Add(item);

                if (lastItem != null) UIHelper.Link(lastItem.RootRes, item.RootRes);
                lastItem = item;
            }

            RootRes->ChildCount = (ushort)(MAX_ITEMS * (1 + Items[0].RootRes->ChildCount));
            RootRes->ChildNode = Items[0].RootRes;
        }

        public void HideAllItems() {
            foreach (var item in Items) item.Hide();
        }

        public void SetVisibility(int idx, bool visible) => Items[idx].SetVisible(visible);

        public void LoadIcon(int idx, ActionIds action) => Items[idx].LoadIcon(action);

        public override void Dispose() {
            for (int idx = 0; idx < MAX_ITEMS; idx++) {
                Items[idx].Dispose();
                Items[idx] = null;
            }
            Items = null;

            if (RootRes != null) {
                RootRes->Destroy(true);
                RootRes = null;
            }
        }
    }
}