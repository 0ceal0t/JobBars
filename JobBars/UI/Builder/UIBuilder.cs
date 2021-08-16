using System;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        public static UIBuilder Builder { get; private set; } = null;

        public static void Initialize(DalamudPluginInterface pluginInterface) {
            Builder?.DisposeInstance();
            Builder = new UIBuilder(pluginInterface);
            Builder.InitializeInstance();
        }

        public static void Dispose() {
            Builder?.DisposeInstance();
            Builder = null;
        }

        // ===== INSTANCE =======

        public DalamudPluginInterface PluginInterface;

        private static readonly uint NODE_IDX_START = 89990001;
        private uint NodeIdx = NODE_IDX_START;

        private UIBuilder(DalamudPluginInterface pi) {
            PluginInterface = pi;
        }

        private void InitializeInstance() {
            var addon = UIHelper.ParameterAddon;
            if (addon == null || addon->UldManager.Assets == null || addon->UldManager.PartsList == null) {
                PluginLog.Debug("Error setting up UI builder");
                return;
            }
            if (addon->UldManager.AssetCount == 1) {
                SetupTex();
                SetupPart(addon);
            }
            Init(addon);
        }

        private void DisposeInstance() {
            GaugeRoot->NextSiblingNode->PrevSiblingNode = null; // unlink
            DisposeCooldowns();
            DisposeGauges();
            DisposeBuffs();

            var addon = UIHelper.ParameterAddon;
            if (addon == null) return;
            addon->UldManager.UpdateDrawNodeList();

            if (PluginInterface.ClientState?.LocalPlayer == null) { // game closing, remove the orphaned assets
                PluginLog.Log("==== UNLOADING TEXTURES =======");
                for (int i = 0; i < addon->UldManager.AssetCount; i++) {
                    UIHelper.TexUnalloc(new IntPtr(addon->UldManager.Assets) + 0x8 + 0x20 * i);
                }
            }
        }

        private void Init(AtkUnitBase* addon) {
            NodeIdx = NODE_IDX_START;

            InitGauges(addon);
            InitBuffs(addon);
            InitCooldowns();

            // ==== INSERT AT THE END ====
            var lastNode = addon->RootNode->ChildNode;
            while (lastNode->PrevSiblingNode != null) {
                lastNode = lastNode->PrevSiblingNode;
            }

            UIHelper.Link(lastNode, GaugeRoot);
            UIHelper.Link(GaugeRoot, BuffRoot);

            addon->UldManager.UpdateDrawNodeList();
        }

        // ==== HELPER FUNCTIONS ============

        private void SetPosition(AtkResNode* node, float X, float Y) {
            var addon = UIHelper.ParameterAddon;
            var p = UIHelper.GetNodePosition(addon->RootNode);
            var pScale = UIHelper.GetNodeScale(addon->RootNode);
            UIHelper.SetPosition(node, (X - p.X) / pScale.X, (Y - p.Y) / pScale.Y);
        }

        private void SetScale(AtkResNode* node, float X, float Y) {
            var addon = UIHelper.ParameterAddon;
            var p = UIHelper.GetNodeScale(addon->RootNode);
            UIHelper.SetScale(node, X / p.X, Y / p.Y);
        }
    }
}
