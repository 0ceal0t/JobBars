using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System.Collections.Generic;
using System.Numerics;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        public AtkResNode* GaugeRoot = null;
        private static readonly int MAX_GAUGES = 7;
        public List<UIBar> Bars = new();
        public List<UIArrow> Arrows = new();
        public List<UIDiamond> Diamonds = new();

        private void InitGauges() {
            GaugeRoot = CreateResNode();
            GaugeRoot->Width = 256;
            GaugeRoot->Height = 100;
            GaugeRoot->Flags = 9395;

            UIDiamond lastDiamond = null;
            for (int idx = 0; idx < MAX_GAUGES; idx++) {
                var newGauge = new UIBar();
                var newArrow = new UIArrow();
                var newDiamond = new UIDiamond();

                Bars.Add(newGauge);
                Arrows.Add(newArrow);
                Diamonds.Add(newDiamond);

                newGauge.RootRes->ParentNode = GaugeRoot;
                newArrow.RootRes->ParentNode = GaugeRoot;
                newDiamond.RootRes->ParentNode = GaugeRoot;

                UIHelper.Link(newGauge.RootRes, newArrow.RootRes);
                UIHelper.Link(newArrow.RootRes, newDiamond.RootRes);

                if (lastDiamond != null) UIHelper.Link(lastDiamond.RootRes, newGauge.RootRes);
                lastDiamond = newDiamond;
            }

            GaugeRoot->ChildCount = (ushort)(MAX_GAUGES * (
                Arrows[0].RootRes->ChildCount + 1 +
                Bars[0].RootRes->ChildCount + 1 +
                Diamonds[0].RootRes->ChildCount + 1
            ));
            GaugeRoot->ChildNode = Bars[0].RootRes;
        }

        private void DisposeGauges() {
            Bars.ForEach(x => x.Dispose());
            Bars = null;

            Arrows.ForEach(x => x.Dispose());
            Arrows = null;

            Diamonds.ForEach(x => x.Dispose());
            Diamonds = null;

            GaugeRoot->Destroy(true);
            GaugeRoot = null;
        }

        public void SetGaugePosition(Vector2 pos) => SetPosition(GaugeRoot, pos.X, pos.Y);
        public void SetGaugeScale(float scale) => SetScale(GaugeRoot, scale, scale);
        public void ShowGauges() => UIHelper.Show(GaugeRoot);
        public void HideGauges() => UIHelper.Hide(GaugeRoot);

        public void HideAllGauges() {
            Bars.ForEach(x => x.Hide());
            Arrows.ForEach(x => x.Hide());
            Diamonds.ForEach(x => x.Hide());
        }
    }
}
