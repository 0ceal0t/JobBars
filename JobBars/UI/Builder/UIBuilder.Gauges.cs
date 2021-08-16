using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System.Collections.Generic;
using System.Numerics;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        public AtkResNode* GaugeRoot = null;
        private static readonly int MAX_GAUGES = 4;
        public List<UIBar> Gauges = new();
        public List<UIArrow> Arrows = new();
        public List<UIDiamond> Diamonds = new();

        private void InitGauges(AtkUnitBase* addon) {
            GaugeRoot = CreateResNode();
            GaugeRoot->Width = 256;
            GaugeRoot->Height = 100;
            GaugeRoot->Flags = 9395;

            UIDiamond lastDiamond = null;
            for (int idx = 0; idx < MAX_GAUGES; idx++) {
                var newGauge = new UIBar(addon);
                var newArrow = new UIArrow(addon);
                var newDiamond = new UIDiamond(addon);

                Gauges.Add(newGauge);
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

            GaugeRoot->ParentNode = addon->RootNode;
            GaugeRoot->ChildCount = (ushort)(MAX_GAUGES * (
                Arrows[0].RootRes->ChildCount + 1 +
                Gauges[0].RootRes->ChildCount + 1 +
                Diamonds[0].RootRes->ChildCount + 1
            ));
            GaugeRoot->ChildNode = Gauges[0].RootRes;
        }

        private void DisposeGauges() {
            Gauges.ForEach(x => x.Dispose());
            Gauges = null;

            Arrows.ForEach(x => x.Dispose());
            Arrows = null;

            Diamonds.ForEach(x => x.Dispose());
            Diamonds = null;

            GaugeRoot->Destroy(true);
            GaugeRoot = null;
        }

        public void SetGaugePosition(Vector2 pos) {
            SetPosition(GaugeRoot, pos.X, pos.Y);
        }

        public void SetGaugeScale(float scale) {
            SetScale(GaugeRoot, scale, scale);
        }

        public void ShowGauges() {
            UIHelper.Show(GaugeRoot);
        }

        public void HideGauges() {
            UIHelper.Hide(GaugeRoot);
        }

        public void HideAllGauges() {
            Gauges.ForEach(x => x.Hide());
            Arrows.ForEach(x => x.Hide());
            Diamonds.ForEach(x => x.Hide());
        }
    }
}
