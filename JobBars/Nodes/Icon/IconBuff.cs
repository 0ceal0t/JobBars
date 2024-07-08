using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using System;

namespace JobBars.Nodes.Icon {
    public unsafe class IconBuff : IconNode {
        private AtkResNode* OriginalOverlay;
        private readonly ImageNode Combo;
        private readonly TextNode BigText;

        public IconBuff( uint adjustedId, uint slotId, int hotbarIdx, int slotIdx, AtkComponentNode* component, IconProps props ) :
            base( adjustedId, slotId, hotbarIdx, slotIdx, component, props ) {
        }

        public override void RefreshVisuals() {
            throw new NotImplementedException();
        }

        public override void SetDone() {
            throw new NotImplementedException();
        }

        public override void SetProgress( float current, float max ) {
            throw new NotImplementedException();
        }

        public override void Tick( float dashPercent, bool border ) {
            throw new NotImplementedException();
        }

        public override void OnDispose() {
            Combo.Dispose();
            BigText.Dispose();
        }
    }
}
