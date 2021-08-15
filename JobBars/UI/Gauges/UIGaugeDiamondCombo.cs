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
    public unsafe class UIGaugeDiamondCombo : UIGaugeElement {
        private UIGauge Gauge;
        private UIDiamond Diamond;

        public UIGaugeDiamondCombo(AtkUnitBase* addon) {
            Gauge = new UIGauge(addon);
            Diamond = new UIDiamond(addon);

            RootRes = UIBuilder.Builder.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = 0;
            RootRes->Height = 0;
            RootRes->ChildCount = (ushort)(Gauge.RootRes->ChildCount + Gauge.RootRes->ChildCount + 2);
            RootRes->ChildNode = Gauge.RootRes;

            Gauge.RootRes->ParentNode = RootRes;
            Diamond.RootRes->ParentNode = RootRes;
            UIHelper.Link(Gauge.RootRes, Diamond.RootRes);

            UIHelper.SetPosition(Gauge.RootRes, 0, 0);
            UIHelper.SetPosition(Diamond.RootRes, 0, 10);
        }

        public override void Dispose() {
            Gauge.Dispose();
            Diamond.Dispose();

            Gauge = null;
            Diamond = null;

            if (RootRes != null) {
                RootRes->Destroy(true);
                RootRes = null;
            }
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
    }
}
