using JobBars.Helper;
using KamiToolKit;
using System.Numerics;

namespace JobBars.Nodes.Builder {
    public partial class NodeBuilder {
        // ==== HELPER FUNCTIONS ============

        public static void SetPositionGlobal( NodeBase node, Vector2 v ) => SetPosition( node, v.X, v.Y );

        public static unsafe void SetPosition( NodeBase node, float x, float y ) {
            var addon = UiHelper.BuffGaugeAttachAddon;
            if( addon == null || node == null ) return;
            var p = UiHelper.GetGlobalPosition( addon->RootNode );
            var scale = UiHelper.GetGlobalScale( addon->RootNode );
            node.Position = new( ( x - p.X ) / scale.X, ( y - p.Y ) / scale.Y );
        }

        public static void SetScaleGlobal( NodeBase node, float v ) => SetScale( node, v, v );

        public static unsafe void SetScale( NodeBase node, float x, float y ) {
            var addon = UiHelper.BuffGaugeAttachAddon;
            if( addon == null || node == null ) return;
            var p = UiHelper.GetGlobalScale( addon->RootNode );
            node.Scale = new( x / p.X, y / p.Y );
        }
    }
}
