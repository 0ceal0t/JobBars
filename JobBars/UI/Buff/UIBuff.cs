using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;

namespace JobBars.UI {
    public unsafe class UIBuff : UIElement {
        public static readonly ushort WIDTH = 37;
        public static readonly ushort HEIGHT = 28;

        private AtkTextNode* TextNode;
        private AtkImageNode* Overlay;
        private AtkImageNode* Icon;
        private AtkNineGridNode* Border;

        private ActionIds LastIconId = 0;
        public ActionIds Iconid => LastIconId;

        private string CurrentText = "";
        private static int BUFFS_HORIZONTAL => JobBars.Config.BuffHorizontal;

        public UIBuff(AtkUldPartsList* partsList) : base() {
            RootRes = UIBuilder.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = WIDTH;
            RootRes->Height = HEIGHT;
            RootRes->ChildCount = 4;

            TextNode = UIBuilder.CreateTextNode();
            TextNode->FontSize = 15;
            TextNode->LineSpacing = 15;
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
            Icon->Flags = 0;
            Icon->WrapMode = 1;

            UIHelper.LoadIcon(Icon, 405);
            UIHelper.UpdatePart(Icon->PartsList, 0, 1, 6, 37, 28);

            Overlay = UIBuilder.CreateImageNode();
            Overlay->AtkResNode.Width = WIDTH;
            Overlay->AtkResNode.Height = 1;
            Overlay->AtkResNode.X = 0;
            Overlay->AtkResNode.Y = 0;
            Overlay->PartId = UIBuilder.BUFF_OVERLAY;
            Overlay->PartsList = partsList;
            Overlay->Flags = 0;
            Overlay->WrapMode = 1;

            Border = UIBuilder.CreateNineNode();
            Border->AtkResNode.Width = (ushort)(WIDTH + 4);
            Border->AtkResNode.Height = (ushort)(HEIGHT + 4);
            Border->AtkResNode.X = -2;
            Border->AtkResNode.Y = -2;
            Border->PartID = UIBuilder.BUFF_BORDER;
            Border->PartsList = partsList;

            Border->TopOffset = 5;
            Border->BottomOffset = 5;
            Border->LeftOffset = 5;
            Border->RightOffset = 5;

            Icon->AtkResNode.ParentNode = RootRes;
            Overlay->AtkResNode.ParentNode = RootRes;
            TextNode->AtkResNode.ParentNode = RootRes;
            Border->AtkResNode.ParentNode = RootRes;

            RootRes->ChildNode = (AtkResNode*)TextNode;

            UIHelper.Link((AtkResNode*)TextNode, (AtkResNode*)Icon);
            UIHelper.Link((AtkResNode*)Icon, (AtkResNode*)Overlay);
            UIHelper.Link((AtkResNode*)Overlay, (AtkResNode*)Border);
            TextNode->SetText("");
        }

        public override void Dispose() {
            if (TextNode != null) {
                TextNode->AtkResNode.Destroy(true);
                TextNode = null;
            }

            if(Icon != null) {
                UIHelper.UnloadIcon(Icon);
                Icon->AtkResNode.Destroy(true);
                Icon = null;
            }

            if (Overlay != null) {
                Overlay->UnloadTexture();
                Overlay->AtkResNode.Destroy(true);
                Overlay = null;
            }

            if (Border != null) {
                // TODO
                Border->AtkResNode.Destroy(true);
                Border = null;
            }

            if (RootRes != null) {
                RootRes->Destroy(true);
                RootRes = null;
            }
        }

        public void SetPosition(int idx) {
            var position_x = idx % BUFFS_HORIZONTAL;
            var position_y = (idx - position_x) / BUFFS_HORIZONTAL;

            int xMod = JobBars.Config.BuffRightToLeft ? -1 : 1;
            int yMod = JobBars.Config.BuffBottomToTop ? -1 : 1;

            UIHelper.SetPosition(RootRes, xMod * (WIDTH + 9) * position_x, yMod * (HEIGHT + 7) * position_y);
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
                TextNode->SetText(text);
                CurrentText = text;
            }
        }

        public void SetPercent(float percent) {
            int h = (int)(HEIGHT * percent);
            int yOffset = HEIGHT - h;
            UIHelper.SetSize(Overlay, null, h);
            UIHelper.SetPosition(Overlay, 0, yOffset);
        }

        public void LoadIcon(ActionIds action) {
            LastIconId = action;
            var icon = UIHelper.GetIcon(action);
            Icon->LoadIconTexture(icon, 0);
        }

        public void SetColor(ElementColor color) {
            var newColor = color;
            newColor.AddBlue -= 50;
            UIColor.SetColor(Border, newColor);
        }
    }
}