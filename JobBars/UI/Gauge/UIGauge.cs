using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System.Numerics;

namespace JobBars.UI {
    public abstract unsafe class UIGauge : UIElement {
        public virtual void SetSplitPosition(Vector2 pos) {
            var p = UIHelper.GetNodePosition(JobBars.Builder.GaugeRoot);
            var pScale = UIHelper.GetNodeScale(JobBars.Builder.GaugeRoot);
            UIHelper.SetPosition(RootRes, (pos.X - p.X) / pScale.X, (pos.Y - p.Y) / pScale.Y);
        }

        public virtual void Cleanup() { }
    }
}