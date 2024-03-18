using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;

namespace JobBars.Atk {
    public unsafe class AtkBuff : AtkElement {
        public static ushort WIDTH => ( ushort )( JobBars.Configuration.BuffSquare ? 40 : 36 );
        public static ushort HEIGHT => ( ushort )( JobBars.Configuration.BuffSquare ? 40 : 28 );

        private AtkTextNode* TextNode;
        private AtkImageNode* Overlay;
        private AtkImageNode* Icon;
        private AtkNineGridNode* Border;

        private ActionIds LastIconId = 0;
        public ActionIds IconId => LastIconId;

        private string CurrentText = "";
        private static int BUFFS_HORIZONTAL => JobBars.Configuration.BuffHorizontal;

        public AtkBuff() : base() {
            RootRes = AtkBuilder.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;

            Icon = AtkBuilder.CreateImageNode();
            Icon->AtkResNode.X = 0;
            Icon->AtkResNode.Y = 0;
            Icon->PartId = 0;
            Icon->Flags = 0;
            Icon->WrapMode = 1;
            AtkHelper.SetupIcon( Icon, 405 );

            Overlay = AtkBuilder.CreateImageNode();
            Overlay->AtkResNode.Height = 1;
            Overlay->AtkResNode.X = 0;
            Overlay->AtkResNode.Y = 0;
            Overlay->Flags = 0;
            Overlay->WrapMode = 1;
            AtkHelper.SetupTexture( Overlay, "ui/uld/IconA_Frame.tex" );
            AtkHelper.UpdatePart( Overlay, 365, 4, 37, 37 );

            Border = AtkBuilder.CreateNineNode();
            Border->AtkResNode.X = -4;
            Border->AtkResNode.Y = -3;
            AtkHelper.SetupTexture( Border, "ui/uld/IconA_Frame.tex" );
            SetBorderThin( JobBars.Configuration.BuffThinBorder );
            Border->TopOffset = 5;
            Border->BottomOffset = 5;
            Border->LeftOffset = 5;
            Border->RightOffset = 5;

            TextNode = AtkBuilder.CreateTextNode();
            TextNode->FontSize = ( byte )JobBars.Configuration.BuffTextSize_v2;
            TextNode->LineSpacing = ( byte )JobBars.Configuration.BuffTextSize_v2;
            TextNode->AlignmentFontType = 52;
            TextNode->AtkResNode.X = 0;
            TextNode->AtkResNode.Y = 0;
            TextNode->AtkResNode.NodeFlags |= NodeFlags.Visible;
            TextNode->AtkResNode.DrawFlags = 1;
            TextNode->EdgeColor = new ByteColor { R = 0, G = 0, B = 0, A = 255 };

            var layout = new LayoutNode( RootRes, new[] {
                new LayoutNode(Icon),
                new LayoutNode(Overlay),
                new LayoutNode(Border),
                new LayoutNode(TextNode)
            } );
            layout.Setup();
            layout.Cleanup();

            TextNode->SetText( "" );
            UpdateSize();
        }

        public void UpdateSize() {
            RootRes->Width = WIDTH;
            RootRes->Height = HEIGHT;

            TextNode->AtkResNode.Width = WIDTH;
            TextNode->AtkResNode.Height = HEIGHT;

            Icon->AtkResNode.Width = WIDTH;
            Icon->AtkResNode.Height = HEIGHT;

            AtkHelper.UpdatePart( Icon, ( ushort )( ( 40 - WIDTH ) / 2 ), ( ushort )( ( 40 - HEIGHT ) / 2 ), WIDTH, HEIGHT );

            Overlay->AtkResNode.Width = WIDTH;

            Border->AtkResNode.Width = ( ushort )( WIDTH + 8 );
            Border->AtkResNode.Height = ( ushort )( HEIGHT + 8 );
        }

        public override void Dispose() {
            if( TextNode != null ) {
                TextNode->AtkResNode.Destroy( true );
                TextNode = null;
            }

            if( Icon != null ) {
                AtkHelper.UnloadTexture( Icon );
                Icon->AtkResNode.Destroy( true );
                Icon = null;
            }

            if( Overlay != null ) {
                AtkHelper.UnloadTexture( Overlay );
                Overlay->AtkResNode.Destroy( true );
                Overlay = null;
            }

            if( Border != null ) {
                AtkHelper.UnloadTexture( Border );
                Border->AtkResNode.Destroy( true );
                Border = null;
            }

            if( RootRes != null ) {
                RootRes->Destroy( true );
                RootRes = null;
            }
        }

        public void SetPosition( int idx ) {
            var position_x = BUFFS_HORIZONTAL == 0 ? 0 : idx % BUFFS_HORIZONTAL;
            var position_y = BUFFS_HORIZONTAL == 0 ? 0 : ( idx - position_x ) / BUFFS_HORIZONTAL;

            var xMod = JobBars.Configuration.BuffRightToLeft ? -1 : 1;
            var yMod = JobBars.Configuration.BuffBottomToTop ? -1 : 1;

            AtkHelper.SetPosition( RootRes, xMod * ( WIDTH + 9 ) * position_x, yMod * ( HEIGHT + 7 ) * position_y );
        }

        public void SetOnCD( float opacity ) {
            RootRes->MultiplyBlue = 75;
            RootRes->MultiplyRed = 75;
            RootRes->MultiplyGreen = 75;
            RootRes->Color.A = ( byte )( 255 * opacity );
        }

        public void SetOffCD() {
            RootRes->MultiplyBlue = 100;
            RootRes->MultiplyRed = 100;
            RootRes->MultiplyGreen = 100;
            RootRes->Color.A = 255;
        }

        public void SetText( string text ) {
            if( text != CurrentText ) {
                TextNode->SetText( text );
                CurrentText = text;
            }
        }

        public void SetPercent( float percent ) {
            var h = ( int )( HEIGHT * percent );
            var yOffset = HEIGHT - h;
            AtkHelper.SetSize( Overlay, null, h );
            AtkHelper.SetPosition( Overlay, 0, yOffset );
        }

        public void LoadIcon( ActionIds action ) {
            LastIconId = action;
            var icon = AtkHelper.GetIcon( action );
            Icon->LoadIconTexture( icon, 0 );
        }

        public void SetColor( ElementColor color ) {
            if( JobBars.Configuration.BuffThinBorder ) {
                AtkColor.SetColor( Border, AtkColor.NoColor );
                return;
            }
            var newColor = color;
            newColor.AddBlue -= 50;
            AtkColor.SetColor( Border, newColor );
        }

        public void SetTextSize( int size ) {
            TextNode->LineSpacing = ( byte )size;
            TextNode->FontSize = ( byte )size;
        }

        public void SetBorderThin( bool thin ) {
            if( thin ) {
                AtkHelper.UpdatePart( Border, 0, 96, 48, 48 );
            }
            else {
                AtkHelper.UpdatePart( Border, 252, 12, 47, 47 );
            }
        }
    }
}