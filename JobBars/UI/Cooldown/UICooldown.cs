using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.UI {
    public unsafe class UICooldownItem : UIElement {
        private AtkImageNode* Icon;
        private AtkTextNode* TextNode;

        public static readonly ushort WIDTH = 30;
        public static readonly ushort HEIGHT = 30;

        public UICooldownItem() {
            RootRes = UIBuilder.Builder.CreateResNode();
            RootRes->Width = WIDTH;
            RootRes->Height = HEIGHT;
            RootRes->ChildCount = 2;

            TextNode = UIBuilder.Builder.CreateTextNode();
            TextNode->FontSize = 15;
            TextNode->LineSpacing = 15;
            TextNode->AlignmentFontType = 20;
            TextNode->AtkResNode.Width = WIDTH;
            TextNode->AtkResNode.Height = HEIGHT;
            TextNode->AtkResNode.X = 0;
            TextNode->AtkResNode.Y = 0;
            TextNode->AtkResNode.Flags |= 0x10;
            TextNode->AtkResNode.Flags_2 = 1;

            Icon = UIBuilder.Builder.CreateImageNode();
            Icon->AtkResNode.Width = WIDTH;
            Icon->AtkResNode.Height = HEIGHT;
            Icon->AtkResNode.X = 0;
            Icon->AtkResNode.Y = 0;
            Icon->PartId = 0;
            Icon->Flags |= (byte)ImageNodeFlags.AutoFit;
            Icon->WrapMode = 1;

            UIHelper.LoadIcon(Icon, 405);
            UIHelper.UpdatePart(Icon->PartsList, 0, 0, 0, 44, 46);

            TextNode->AtkResNode.ParentNode = RootRes;
            Icon->AtkResNode.ParentNode = RootRes;

            RootRes->ChildNode = (AtkResNode*)Icon;

            UIHelper.Link((AtkResNode*)Icon, (AtkResNode*)TextNode);
            TextNode->SetText("");
        }

        public void SetText(string text) {
            TextNode->SetText(text);
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

        public void LoadIcon(ActionIds action) {
            var icon = DataManager.GetIcon(action);
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

            if (RootRes != null) {
                RootRes->Destroy(true);
                RootRes = null;
            }
        }
    }
    
    public unsafe class UICooldown : UIElement {
        public static readonly int MAX_ITEMS = 5;

        public List<UICooldownItem> Items = new();

        public UICooldown() {
            RootRes = UIBuilder.Builder.CreateResNode();
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