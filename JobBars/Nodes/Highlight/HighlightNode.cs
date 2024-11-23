using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace JobBars.Nodes.Highlight {
    public unsafe class HighlightNode : NodeBase<AtkResNode> {
        private readonly NineGridNode Highlight;

        public HighlightNode() : base( NodeType.Res ) {
            NodeID = JobBars.NodeId++;
            NodeFlags = NodeFlags.AnchorLeft | NodeFlags.AnchorTop;
            Size = new( 100, 100 );

            Highlight = new NineGridNode() {
                NodeID = JobBars.NodeId++,
                Size = new( 320, 48 ),
                Position = new( 52, 18 ),
                TextureCoordinates = new( 112, 0 ),
                TextureSize = new( 48, 48 ),
                Offsets = new( 20, 20, 20, 20 ),
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
                MultiplyColor = new( 150f / 255f, 100f / 255f, 50 / 255f ),
            };
            Highlight.LoadTexture( "ui/uld/PartyListTargetBase.tex" );

            JobBars.NativeController.AttachToNode( [
                Highlight
            ], this, NodePosition.AsLastChild );
        }

        protected override void Dispose( bool disposing ) {
            if( disposing ) {
                Highlight.Dispose();
                base.Dispose( disposing );
            }
        }
    }
}
