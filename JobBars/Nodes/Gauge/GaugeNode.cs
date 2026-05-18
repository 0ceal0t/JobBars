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

        public virtual unsafe void SetSplitPosition( GaugeRoot root, Vector2 pos ) {
            Position = new( pos.X - root.Position.X, pos.Y - root.Position.Y );
        }
    }
}
