using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace JobBars.Nodes.Gauge.Diamond {
    public unsafe class DiamondTick : NodeBase<AtkResNode> {
        public readonly ImageNode Background;
        public readonly ResNode SelectedContainer;
        public readonly ImageNode Selected;
        public readonly TextNode Text;

        private ElementColor TickColor = ColorConstants.NoColor;

        public DiamondTick() : base( NodeType.Res ) {
            Size = new( 32, 32 );

            Background = new SimpleImageNode() {
                Size = new( 32, 32 ),
                TextureCoordinates = new( 0, 0 ),
                TextureSize = new( 32, 32 ),
                NodeFlags = NodeFlags.Visible,
                WrapMode = WrapMode.Tile,
                ImageNodeFlags = 0,
                TexturePath = "ui/uld/JobHudSimple_StackA.tex"
            };

            SelectedContainer = new ResNode() {
                Size = new( 32, 32 ),
                Origin = new( 16, 16 ),
                NodeFlags = NodeFlags.Visible,
            };

            Selected = new SimpleImageNode() {
                Size = new( 32, 32 ),
                Origin = new( 16, 16 ),
                TextureCoordinates = new( 32, 0 ),
                TextureSize = new( 32, 32 ),
                NodeFlags = NodeFlags.Visible,
                WrapMode = WrapMode.Tile,
                ImageNodeFlags = 0,
                TexturePath = "ui/uld/JobHudSimple_StackA.tex"
            };

            Text = new TextNode() {
                Position = new( 0, 20 ),
                Size = new( 32, 32 ),
                FontSize = 14,
                LineSpacing = 14,
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorRight,
                TextColor = new( 1, 1, 1, 1 ),
                TextOutlineColor = new( 40f / 255f, 40f / 255f, 40f / 255f, 1 ),
                TextId = 0,
                TextFlags = TextFlags.Glare,
                String = "",
            };
            Text.Node->AlignmentFontType = 4;

            Selected.AttachNode( SelectedContainer );
            Text.AttachNode( SelectedContainer );

            Background.AttachNode( this );
            SelectedContainer.AttachNode( this );
        }

        public void SetColor( ElementColor color ) {
            TickColor = color;
            TickColor.SetColor( Selected );
        }

        public void Tick( float percent ) => TickColor.SetColorPulse( Selected, percent );
    }
}
