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

        private ActionIds LastIconId = 0;
        public ActionIds IconId => LastIconId;

        public UICooldownItem() {
            RootRes = UIBuilder.CreateResNode();
            RootRes->Width = WIDTH;
            RootRes->Height = HEIGHT;

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
            UIHelper.SetupIcon(Icon, 405);
            UIHelper.UpdatePart(Icon, 0, 0, 44, 46);

            Border = UIBuilder.CreateImageNode();
            Border->AtkResNode.Width = 49;
            Border->AtkResNode.Height = 47;
            Border->AtkResNode.X = -4;
            Border->AtkResNode.Y = -2;
            UIHelper.SetupTexture(Border, "ui/uld/IconA_Frame.tex");
            UIHelper.UpdatePart(Border, 0, 96, 48, 48);
            Border->Flags = 0;
            Border->WrapMode = 1;
            UIHelper.SetScale((AtkResNode*)Border, ((float)WIDTH + 8) / 49.0f, ((float)HEIGHT + 6) / 47.0f);

            var layout = new LayoutNode(RootRes, new[] {
                new LayoutNode(Icon),
                new LayoutNode(Border),
                new LayoutNode(TextNode)
            });
            layout.Setup();
            layout.Cleanup();

            TextNode->SetText("");
        }

        private void SetTextSize(byte size) {
            TextNode->FontSize = size;
        }

        public void SetNoDash() {
            UIHelper.UpdatePart(Border, 0, 96, 48, 48);
        }

        public void SetDash(float percent) {
            var partId = (int)(percent * 7); // 0 - 6

            var row = partId % 3;
            var column = (partId - row) / 3;

            var u = (ushort)(96 + (48 * row));
            var v = (ushort)(48 * column);

            UIHelper.UpdatePart(Border, u, v, 48, 48);
        }

        public void SetText(string text) {
            SetTextSize(text.Length > 2 ? (byte)9 : (byte)13);
            TextNode->SetText(text);
        }

        public void SetOnCD(float opacity) {
            Icon->AtkResNode.MultiplyBlue = 75;
            Icon->AtkResNode.MultiplyRed = 75;
            Icon->AtkResNode.MultiplyGreen = 75;
            RootRes->Color.A = (byte)(255 * opacity);
        }

        public void SetOffCD() {
            Icon->AtkResNode.MultiplyBlue = 100;
            Icon->AtkResNode.MultiplyRed = 100;
            Icon->AtkResNode.MultiplyGreen = 100;
            RootRes->Color.A = 255;
        }

        public void LoadIcon(ActionIds action) {
            LastIconId = action;
            var icon = UIHelper.GetIcon(action);
            Icon->LoadIconTexture(icon, 0);
        }

        public override void Dispose() {
            if (TextNode != null) {
                TextNode->AtkResNode.Destroy(true);
                TextNode = null;
            }

            if (Icon != null) {
                UIHelper.UnloadTexture(Icon);
                Icon->AtkResNode.Destroy(true);
                Icon = null;
            }

            if (Border != null) {
                UIHelper.UnloadTexture(Border);
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
        public static readonly int MAX_ITEMS = 10;

        public List<UICooldownItem> Items = new();

        public UICooldown() {
            RootRes = UIBuilder.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = (ushort)((3 + UICooldownItem.WIDTH) * MAX_ITEMS); // with padding
            RootRes->Height = UICooldownItem.HEIGHT;

            UICooldownItem lastItem = null;
            for (var idx = 0; idx < MAX_ITEMS; idx++) {
                var item = new UICooldownItem();
                item.RootRes->ParentNode = RootRes;
                UIHelper.SetPosition(item.RootRes, -(5 + UICooldownItem.WIDTH) * idx, 0);
                Items.Add(item);

                if (lastItem != null) UIHelper.Link(lastItem.RootRes, item.RootRes);
                lastItem = item;
            }

            RootRes->ChildCount = (ushort)(MAX_ITEMS * (1 + Items[0].RootRes->ChildCount));
            RootRes->ChildNode = Items[0].RootRes;
        }

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