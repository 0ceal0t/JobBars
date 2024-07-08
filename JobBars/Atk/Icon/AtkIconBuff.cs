using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System;

namespace JobBars.Atk {
    public unsafe class AtkIconBuff : AtkIcon {
        private AtkResNode* OriginalOverlay;
        private readonly AtkImageNode* Combo;
        private readonly AtkTextNode* BigText;

        public AtkIconBuff( uint adjustedId, uint slotId, int hotbarIdx, int slotIdx, AtkComponentNode* component, IconProps props ) :
            base( adjustedId, slotId, hotbarIdx, slotIdx, component, props ) {

            var nodeList = Component->Component->UldManager.NodeList;
            OriginalOverlay = nodeList[1];
            var originalBorder = ( AtkImageNode* )nodeList[4];

            Combo = UiHelper.CloneNode( originalBorder );
            Combo->AtkResNode.NodeId = NodeIdx++;
            Combo->AtkResNode.X = -2;
            Combo->AtkResNode.Width = 48;
            Combo->AtkResNode.Height = 48;
            Combo->WrapMode = 1;
            Combo->PartId = 0;
            Combo->PartsList = originalBorder->PartsList;

            // ====================

            BigText = UiHelper.CleanAlloc<AtkTextNode>();
            BigText->Ctor();
            BigText->AtkResNode.NodeId = NodeIdx++;
            BigText->AtkResNode.Type = NodeType.Text;
            BigText->AtkResNode.NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop | NodeFlags.Enabled | NodeFlags.EmitsEvents;
            RefreshVisuals();
            BigText->SetText( "" );

            var rootNode = ( AtkResNode* )Component;
            Combo->AtkResNode.ParentNode = rootNode;
            BigText->AtkResNode.ParentNode = rootNode;

            UiHelper.Link( OriginalOverlay, ( AtkResNode* )Combo );
            UiHelper.Link( ( AtkResNode* )Combo, ( AtkResNode* )BigText );
            UiHelper.Link( ( AtkResNode* )BigText, nodeList[15] );

            Component->Component->UldManager.UpdateDrawNodeList();

            UiHelper.Hide( Combo );
            UiHelper.Hide( BigText );
        }

        public override void SetProgress( float current, float max ) {
            if( State != IconState.BuffRunning ) {
                State = IconState.BuffRunning;
                UiHelper.Hide( OriginalOverlay );
                UiHelper.Show( BigText );
                UiHelper.Show( Combo );
            }
            BigText->SetText( ( ( int )Math.Round( current ) ).ToString() );
        }

        public override void SetDone() {
            if( State == IconState.None ) return;
            State = IconState.None;

            UiHelper.Hide( BigText );
            UiHelper.Hide( Combo );
            UiHelper.Show( OriginalOverlay );
        }

        public override void Tick( float dashPercent, bool border ) {
            // avoid doubling up on borders if combo_or_active
            var showBorder = CalcShowBorder( State == IconState.BuffRunning, false );
            Combo->PartId = !showBorder ? ( ushort )0 : ( ushort )( 6 + dashPercent * 7 );
            UiHelper.SetVisibility( Combo, showBorder );
        }

        public override void OnDispose() {
            UiHelper.Link( OriginalOverlay, BigText->AtkResNode.PrevSiblingNode );
            Component->Component->UldManager.UpdateDrawNodeList();

            UiHelper.Show( OriginalOverlay );
            OriginalOverlay = null;
        }

        public override void RefreshVisuals() {
            if( JobBars.Configuration.IconBuffLarge ) {
                BigText->AtkResNode.X = 2;
                BigText->AtkResNode.Y = 7;
                SetTextLarge( BigText );
            }
            else {
                BigText->AtkResNode.X = 0;
                BigText->AtkResNode.Y = 37;
                SetTextSmall( BigText );
            }
        }
    }
}
