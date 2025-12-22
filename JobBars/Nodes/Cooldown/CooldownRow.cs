using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Nodes.Cooldown {
    public unsafe class CooldownRow : NodeBase<AtkResNode> {
        public readonly List<CooldownNode> Nodes = [];

        public static readonly int MAX_ITEMS = 10;

        public CooldownRow() : base( NodeType.Res ) {
            Size = new( ( 3 + CooldownNode.WIDTH ) * MAX_ITEMS, CooldownNode.HEIGHT );

            for( var idx = 0; idx < MAX_ITEMS; idx++ ) {
                var node = new CooldownNode {
                    Position = new( -( 5 + CooldownNode.WIDTH ) * idx, 0 )
                };
                Nodes.Add( node );
            }

            Nodes.ForEach( x => x.AttachNode(this) );
        }
    }
}
