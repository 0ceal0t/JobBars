using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System;

namespace JobBars.Atk {
    public unsafe class AtkIconBuff : AtkIcon {
        private AtkResNode* OriginalOverlay;
        private AtkImageNode* Combo;
        private AtkTextNode* BigText;

        public AtkIconBuff( uint adjustedId, uint slotId, int hotbarIdx, int slotIdx, AtkComponentNode* component, UIIconProps props ) :
            base( adjustedId, slotId, hotbarIdx, slotIdx, component, props ) {

            var nodeList = Component->Component->UldManager.NodeList;
            OriginalOverlay = nodeList[1];
            var originalBorder = ( AtkImageNode* )nodeList[4];

            Combo = AtkHelper.CleanAlloc<AtkImageNode>();
            Combo->Ctor();
            Combo->AtkResNode.NodeId = NodeIdx++;
            Combo->AtkResNode.Type = NodeType.Image;
            Combo->AtkResNode.X = -2;
            Combo->AtkResNode.Width = 48;
            Combo->AtkResNode.Height = 48;
            Combo->AtkResNode.NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop;
            Combo->AtkResNode.DrawFlags = 1;
            Combo->AtkResNode.DrawFlags |= 4;
            Combo->WrapMode = 1;
            Combo->PartId = 0;
            Combo->PartsList = originalBorder->PartsList;

            BigText = AtkHelper.CleanAlloc<AtkTextNode>();
            BigText->Ctor();
            BigText->AtkResNode.NodeId = NodeIdx++;
            BigText->AtkResNode.Type = NodeType.Text;
            BigText->AtkResNode.NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop;
            BigText->AtkResNode.DrawFlags = 1;
            BigText->AtkResNode.DrawFlags |= 4;
            RefreshVisuals();
            BigText->SetText( "" );

            var rootNode = ( AtkResNode* )Component;
            var macroIcon = nodeList[15];
            Combo->AtkResNode.ParentNode = rootNode;
            BigText->AtkResNode.ParentNode = rootNode;

            AtkHelper.Link( OriginalOverlay, ( AtkResNode* )Combo );
            AtkHelper.Link( ( AtkResNode* )Combo, ( AtkResNode* )BigText );
            AtkHelper.Link( ( AtkResNode* )BigText, macroIcon );

            Component->Component->UldManager.UpdateDrawNodeList();

            AtkHelper.Hide( Combo );
            AtkHelper.Hide( BigText );
        }

        public override void SetProgress( float current, float max ) {
            if( State != IconState.BuffRunning ) {
                State = IconState.BuffRunning;
                AtkHelper.Hide( OriginalOverlay );
                AtkHelper.Show( BigText );
                AtkHelper.Show( Combo );
            }
            BigText->SetText( ( ( int )Math.Round( current ) ).ToString() );
        }

        public override void SetDone() {
            if( State == IconState.None ) return;
            State = IconState.None;

            AtkHelper.Hide( BigText );
            AtkHelper.Hide( Combo );
            AtkHelper.Show( OriginalOverlay );
        }

        public override void Tick( float dashPercent, bool border ) {
            // avoid doubling up on borders if combo_or_active
            var showBorder = CalcShowBorder( State == IconState.BuffRunning, false );
            Combo->PartId = !showBorder ? ( ushort )0 : ( ushort )( 6 + dashPercent * 7 );
            AtkHelper.SetVisibility( Combo, showBorder );
        }

        public override void OnDispose() {
            AtkHelper.Link( OriginalOverlay, BigText->AtkResNode.PrevSiblingNode );
            Component->Component->UldManager.UpdateDrawNodeList();

            if( Combo != null ) {
                Combo->AtkResNode.Destroy( true );
                Combo = null;
            }

            if( BigText != null ) {
                BigText->AtkResNode.Destroy( true );
                BigText = null;
            }

            AtkHelper.Show( OriginalOverlay );
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
