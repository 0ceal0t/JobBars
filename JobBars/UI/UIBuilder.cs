using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

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
        private static readonly int MAX_GAUGES = 4;
        public List<UIGauge> Gauges = new();
        public List<UIArrow> Arrows = new();
        public List<UIDiamond> Diamonds = new();

        private AtkResNode* BuffRoot = null;
        public List<UIBuff> Buffs = new();
        private static IconIds[] Icons => (IconIds[])Enum.GetValues(typeof(IconIds));
        public Dictionary<IconIds, UIBuff> IconToBuff = new();

        private static readonly uint NODE_IDX_START = 89990001;
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
            var addon = UIHelper.Addon;
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

            Gauges.ForEach(x => x.Dispose());
            Gauges = null;

            Arrows.ForEach(x => x.Dispose());
            Arrows = null;

            Diamonds.ForEach(x => x.Dispose());
            Diamonds = null;

            Buffs.ForEach(x => x.Dispose());
            Buffs = null;

            GaugeRoot->Destroy(true);
            GaugeRoot = null;

            BuffRoot->Destroy(true);
            BuffRoot = null;

            var addon = UIHelper.Addon;
            if (addon == null) return;
            addon->UldManager.UpdateDrawNodeList();

            if (PluginInterface.ClientState?.LocalPlayer == null) { // game closing, remove the orphaned assets
                PluginLog.Log("==== UNLOADING TEXTURES =======");
                for (int i = 0; i < addon->UldManager.AssetCount; i++) {
                    UIHelper.TexUnalloc(new IntPtr(addon->UldManager.Assets) + 0x8 + 0x20 * i);
                }
            }
        }

        private void LoadAssets(string[] paths) { // is this kind of gross? yes. does it work? probably
            var numPaths = paths.Length;
            var addon = UIHelper.Addon;
            AtkUldAsset* oldAssets = addon->UldManager.Assets;

            var allocator = UIHelper.GetGameAllocator();

            var assetMapping = UIHelper.LoadTexAlloc(allocator, 4 * numPaths, 16);
            for (int i = 0; i < numPaths; i++) {
                Marshal.WriteInt32(assetMapping + 4 * i, i + 1); // 0->1, 1->2, etc.
            }

            IntPtr manager = (IntPtr)addon + 0x28;
            var size = 16 + 56 * numPaths + 16; // a bit of extra padding on the end
            IntPtr texPaths = UIHelper.Alloc(size);
            var data = new byte[size];

            var start = Encoding.ASCII.GetBytes("ashd0101");
            Buffer.BlockCopy(start, 0, data, 0, start.Length);
            var numData = BitConverter.GetBytes(numPaths);
            Buffer.BlockCopy(numData, 0, data, 8, 4); // count

            for (int i = 0; i < numPaths; i++) {
                var startAddr = 16 + 56 * i;
                var pathId = BitConverter.GetBytes(i + 1);
                Buffer.BlockCopy(pathId, 0, data, startAddr, 4);
                var text = Encoding.ASCII.GetBytes(paths[i]);
                Buffer.BlockCopy(text, 0, data, startAddr + 4, text.Length);
            }

            var end = Encoding.ASCII.GetBytes("tphd0100"); // idk if this is necessary, but whatever
            Buffer.BlockCopy(end, 0, data, 16 + 56 * numPaths, end.Length);

            Marshal.Copy(data, 0, texPaths, size);
            UIHelper.LoadTex(manager, allocator, texPaths, assetMapping, (uint)numPaths, 1);

            for (int i = 0; i < 7; i++) { // _ParameterGauges is nice because it only has 1 PartList and 1 Asset
                addon->UldManager.PartsList[0].Parts[i].UldAsset = addon->UldManager.Assets; // reset all the assets
            }

            addon->UldManager.LoadedState = 3; // maybe reset this after the parts are set up? idk
            UIHelper.TexUnalloc(new IntPtr(oldAssets) + 0x8); // unallocate the old AtkTexture
        }

        private void SetupTex() {
            PluginLog.Log("LOADING TEXTURES");
            List<string> assets = new();
            assets.Add("ui/uld/Parameter_Gauge.tex"); // existing asset
            assets.Add("ui/uld/Parameter_Gauge.tex");
            assets.Add("ui/uld/JobHudNumBg.tex");
            assets.Add("ui/uld/JobHudSimple_StackB.tex");
            assets.Add("ui/uld/JobHudSimple_StackA.tex");
            assets.Add("ui/uld/IconA_Frame.tex");
            LoadAssets(assets.ToArray());
        }

        private void SetupPart(AtkUnitBase* addon) {
            PluginLog.Log("LOADING PARTS");
            var manager = addon->UldManager;
            addon->UldManager.PartsList->Parts = UIHelper.ExpandPartList(addon->UldManager, 99);
            UIHelper.AddPart(manager, GAUGE_ASSET, GAUGE_BG_PART, 0, 100, 160, 20);
            UIHelper.AddPart(manager, GAUGE_ASSET, GAUGE_FRAME_PART, 0, 0, 160, 20);
            UIHelper.AddPart(manager, GAUGE_ASSET, GAUGE_BAR_MAIN, 0, 40, 160, 20);
            UIHelper.AddPart(manager, BLUR_ASSET, GAUGE_TEXT_BLUR_PART, 0, 0, 60, 40);
            UIHelper.AddPart(manager, ARROW_ASSET, ARROW_BG, 0, 0, 32, 32);
            UIHelper.AddPart(manager, ARROW_ASSET, ARROW_FG, 32, 0, 32, 32);
            UIHelper.AddPart(manager, DIAMOND_ASSET, DIAMOND_BG, 0, 0, 32, 32);
            UIHelper.AddPart(manager, DIAMOND_ASSET, DIAMOND_FG, 32, 0, 32, 32);
            UIHelper.AddPart(manager, BUFF_OVERLAY_ASSET, BUFF_BORDER, 252, 12, 47, 47);
            UIHelper.AddPart(manager, BUFF_OVERLAY_ASSET, BUFF_OVERLAY, 365, 4, 37, 37);
        }

        private void Init(AtkUnitBase* addon) {
            NodeIdx = NODE_IDX_START;

            // ======== CREATE GAUGES =======
            GaugeRoot = CreateResNode();
            GaugeRoot->Width = 256;
            GaugeRoot->Height = 100;
            GaugeRoot->Flags = 9395;
            GaugeRoot->Flags_2 = 4;

            UIDiamond lastDiamond = null;
            for (int idx = 0; idx < MAX_GAUGES; idx++) {
                var newGauge = new UIGauge(addon);
                var newArrow = new UIArrow(addon);
                var newDiamond = new UIDiamond(addon);

                Gauges.Add(newGauge);
                Arrows.Add(newArrow);
                Diamonds.Add(newDiamond);

                newGauge.RootRes->ParentNode = GaugeRoot;
                newArrow.RootRes->ParentNode = GaugeRoot;
                newDiamond.RootRes->ParentNode = GaugeRoot;

                UIHelper.Link(newGauge.RootRes, newArrow.RootRes);
                UIHelper.Link(newArrow.RootRes, newDiamond.RootRes);

                if (lastDiamond != null) UIHelper.Link(lastDiamond.RootRes, newGauge.RootRes);
                lastDiamond = newDiamond;
            }

            GaugeRoot->ParentNode = addon->RootNode;
            GaugeRoot->ChildCount = (ushort)(MAX_GAUGES * (
                Arrows[0].RootRes->ChildCount + 1 +
                Gauges[0].RootRes->ChildCount + 1 +
                Diamonds[0].RootRes->ChildCount + 1
            ));
            GaugeRoot->ChildNode = Gauges[0].RootRes;

            // ======= CREATE BUFFS =========
            BuffRoot = CreateResNode();
            BuffRoot->Width = 256;
            BuffRoot->Height = 100;
            BuffRoot->Flags = 9395;
            BuffRoot->Flags_2 = 4;
            BuffRoot->ParentNode = addon->RootNode;

            UIBuff lastBuff = null;
            foreach (var icon in Icons) {
                var newBuff = new UIBuff(addon, (int)icon);

                Buffs.Add(newBuff);
                IconToBuff[icon] = newBuff;

                if (lastBuff != null) UIHelper.Link(lastBuff.RootRes, newBuff.RootRes);
                lastBuff = newBuff;
            }

            BuffRoot->ChildCount = (ushort)(5 * Buffs.Count);
            BuffRoot->ChildNode = Buffs[0].RootRes;

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
        public void SetGaugePosition(Vector2 pos) {
            SetPosition(GaugeRoot, pos.X, pos.Y);
        }

        public void SetBuffPosition(Vector2 pos) {
            SetPosition(BuffRoot, pos.X, pos.Y);
        }

        private void SetPosition(AtkResNode* node, float X, float Y) {
            var addon = UIHelper.Addon;
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
            var addon = UIHelper.Addon;
            var p = UIHelper.GetNodeScale(addon->RootNode);
            UIHelper.SetScale(node, X / p.X, Y / p.Y);
        }

        public void Hide() {
            HideGauges();
            HideBuffs();
        }

        public void Show() {
            ShowGauges();
            ShowBuffs();
        }

        public void ShowGauges() {
            UIHelper.Show(GaugeRoot);
        }

        public void ShowBuffs() {
            UIHelper.Show(BuffRoot);
        }

        public void HideGauges() {
            UIHelper.Hide(GaugeRoot);
        }

        public void HideBuffs() {
            UIHelper.Hide(BuffRoot);
        }

        public void HideAllGauges() {
            Gauges.ForEach(x => x.Hide());
            Arrows.ForEach(x => x.Hide());
            Diamonds.ForEach(x => x.Hide());
        }

        public void HideAllBuffs() {
            Buffs.ForEach(x => x.Hide());
        }
    }
}
