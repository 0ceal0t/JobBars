using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
using JobBars.Data;
using JobBars.Helper;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace JobBars.Nodes.Buff {
    public unsafe class BuffNode : NodeBase<AtkResNode> {
        public static ushort WIDTH => ( ushort )( JobBars.Configuration.BuffSquare ? 40 : 36 );
        public static ushort HEIGHT => ( ushort )( JobBars.Configuration.BuffSquare ? 40 : 28 );

        private readonly TextNode Text;
        private readonly SimpleImageNode Overlay;
        private readonly IconImageNode Icon;
        private readonly SimpleNineGridNode Border;

        private ActionIds LastAction = 0;
        public ActionIds IconId => LastAction;

        private string CurrentText = "";

        public BuffNode() : base( NodeType.Res ) {
            NodeID = JobBars.NodeId++;

            Icon = new IconImageNode() {
                NodeID = JobBars.NodeId++,
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
                WrapMode = WrapMode.Unknown,
                ImageNodeFlags = 0,
                IconId = 405
            };

            Overlay = new SimpleImageNode() {
                NodeID = JobBars.NodeId++,
                Height = 1,
                TextureCoordinates = new( 365, 4 ),
                TextureSize = new( 37, 37 ),
                NodeFlags = NodeFlags.Visible,
                WrapMode = WrapMode.Unknown,
                ImageNodeFlags = 0,
            };
            Overlay.LoadTexture( "ui/uld/IconA_Frame.tex", JobBars.Configuration.Use4K );

            Border = new SimpleNineGridNode() {
                NodeID = JobBars.NodeId++,
                Position = new( -4, -3 ),
                Offsets = new( 5, 5, 5, 5 ),
                PartsRenderType = PartsRenderType.RenderType,
                NodeFlags = NodeFlags.Visible,
            };
            Border.LoadTexture( "ui/uld/IconA_Frame.tex" );

            Text = new TextNode() {
                NodeID = JobBars.NodeId++,
                FontSize = ( byte )JobBars.Configuration.BuffTextSize_v2,
                LineSpacing = ( byte )JobBars.Configuration.BuffTextSize_v2,
                AlignmentFontType = 52,
                NodeFlags = NodeFlags.Visible,
                TextColor = new( 1, 1, 1, 1 ),
                TextOutlineColor = new( 0, 0, 0, 1 ),
                TextId = 0,
                TextFlags = TextFlags.Glare,
                TextFlags2 = 0,
                Text = "",
            };

            JobBars.NativeController.AttachToNode( [
                Icon,
                Overlay,
                Border,
                Text
            ], this, NodePosition.AsLastChild );

            Update();
        }

        public void Update() {
            Size = new( WIDTH, HEIGHT );
            Text.Size = new( WIDTH, HEIGHT );
            Icon.Size = new( WIDTH, HEIGHT );
            Overlay.Width = WIDTH;

            Icon.TextureCoordinates = new( ( 40 - WIDTH ) / 2, ( 40 - HEIGHT ) / 2 );
            Icon.TextureSize = new( WIDTH, HEIGHT );

            Border.Size = new( WIDTH + 8, HEIGHT + 8 );
            Border.TextureCoordinates = JobBars.Configuration.BuffThinBorder ? new( 0, 96 ) : new( 252, 12 );
            Border.TextureSize = JobBars.Configuration.BuffThinBorder ? new( 48, 48 ) : new( 47, 47 );

            Text.LineSpacing = ( uint )JobBars.Configuration.BuffTextSize_v2;
            Text.FontSize = ( uint )JobBars.Configuration.BuffTextSize_v2;
        }

        public void SetOnCd() {
            MultiplyColor = new( 75f / 255f, 75f / 255f, 75f / 255f );
            Color = Color with {
                W = JobBars.Configuration.BuffOnCDOpacity
            };
        }

        public void SetOffCd() {
            MultiplyColor = new( 100f / 255f, 100f / 255f, 100f / 255f );
            Color = Color with {
                W = 1f
            };
        }

        public void SetPercent( float percent ) {
            Overlay.Size = new( WIDTH, HEIGHT * percent );
            Overlay.Position = new( 0, HEIGHT * ( 1f - percent ) );
        }

        public void LoadIcon( ActionIds action ) {
            if( action == LastAction ) return;
            LastAction = action;
            Icon.IconId = UiHelper.GetIcon( action );
        }

        public void SetText( string text ) {
            if( text != CurrentText ) {
                Text.Text = text;
                CurrentText = text;
            }
            Text.IsVisible = true;
        }

        public void SetColor( ElementColor color ) {
            if( JobBars.Configuration.BuffThinBorder ) color.AddBlue -= 40;
            color.SetColor( Border );
        }

        protected override void Dispose( bool disposing ) {
            if( disposing ) {
                Text.Dispose();
                Overlay.Dispose();
                Icon.Dispose();
                Border.Dispose();
                base.Dispose( disposing );
            }
        }
    }
}
