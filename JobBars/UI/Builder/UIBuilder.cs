using System;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        private static readonly uint NODE_IDX_START = 89990001;
        private static uint NodeIdx = NODE_IDX_START;

        public UIBuilder() {
            NodeIdx = NODE_IDX_START;
            InitTextures(); // init first
            InitGauges(GaugeBuffAssets.PartsList);
            InitBuffs(GaugeBuffAssets.PartsList);
            InitCooldowns(CooldownAssets.PartsList);
            InitCursor(CursorAssets.PartsList);

            UIHelper.Link(GaugeRoot, BuffRoot);
            UIHelper.Link(BuffRoot, CursorRoot);
        }

        public void Dispose() {
            if (GaugeRoot->NextSiblingNode != null && GaugeRoot->NextSiblingNode->PrevSiblingNode == GaugeRoot) {
                GaugeRoot->NextSiblingNode->PrevSiblingNode = null; // unlink
            }

            DisposeCooldowns();
            DisposeGauges();
            DisposeBuffs();
            DisposeCursor();
            DisposeTextures(); // dispose last

            var attachAddon = UIHelper.AttachAddon;
            if (attachAddon != null) attachAddon->UldManager.UpdateDrawNodeList();

            var partyListAddon = UIHelper.PartyListAddon;
            if (partyListAddon != null) partyListAddon->AtkUnitBase.UldManager.UpdateDrawNodeList();
        }

        public void Attach() {
            var attachAddon = UIHelper.AttachAddon;
            var partyListAddon = UIHelper.PartyListAddon;

            // ===== CONTAINERS =========

            GaugeRoot->ParentNode = attachAddon->RootNode;
            BuffRoot->ParentNode = attachAddon->RootNode;
            CursorRoot->ParentNode = attachAddon->RootNode;

            var lastNode = attachAddon->RootNode->ChildNode;
            while (lastNode->PrevSiblingNode != null) lastNode = lastNode->PrevSiblingNode;

            UIHelper.Link(lastNode, GaugeRoot);

            // ===== BUFF PARTYLIST ======

            for(var i = 0; i < PartyListBuffs.Count; i++) {
                var partyMember = partyListAddon->PartyMember[i];
                PartyListBuffs[i].AttachTo(partyMember.TargetGlowContainer);
                partyMember.PartyMemberComponent->UldManager.UpdateDrawNodeList();
            }

            // ===== COOLDOWNS =========

            CooldownRoot->ParentNode = partyListAddon->AtkUnitBase.RootNode;
            partyListAddon->AtkUnitBase.UldManager.NodeList[25]->PrevSiblingNode = CooldownRoot;

            // ======================

            attachAddon->UldManager.UpdateDrawNodeList();
            partyListAddon->AtkUnitBase.UldManager.UpdateDrawNodeList();
        }

        public void Tick(float percent) {
            Arrows.ForEach(x => x.Tick(percent));
            Diamonds.ForEach(x => x.Tick(percent));
        }

        // ==== HELPER FUNCTIONS ============

        private void SetPosition(AtkResNode* node, float X, float Y) {
            var addon = UIHelper.AttachAddon;
            if (addon == null) return;
            var p = UIHelper.GetNodePosition(addon->RootNode);
            var pScale = UIHelper.GetNodeScale(addon->RootNode);
            UIHelper.SetPosition(node, (X - p.X) / pScale.X, (Y - p.Y) / pScale.Y);
        }

        private void SetScale(AtkResNode* node, float X, float Y) {
            var addon = UIHelper.AttachAddon;
            if (addon == null) return;
            var p = UIHelper.GetNodeScale(addon->RootNode);
            UIHelper.SetScale(node, X / p.X, Y / p.Y);
        }
    }
}
