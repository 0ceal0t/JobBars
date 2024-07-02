using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;

namespace JobBars.Atk {
    public unsafe class AtkBuffPartyList {
        private AtkNineGridNode* Highlight;
        private AtkTextNode* TextNode;
        private bool Attached = false;

        public AtkBuffPartyList() {
            Highlight = AtkBuilder.CreateNineNode();
            Highlight->AtkResNode.Width = 320;
            Highlight->AtkResNode.Height = 48;
            AtkHelper.SetupTexture( Highlight, "ui/uld/PartyListTargetBase.tex" );
            AtkHelper.UpdatePart( Highlight->PartsList, 0, 112, 0, 48, 48 );
            Highlight->TopOffset = 20;
            Highlight->BottomOffset = 20;
            Highlight->RightOffset = 20;
            Highlight->LeftOffset = 20;
            Highlight->PartsTypeRenderType = 220;
            Highlight->AtkResNode.NodeId = 23;
            Highlight->AtkResNode.DrawFlags = 0;
            Highlight->AtkResNode.NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop;
            Highlight->AtkResNode.MultiplyBlue = 50;
            Highlight->AtkResNode.MultiplyRed = 150;
            AtkHelper.SetPosition( Highlight, 47, 21 );
            AtkHelper.Hide( Highlight );

            TextNode = AtkBuilder.CreateTextNode();
            TextNode->FontSize = ( byte )JobBars.Configuration.BuffTextSize_v2;
            TextNode->LineSpacing = ( byte )JobBars.Configuration.BuffTextSize_v2;
            TextNode->AlignmentFontType = 20;
            TextNode->FontSize = 14;
            TextNode->TextColor = new ByteColor { R = 232, G = 255, B = 254, A = 255 };
            TextNode->EdgeColor = new ByteColor { R = 8, G = 80, B = 152, A = 255 };
            TextNode->AtkResNode.X = 5;
            TextNode->AtkResNode.Y = -15;
            TextNode->AtkResNode.NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop;
            TextNode->AtkResNode.DrawFlags = 1;
            TextNode->AtkResNode.Priority = 1;
            TextNode->AtkResNode.NodeId = 24;
            TextNode->SetText( "" );
            AtkHelper.Hide( TextNode );
        }

        public void Dispose() {
            if( TextNode != null ) {
                TextNode->AtkResNode.Destroy( true );
                TextNode = null;
            }

            if( Highlight != null ) {
                AtkHelper.UnloadTexture( Highlight );
                Highlight->AtkResNode.Destroy( true );
                Highlight = null;
            }
        }

        public void AttachTo( AtkResNode* targetGlowContainer, AtkResNode* emnityBarContainer ) {
            if( Attached ) {
                Dalamud.Error( "Already attached" );
                return;
            }
            if( targetGlowContainer == null || emnityBarContainer == null ) return;

            targetGlowContainer->ChildCount = 4;
            Highlight->AtkResNode.ParentNode = targetGlowContainer;
            AtkHelper.Link( targetGlowContainer->ChildNode->PrevSiblingNode->PrevSiblingNode, ( AtkResNode* )Highlight );

            emnityBarContainer->ChildCount = 3;
            TextNode->AtkResNode.ParentNode = emnityBarContainer;
            AtkHelper.Link( emnityBarContainer->ChildNode->PrevSiblingNode, ( AtkResNode* )TextNode );

            Attached = true;
        }

        public void DetachFrom( AtkResNode* targetGlowContainer, AtkResNode* emnityBarContainer ) {
            if( !Attached ) {
                Dalamud.Error( "Not attached yet" );
                return;
            }
            if( targetGlowContainer == null || emnityBarContainer == null ) return;

            targetGlowContainer->ChildCount = 3;
            AtkHelper.Detach( ( AtkResNode* )Highlight );

            emnityBarContainer->ChildCount = 2;
            AtkHelper.Detach( ( AtkResNode* )TextNode );

            Attached = false;
        }

        public void SetHighlightVisibility( bool visible ) => AtkHelper.SetVisibility( Highlight, visible );

        public void SetText( string text ) {
            if( string.IsNullOrEmpty( text ) ) {
                AtkHelper.Hide( TextNode );
                return;
            }
            AtkHelper.Show( TextNode );
            TextNode->SetText( text );
        }
    }
}
