using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Numerics;

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

        public AtkResNode* GaugeRoot = null;
        public List<UIElement> Gauges = new();

        private AtkResNode* BuffRoot = null;
        public List<UIBuff> Buffs = new();
        private static IconIds[] Icons => (IconIds[])Enum.GetValues(typeof(IconIds));
        public Dictionary<IconIds, UIBuff> IconToBuff = new();

        private static readonly uint NODE_IDX_START = 90000;
        private uint NodeIdx = NODE_IDX_START;

        private static readonly ushort ASSET_START = 1;
        private static readonly ushort PART_START = 7;

        public static readonly ushort GAUGE_ASSET = ASSET_START;
        public static readonly ushort BLUR_ASSET = (ushort)(ASSET_START + 1);
        public static readonly ushort ARROW_ASSET = (ushort)(ASSET_START + 2);
        public static readonly ushort DIAMOND_ASSET = (ushort)(ASSET_START + 3);
        public static readonly ushort BUFF_OVERLAY_ASSET = (ushort)(ASSET_START + 4);

        public static readonly ushort GAUGE_BG_PART = PART_START;
        public static readonly ushort GAUGE_FRAME_PART = (ushort)(PART_START + 1);
        public static readonly ushort GAUGE_TEXT_BLUR_PART = (ushort)(PART_START + 2);
        public static readonly ushort GAUGE_BAR_MAIN = (ushort)(PART_START + 3);
        public static readonly ushort ARROW_BG = (ushort)(PART_START + 4);
        public static readonly ushort ARROW_FG = (ushort)(PART_START + 5);
        public static readonly ushort DIAMOND_BG = (ushort)(PART_START + 6);
        public static readonly ushort DIAMOND_FG = (ushort)(PART_START + 7);
        public static readonly ushort BUFF_BORDER = (ushort)(PART_START + 8);
        public static readonly ushort BUFF_OVERLAY = (ushort)(PART_START + 9);

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

        public void AppendGauge(UIGaugeElement element) => Append(Gauges, element, GaugeRoot);
        private void Append(List<UIElement> list, UIElement element, AtkResNode* root) {
            if (element == null) return;

            list.Add(element);
            root->ChildCount += (ushort)(element.RootRes->ChildCount + 1);
            element.RootRes->ParentNode = root;
            UIHelper.Link(element.RootRes, root->ChildNode);
            root->ChildNode = element.RootRes;
            UIHelper.ParameterAddon->UldManager.UpdateDrawNodeList();
        }

        private void Remove(List<UIElement> list, UIElement element, AtkResNode* root) {
            if (element == null) return;
            if (!list.Contains(element)) return;

            list.Remove(element);

            root->ChildCount -= (ushort)(element.RootRes->ChildCount + 1);
            if (root->ChildNode == element.RootRes) root->ChildNode = list.Count > 0 ? list[0].RootRes : null;

            var prev = element.RootRes->PrevSiblingNode;
            var next = element.RootRes->NextSiblingNode;
            if (prev != null) prev->NextSiblingNode = null;
            if (next != null) next->PrevSiblingNode = null;
            UIHelper.Link(next, prev);

            element.Dispose();

            UIHelper.ParameterAddon->UldManager.UpdateDrawNodeList();
        }

        public void ResetGauges() {
            Gauges.ForEach(x => x.Dispose());
            Gauges.Clear();
            GaugeRoot->ChildCount = 0;
            GaugeRoot->ChildNode = null;
            UIHelper.ParameterAddon->UldManager.UpdateDrawNodeList();
        }

        private void DisposeInstance() {
            GaugeRoot->NextSiblingNode->PrevSiblingNode = null; // unlink

            Gauges.ForEach(x => x.Dispose());
            Gauges = null;

            Buffs.ForEach(x => x.Dispose());
            Buffs = null;

            GaugeRoot->Destroy(true);
            GaugeRoot = null;

            BuffRoot->Destroy(true);
            BuffRoot = null;

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

        // ==== HELPER FUNCTIONS ============
        public void SetGaugePosition(Vector2 pos) {
            SetPosition(GaugeRoot, pos.X, pos.Y);
        }

        public void SetGaugeVisible(bool visible) {
            UIHelper.SetVisibility(GaugeRoot, visible);
        }

        public void SetBuffPosition(Vector2 pos) {
            SetPosition(BuffRoot, pos.X, pos.Y);
        }

        private void SetPosition(AtkResNode* node, float X, float Y) {
            var addon = UIHelper.ParameterAddon;
            var p = UIHelper.GetNodePosition(addon->RootNode);
            var pScale = UIHelper.GetNodeScale(addon->RootNode);
            UIHelper.SetPosition(node, (X - p.X) / pScale.X, (Y - p.Y) / pScale.Y);
        }

        public void SetGaugeScale(float scale) {
            SetScale(GaugeRoot, scale, scale);
        }

        public void SetBuffScale(float scale) {
            SetScale(BuffRoot, scale, scale);
        }

        private void SetScale(AtkResNode* node, float X, float Y) {
            var addon = UIHelper.ParameterAddon;
            var p = UIHelper.GetNodeScale(addon->RootNode);
            UIHelper.SetScale(node, X / p.X, Y / p.Y);
        }

        public void ShowBuffs() {
            UIHelper.Show(BuffRoot);
        }

        public void HideBuffs() {
            UIHelper.Hide(BuffRoot);
        }

        public void HideAllBuffs() {
            Buffs.ForEach(x => x.Hide());
        }
    }
}
