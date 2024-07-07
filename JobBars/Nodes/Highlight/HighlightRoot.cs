using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Nodes.Highlight {
    public unsafe class HighlightRoot : NodeBase<AtkResNode> {
        public readonly List<HighlightNode> Highlights = [];

        public HighlightRoot() : base( NodeType.Res ) {
            NodeID = JobBars.NodeId++;
            NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop;
            Size = new( 100, 100 );
            Position = new( -12, 19 );

            for( var i = 0; i < 8; i++ ) {
                Highlights.Add( new HighlightNode() );
                Highlights[i].Position = new( 0, 40 * i );
            }
            JobBars.NativeController.AttachToNode( Highlights.Select( x => ( NodeBase )x ).ToList(), this, NodePosition.AsLastChild );
        }

        public void HideAll() {
            foreach( var item in Highlights ) item.IsVisible = false;
        }

        protected override void Dispose( bool disposing ) {
            if( disposing ) {
                foreach( var item in Highlights ) item.Dispose();
                base.Dispose( disposing );
            }
        }
    }
}
