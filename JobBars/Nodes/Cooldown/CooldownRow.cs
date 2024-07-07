using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Nodes.Cooldown {
    public unsafe class CooldownRow : NodeBase<AtkResNode> {
        public readonly List<CooldownNode> Nodes = [];

        public static readonly int MAX_ITEMS = 10;

        public CooldownRow() : base( NodeType.Res ) {
            NodeID = JobBars.NodeId++;
            Size = new( ( 3 + CooldownNode.WIDTH ) * MAX_ITEMS, CooldownNode.HEIGHT );

            for( var idx = 0; idx < MAX_ITEMS; idx++ ) {
                var node = new CooldownNode {
                    Position = new( -( 5 + CooldownNode.WIDTH ) * idx, 0 )
                };
                Nodes.Add( node );
            }
            JobBars.NativeController.AttachToNode( Nodes.Select( x => ( NodeBase )x ).ToList(), this, NodePosition.AsLastChild );
        }

        protected override void Dispose( bool disposing ) {
            if( disposing ) {
                foreach( var buff in Nodes ) buff.Dispose();
                base.Dispose( disposing );
            }
        }
    }
}
