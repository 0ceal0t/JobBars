using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;

namespace JobBars.UI {
    public unsafe class UIBuffPartyList {
        private AtkNineGridNode* Highlight;

        public UIBuffPartyList(AtkUldPartsList* partsList) {
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
            UIHelper.SetPosition(Highlight, 47, 21);
            UIHelper.Hide(Highlight);
        }

        public void Dispose() {
            if (Highlight != null) {
                Highlight->AtkResNode.Destroy(true);
                Highlight = null;
            }
        }

        public void AttachTo(AtkResNode* container) {
            container->ChildCount = 4;
            Highlight->AtkResNode.ParentNode = container;
            UIHelper.Link(container->ChildNode->PrevSiblingNode->PrevSiblingNode, (AtkResNode*)Highlight);
        }

        public void DetachFrom(AtkResNode* container) {
            container->ChildCount = 3;
            Highlight->AtkResNode.NextSiblingNode->PrevSiblingNode = null;
        }

        public void SetVisibility(bool visible) => UIHelper.SetVisibility(Highlight, visible);
    }
}
