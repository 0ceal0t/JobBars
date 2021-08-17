using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace JobBars.Helper {
    public unsafe struct Asset_PartList {
        public AtkUldAsset* Asset;
        public uint AssetCount;
        public AtkUldPartsList* PartsList;
    }

    public struct PartStruct {
        public ushort U;
        public ushort V;
        public ushort Width;
        public ushort Height;

        public PartStruct(ushort u, ushort v, ushort w, ushort h) {
            U = u;
            V = v;
            Width = w;
            Height = h;
        }
    }

    public unsafe partial class UIHelper {
        public static void LoadIcon(AtkImageNode* node, int icon) {
            var partsList = CreatePartsList(1);
            var assetList = CreateAssets(1);
            SetPartAsset(partsList, 0, assetList, 0);
            node->PartsList = partsList;
            node->LoadIconTexture(icon, 0);
        }

        public static void UnloadIcon(AtkImageNode* node) {
            node->UnloadTexture();
            DisposeIconPartsList(node->PartsList);
            node->PartsList = null;
        }

        public static void DisposeIconPartsList(AtkUldPartsList* partsList) {
            DisposeAsset(partsList->Parts[0].UldAsset, 1);
            DisposePartsList(partsList);
        }

        // ===========================

        public static Asset_PartList LoadLayout(Dictionary<string, PartStruct[]> layout) {
            var ret = new Asset_PartList();

            var partCount = 0;
            List<string> paths = new();
            foreach (var entry in layout) {
                paths.Add(entry.Key);
                partCount += entry.Value.Length;
            }
            ret.AssetCount = (uint) paths.Count;
            ret.Asset = CreateAssets(paths);

            ret.PartsList = CreatePartsList((uint)partCount);
            var partIdx = 0;
            var assetIdx = 0;
            foreach(var entry in layout) {
                foreach(var part in entry.Value) {
                    UpdatePart(ret.PartsList, partIdx, ret.Asset, assetIdx, part.U, part.V, part.Width, part.Height);
                    partIdx++;
                }
                assetIdx++;
            }

            return ret;
        }

        public static void DisposeLayout(Asset_PartList layout) {
            DisposeAsset(layout.Asset, layout.AssetCount);
            DisposePartsList(layout.PartsList);
        }

        // ==========================

        public static void DisposePartsList(AtkUldPartsList* partsList) {
            var partsCount = partsList->PartCount;
            IMemorySpace.Free(partsList->Parts, (ulong)sizeof(AtkUldPart) * partsCount);
            IMemorySpace.Free(partsList, (ulong)sizeof(AtkUldPartsList));
        }

        public static void DisposeAsset(AtkUldAsset* assets, uint assetCount) {
            IMemorySpace.Free(assets, (ulong)sizeof(AtkUldAsset) * assetCount);
        }

        public unsafe static AtkUldAsset* CreateAssets(List<string> paths) {
            var ret = CreateAssets((uint)paths.Count);
            for (int i = 0; i < paths.Count; i++) {
                TextureLoadPath((AtkTexture*)(new IntPtr(ret) + 0x20 * i + 0x8), paths[i], 1);
            }
            return ret;
        }

        public unsafe static AtkUldAsset* CreateAssets(uint assetCount) {
            var asset = (AtkUldAsset*)Alloc((ulong)sizeof(AtkUldAsset) * assetCount);

            for (int i = 0; i < assetCount; i++) {
                asset[i].AtkTexture.Ctor();
                asset[i].Id = (uint)(i + 1);
            }
            return asset;
        }

        public static AtkUldPartsList* CreatePartsList(uint partCount) {
            var partsList = Alloc<AtkUldPartsList>();
            if (partsList == null) {
                PluginLog.Debug("Failed to allocate memory for parts list");
            }

            partsList->Id = 1;
            partsList->PartCount = partCount;

            var part = (AtkUldPart*)Alloc((ulong)sizeof(AtkUldPart) * partCount);
            if (part == null) {
                PluginLog.Debug("Failed to allocate memory for part");
                IMemorySpace.Free(partsList, (ulong)sizeof(AtkUldPartsList));
                return null;
            }

            for (int i = 0; i < partCount; i++) {
                part[i].U = 0;
                part[i].V = 0;
                part[i].Width = 24;
                part[i].Height = 24;
            }

            partsList->Parts = part;

            return partsList;
        }

        public static void UpdatePart(AtkUldPartsList* partsList, int partIdx, AtkUldAsset* assets, int assetIdx, ushort u, ushort v, ushort width, ushort height) {
            SetPartAsset(partsList, partIdx, assets, assetIdx);
            UpdatePart(partsList, partIdx, u, v, width, height);
        }

        public static void UpdatePart(AtkUldPartsList* partsList, int partIdx, ushort u, ushort v, ushort width, ushort height) {
            partsList->Parts[partIdx].U = u;
            partsList->Parts[partIdx].V = v;
            partsList->Parts[partIdx].Width = width;
            partsList->Parts[partIdx].Height = height;
        }

        public static void SetPartAsset(AtkUldPartsList* partsList, int partIdx, AtkUldAsset* assets, int assetIdx) {
            partsList->Parts[partIdx].UldAsset = (AtkUldAsset*)(new IntPtr(assets) + assetIdx * sizeof(AtkUldAsset));
        }

        // ======= USED FOR LOADING TEXTURES INTO ADDON ==============

        public static AtkUldPart* ExpandPartList(AtkUldManager manager, ushort addSize) {
            var oldLength = manager.PartsList->PartCount;
            var newLength = oldLength + addSize + 1;

            var oldSize = oldLength * 0x10;
            var newSize = newLength * 0x10;

            var part = (AtkUldPart*)Alloc((ulong)sizeof(AtkUldPart) * newLength);
            var newListPtr = new IntPtr(part);
            var oldListPtr = new IntPtr(manager.PartsList->Parts);

            byte[] oldData = new byte[oldSize];
            Marshal.Copy(oldListPtr, oldData, 0, (int)oldSize);
            Marshal.Copy(oldData, 0, newListPtr, (int)oldSize);
            return (AtkUldPart*)newListPtr;
        }

        public static void AddPart(AtkUldManager manager, ushort assetIdx, ushort partIdx, ushort U, ushort V, ushort Width, ushort Height) {
            var asset = Alloc<AtkUldAsset>();
            asset->Id = manager.Assets[assetIdx].Id;
            asset->AtkTexture = manager.Assets[assetIdx].AtkTexture;

            var partsList = manager.PartsList;

            partsList->Parts[partIdx].UldAsset = asset;
            partsList->Parts[partIdx].U = U;
            partsList->Parts[partIdx].V = V;
            partsList->Parts[partIdx].Width = Width;
            partsList->Parts[partIdx].Height = Height;

            if ((partIdx + 1) > partsList->PartCount) {
                partsList->PartCount = (ushort)(partIdx + 1);
            }
        }
    }
}
