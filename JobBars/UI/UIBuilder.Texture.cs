using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        private void LoadAssets(string[] paths) { // is this kind of gross? yes. does it work? probably
            var numPaths = paths.Length;
            var addon = UIHelper.ParameterAddon;
            var oldAssets = addon->UldManager.Assets;

            var allocator = new IntPtr(IMemorySpace.GetUISpace());

            var assetMapping = UIHelper.LoadTexAlloc(allocator, 4 * numPaths, 16);
            for (int i = 0; i < numPaths; i++) {
                Marshal.WriteInt32(assetMapping + 4 * i, i + 1); // 0->1, 1->2, etc.
            }

            var manager = (IntPtr)addon + 0x28;
            var size = 16 + 56 * numPaths + 16; // a bit of extra padding on the end
            var texPaths = new IntPtr(UIHelper.Alloc((ulong)size));
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
    }
}