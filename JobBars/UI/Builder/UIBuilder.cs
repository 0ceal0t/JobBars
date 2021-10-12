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

            var addon = UIHelper.ChatLogAddon;
            if (addon == null) return;
            addon->UldManager.UpdateDrawNodeList();
        }

        public void Attach() {
            var chatAddon = UIHelper.ChatLogAddon;

            GaugeRoot->ParentNode = chatAddon->RootNode;
            BuffRoot->ParentNode = chatAddon->RootNode;
            CursorRoot->ParentNode = chatAddon->RootNode;

            var lastNode = chatAddon->RootNode->ChildNode;
            while (lastNode->PrevSiblingNode != null) lastNode = lastNode->PrevSiblingNode;

            UIHelper.Link(lastNode, GaugeRoot);
            chatAddon->UldManager.UpdateDrawNodeList();

            // ===== COOLDOWNS =========

            var partyListAddon = UIHelper.PartyListAddon;

            CooldownRoot->ParentNode = partyListAddon->AtkUnitBase.RootNode;
            partyListAddon->AtkUnitBase.UldManager.NodeList[21]->PrevSiblingNode = CooldownRoot;
            partyListAddon->AtkUnitBase.UldManager.UpdateDrawNodeList();
        }

        // ==== HELPER FUNCTIONS ============

        private void SetPosition(AtkResNode* node, float X, float Y) {
            var addon = UIHelper.ChatLogAddon;
            if (addon == null) return;
            var p = UIHelper.GetNodePosition(addon->RootNode);
            var pScale = UIHelper.GetNodeScale(addon->RootNode);
            UIHelper.SetPosition(node, (X - p.X) / pScale.X, (Y - p.Y) / pScale.Y);
        }

        private void SetScale(AtkResNode* node, float X, float Y) {
            var addon = UIHelper.ChatLogAddon;
            if (addon == null) return;
            var p = UIHelper.GetNodeScale(addon->RootNode);
            UIHelper.SetScale(node, X / p.X, Y / p.Y);
        }
    }
}
