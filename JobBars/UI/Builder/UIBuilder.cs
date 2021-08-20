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

        private DalamudPluginInterface PluginInterface;
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
            Init(addon);
        }

        private void DisposeInstance() {
            if(GaugeRoot->NextSiblingNode != null && GaugeRoot->NextSiblingNode->PrevSiblingNode == GaugeRoot) {
                GaugeRoot->NextSiblingNode->PrevSiblingNode = null; // unlink
            }
            DisposeCooldowns();
            DisposeGauges();
            DisposeBuffs();
            DisposeTextures(); // dispose last

            var addon = UIHelper.ParameterAddon;
            if (addon == null) return;
            addon->UldManager.UpdateDrawNodeList();
        }

        private void Init(AtkUnitBase* addon) {
            NodeIdx = NODE_IDX_START;

            InitTextures(); // init first
            InitGauges(addon, GaugeBuffAssets.PartsList);
            InitBuffs(addon, GaugeBuffAssets.PartsList);
            InitCooldowns(CooldownAssets.PartsList);

            UIHelper.Link(GaugeRoot, BuffRoot);
            Attach(addon);
        }

        public void Attach() => Attach(UIHelper.ParameterAddon);
        public void Attach(AtkUnitBase* parameterAddon) {
            // ==== INSERT AT THE END ====
            var lastNode = parameterAddon->RootNode->ChildNode;
            while (lastNode->PrevSiblingNode != null) {
                lastNode = lastNode->PrevSiblingNode;
            }

            UIHelper.Link(lastNode, GaugeRoot);
            parameterAddon->UldManager.UpdateDrawNodeList();

            AttachCooldown();
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
