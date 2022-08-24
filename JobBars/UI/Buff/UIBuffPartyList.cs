using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;

namespace JobBars.UI {
    public unsafe class UIBuffPartyList {
        private AtkNineGridNode* Highlight;
        private AtkTextNode* TextNode;
        private bool Attached = false;

        public UIBuffPartyList() {
            Highlight = UIBuilder.CreateNineNode();
            Highlight->AtkResNode.Width = 320;
            Highlight->AtkResNode.Height = 48;
            UIHelper.SetupTexture(Highlight, "ui/uld/PartyListTargetBase.tex");
            UIHelper.UpdatePart(Highlight->PartsList, 0, 112, 0, 48, 48);
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
            TextNode->FontSize = (byte)JobBars.Config.BuffTextSize_v2;
            TextNode->LineSpacing = (byte)JobBars.Config.BuffTextSize_v2;
            TextNode->AlignmentFontType = 20;
            TextNode->FontSize = 14;
            TextNode->TextColor = new ByteColor { R = 232, G = 255, B = 254, A = 255 };
            TextNode->EdgeColor = new ByteColor { R = 8, G = 80, B = 152, A = 255 };
            TextNode->AtkResNode.X = 30;
            TextNode->AtkResNode.Y = 20;
            TextNode->AtkResNode.Flags |= 0x10;
            TextNode->AtkResNode.Flags_2 = 1;
            TextNode->AtkResNode.Priority = 0;
            TextNode->SetText("");
            UIHelper.Hide(TextNode);
        }

        public void Dispose() {
            if (TextNode != null) {
                TextNode->AtkResNode.Destroy(true);
                TextNode = null;
            }

            if (Highlight != null) {
                UIHelper.UnloadTexture(Highlight);
                Highlight->AtkResNode.Destroy(true);
                Highlight = null;
            }
        }

        public void AttachTo(AtkResNode* targetGlowContainer, AtkTextNode* iconBottomLeftText) {
            if (Attached) {
                PluginLog.Error("Already attached");
                return;
            }
            if (targetGlowContainer == null || iconBottomLeftText == null) return;

            targetGlowContainer->ChildCount = 4;
            Highlight->AtkResNode.ParentNode = targetGlowContainer;
            UIHelper.Link(targetGlowContainer->ChildNode->PrevSiblingNode->PrevSiblingNode, (AtkResNode*)Highlight);

            // parent is the component, so don't have to worry about child count
            TextNode->AtkResNode.ParentNode = iconBottomLeftText->AtkResNode.ParentNode;
            UIHelper.Link((AtkResNode*)iconBottomLeftText, (AtkResNode*)TextNode);

            Attached = true;
        }

        public void DetachFrom(AtkResNode* targetGlowContainer, AtkTextNode* iconBottomLeftText) {
            if (!Attached) {
                PluginLog.Error("Not attached yet");
                return;
            }
            if (targetGlowContainer == null || iconBottomLeftText == null) return;

            targetGlowContainer->ChildCount = 3;
            UIHelper.Detach((AtkResNode*)Highlight);
            UIHelper.Detach((AtkResNode*)TextNode);

            Attached = false;
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
