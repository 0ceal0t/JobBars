using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;

namespace JobBars.Atk {
    public unsafe class AtkBuffPartyList {
        private AtkNineGridNode* Highlight;
        private bool Attached = false;

        public AtkBuffPartyList() { }

        public void Dispose() {
            if( Highlight != null ) {
                Highlight->AtkResNode.Destroy( true );
                Highlight = null;
            }
        }

        public void AttachTo( AtkResNode* targetGlowContainer ) {
            if( Attached ) {
                Dalamud.Error( "Already attached" );
                return;
            }
            if( targetGlowContainer == null ) return;

            if( Highlight == null ) {
                var toCopy = ( AtkNineGridNode* )targetGlowContainer->ChildNode;
                Highlight = AtkHelper.CloneNode( toCopy );
                Highlight->ParentNode = toCopy->ParentNode;

                Highlight->AtkResNode.MultiplyBlue = 50;
                Highlight->AtkResNode.MultiplyRed = 150;
                Highlight->AtkResNode.NodeId = 23;
                AtkHelper.UpdatePart( Highlight->PartsList, 0, 112, 0, 48, 48 );
                AtkHelper.Hide( Highlight );
            }

            AtkHelper.Link( targetGlowContainer->ChildNode->PrevSiblingNode->PrevSiblingNode, ( AtkResNode* )Highlight );

            Attached = true;
        }

        public void DetachFrom( AtkResNode* targetGlowContainer ) {
            if( !Attached ) {
                Dalamud.Error( "Not attached yet" );
                return;
            }
            if( targetGlowContainer == null ) return;
            targetGlowContainer->ChildCount = 3;

            AtkHelper.Detach( ( AtkResNode* )Highlight );

            Attached = false;
        }

        public void SetHighlightVisibility( bool visible ) {
            if( Highlight == null ) return;
            AtkHelper.SetVisibility( Highlight, visible );
        }
    }
}
