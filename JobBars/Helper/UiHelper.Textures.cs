using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.System.Resource.Handle;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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
            DisposeAsset(node->PartsList->Parts[0].UldAsset, 1);
            DisposePartsList(node->PartsList);
            node->PartsList = null;
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
            ret.AssetCount = (uint)paths.Count;
            ret.Asset = CreateAssets(paths);

            ret.PartsList = CreatePartsList((uint)partCount);
            var partIdx = 0;
            var assetIdx = 0;
            foreach (var entry in layout) {
                foreach (var part in entry.Value) {
                    UpdatePart(ret.PartsList, partIdx, ret.Asset, assetIdx, part.U, part.V, part.Width, part.Height);
                    partIdx++;
                }
                assetIdx++;
            }

            return ret;
        }

        public static void DisposeLayout(Asset_PartList layout) {
            for (var i = 0; i < layout.AssetCount; i++) {
                layout.Asset[i].AtkTexture.ReleaseTexture();
            }
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
                var hd = JobBars.Config.Use4K ? (byte)2 : (byte)1;

                var tex = (AtkTexture*)(new IntPtr(ret) + 0x20 * i + 0x8);
                var path = paths[i];

                // get mapping to some file in necessary (icon.tex -> C:/mods/icon.tex)
                // could also be icon.tex -> icon.tex
                var resolvedPath = GetResolvedPath(JobBars.Config.Use4K ? path.Replace(".tex", "_hr1.tex") : path);
                PluginLog.Log($"Resolved {path} -> {resolvedPath}");

                if (Path.IsPathRooted(resolvedPath)) {
                    TextureLoadPath(tex, resolvedPath, 1); // don't want to re-apply _hr1
                    Marshal.WriteByte(new IntPtr(tex->Resource) + 0x1a, hd);
                }
                else {
                    TextureLoadPath(tex, path, hd);
                }
            }
            return ret;
        }

        private unsafe static string GetResolvedPath(string texPath) {
            var pathBytes = Encoding.ASCII.GetBytes(texPath);
            var bPath = stackalloc byte[pathBytes.Length + 1];
            Marshal.Copy(pathBytes, 0, new IntPtr(bPath), pathBytes.Length);
            var pPath = (char*)bPath;

            var typeBytes = Encoding.ASCII.GetBytes("xet");
            var bType = stackalloc byte[typeBytes.Length + 1];
            Marshal.Copy(typeBytes, 0, new IntPtr(bType), typeBytes.Length);
            var pResourceType = (char*)bType;

            // TODO: might need to change this based on path
            var categoryBytes = BitConverter.GetBytes((uint)6);
            var bCategory = stackalloc byte[categoryBytes.Length + 1];
            Marshal.Copy(categoryBytes, 0, new IntPtr(bCategory), categoryBytes.Length);
            var pCategoryId = (uint*)bCategory;

            Crc32.Init();
            Crc32.Update(pathBytes);
            var hashBytes = BitConverter.GetBytes(Crc32.Checksum);
            var bHash = stackalloc byte[hashBytes.Length + 1];
            Marshal.Copy(hashBytes, 0, new IntPtr(bHash), hashBytes.Length);
            var pResourceHash = (uint*)bHash;

            var resource = (TextureResourceHandle*) GetResourceSync(GetFileManager(), pCategoryId, pResourceType, pResourceHash, pPath, (void*)IntPtr.Zero);
            var resolvedPath = resource->ResourceHandle.FileName.ToString();
            resource->ResourceHandle.DecRef(); // not actually using this

            PluginLog.Log($"RefCount {texPath} {resource->ResourceHandle.RefCount}");

            return resolvedPath;
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
    }
}
