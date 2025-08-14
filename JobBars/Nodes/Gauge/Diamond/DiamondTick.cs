using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
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
            NodeID = JobBars.NodeId++;
            Size = new( 32, 32 );

            Background = new ImageNode() {
                NodeID = JobBars.NodeId++,
                Size = new( 32, 32 ),
                TextureCoordinates = new( 0, 0 ),
                TextureSize = new( 32, 32 ),
                NodeFlags = NodeFlags.Visible,
                WrapMode = WrapMode.Unknown,
                ImageNodeFlags = 0,
            };
            Background.LoadTexture( "ui/uld/JobHudSimple_StackA.tex", JobBars.Configuration.Use4K ? 2u : 1u );

            SelectedContainer = new ResNode() {
                NodeID = JobBars.NodeId++,
                Size = new( 32, 32 ),
                Origin = new( 16, 16 ),
                NodeFlags = NodeFlags.Visible,
            };

            Selected = new ImageNode() {
                NodeID = JobBars.NodeId++,
                Size = new( 32, 32 ),
                Origin = new( 16, 16 ),
                TextureCoordinates = new( 32, 0 ),
                TextureSize = new( 32, 32 ),
                NodeFlags = NodeFlags.Visible,
                WrapMode = WrapMode.Unknown,
                ImageNodeFlags = 0,
            };
            Selected.LoadTexture( "ui/uld/JobHudSimple_StackA.tex", JobBars.Configuration.Use4K ? 2u : 1u );

            Text = new TextNode() {
                NodeID = JobBars.NodeId++,
                Position = new( 0, 20 ),
                Size = new( 32, 32 ),
                FontSize = 14,
                LineSpacing = 14,
                AlignmentFontType = 4,
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorRight,
                TextColor = new( 1, 1, 1, 1 ),
                TextOutlineColor = new( 40f / 255f, 40f / 255f, 40f / 255f, 1 ),
                TextId = 0,
                TextFlags = TextFlags.Glare,
                Text = "",
            };

            JobBars.NativeController.AttachToNode( [
                Selected,
                Text
            ], SelectedContainer, NodePosition.AsLastChild );

            JobBars.NativeController.AttachToNode( [
                Background,
                SelectedContainer,
            ], this, NodePosition.AsLastChild );
        }

        public void SetColor( ElementColor color ) {
            TickColor = color;
            TickColor.SetColor( Selected );
        }

        public void Tick( float percent ) => TickColor.SetColorPulse( Selected, percent );

        protected override void Dispose( bool disposing ) {
            if( disposing ) {
                Background.Dispose();
                SelectedContainer.Dispose();
                Selected.Dispose();
                Text.Dispose();
                base.Dispose( disposing );
            }
        }
    }
}
