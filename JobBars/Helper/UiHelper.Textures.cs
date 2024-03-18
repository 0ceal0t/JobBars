using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;

namespace JobBars.Helper {
    public unsafe partial class AtkHelper {
        private struct TextureToLoadStruct {
            public bool IsIcon;
            public string Path;
            public int IconId;
            public AtkImageNode* Node;
        }

        private static readonly List<TextureToLoadStruct> NodesToLoad = [];

        public static void SetupIcon( AtkImageNode* node, int icon ) {
            AllocatePartList( node );
            NodesToLoad.Add( new TextureToLoadStruct { // Load this icon in the next framework tick
                IsIcon = true,
                IconId = icon,
                Node = node
            } );
        }

        public static void SetupTexture( AtkImageNode* node, string texture ) {
            AllocatePartList( node );
            NodesToLoad.Add( new TextureToLoadStruct { // Load this texture in the next framework tick
                IsIcon = false,
                Path = texture,
                Node = node
            } );
        }

        public static void SetupTexture( AtkNineGridNode* node, string texture ) => SetupTexture( ( AtkImageNode* )node, texture );

        private static void AllocatePartList( AtkImageNode* node ) {
            var partsList = CreatePartsList( 1 );
            var assetList = CreateAssets( 1 );
            SetPartAsset( partsList, 0, assetList, 0 );
            node->PartsList = partsList;
        }

        public static void UnloadTexture( AtkImageNode* node ) {
            node->UnloadTexture();
            DisposeAsset( node->PartsList->Parts[0].UldAsset, 1 );
            DisposePartsList( node->PartsList );
            node->PartsList = null;
        }

        public static void UnloadTexture( AtkNineGridNode* node ) => UnloadTexture( ( AtkImageNode* )node );

        public static void TickTextures() {
            if( TickTextureLoad() ) return; // Don't bother with the HD stuff if textures are still being loaded
        }

        private static bool TickTextureLoad() {
            if( NodesToLoad.Count == 0 ) return false;

            foreach( var node in NodesToLoad ) {
                if( node.IsIcon ) { // Load icon
                    node.Node->LoadIconTexture( node.IconId, 0 );
                }
                else { // Load texture path
                    var imageNode = node.Node;
                    var path = node.Path;

                    var version = JobBars.Configuration.Use4K ? 2u : 1u;
                    imageNode->LoadTexture( path, version );
                }
            }

            NodesToLoad.Clear();
            return true;
        }

        // ===============

        public static void DisposePartsList( AtkUldPartsList* partsList ) {
            var partsCount = partsList->PartCount;
            IMemorySpace.Free( partsList->Parts, ( ulong )sizeof( AtkUldPart ) * partsCount );
            IMemorySpace.Free( partsList, ( ulong )sizeof( AtkUldPartsList ) );
        }

        public static void DisposeAsset( AtkUldAsset* assets, uint assetCount ) {
            IMemorySpace.Free( assets, ( ulong )sizeof( AtkUldAsset ) * assetCount );
        }

        public static unsafe AtkUldAsset* CreateAssets( uint assetCount ) {
            var asset = ( AtkUldAsset* )Alloc( ( ulong )sizeof( AtkUldAsset ) * assetCount );

            for( var i = 0; i < assetCount; i++ ) {
                asset[i].AtkTexture.Ctor();
                asset[i].Id = ( uint )( i + 1 );
            }
            return asset;
        }

        public static AtkUldPartsList* CreatePartsList( uint partCount ) {
            var partsList = Alloc<AtkUldPartsList>();
            if( partsList == null ) {
                Dalamud.Error( "Failed to allocate memory for parts list" );
            }

            partsList->Id = 1;
            partsList->PartCount = partCount;

            var part = ( AtkUldPart* )Alloc( ( ulong )sizeof( AtkUldPart ) * partCount );
            if( part == null ) {
                Dalamud.Error( "Failed to allocate memory for part" );
                IMemorySpace.Free( partsList, ( ulong )sizeof( AtkUldPartsList ) );
                return null;
            }

            for( var i = 0; i < partCount; i++ ) {
                part[i].U = 0;
                part[i].V = 0;
                part[i].Width = 24;
                part[i].Height = 24;
            }

            partsList->Parts = part;

            return partsList;
        }

        public static void UpdatePart( AtkNineGridNode* node, ushort u, ushort v, ushort width, ushort height ) => UpdatePart( node->PartsList, 0, u, v, width, height );

        public static void UpdatePart( AtkImageNode* node, ushort u, ushort v, ushort width, ushort height ) => UpdatePart( node->PartsList, 0, u, v, width, height );

        public static void UpdatePart( AtkUldPartsList* partsList, int partIdx, AtkUldAsset* assets, int assetIdx, ushort u, ushort v, ushort width, ushort height ) {
            SetPartAsset( partsList, partIdx, assets, assetIdx );
            UpdatePart( partsList, partIdx, u, v, width, height );
        }

        public static void UpdatePart( AtkUldPartsList* partsList, int partIdx, ushort u, ushort v, ushort width, ushort height ) {
            partsList->Parts[partIdx].U = u;
            partsList->Parts[partIdx].V = v;
            partsList->Parts[partIdx].Width = width;
            partsList->Parts[partIdx].Height = height;
        }

        public static void SetPartAsset( AtkUldPartsList* partsList, int partIdx, AtkUldAsset* assets, int assetIdx ) {
            partsList->Parts[partIdx].UldAsset = ( AtkUldAsset* )( new IntPtr( assets ) + assetIdx * sizeof( AtkUldAsset ) );
        }
    }
}
