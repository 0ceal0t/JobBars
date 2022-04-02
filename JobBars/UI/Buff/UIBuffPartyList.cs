using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;

namespace JobBars.UI {
    public unsafe class UIBuffPartyList {
        private AtkResNode* RootRes;
        private AtkNineGridNode* Highlight;
        private AtkTextNode* TextNode;

        public UIBuffPartyList(AtkUldPartsList* partsList) {
            RootRes = UIBuilder.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->ChildCount = 2;

            Highlight = UIBuilder.CreateNineNode();
            Highlight->AtkResNode.Width = 320;
            Highlight->AtkResNode.Height = 48;
            Highlight->PartID = UIBuilder.BUFF_PARTYLIST;
            Highlight->PartsList = partsList;
            Highlight->TopOffset = 20;
            Highlight->BottomOffset = 20;
            Highlight->RightOffset = 20;
            Highlight->LeftOffset = 20;
            Highlight->PartsTypeRenderType = 220;
            Highlight->AtkResNode.NodeID = 31;
            Highlight->AtkResNode.Flags_2 = 0;
            Highlight->AtkResNode.DrawFlags = 0;
            Highlight->AtkResNode.Flags = 8243;
            Highlight->AtkResNode.MultiplyBlue = 50;
            Highlight->AtkResNode.MultiplyRed = 150;
            UIHelper.SetPosition(Highlight, 47, 21);
            UIHelper.Hide(Highlight);

            TextNode = UIBuilder.CreateTextNode();
            TextNode->FontSize = (byte)JobBars.Config.BuffTextSize;
            TextNode->LineSpacing = (byte)JobBars.Config.BuffTextSize;
            TextNode->AlignmentFontType = 20;
            TextNode->FontSize = 14;
            TextNode->TextColor = new ByteColor { R = 232, G = 255, B = 254, A = 255 };
            TextNode->EdgeColor = new ByteColor { R = 8, G = 80, B = 152, A = 255 };
            TextNode->AtkResNode.X = 25;
            TextNode->AtkResNode.Y = 35;
            TextNode->AtkResNode.Flags |= 0x10;
            TextNode->AtkResNode.Flags_2 = 1;
            TextNode->AtkResNode.Priority = 1;
            TextNode->SetText("15");

            UIHelper.Link((AtkResNode*)Highlight, (AtkResNode*)TextNode);

            Highlight->AtkResNode.ParentNode = RootRes;
            TextNode->AtkResNode.ParentNode = RootRes;

            RootRes->ChildCount = 2;
            RootRes->ChildNode = (AtkResNode*)Highlight;
        }

        public void Dispose() {
            if (RootRes != null) {
                RootRes->Destroy(true);
                RootRes = null;
            }

            if (TextNode != null) {
                TextNode->AtkResNode.Destroy(true);
                TextNode = null;
            }

            if (Highlight != null) {
                Highlight->AtkResNode.Destroy(true);
                Highlight = null;
            }
        }

        public void AttachTo(AtkResNode* container) {
            container->ChildCount = 6;
            RootRes->ParentNode = container;
            UIHelper.Link(container->ChildNode->PrevSiblingNode->PrevSiblingNode, RootRes);
        }

        public void DetachFrom(AtkResNode* container) {
            container->ChildCount = 3;
            RootRes->NextSiblingNode->PrevSiblingNode = null;
        }

        public void SetHighlightVisibility(bool visible) => UIHelper.SetVisibility(Highlight, visible);

        public void SetText(string text) {
            if (string.IsNullOrEmpty(text)) {
                UIHelper.Hide(TextNode);
                return;
            }
            UIHelper.Show(TextNode);
            TextNode->SetText(text);
        }
    }
}
