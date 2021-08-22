using System;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        private static readonly uint NODE_IDX_START = 89990001;
        private uint NodeIdx = NODE_IDX_START;

        public void Initialize() {
            var addon = UIHelper.ChatLogAddon;
            if (addon == null || addon->UldManager.Assets == null || addon->UldManager.PartsList == null) {
                PluginLog.Debug("Error setting up UI builder");
                return;
            }
            Init(addon);
        }

        public void Dispose() {
            if(GaugeRoot->NextSiblingNode != null && GaugeRoot->NextSiblingNode->PrevSiblingNode == GaugeRoot) {
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

        private void Init(AtkUnitBase* addon) {
            NodeIdx = NODE_IDX_START;

            InitTextures(); // init first
            InitGauges(GaugeBuffAssets.PartsList);
            InitBuffs(GaugeBuffAssets.PartsList);
            InitCooldowns(CooldownAssets.PartsList);
            InitCursor(CursorAssets.PartsList);

            GaugeRoot->ParentNode = addon->RootNode;
            BuffRoot->ParentNode = addon->RootNode;
            CursorRoot->ParentNode = addon->RootNode;
            UIHelper.Link(GaugeRoot, BuffRoot);
            UIHelper.Link(BuffRoot, CursorRoot);

            Attach(addon);
        }

        public void Attach() => Attach(UIHelper.ChatLogAddon);
        public void Attach(AtkUnitBase* addon) {
            // ==== INSERT AT THE END ====
            var lastNode = addon->RootNode->ChildNode;
            while (lastNode->PrevSiblingNode != null) {
                lastNode = lastNode->PrevSiblingNode;
            }

            UIHelper.Link(lastNode, GaugeRoot);
            addon->UldManager.UpdateDrawNodeList();

            AttachCooldown();
        }

        // ==== HELPER FUNCTIONS ============

        private void SetPosition(AtkResNode* node, float X, float Y) {
            var addon = UIHelper.ChatLogAddon;
            var p = UIHelper.GetNodePosition(addon->RootNode);
            var pScale = UIHelper.GetNodeScale(addon->RootNode);
            UIHelper.SetPosition(node, (X - p.X) / pScale.X, (Y - p.Y) / pScale.Y);
        }

        private void SetScale(AtkResNode* node, float X, float Y) {
            var addon = UIHelper.ChatLogAddon;
            var p = UIHelper.GetNodeScale(addon->RootNode);
            UIHelper.SetScale(node, X / p.X, Y / p.Y);
        }
    }
}
