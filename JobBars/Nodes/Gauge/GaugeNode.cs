using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using KamiToolKit;
using System.Numerics;

namespace JobBars.Nodes.Gauge {
    public abstract class GaugeNode : NodeBase<AtkResNode>, IGaugeNode {
        public GaugeNode() : base( NodeType.Res ) { }

        public void SetPosition( Vector2 pos ) {
            Position = pos;
        }

        public void SetScale( float scale ) {
            Scale = new( scale, scale );
        }

        public void SetVisible( bool visible ) {
            IsVisible = visible;
        }

        public virtual unsafe void SetSplitPosition( Vector2 pos ) {
            var p = UiHelper.GetGlobalPosition( JobBars.NodeBuilder.GaugeRoot.Node );
            var pScale = UiHelper.GetGlobalScale( JobBars.NodeBuilder.GaugeRoot.Node );
            Position = new( ( pos.X - p.X ) / pScale.X, ( pos.Y - p.Y ) / pScale.Y );
        }
    }
}
