using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System.Numerics;
using static JobBars.UI.UIColor;

namespace JobBars.UI {
    public abstract unsafe class UIGaugeElement : UIElement {
        public virtual void SetSplitPosition(Vector2 pos) {
            var p = UIHelper.GetNodePosition(UIBuilder.Builder.GaugeRoot);
            var pScale = UIHelper.GetNodeScale(UIBuilder.Builder.GaugeRoot);
            UIHelper.SetPosition(RootRes, (pos.X - p.X) / pScale.X, (pos.Y - p.Y) / pScale.Y);
        }

        public abstract int GetHeight(int param);
        public abstract int GetWidth(int param);
        public abstract int GetHorizontalYOffset();
    }
}