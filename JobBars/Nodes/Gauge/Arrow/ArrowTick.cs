using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace JobBars.Nodes.Gauge.Arrow {
    public unsafe class ArrowTick : NodeBase<AtkResNode> {
        public readonly ImageNode Background;
        public readonly ResNode SelectedContainer;
        public readonly ImageNode Selected;

        private ElementColor TickColor = ColorConstants.NoColor;

        public ArrowTick() : base( NodeType.Res ) {
            Size = new( 32, 32 );

            Background = new SimpleImageNode() {
                Size = new( 32, 32 ),
                TextureCoordinates = new( 0, 0 ),
                TextureSize = new( 32, 32 ),
                NodeFlags = NodeFlags.Visible,
                WrapMode = WrapMode.Tile,
                ImageNodeFlags = 0,
                TexturePath = "ui/uld/JobHudSimple_StackB.tex"
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
                TexturePath = "ui/uld/JobHudSimple_StackB.tex"
            };

            Selected.AttachNode( SelectedContainer );

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
