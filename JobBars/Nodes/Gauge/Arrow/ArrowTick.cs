using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace JobBars.Nodes.Gauge.Arrow {
    public unsafe class ArrowTick : NodeBase<AtkResNode> {
        public readonly ImageNode Background;
        public readonly ResNode SelectedContainer;
        public readonly ImageNode Selected;

        private ElementColor TickColor = ColorConstants.NoColor;

        public ArrowTick() : base( NodeType.Res ) {
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
            Background.LoadTexture( "ui/uld/JobHudSimple_StackB.tex", JobBars.Configuration.Use4K ? 2u : 1u );

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
            Selected.LoadTexture( "ui/uld/JobHudSimple_StackB.tex", JobBars.Configuration.Use4K ? 2u : 1u );

            JobBars.NativeController.AttachToNode( [
                Selected,
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
                base.Dispose( disposing );
            }
        }
    }
}
