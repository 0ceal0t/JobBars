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
        protected UIBuilder UI;
        public AtkResNode* RootRes;

        public UIElement(UIBuilder ui) {
            UI = ui;
        }

        public abstract void Dispose();

        public void SetVisible(bool value) {
            if (value) Show();
            else Hide();
        }

        public virtual void Hide() {
            UiHelper.Hide(RootRes);
        }

        public virtual void Show() {
            UiHelper.Show(RootRes);
        }

        public virtual void SetSplitPosition(Vector2 pos) {
            var p = UiHelper.GetNodePosition(UI.G_RootRes);
            var pScale = UiHelper.GetNodeScale(UI.G_RootRes);
            UiHelper.SetPosition(RootRes, (pos.X - p.X) / pScale.X, (pos.Y - p.Y) / pScale.Y);
        }

        public virtual void SetPosition(Vector2 pos) {
            UiHelper.SetPosition(RootRes, pos.X, pos.Y);
        }

        public virtual void SetScale(float scale) {
            UiHelper.SetScale(RootRes, scale, scale);
        }

        public virtual void Cleanup() { }

        public abstract void SetColor(ElementColor color);
        public abstract int GetHeight(int param);
        public abstract int GetWidth(int param);
        public abstract int GetHorizontalYOffset();
    }
}
