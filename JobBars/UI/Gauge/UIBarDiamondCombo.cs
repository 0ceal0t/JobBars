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
    public unsafe class UIBarDiamondCombo : UIGaugeElement {
        private readonly UIBar Gauge;
        private readonly UIDiamond Diamond;

        public UIBarDiamondCombo(UIBar gauge, UIDiamond diamond) {
            Gauge = gauge;
            Diamond = diamond;
            RootRes = gauge.RootRes;
        }

        public override void Dispose() { }

        public override void Hide() {
            UIHelper.Hide(RootRes);
            UIHelper.Hide(Diamond.RootRes);
        }

        public override void Show() {
            UIHelper.Show(RootRes);
            UIHelper.Show(Diamond.RootRes);
        }

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

        public void SetDiamondValue(int value, int start, int count) {
            Diamond.SetValue(value, start, count);
        }

        public void SetDiamondValue(int value) {
            Diamond.SetValue(value);
        }

        public override void SetColor(ElementColor color) {
            SetGaugeColor(color);
            SetDiamondColor(color);
        }

        public void SetGaugeColor(ElementColor color) {
            Gauge.SetColor(color);
        }

        public void SetDiamondColor(ElementColor color) {
            Diamond.SetColor(color);
        }

        public void SetDiamondColor(ElementColor color, int start, int count) {
            Diamond.SetColor(color, start, count);
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

        public override void SetSplitPosition(Vector2 pos) {
            var p = UIHelper.GetNodePosition(UIBuilder.Builder.GaugeRoot);
            var pScale = UIHelper.GetNodeScale(UIBuilder.Builder.GaugeRoot);
            var x = (pos.X - p.X) / pScale.X;
            var y = (pos.Y - p.Y) / pScale.Y;

            UIHelper.SetPosition(Gauge.RootRes, x, y);
            UIHelper.SetPosition(Diamond.RootRes, x, y + 10);
        }

        public override void SetScale(float scale) {
            UIHelper.SetScale(Gauge.RootRes, scale, scale);
            UIHelper.SetScale(Diamond.RootRes, scale, scale);
        }

        public override void SetPosition(Vector2 pos) {
            UIHelper.SetPosition(Gauge.RootRes, pos.X, pos.Y);
            UIHelper.SetPosition(Diamond.RootRes, pos.X, pos.Y + 10);
        }
    }
}
