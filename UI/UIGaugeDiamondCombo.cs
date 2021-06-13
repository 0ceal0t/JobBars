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
    public unsafe class UIGaugeDiamondCombo : UIElement {
        private UIGauge Gauge;
        private UIDiamond Diamond;
        
        public UIGaugeDiamondCombo(UIBuilder _ui, UIGauge gauge, UIDiamond diamond) : base(_ui) {
            Gauge = gauge;
            Diamond = diamond;
            RootRes = gauge.RootRes;
        }

        public override void Init() { }
        public override unsafe void LoadExisting(AtkResNode* node) { }

        public void SetText(string text) {
            Gauge.SetText(text);
        }

        public void SetTextColor(ElementColor color) {
            Gauge.SetTextColor(color);
        }

        public void SetPercent(float value) {
            Gauge.SetPercent(value);
        }

        public void SetMaxValue(int value) {
            Diamond.SetMaxValue(value);
        }

        public void SetDiamondValue(int value) {
            Diamond.SetValue(value);
        }

        public override int GetHeight(int param) {
            return 50;
        }

        public override int GetWidth(int param) {
            return Gauge.GetWidth(param);
        }

        public override int GetHorizontalYOffset() {
            return Gauge.GetHorizontalYOffset();
        }

        public override void SetColor(ElementColor color) {
            Gauge.SetColor(color);
            Diamond.SetColor(color);
        }

        public override void SetSplitPosition(Vector2 pos) {
            var p = UiHelper.GetNodePosition(UI.G_RootRes);
            var pScale = UiHelper.GetNodeScale(UI.G_RootRes);
            var x = (pos.X - p.X) / pScale.X;
            var y = (pos.Y - p.Y) / pScale.Y;

            UiHelper.SetPosition(Gauge.RootRes, x, y);
            UiHelper.SetPosition(Diamond.RootRes, x, y + 10);
        }

        public override void SetPosition(Vector2 pos) {
            UiHelper.SetPosition(Gauge.RootRes, pos.X, pos.Y);
            UiHelper.SetPosition(Diamond.RootRes, pos.X, pos.Y + 10);
        }
    }
}
