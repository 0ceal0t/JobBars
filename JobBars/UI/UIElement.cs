using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars.UI {
    public abstract unsafe class UIElement {
        public AtkResNode* RootRes;

        public abstract void Dispose();

        public void SetVisible(bool value) {
            if (value) Show();
            else Hide();
        }

        public virtual void Hide() {
            UIHelper.Hide(RootRes);
        }

        public virtual void Show() {
            UIHelper.Show(RootRes);
        }

        public virtual void SetSplitPosition(Vector2 pos) {
            var p = UIHelper.GetNodePosition(UIBuilder.Builder.GaugeRoot);
            var pScale = UIHelper.GetNodeScale(UIBuilder.Builder.GaugeRoot);
            UIHelper.SetPosition(RootRes, (pos.X - p.X) / pScale.X, (pos.Y - p.Y) / pScale.Y);
        }

        public virtual void SetPosition(Vector2 pos) {
            UIHelper.SetPosition(RootRes, pos.X, pos.Y);
        }

        public virtual void SetScale(float scale) {
            UIHelper.SetScale(RootRes, scale, scale);
        }

        public virtual void Cleanup() { }

        public abstract void SetColor(ElementColor color);
        public abstract int GetHeight(int param);
        public abstract int GetWidth(int param);
        public abstract int GetHorizontalYOffset();
    }
}
