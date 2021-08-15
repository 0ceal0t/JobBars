using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Runtime.InteropServices;

namespace JobBars.Helper {
    public unsafe partial class UIHelper {
        public static void LoadIcon(AtkImageNode* node, int icon) {
            var partsList = CreatePartsList(1);
            node->PartsList = partsList;
            node->LoadIconTexture(icon, 0);
        }

        public static void UnloadIcon(AtkImageNode* node) {
            node->UnloadTexture();
            DisposePartsList(node->PartsList);
            node->PartsList = null;
        }

        public static AtkUldPartsList* CreatePartsList(uint partCount) {
            var partsList = (AtkUldPartsList*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkUldPartsList), 8);
            if (partsList == null) {
                PluginLog.Debug("Failed to allocate memory for parts list");
            }

            partsList->Id = 0;
            partsList->PartCount = partCount;

            var part = (AtkUldPart*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkUldPart) * partCount, 8);
            if (part == null) {
                PluginLog.Debug("Failed to allocate memory for part");
                IMemorySpace.Free(partsList, (ulong)sizeof(AtkUldPartsList));
                return null;
            }

            for (int i = 0; i < 1; i++) {
                part[i].U = 0;
                part[i].V = 0;
                part[i].Width = 24;
                part[i].Height = 32;

                var asset = (AtkUldAsset*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkUldAsset), 8);
                if (asset == null) {
                    PluginLog.Debug("Failed to allocate memory for asset");
                    IMemorySpace.Free(part, (ulong)sizeof(AtkUldPart) * partCount);
                    IMemorySpace.Free(partsList, (ulong)sizeof(AtkUldPartsList));
                    return null;
                }

                asset->Id = 0;
                asset->AtkTexture.Ctor();

                part[i].UldAsset = asset;
            }

            partsList->Parts = part;

            return partsList;
        }

        public static void DisposePartsList(AtkUldPartsList* partsList) {
            var partsCount = partsList->PartCount;

            for (int i = 0; i < partsCount; i++) {
                IMemorySpace.Free(partsList->Parts[i].UldAsset, (ulong)sizeof(AtkUldAsset));
            }
            IMemorySpace.Free(partsList->Parts, (ulong)sizeof(AtkUldPart) * partsCount);
            IMemorySpace.Free(partsList, (ulong)sizeof(AtkUldPartsList));
        }

        public static void UpdatePart(AtkUldPartsList* partsList, int idx, ushort u, ushort v, ushort width, ushort height) {
            partsList->Parts[idx].U = u;
            partsList->Parts[idx].V = v;
            partsList->Parts[idx].Width = width;
            partsList->Parts[idx].Height = height;
        }

        public static AtkUldPart* ExpandPartList(AtkUldManager manager, ushort addSize) {
            var oldLength = manager.PartsList->PartCount;
            var newLength = oldLength + addSize + 1;

            var oldSize = oldLength * 0x10;
            var newSize = newLength * 0x10;

            var part = (AtkUldPart*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkUldPart) * newLength, 8);
            var newListPtr = new IntPtr(part);
            var oldListPtr = new IntPtr(manager.PartsList->Parts);

            byte[] oldData = new byte[oldSize];
            Marshal.Copy(oldListPtr, oldData, 0, (int)oldSize);
            Marshal.Copy(oldData, 0, newListPtr, (int)oldSize);
            return (AtkUldPart*)newListPtr;
        }

        public static void AddPart(AtkUldManager manager, ushort assetIdx, ushort partIdx, ushort U, ushort V, ushort Width, ushort Height) {
            var asset = (AtkUldAsset*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkUldAsset), 8);
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
