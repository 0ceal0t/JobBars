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
    public unsafe partial class UIHelper {
        private static readonly Dictionary<string, string> ResolvedPaths = new();
        private static readonly List<IntPtr> NodesToHD = new();

        public static void LoadIcon(AtkImageNode* node, int icon) {
            SetupTexture(node);
            node->LoadIconTexture(icon, 0);
        }

        public static void LoadTexture(AtkImageNode* node, string texture) {
            SetupTexture(node);

            var version = JobBars.Config.Use4K ? 2u : 1u;

            // get mapping to some file in necessary (icon.tex -> C:/mods/icon.tex)
            // could also be icon.tex -> icon.tex

            var resolvedPath = texture;
            if(!ResolvedPaths.TryGetValue(texture, out resolvedPath)) {
                resolvedPath = GetResolvedPath(JobBars.Config.Use4K ? texture.Replace(".tex", "_hr1.tex") : texture).ToLower();
                PluginLog.Log($"Resolved {texture} -> {resolvedPath}");
                ResolvedPaths.Add(texture, resolvedPath);
            }

            if (Path.IsPathRooted(resolvedPath)) {
                node->LoadTexture(resolvedPath, 1); // don't want to re-apply _hr1
                if (JobBars.Config.Use4K) NodesToHD.Add(new IntPtr(node));
            }
            else {
                node->LoadTexture(texture, version); // don't use the resolved path or else we'll end up with _hr1_hr1
            }
        }

        public static void LoadTexture(AtkNineGridNode* node, string texture) => LoadTexture((AtkImageNode*)node, texture);

        private static void SetupTexture(AtkImageNode* node) {
            var partsList = CreatePartsList(1);
            var assetList = CreateAssets(1);
            SetPartAsset(partsList, 0, assetList, 0);
            node->PartsList = partsList;
        }

        public static void UnloadTexture(AtkImageNode* node) {
            node->UnloadTexture();
            DisposeAsset(node->PartsList->Parts[0].UldAsset, 1);
            DisposePartsList(node->PartsList);
            node->PartsList = null;
        }

        public static void UnloadTexture(AtkNineGridNode* node) => UnloadTexture((AtkImageNode*)node);

        public static void TickTextures() {
            if (NodesToHD.Count == 0) return;

            var newNodes = new List<IntPtr>();
            foreach(var node in NodesToHD) {
                var imageNode = (AtkImageNode*)node;
                if (imageNode->PartsList == null) {
                    newNodes.Add(node);
                    continue;
                }
                if (imageNode->PartsList->Parts == null) {
                    newNodes.Add(node);
                    continue;
                }
                if (imageNode->PartsList->Parts[0].UldAsset == null) {
                    newNodes.Add(node);
                    continue;
                }

                if (!imageNode->PartsList->Parts[0].UldAsset->AtkTexture.IsTextureReady()) {
                    newNodes.Add(node);
                    continue;
                }

                var tex = imageNode->PartsList->Parts[0].UldAsset->AtkTexture.Resource;
                Marshal.WriteByte(new IntPtr(tex) + 0x1a, 2);
            }

            NodesToHD.Clear();
            NodesToHD.AddRange(newNodes);
        }

        // ===============

        public static void DisposePartsList(AtkUldPartsList* partsList) {
            var partsCount = partsList->PartCount;
            IMemorySpace.Free(partsList->Parts, (ulong)sizeof(AtkUldPart) * partsCount);
            IMemorySpace.Free(partsList, (ulong)sizeof(AtkUldPartsList));
        }

        public static void DisposeAsset(AtkUldAsset* assets, uint assetCount) {
            IMemorySpace.Free(assets, (ulong)sizeof(AtkUldAsset) * assetCount);
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
            if (resource != null && resource->ResourceHandle.RefCount > 0) resource->ResourceHandle.DecRef();

            PluginLog.Log($"+ RefCount {resolvedPath} {resource->ResourceHandle.RefCount}");

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
