using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        public DalamudPluginInterface PluginInterface;
        public UIIconManager Icon;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr LoadTextureDelegate(IntPtr a1, string path, uint a3);
        public LoadTextureDelegate LoadTexture;

        private AtkResNode* NewRes = null;
        private static int MAX_GAUGES = 4;
        public UIGauge[] Gauges;
        public UIArrow[] Arrows;

        public UIBuilder(DalamudPluginInterface pi) {
            PluginInterface = pi;
            var scanner = PluginInterface.TargetModuleScanner;
            var loadTexAddr = scanner.ScanText("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 30 80 79 10 01 41 8B E8 48 8B FA 48 8B D9");
            LoadTexture = Marshal.GetDelegateForFunctionPointer<LoadTextureDelegate>(loadTexAddr);
            Gauges = new UIGauge[MAX_GAUGES];
            Arrows = new UIArrow[MAX_GAUGES];
            Icon = new UIIconManager(pi);
        }

        public void Dispose() {
            Icon.Dispose();
            if(NewRes != null) {
                RecurseHide(NewRes);
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

        public static ushort ASSET_START = 1;
        public static ushort PART_START = 8;

        public static ushort GAUGE_ASSET = ASSET_START;
        public static ushort BLUR_ASSET = (ushort)(ASSET_START + 1);
        public static ushort GAUGE_BG_PART = PART_START;
        public static ushort GAUGE_FRAME_PART = (ushort)(PART_START + 1);
        public static ushort GAUGE_TEXT_BLUR_PART = (ushort)(PART_START + 2);
        public static ushort GAUGE_BAR_MAIN = (ushort)(PART_START + 3);

        public static ushort ARROW_ASSET = (ushort)(ASSET_START + 2);
        public static ushort ARROW_BG = (ushort)(PART_START + 4);
        public static ushort ARROW_FG = (ushort)(PART_START + 5);

        public void SetupTex() {
            var addon = _ADDON;
            if (addon->UldManager.NodeListCount > 4) return;
            UiHelper.ExpandNodeList(addon, 999);

            LoadTex(GAUGE_ASSET, @"ui/uld/Parameter_Gauge.tex");
            AddPart(GAUGE_ASSET, GAUGE_BG_PART, 0, 100, 160, 20);
            AddPart(GAUGE_ASSET, GAUGE_FRAME_PART, 0, 0, 160, 20);
            AddPart(GAUGE_ASSET, GAUGE_BAR_MAIN, 0, 40, 160, 20);
            LoadTex(BLUR_ASSET, @"ui/uld/JobHudNumBg.tex");
            AddPart(BLUR_ASSET, GAUGE_TEXT_BLUR_PART, 0, 0, 60, 40);
            LoadTex(ARROW_ASSET, @"ui/uld/JobHudSimple_StackB.tex");
            AddPart(ARROW_ASSET, ARROW_BG, 0, 0, 32, 32);
            AddPart(ARROW_ASSET, ARROW_FG, 32, 0, 32, 32);
        }

        public void PrintAllParts() {
            var addon = (AtkUnitBase*)PluginInterface?.Framework.Gui.GetUiObjectByName("JobHudDRG0", 1);
            for (int i = 0; i < addon->UldManager.PartsListCount; i++) {
                var list = addon->UldManager.PartsList[i];
                for (int j = 0; j < list.PartCount; j++) {
                    var p = list.Parts[j];
                    var textPtr = p.UldAsset->AtkTexture.Resource->TexFileResourceHandle->ResourceHandle.FileName;
                    var texString = Marshal.PtrToStringAnsi(new IntPtr(textPtr));
                    PluginLog.Log($"LIST {i} PART {j}: {p.U} {p.V} {p.Width} {p.Height} / {texString}");
                }
            }
        }

        public void LoadExisting() {
            PluginLog.Log("==== LOAD EXISTING =====");

            var addon = _ADDON;
            NewRes = addon->RootNode->ChildNode->PrevSiblingNode->PrevSiblingNode->PrevSiblingNode;
            RecurseHide(NewRes, false);
            // =======
            var n = NewRes->ChildNode;
            for(int idx = 0; idx < MAX_GAUGES; idx++) {
                Gauges[idx] = new UIGauge(this, n);
                n = n->PrevSiblingNode;
                Arrows[idx] = new UIArrow(this, n);
                n = n->PrevSiblingNode;
            }
        }

        public void Init() {
            var nameplateAddon = _ADDON;
            if (nameplateAddon->UldManager.NodeListCount > 4) {
                LoadExisting();
                return;
            }

            for(int idx = 0; idx < MAX_GAUGES; idx++) {
                Gauges[idx] = new UIGauge(this, null);
                Arrows[idx] = new UIArrow(this, null);
            }

            NewRes = CreateResNode();
            NewRes->Width = 256;
            NewRes->Height = 100;
            NewRes->Flags = 9395;
            NewRes->ChildCount = (ushort)(Arrows[0].RootRes->ChildCount * MAX_GAUGES + Gauges[0].RootRes->ChildCount * MAX_GAUGES + 2 * MAX_GAUGES);
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = NewRes;

            NewRes->ChildNode = Gauges[0].RootRes;
            for(int idx = 0; idx < MAX_GAUGES; idx++) {
                Gauges[idx].RootRes->ParentNode = NewRes;
                Arrows[idx].RootRes->ParentNode = NewRes;

                Gauges[idx].RootRes->PrevSiblingNode = Arrows[idx].RootRes;
                Arrows[idx].RootRes->NextSiblingNode = Gauges[idx].RootRes;
                if(idx < (MAX_GAUGES - 1)) {
                    Arrows[idx].RootRes->PrevSiblingNode = Gauges[idx + 1].RootRes;
                    Gauges[idx + 1].RootRes->NextSiblingNode = Arrows[idx].RootRes;
                }
            }

            SetPosition(1500, 500); // TEMP
            SetScale(1, 1);

            // ==== INSERT AT THE END ====
            var n = nameplateAddon->RootNode->ChildNode;
            while (n->PrevSiblingNode != null) {
                n = n->PrevSiblingNode;
            }
            n->PrevSiblingNode = NewRes;
        }

        public void SetPosition(float X, float Y) {
            var addon = _ADDON;
            var p = UiHelper.GetNodePosition(_ADDON->RootNode);
            UiHelper.SetPosition(NewRes, X - p.X, Y - p.Y);
        }

        public void SetScale(float X, float Y) {
            var addon = _ADDON;
            var p = UiHelper.GetNodeScale(_ADDON->RootNode);
            UiHelper.SetScale(NewRes, X / p.X, Y / p.Y);
        }

        public void HideAllGauges() {
            foreach(var g in Gauges) {
                RecurseHide(g.RootRes);
            }
            foreach (var g in Arrows) {
                RecurseHide(g.RootRes);
            }
        }

        public void Show(UIElement element) {
            RecurseHide(element.RootRes, false, false);
        }
    }
}
