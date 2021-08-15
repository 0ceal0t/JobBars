using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System.Numerics;
using static JobBars.UI.UIColor;

namespace JobBars.UI {
    public abstract unsafe class UIElement {
        public AtkResNode* RootRes;
        public void SetVisible(bool value) {
            if (value) Show();
            else Hide();
        }

        public void Hide() {
            UIHelper.Hide(RootRes);
        }

        public void Show() {
            UIHelper.Show(RootRes);
        }

        public void SetPosition(Vector2 pos) {
            UIHelper.SetPosition(RootRes, pos.X, pos.Y);
        }

        public void SetScale(float scale) {
            UIHelper.SetScale(RootRes, scale, scale);
        }

        public abstract void Dispose();
    }
}
