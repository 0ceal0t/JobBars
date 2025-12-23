using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace JobBars.Nodes.Cooldown {
    public unsafe class CooldownNode : NodeBase<AtkResNode> {
        public static readonly ushort WIDTH = 30;
        public static readonly ushort HEIGHT = 30;

        private readonly TextNode Text;
        private readonly SimpleImageNode Icon;
        private readonly SimpleImageNode Border;

        private ActionIds LastAction = 0;
        public ActionIds IconId => LastAction;

        public CooldownNode() : base( NodeType.Res ) {
            Size = new( WIDTH, HEIGHT );

            Icon = new IconImageNode() {
                Size = new( WIDTH, HEIGHT ),
                NodeFlags = NodeFlags.Visible,
                ImageNodeFlags = ImageNodeFlags.AutoFit,
                TextureSize = new( 44, 46 ),
                IconId = 405,
                FitTexture = true
            };

            Border = new SimpleImageNode() {
                Size = new( 49, 47 ),
                NodeFlags = NodeFlags.Visible,
                WrapMode = WrapMode.Tile,
                ImageNodeFlags = 0,
                Position = new( -4, -2 ),
                TextureCoordinates = new( 0, 96 ),
                TextureSize = new( 48, 48 ),
                Scale = new( ( ( float )WIDTH + 8 ) / 49.0f, ( ( float )HEIGHT + 6 ) / 47.0f ),
                TexturePath = "ui/uld/IconA_Frame.tex"
            };

            Text = new TextNode() {
                Size = new( WIDTH, HEIGHT ),
                FontSize = 21,
                LineSpacing = ( byte )HEIGHT,
                TextColor = new( 1, 1, 1, 1 ),
                TextOutlineColor = new( 0, 0, 0, 1 ),
                TextId = 0,
                TextFlags = TextFlags.Glare,
                String = "",
            };
            Text.Node->AlignmentFontType = 52;

            Icon.AttachNode( this );
            Border.AttachNode( this );
            Text.AttachNode( this );
        }

        public void SetNoDash() {
            Border.TextureCoordinates = new( 0, 96 );
            Border.TextureSize = new( 48, 48 );
        }

        public void SetDash( float percent ) {
            var partId = ( int )( percent * 7 ); // 0 - 6

            var row = partId % 3;
            var column = ( partId - row ) / 3;

            var u = ( ushort )( 96 + ( 48 * row ) );
            var v = ( ushort )( 48 * column );

            Border.TextureCoordinates = new( u, v );
            Border.TextureSize = new( 48, 48 );
        }

        public void SetText( string text ) {
            Text.FontSize = text.Length > 2 ? ( byte )17 : ( byte )21;
            Text.String = text;
            Text.IsVisible = true;
        }

        public void SetOnCd() {
            Icon.MultiplyColor = new( 75f / 100f, 75f / 100f, 75f / 100f );
            Color = Color with {
                W = JobBars.Configuration.CooldownsOnCDOpacity
            };
        }

        public void SetOffCd() {
            Icon.MultiplyColor = new( 100f / 100f, 100f / 100f, 100f / 100f );
            Color = Color with {
                W = 1f
            };
        }

        public void LoadIcon( ActionIds action ) {
            if( action == LastAction ) return;
            LastAction = action;
            Icon.LoadIcon( UiHelper.GetIcon( action ) );
        }
    }
}
