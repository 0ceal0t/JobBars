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
        public DalamudPluginInterface PluginInterface;

        public AtkResNode* G_RootRes = null;
        private static readonly int MAX_GAUGES = 4;
        public List<UIGauge> Gauges;
        public List<UIArrow> Arrows;
        public List<UIDiamond> Diamonds;

        private AtkResNode* B_RootRes = null;
        public List<UIBuff> Buffs;
        private static IconIds[] Icons => (IconIds[])Enum.GetValues(typeof(IconIds));
        public Dictionary<IconIds, UIBuff> IconToBuff = new();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr LoadTexDelegate(IntPtr uldManager, IntPtr allocator, IntPtr texPaths, IntPtr assetMapping, uint assetNumber, byte a6);
        public LoadTexDelegate LoadTex;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr LoadTexAllocDelegate(IntPtr allocator, Int64 size, UInt64 a3);
        public LoadTexAllocDelegate LoadTexAlloc;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr TexUnallocDelegate(IntPtr tex);
        public TexUnallocDelegate TexUnalloc;

        public AtkUnitBase* ADDON => (AtkUnitBase*)PluginInterface?.Framework.Gui.GetUiObjectByName("_ParameterWidget", 1);

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

        public UIBuilder(DalamudPluginInterface pi) {
            PluginInterface = pi;

            var loadAssetsPtr = PluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 48 8B 84 24 ?? ?? ?? ?? 41 B9 ?? ?? ?? ??");
            LoadTex = Marshal.GetDelegateForFunctionPointer<LoadTexDelegate>(loadAssetsPtr);

            var loadTexAllocPtr = PluginInterface.TargetModuleScanner.ScanText("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8B 01 49 8B D8 48 8B FA 48 8B F1 FF 50 48");
            LoadTexAlloc = Marshal.GetDelegateForFunctionPointer<LoadTexAllocDelegate>(loadTexAllocPtr);

            var texUnallocPtr = PluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? C6 43 10 02");
            TexUnalloc = Marshal.GetDelegateForFunctionPointer<TexUnallocDelegate>(texUnallocPtr);

            Gauges = new();
            Arrows = new();
            Diamonds = new();
            Buffs = new();
        }

        public void Dispose() {
            G_RootRes->NextSiblingNode->PrevSiblingNode = null; // unlink

            Gauges.ForEach(x => x.Dispose());
            Gauges = null;

            Arrows.ForEach(x => x.Dispose());
            Arrows = null;

            Diamonds.ForEach(x => x.Dispose());
            Diamonds = null;

            Buffs.ForEach(x => x.Dispose());
            Buffs = null;

            G_RootRes->Destroy(true);
            G_RootRes = null;

            B_RootRes->Destroy(true);
            B_RootRes = null;

            var addon = ADDON;
            if (addon == null) return;
            addon->UldManager.UpdateDrawNodeList();

            if (PluginInterface.ClientState?.LocalPlayer == null) { // game closing, remove the orphaned assets
                for (int i = 0; i < addon->UldManager.AssetCount; i++) {
                    TexUnalloc(new IntPtr(addon->UldManager.Assets) + 0x8 + 0x20 * i);
                }
            }
        }

        public bool Setup() {
            var addon = ADDON;
            if (addon == null || addon->UldManager.Assets == null || addon->UldManager.PartsList == null) return false;
            if(addon->UldManager.AssetCount == 1) {
                SetupTex();
                SetupPart();
            }
            Init();

            return true;
        }

        private void LoadAssets(string[] paths) { // is this kind of gross? yes. does it work? probably
            var numPaths = paths.Length;
            var addon = ADDON;
            AtkUldAsset* oldAssets = addon->UldManager.Assets;

            var allocator = UiHelper.GetGameAllocator();

            var assetMapping = LoadTexAlloc(allocator, 4 * numPaths, 16);
            for (int i = 0; i < numPaths; i++) {
                Marshal.WriteInt32(assetMapping + 4 * i, i + 1); // 0->1, 1->2, etc.
            }

            IntPtr manager = (IntPtr)addon + 0x28;
            var size = 16 + 56 * numPaths + 16; // a bit of extra padding on the end
            IntPtr texPaths = UiHelper.Alloc(size);
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
            LoadTex(manager, allocator, texPaths, assetMapping, (uint)numPaths, 1);

            for (int i = 0; i < 7; i++) { // _ParameterGauges is nice because it only has 1 PartList and 1 Asset
                addon->UldManager.PartsList[0].Parts[i].UldAsset = addon->UldManager.Assets; // reset all the assets
            }

            addon->UldManager.LoadedState = 3; // maybe reset this after the parts are set up? idk
            TexUnalloc(new IntPtr(oldAssets) + 0x8); // unallocate the old AtkTexture
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

        private void SetupPart() {
            PluginLog.Log("LOADING PARTS");
            var addon = ADDON;
            var manager = addon->UldManager;
            addon->UldManager.PartsList->Parts = UiHelper.ExpandPartList(addon->UldManager, 99);
            UiHelper.AddPart(manager, GAUGE_ASSET, GAUGE_BG_PART, 0, 100, 160, 20);
            UiHelper.AddPart(manager, GAUGE_ASSET, GAUGE_FRAME_PART, 0, 0, 160, 20);
            UiHelper.AddPart(manager, GAUGE_ASSET, GAUGE_BAR_MAIN, 0, 40, 160, 20);
            UiHelper.AddPart(manager, BLUR_ASSET, GAUGE_TEXT_BLUR_PART, 0, 0, 60, 40);
            UiHelper.AddPart(manager, ARROW_ASSET, ARROW_BG, 0, 0, 32, 32);
            UiHelper.AddPart(manager, ARROW_ASSET, ARROW_FG, 32, 0, 32, 32);
            UiHelper.AddPart(manager, DIAMOND_ASSET, DIAMOND_BG, 0, 0, 32, 32);
            UiHelper.AddPart(manager, DIAMOND_ASSET, DIAMOND_FG, 32, 0, 32, 32);
            UiHelper.AddPart(manager, BUFF_OVERLAY_ASSET, BUFF_BORDER, 252, 12, 47, 47);
            UiHelper.AddPart(manager, BUFF_OVERLAY_ASSET, BUFF_OVERLAY, 365, 4, 37, 37);
        }

        private void Init() {
            var addon = ADDON;
            NodeIdx = NODE_IDX_START;

            // ======== CREATE GAUGES =======
            G_RootRes = CreateResNode();
            G_RootRes->Width = 256;
            G_RootRes->Height = 100;
            G_RootRes->Flags = 9395;
            G_RootRes->Flags_2 = 4;

            UIDiamond lastDiamond = null;
            for (int idx = 0; idx < MAX_GAUGES; idx++) {
                var newGauge = new UIGauge(this);
                var newArrow = new UIArrow(this);
                var newDiamond = new UIDiamond(this);

                Gauges.Add(newGauge);
                Arrows.Add(newArrow);
                Diamonds.Add(newDiamond);

                newGauge.RootRes->ParentNode = G_RootRes;
                newArrow.RootRes->ParentNode = G_RootRes;
                newDiamond.RootRes->ParentNode = G_RootRes;

                UiHelper.Link(newGauge.RootRes, newArrow.RootRes);
                UiHelper.Link(newArrow.RootRes, newDiamond.RootRes);

                if(lastDiamond != null) UiHelper.Link(lastDiamond.RootRes, newGauge.RootRes);
                lastDiamond = newDiamond;
            }

            G_RootRes->ParentNode = addon->RootNode;
            G_RootRes->ChildCount = (ushort)(MAX_GAUGES * (
                Arrows[0].RootRes->ChildCount + 1 +
                Gauges[0].RootRes->ChildCount + 1 +
                Diamonds[0].RootRes->ChildCount + 1
            ));
            G_RootRes->ChildNode = Gauges[0].RootRes;

            // ======= CREATE BUFFS =========
            B_RootRes = CreateResNode();
            B_RootRes->Width = 256;
            B_RootRes->Height = 100;
            B_RootRes->Flags = 9395;
            B_RootRes->Flags_2 = 4;
            B_RootRes->ParentNode = addon->RootNode;

            UIBuff lastBuff = null;
            foreach(var icon in Icons) {
                var newBuff = new UIBuff(this, (int)icon);

                Buffs.Add(newBuff);
                IconToBuff[icon] = newBuff;

                if (lastBuff != null) UiHelper.Link(lastBuff.RootRes, newBuff.RootRes);
                lastBuff = newBuff;
            }

            B_RootRes->ChildCount = (ushort)(5 * Buffs.Count);
            B_RootRes->ChildNode = Buffs[0].RootRes;

            // ==== INSERT AT THE END ====
            var lastNode = addon->RootNode->ChildNode;
            while (lastNode->PrevSiblingNode != null) {
                lastNode = lastNode->PrevSiblingNode;
            }

            UiHelper.Link(lastNode, G_RootRes);
            UiHelper.Link(G_RootRes, B_RootRes);

            addon->UldManager.UpdateDrawNodeList();
        }

        // ==== HELPER FUNCTIONS ============
        public void SetGaugePosition(Vector2 pos) {
            SetPosition(G_RootRes, pos.X, pos.Y);
        }

        public void SetBuffPosition(Vector2 pos) {
            SetPosition(B_RootRes, pos.X, pos.Y);
        }

        private void SetPosition(AtkResNode* node, float X, float Y) {
            var addon = ADDON;
            var p = UiHelper.GetNodePosition(ADDON->RootNode);
            var pScale = UiHelper.GetNodeScale(ADDON->RootNode);
            UiHelper.SetPosition(node, (X - p.X) / pScale.X, (Y - p.Y) / pScale.Y);
        }

        public void SetGaugeScale(float scale) {
            SetScale(G_RootRes, scale, scale);
        }

        public void SetBuffScale(float scale) {
            SetScale(B_RootRes, scale, scale);
        }

        private void SetScale(AtkResNode* node, float X, float Y) {
            var addon = ADDON;
            var p = UiHelper.GetNodeScale(ADDON->RootNode);
            UiHelper.SetScale(node, X / p.X, Y / p.Y);
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
            UiHelper.Show(G_RootRes);
        }

        public void ShowBuffs() {
            UiHelper.Show(B_RootRes);
        }

        public void HideGauges() {
            UiHelper.Hide(G_RootRes);
        }

        public void HideBuffs() {
            UiHelper.Hide(B_RootRes);
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
