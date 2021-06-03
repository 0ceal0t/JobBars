using Dalamud.Hooking;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.GameStructs;
using JobBars.Gauges;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        public DalamudPluginInterface PluginInterface;
        public UIIconManager Icon;

        private AtkResNode* G_RootRes = null;
        private static int MAX_GAUGES = 4;
        public UIGauge[] Gauges;
        public UIArrow[] Arrows;
        public UIDiamond[] Diamonds;

        private AtkResNode* B_RootRes = null;
        public UIBuff[] Buffs;
        private IconIds[] _Icons => (IconIds[])Enum.GetValues(typeof(IconIds));
        public Dictionary<IconIds, UIBuff> IconToBuff = new Dictionary<IconIds, UIBuff>();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr LoadTexDelegate(IntPtr uldManager, IntPtr allocator, IntPtr texPaths, IntPtr assetMapping, uint assetNumber, byte a6);
        public LoadTexDelegate LoadTex;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr LoadTexAllocDelegate(IntPtr allocator, Int64 size, UInt64 a3);
        public LoadTexAllocDelegate LoadTexAlloc;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr TexUnallocDelegate(IntPtr tex);
        public TexUnallocDelegate TexUnalloc;

        public UIBuilder(DalamudPluginInterface pi) {
            PluginInterface = pi;

            IntPtr loadAssetsPtr = PluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 48 8B 84 24 ?? ?? ?? ?? 41 B9 ?? ?? ?? ??");
            LoadTex = Marshal.GetDelegateForFunctionPointer<LoadTexDelegate>(loadAssetsPtr);

            IntPtr loadTexAllocPtr = PluginInterface.TargetModuleScanner.ScanText("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8B 01 49 8B D8 48 8B FA 48 8B F1 FF 50 48");
            LoadTexAlloc = Marshal.GetDelegateForFunctionPointer<LoadTexAllocDelegate>(loadTexAllocPtr);

            IntPtr texUnallocPtr = PluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? C6 43 10 02");
            TexUnalloc = Marshal.GetDelegateForFunctionPointer<TexUnallocDelegate>(texUnallocPtr);

            Gauges = new UIGauge[MAX_GAUGES];
            Arrows = new UIArrow[MAX_GAUGES];
            Diamonds = new UIDiamond[MAX_GAUGES];
            Buffs = new UIBuff[_Icons.Length];
            Icon = new UIIconManager(pi);
        }

        public void Dispose() {
            Icon?.Dispose();

            if (G_RootRes != null) {
                UiHelper.Hide(G_RootRes);
            }
            if (B_RootRes != null) {
                UiHelper.Hide(B_RootRes);
            }

            if(PluginInterface.ClientState?.LocalPlayer == null) { // game closing
                var addon = _ADDON;
                for (int i = 0; i < addon->UldManager.AssetCount; i++) {
                    TexUnalloc(new IntPtr(addon->UldManager.Assets) + 0x8 + 0x20 * i);
                }
            }
        }

        public static void RecurseHide(AtkResNode* node, bool hide = true, bool siblings = true) {
            if (hide) {
                UiHelper.Hide(node);
            }
            else {
                UiHelper.Show(node);
            }

            if (node->ChildNode != null) {
                RecurseHide(node->ChildNode, hide);
            }
            if (siblings && node->PrevSiblingNode != null) {
                RecurseHide(node->PrevSiblingNode, hide);
            }
        }

        public uint nodeIdx = 99990001;
        public AtkUnitBase* _ADDON => (AtkUnitBase*)PluginInterface?.Framework.Gui.GetUiObjectByName("_ParameterWidget", 1);
        public bool IsInitialized() {
            var addon = _ADDON;
            if(addon != null && addon->UldManager.NodeListCount > 4) {
                return true;
            }
            return false;
        }

        public static ushort ASSET_START = 1;
        public static ushort PART_START = 7;

        public static ushort GAUGE_ASSET = ASSET_START;
        public static ushort BLUR_ASSET = (ushort)(ASSET_START + 1);
        public static ushort ARROW_ASSET = (ushort)(ASSET_START + 2);
        public static ushort DIAMOND_ASSET = (ushort)(ASSET_START + 3);
        public static ushort BUFF_OVERLAY_ASSET = (ushort)(ASSET_START + 4);
        public static ushort BUFF_ASSET_START = (ushort)(ASSET_START + 5);

        public static ushort GAUGE_BG_PART = PART_START;
        public static ushort GAUGE_FRAME_PART = (ushort)(PART_START + 1);
        public static ushort GAUGE_TEXT_BLUR_PART = (ushort)(PART_START + 2);
        public static ushort GAUGE_BAR_MAIN = (ushort)(PART_START + 3);
        public static ushort ARROW_BG = (ushort)(PART_START + 4);
        public static ushort ARROW_FG = (ushort)(PART_START + 5);
        public static ushort DIAMOND_BG = (ushort)(PART_START + 6);
        public static ushort DIAMOND_FG = (ushort)(PART_START + 7);
        public static ushort BUFF_BORDER = (ushort)(PART_START + 8);
        public static ushort BUFF_OVERLAY = (ushort)(PART_START + 9);
        public static ushort BUFF_PART_START = (ushort)(PART_START + 10);

        public Dictionary<IconIds, ushort> IconToPartId = new Dictionary<IconIds, ushort>();


        public void LoadAssets(string[] paths) { // is this kind of gross? yes. does it work? probably
            var numPaths = paths.Length;
            var addon = _ADDON;
            AtkUldAsset* oldAssets = addon->UldManager.Assets;

            var allocator = UiHelper._getGameAllocator();
            var assetMapping = LoadTexAlloc(allocator, 4 * numPaths, 16);
            for(int i = 0; i < numPaths; i++) {
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

            for(int i = 0; i < numPaths; i++) {
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

        public void SetupTex() {
            var addon = _ADDON;
            if (addon->UldManager.NodeListCount > 4) return;

            List<string> assets = new List<string>();
            assets.Add("ui/uld/Parameter_Gauge.tex"); // existing asset
            assets.Add("ui/uld/Parameter_Gauge.tex");
            assets.Add("ui/uld/JobHudNumBg.tex");
            assets.Add("ui/uld/JobHudSimple_StackB.tex");
            assets.Add("ui/uld/JobHudSimple_StackA.tex");
            assets.Add("ui/uld/IconA_Frame.tex");
            foreach (var icon in _Icons) {
                var _icon = (uint)icon;
                var path = string.Format("ui/icon/{0:D3}000/{1}{2:D6}.tex", _icon / 1000, "", _icon);
                assets.Add(path);
            }
            LoadAssets(assets.ToArray());
        }

        public void SetupPart() {
            var addon = _ADDON;
            if (addon->UldManager.NodeListCount > 4) return;

            addon->UldManager.PartsList->Parts = UiHelper.ExpandPartList(addon->UldManager, 99);
            AddPart(GAUGE_ASSET, GAUGE_BG_PART, 0, 100, 160, 20);
            AddPart(GAUGE_ASSET, GAUGE_FRAME_PART, 0, 0, 160, 20);
            AddPart(GAUGE_ASSET, GAUGE_BAR_MAIN, 0, 40, 160, 20);
            AddPart(BLUR_ASSET, GAUGE_TEXT_BLUR_PART, 0, 0, 60, 40);
            AddPart(ARROW_ASSET, ARROW_BG, 0, 0, 32, 32);
            AddPart(ARROW_ASSET, ARROW_FG, 32, 0, 32, 32);
            AddPart(DIAMOND_ASSET, DIAMOND_BG, 0, 0, 32, 32);
            AddPart(DIAMOND_ASSET, DIAMOND_FG, 32, 0, 32, 32);
            AddPart(BUFF_OVERLAY_ASSET, BUFF_BORDER, 252, 12, 47, 47);
            AddPart(BUFF_OVERLAY_ASSET, BUFF_OVERLAY, 365, 4, 37, 37);

            var current_asset = BUFF_ASSET_START;
            var current_part = BUFF_PART_START;
            foreach (var icon in _Icons) {
                AddPart(current_asset, current_part, (ushort)((40 - UIBuff.WIDTH) / 2), (ushort)((40 - UIBuff.HEIGHT) / 2), UIBuff.WIDTH, UIBuff.HEIGHT);
                IconToPartId[icon] = current_part;
                current_asset++;
                current_part++;
            }
        }

        // JUST LOAD EVERYTHING INTO PARTLIST #0, I DON'T CARE LMAO
        public void AddPart(ushort assetIdx, ushort partIdx, ushort U, ushort V, ushort Width, ushort Height) {
            var addon = _ADDON;

            var asset = UiHelper.CleanAlloc<AtkUldAsset>();
            asset->Id = addon->UldManager.Assets[assetIdx].Id;
            asset->AtkTexture = addon->UldManager.Assets[assetIdx].AtkTexture;

            addon->UldManager.PartsList->Parts[partIdx].UldAsset = asset;
            addon->UldManager.PartsList->Parts[partIdx].U = U;
            addon->UldManager.PartsList->Parts[partIdx].V = V;
            addon->UldManager.PartsList->Parts[partIdx].Width = Width;
            addon->UldManager.PartsList->Parts[partIdx].Height = Height;

            if ((partIdx + 1) > addon->UldManager.PartsList->PartCount) {
                addon->UldManager.PartsList->PartCount = (ushort)(partIdx + 1);
            }
        }

        public void LoadExisting() {
            PluginLog.Log("===== LOAD EXISTING =====");

            var addon = _ADDON;
            // ===== LOAD EXISTING GAUGES =====
            G_RootRes = addon->RootNode->ChildNode->PrevSiblingNode->PrevSiblingNode->PrevSiblingNode;
            UiHelper.Show(G_RootRes);

            var n = G_RootRes->ChildNode;
            for(int idx = 0; idx < MAX_GAUGES; idx++) {
                Gauges[idx] = new UIGauge(this, n);
                n = n->PrevSiblingNode;
                Arrows[idx] = new UIArrow(this, n);
                n = n->PrevSiblingNode;
                Diamonds[idx] = new UIDiamond(this, n);
                n = n->PrevSiblingNode;
            }
            // ====== LOAD EXISTING BUFFS =======
            B_RootRes = G_RootRes->PrevSiblingNode;
            UiHelper.Show(B_RootRes);

            var n2 = B_RootRes->ChildNode;
            for(int idx = 0; idx < Buffs.Length; idx++) {
                Buffs[idx] = new UIBuff(this, 0, n2);
                var icon = _Icons[idx];
                IconToBuff[icon] = Buffs[idx];
                n2 = n2->PrevSiblingNode;
            }
        }

        public void Init() {
            var addon = _ADDON;
            if (addon->UldManager.NodeListCount > 4) {
                LoadExisting();
                return;
            }

            UiHelper.ExpandNodeList(addon, 999);
            // ======== CREATE GAUGES =======
            G_RootRes = CreateResNode();
            G_RootRes->Width = 256;
            G_RootRes->Height = 100;
            G_RootRes->Flags = 9395;
            addon->UldManager.NodeList[addon->UldManager.NodeListCount++] = G_RootRes;
            for (int idx = 0; idx < MAX_GAUGES; idx++) {
                Gauges[idx] = new UIGauge(this, null);
                Arrows[idx] = new UIArrow(this, null);
                Diamonds[idx] = new UIDiamond(this, null);
            }
            G_RootRes->ParentNode = addon->RootNode;
            G_RootRes->ChildCount = (ushort)(
                Arrows[0].RootRes->ChildCount * MAX_GAUGES + 
                Gauges[0].RootRes->ChildCount * MAX_GAUGES + 
                Diamonds[0].RootRes->ChildCount * MAX_GAUGES +
                3 * MAX_GAUGES
            );
            G_RootRes->ChildNode = Gauges[0].RootRes;

            for(int idx = 0; idx < MAX_GAUGES; idx++) {
                Gauges[idx].RootRes->ParentNode = G_RootRes;
                Arrows[idx].RootRes->ParentNode = G_RootRes;
                Diamonds[idx].RootRes->ParentNode = G_RootRes;

                Gauges[idx].RootRes->PrevSiblingNode = Arrows[idx].RootRes;
                Arrows[idx].RootRes->PrevSiblingNode = Diamonds[idx].RootRes;

                Arrows[idx].RootRes->NextSiblingNode = Gauges[idx].RootRes;
                Diamonds[idx].RootRes->NextSiblingNode = Arrows[idx].RootRes;

                if(idx < (MAX_GAUGES - 1)) {
                    Diamonds[idx].RootRes->PrevSiblingNode = Gauges[idx + 1].RootRes;
                    Gauges[idx + 1].RootRes->NextSiblingNode = Diamonds[idx].RootRes;
                }
            }
            SetGaugePosition(Configuration.Config.GaugePosition);
            SetGaugeScale(Configuration.Config.GaugeScale);

            // ======= CREATE BUFFS =========
            B_RootRes = CreateResNode();
            B_RootRes->Width = 256;
            B_RootRes->Height = 100;
            B_RootRes->Flags = 9395;
            B_RootRes->ParentNode = addon->RootNode;
            addon->UldManager.NodeList[addon->UldManager.NodeListCount++] = B_RootRes;
            int bIdx = 0;
            foreach (var entry in IconToPartId) {
                Buffs[bIdx] = new UIBuff(this, entry.Value, null);
                IconToBuff[entry.Key] = Buffs[bIdx];
                bIdx++;
            }
            B_RootRes->ChildCount = (ushort)(5 * Buffs.Length);
            B_RootRes->ChildNode = Buffs[0].RootRes;

            for (int idx = 0; idx < Buffs.Length; idx++) {
                Buffs[idx].RootRes->ParentNode = B_RootRes;

                if (idx < (Buffs.Length - 1)) {
                    Buffs[idx].RootRes->PrevSiblingNode = Buffs[idx + 1].RootRes;
                    Buffs[idx + 1].RootRes->NextSiblingNode = Buffs[idx].RootRes;
                }
            }
            SetBuffPosition(Configuration.Config.BuffPosition);
            SetBuffScale(Configuration.Config.BuffScale);

            // ==== INSERT AT THE END ====
            var n = addon->RootNode->ChildNode;
            while (n->PrevSiblingNode != null) {
                n = n->PrevSiblingNode;
            }
            n->PrevSiblingNode = G_RootRes;
            G_RootRes->PrevSiblingNode = B_RootRes;
        }

        // ==== HELPER FUNCTIONS ============
        public void SetGaugePosition(Vector2 pos) {
            SetPosition(G_RootRes, pos.X, pos.Y);
        }
        public void SetBuffPosition(Vector2 pos) {
            SetPosition(B_RootRes, pos.X, pos.Y);
        }
        public void SetPosition(AtkResNode* node, float X, float Y) {
            var addon = _ADDON;
            var p = UiHelper.GetNodePosition(_ADDON->RootNode);
            var pScale = UiHelper.GetNodeScale(_ADDON->RootNode);
            UiHelper.SetPosition(node, (X - p.X) / pScale.X, (Y - p.Y) / pScale.Y);
        }
        public void SetGaugeScale(float scale) {
            SetScale(G_RootRes, scale, scale);
        }
        public void SetBuffScale(float scale) {
            SetScale(B_RootRes, scale, scale);
        }
        public void SetScale(AtkResNode* node, float X, float Y) {
            var addon = _ADDON;
            var p = UiHelper.GetNodeScale(_ADDON->RootNode);
            UiHelper.SetScale(node, X / p.X, Y / p.Y);
        }
        public void HideAllGauges() {
            foreach (var gauge in Gauges) {
                gauge.Hide();
            }

            foreach (var arrow in Arrows) {
                arrow.Hide();
            }

            foreach (var diamond in Diamonds) {
                diamond.Hide();
            }
        }
        public void HideAllBuffs() {
            foreach(var buff in Buffs) {
                buff.Hide();
            }
        }
    }
}
