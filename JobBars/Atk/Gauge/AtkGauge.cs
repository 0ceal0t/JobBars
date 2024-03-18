using JobBars.Helper;
using System.Numerics;

namespace JobBars.Atk {
    public abstract unsafe class AtkGauge : AtkElement {
        public virtual void SetSplitPosition( Vector2 pos ) {
            var p = AtkHelper.GetNodePosition( JobBars.Builder.GaugeRoot );
            var pScale = AtkHelper.GetNodeScale( JobBars.Builder.GaugeRoot );
            AtkHelper.SetPosition( RootRes, ( pos.X - p.X ) / pScale.X, ( pos.Y - p.Y ) / pScale.Y );
        }

        public virtual void Cleanup() { }
    }
}