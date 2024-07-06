using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System;

namespace JobBars.Atk {
    public unsafe class AtkIconTimer : AtkIcon {
        private AtkImageNode* OriginalImage;

        private bool Dimmed = false;

        private AtkResNode* OriginalRecastContainer;
        private AtkResNode* OriginalPreCombo;
        private AtkResNode* OriginalComboContainer;
        private AtkImageNode* OriginalCombo;
        public AtkImageNode* OriginalText;

        private AtkImageNode* Ring;
        private AtkTextNode* Text;
        private AtkImageNode* Combo;

        public AtkIconTimer( uint adjustedId, uint slotId, int hotbarIdx, int slotIdx, AtkComponentNode* component, UIIconProps props ) :
            base( adjustedId, slotId, hotbarIdx, slotIdx, component, props ) {

            var nodeList = Component->Component->UldManager.NodeList;

            OriginalRecastContainer = nodeList[3];
            OriginalImage = ( AtkImageNode* )nodeList[0];
            var originalRing = ( AtkImageNode* )nodeList[7];

            OriginalPreCombo = IconComponent->Frame->PrevSiblingNode;
            OriginalComboContainer = IconComponent->ComboBorder;
            OriginalCombo = ( AtkImageNode* )OriginalComboContainer->ChildNode;

            OriginalText = IconComponent->UnknownImageNode;

            Combo = AtkHelper.CloneNode( OriginalCombo );
            Combo->AtkResNode.NodeId = NodeIdx++;
            Combo->AtkResNode.X = 0;
            Combo->AtkResNode.Width = 48;
            Combo->AtkResNode.Height = 48;
            Combo->WrapMode = 1;
            Combo->PartId = 0;
            Combo->PartsList = OriginalCombo->PartsList;
            Combo->PartId = 0;

            Combo->AtkResNode.ParentNode = OriginalComboContainer->ParentNode;

            AtkHelper.Link( OriginalPreCombo, ( AtkResNode* )Combo );
            AtkHelper.Link( ( AtkResNode* )Combo, OriginalComboContainer->PrevSiblingNode );

            // ========================

            Text = AtkHelper.CleanAlloc<AtkTextNode>();
            Text->Ctor();
            Text->AtkResNode.NodeId = NodeIdx++;
            Text->AtkResNode.Type = NodeType.Text;
            Text->AtkResNode.NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop | NodeFlags.Enabled | NodeFlags.EmitsEvents;
            RefreshVisuals();
            Text->SetText( "" );

            Text->AtkResNode.ParentNode = OriginalText->AtkResNode.ParentNode;

            AtkHelper.Link( OriginalText->AtkResNode.NextSiblingNode, ( AtkResNode* )Text );
            AtkHelper.Link( ( AtkResNode* )Text, OriginalText->AtkResNode.PrevSiblingNode );

            // ==========================

            Ring = AtkHelper.CloneNode( originalRing );
            Ring->AtkResNode.NodeId = NodeIdx++;
            Ring->AtkResNode.X = 2;
            Ring->AtkResNode.Y = 2;
            Ring->AtkResNode.Width = 44;
            Ring->AtkResNode.Height = 46;
            Ring->WrapMode = 1;
            Ring->PartId = 0;
            Ring->PartsList = originalRing->PartsList;

            Ring->AtkResNode.ParentNode = OriginalRecastContainer;
            OriginalRecastContainer->ChildNode->PrevSiblingNode->PrevSiblingNode = ( AtkResNode* )Ring;

            Component->Component->UldManager.UpdateDrawNodeList();

            AtkHelper.Show( Combo );
            AtkHelper.Hide( Text );
            AtkHelper.Hide( Ring );
        }

        public override void SetProgress( float current, float max ) {
            if( State == IconState.TimerDone && current <= 0 ) return;
            State = IconState.TimerRunning;

            AtkHelper.Show( Text );
            Text->SetText( ( ( int )Math.Round( current ) ).ToString() );

            if( ShowRing ) {
                AtkHelper.Show( Ring );
                Ring->PartId = ( ushort )( 80 - ( float )( current / max ) * 80 );
            }

            JobBars.IconBuilder.AddIconOverride( new IntPtr( OriginalImage ) );
            SetDimmed( true );
        }

        public override void SetDone() {
            State = IconState.TimerDone;
            AtkHelper.Hide( Text );

            AtkHelper.Hide( Ring );

            JobBars.IconBuilder.RemoveIconOverride( new IntPtr( OriginalImage ) );
            SetDimmed( false );
        }

        private void SetDimmed( bool dimmed ) {
            Dimmed = dimmed;
            SetDimmed( OriginalImage, dimmed );
        }

        public static void SetDimmed( AtkImageNode* image, bool dimmed ) {
            var val = ( byte )( dimmed ? 50 : 100 );
            image->AtkResNode.MultiplyRed = val;
            image->AtkResNode.MultiplyRed_2 = val;
            image->AtkResNode.MultiplyGreen = val;
            image->AtkResNode.MultiplyGreen_2 = val;
            image->AtkResNode.MultiplyBlue = val;
            image->AtkResNode.MultiplyBlue_2 = val;
        }

        public override void Tick( float dashPercent, bool border ) {
            var showBorder = CalcShowBorder( State == IconState.TimerDone, border );
            Combo->PartId = !showBorder ? ( ushort )0 : ( ushort )( 6 + dashPercent * 7 );
        }

        public override void OnDispose() {
            OriginalRecastContainer->ChildNode->PrevSiblingNode->PrevSiblingNode = null;

            // ======================

            OriginalPreCombo->PrevSiblingNode = OriginalComboContainer;
            Combo->AtkResNode.PrevSiblingNode->NextSiblingNode = OriginalComboContainer;

            // =====================

            Text->AtkResNode.NextSiblingNode->PrevSiblingNode = ( AtkResNode* )OriginalText;
            Text->AtkResNode.PrevSiblingNode->NextSiblingNode = ( AtkResNode* )OriginalText;

            // =====================

            Component->Component->UldManager.UpdateDrawNodeList();

            JobBars.IconBuilder.RemoveIconOverride( new IntPtr( OriginalImage ) );
            if( Dimmed ) SetDimmed( false );

            OriginalPreCombo = null;
            OriginalComboContainer = null;
            OriginalCombo = null;
            OriginalText = null;
            OriginalRecastContainer = null;
            OriginalImage = null;
        }

        public override void RefreshVisuals() {
            if( JobBars.Configuration.IconTimerLarge ) {
                Text->AtkResNode.X = 5;
                Text->AtkResNode.Y = 7;
                SetTextLarge( Text );
            }
            else {
                Text->AtkResNode.X = 3;
                Text->AtkResNode.Y = 37;
                SetTextSmall( Text );
            }
        }
    }
}
