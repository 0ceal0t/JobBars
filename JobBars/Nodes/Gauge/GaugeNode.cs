using KamiToolKit.Premade.Node.Simple;
using System.Numerics;

namespace JobBars.Nodes.Gauge {
    public abstract class GaugeNode : SimpleOverlayNode, IGaugeNode {
        public GaugeNode() { }

        public void SetPosition( Vector2 pos ) {
            Position = pos;
        }

        public void SetScale( float scale ) {
            Scale = new( scale, scale );
        }

        public void SetVisible( bool visible ) {
            IsVisible = visible;
        }

        public virtual void SetSplitPosition( GaugeRoot root, Vector2 pos ) {
            Position = new( pos.X - root.Position.X, pos.Y - root.Position.Y );
        }
    }
}
