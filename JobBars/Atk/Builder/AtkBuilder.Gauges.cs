using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System.Collections.Generic;
using System.Numerics;

namespace JobBars.Atk {
    public unsafe partial class AtkBuilder {
        public AtkResNode* GaugeRoot = null;
        private static readonly int MAX_GAUGES = 7;
        public List<AtkBar> Bars = new();
        public List<AtkArrow> Arrows = new();
        public List<AtkDiamond> Diamonds = new();

        private void InitGauges() {
            GaugeRoot = CreateResNode();
            GaugeRoot->Width = 256;
            GaugeRoot->Height = 100;
            GaugeRoot->NodeFlags = (NodeFlags)9395;

            AtkDiamond lastDiamond = null;
            for (int idx = 0; idx < MAX_GAUGES; idx++) {
                var newGauge = new AtkBar();
                var newArrow = new AtkArrow();
                var newDiamond = new AtkDiamond();

                Bars.Add(newGauge);
                Arrows.Add(newArrow);
                Diamonds.Add(newDiamond);

                newGauge.RootRes->ParentNode = GaugeRoot;
                newArrow.RootRes->ParentNode = GaugeRoot;
                newDiamond.RootRes->ParentNode = GaugeRoot;

                AtkHelper.Link(newGauge.RootRes, newArrow.RootRes);
                AtkHelper.Link(newArrow.RootRes, newDiamond.RootRes);

                if (lastDiamond != null) AtkHelper.Link(lastDiamond.RootRes, newGauge.RootRes);
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
        public void ShowGauges() => AtkHelper.Show(GaugeRoot);
        public void HideGauges() => AtkHelper.Hide(GaugeRoot);

        public void HideAllGauges() {
            Bars.ForEach(x => x.Hide());
            Arrows.ForEach(x => x.Hide());
            Diamonds.ForEach(x => x.Hide());
        }
    }
}
