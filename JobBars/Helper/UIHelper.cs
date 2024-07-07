using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Numerics;

namespace JobBars.Helper {
    public static unsafe partial class UiHelper {
        public static unsafe Vector2 GetGlobalPosition( AtkResNode* node ) {
            var pos = new Vector2( node->X, node->Y );
            var par = node->ParentNode;
            while( par != null ) {
                pos *= new Vector2( par->ScaleX, par->ScaleY );
                pos += new Vector2( par->X, par->Y );
                par = par->ParentNode;
            }
            return pos;
        }

        public static unsafe Vector2 GetGlobalScale( AtkResNode* node ) {
            if( node == null ) return new Vector2( 1, 1 );
            var scale = new Vector2( node->ScaleX, node->ScaleY );
            while( node->ParentNode != null ) {
                node = node->ParentNode;
                scale *= new Vector2( node->ScaleX, node->ScaleY );
            }
            return scale;
        }

        public static void Hide( AtkResNode* node ) {
            node->NodeFlags &= ~NodeFlags.Visible;
            node->DrawFlags |= 0x1;
        }

        public static void Show( AtkResNode* node ) {
            node->NodeFlags |= NodeFlags.Visible;
            node->DrawFlags |= 0x1;
        }

        public static void SetVisibility( AtkResNode* node, bool visiblity ) {
            if( visiblity ) Show( node );
            else Hide( node );
        }

        public static void Link( AtkResNode* next, AtkResNode* prev ) {
            if( next == null || prev == null ) return;
            next->PrevSiblingNode = prev;
            prev->NextSiblingNode = next;
        }

        public static void Detach( AtkResNode* node ) {
            if( node->NextSiblingNode != null && node->NextSiblingNode->PrevSiblingNode == node ) {
                node->NextSiblingNode->PrevSiblingNode = null; // unlink
            }
            node->NextSiblingNode = null;
        }
    }
}
