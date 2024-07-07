using System.Numerics;

namespace JobBars.Nodes.Gauge {
    public interface IGaugeNode {
        public void SetSplitPosition( Vector2 pos );
        public void SetVisible( bool visible );
        public void SetPosition( Vector2 pos );
        public void SetScale( float scale );
    }
}
