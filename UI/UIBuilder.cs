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
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr LoadTextureDelegate(IntPtr a1, string path, uint a3);
        public LoadTextureDelegate LoadTexture;

        private AtkResNode* NewRes = null;
        private UIGauge Gauge;

        public UIBuilder(DalamudPluginInterface pi) {
            PluginInterface = pi;
            var scanner = PluginInterface.TargetModuleScanner;
            var loadTexAddr = scanner.ScanText("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 30 80 79 10 01 41 8B E8 48 8B FA 48 8B D9");
            LoadTexture = Marshal.GetDelegateForFunctionPointer<LoadTextureDelegate>(loadTexAddr);
        }

        public void Dispose() {
            if(NewRes != null) {
                RecurseHide(NewRes);
            }
        }

        private void RecurseHide(AtkResNode* node, bool hide = true) {
            if (hide) {
                UiHelper.Hide(node);
            }
            else {
                UiHelper.Show(node);
            }

            if (node->ChildNode != null) {
                RecurseHide(node->ChildNode, hide);
            }
            if (node->PrevSiblingNode != null) {
                RecurseHide(node->PrevSiblingNode, hide);
            }
        }

        public uint nodeIdx = 99990001;
        public AtkUnitBase* _ADDON => (AtkUnitBase*)PluginInterface?.Framework.Gui.GetUiObjectByName("_ParameterWidget", 1);

        public static ushort GAUGE_ASSET = 1;
        public static ushort BLUR_ASSET = 2;

        public static ushort GAUGE_BG_PART = 8;
        public static ushort GAUGE_FRAME_PART = 9;
        public static ushort GAUGE_TEXT_BLUR_PART = 10;
        public static ushort GAUGE_BAR_MAIN = 11;

        public void SetupTex() {
            var addon = _ADDON;
            if (addon->UldManager.NodeListCount > 4) return;

            LoadTex(GAUGE_ASSET, @"ui/uld/Parameter_Gauge.tex");
            AddPart(GAUGE_ASSET, GAUGE_BG_PART, 0, 100, 160, 20);
            AddPart(GAUGE_ASSET, GAUGE_FRAME_PART, 0, 0, 160, 20);
            AddPart(GAUGE_ASSET, GAUGE_BAR_MAIN, 0, 40, 160, 20);
            LoadTex(BLUR_ASSET, @"ui/uld/JobHudNumBg.tex");
            AddPart(BLUR_ASSET, GAUGE_TEXT_BLUR_PART, 0, 0, 60, 40);
        }

        public void PrintAllParts() {
            var addon = _ADDON;
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

        public void Pickup() {
            var addon = _ADDON;
            NewRes = addon->RootNode->ChildNode->PrevSiblingNode->PrevSiblingNode->PrevSiblingNode;
            RecurseHide(NewRes, false);
            // =======
            Gauge = new UIGauge(this, NewRes->ChildNode);
            Gauge.SetPercent(0.5f);
            Gauge.SetText("30");
            Gauge.SetColor(UIColor.Purple);
        }

        public void Init() {
            var nameplateAddon = _ADDON;
            if (nameplateAddon->UldManager.NodeListCount > 4) {
                Pickup();
                return;
            }

            Gauge = new UIGauge(this, null);

            NewRes = CreateResNode();
            NewRes->Width = 256;
            NewRes->Height = 100;
            NewRes->Flags = 9395;
            NewRes->ChildCount = (ushort)(Gauge.RootRes->ChildCount + 1);
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = NewRes;

            Gauge.RootRes->ParentNode = NewRes;
            NewRes->ChildNode = Gauge.RootRes;

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
    }
}
