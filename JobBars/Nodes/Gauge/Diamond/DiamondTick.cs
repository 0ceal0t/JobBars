using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Simple;

namespace JobBars.Nodes.Gauge.Diamond {
    public unsafe class DiamondTick : SimpleOverlayNode {
        public readonly ImageNode Background;
        public readonly SimpleOverlayNode SelectedContainer;
        public readonly ImageNode Selected;
        public readonly TextNode Text;

        private ElementColor TickColor = ColorConstants.NoColor;

        public DiamondTick() {
            Size = new( 32, 32 );

            Background = new SimpleImageNode() {
                Size = new( 32, 32 ),
                TextureCoordinates = new( 0, 0 ),
                TextureSize = new( 32, 32 ),
                WrapMode = WrapMode.Tile,
                ImageNodeFlags = 0,
                TexturePath = "ui/uld/JobHudSimple_StackA.tex"
            };

            SelectedContainer = new SimpleOverlayNode() {
                Size = new( 32, 32 ),
                Origin = new( 16, 16 ),
            };

            Selected = new SimpleImageNode() {
                Size = new( 32, 32 ),
                Origin = new( 16, 16 ),
                TextureCoordinates = new( 32, 0 ),
                TextureSize = new( 32, 32 ),
                WrapMode = WrapMode.Tile,
                ImageNodeFlags = 0,
                TexturePath = "ui/uld/JobHudSimple_StackA.tex"
            };

            Text = new TextNode() {
                Position = new( 0, 20 ),
                Size = new( 32, 32 ),
                FontSize = 14,
                LineSpacing = 14,
                TextColor = new( 1, 1, 1, 1 ),
                TextOutlineColor = new( 40f / 255f, 40f / 255f, 40f / 255f, 1 ),
                TextFlags = TextFlags.Glare,
            };
            Text.Node->AlignmentFontType = 4;

            Background.AttachNode( this );
            SelectedContainer.AttachNode( this );

            Selected.AttachNode( SelectedContainer );
            Text.AttachNode( SelectedContainer );
        }

        public void SetColor( ElementColor color ) {
            TickColor = color;
            TickColor.SetColor( Selected );
        }
    }
}
