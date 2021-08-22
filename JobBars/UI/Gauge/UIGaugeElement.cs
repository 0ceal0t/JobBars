using JobBars.Helper;
using System.Numerics;

namespace JobBars.UI {
    public abstract unsafe class UIGaugeElement : UIElement {
        public virtual void SetSplitPosition(Vector2 pos) {
            var p = UIHelper.GetNodePosition(JobBars.Builder.GaugeRoot);
            var pScale = UIHelper.GetNodeScale(JobBars.Builder.GaugeRoot);
            UIHelper.SetPosition(RootRes, (pos.X - p.X) / pScale.X, (pos.Y - p.Y) / pScale.Y);
        }

        public abstract int GetHeight(int param);
        public abstract int GetWidth(int param);
        public abstract int GetHorizontalYOffset();
        public abstract void SetColor(ElementColor color);

        public virtual void Cleanup() { }
    }
}