using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Nodes.Buff {
    public unsafe class BuffRoot : NodeBase<AtkResNode> {
        public readonly List<BuffNode> Buffs = [];

        public static readonly int MAX_BUFFS = 25;
        public static int BUFFS_HORIZONTAL => JobBars.Configuration.BuffHorizontal;

        public BuffRoot() : base( NodeType.Res ) {
            NodeID = JobBars.NodeId++;
            Size = new( 256, 100 );

            for( var i = 0; i < MAX_BUFFS; i++ ) Buffs.Add( new BuffNode() );
            JobBars.NativeController.AttachToNode( Buffs.Select( x => ( NodeBase )x ).ToList(), this, NodePosition.AsLastChild );

            Update();
        }

        public void Update() {
            for( var idx = 0; idx < Buffs.Count; idx++ ) {
                var position_x = BUFFS_HORIZONTAL == 0 ? 0 : idx % BUFFS_HORIZONTAL;
                var position_y = BUFFS_HORIZONTAL == 0 ? 0 : ( idx - position_x ) / BUFFS_HORIZONTAL;

                var xMod = JobBars.Configuration.BuffRightToLeft ? -1 : 1;
                var yMod = JobBars.Configuration.BuffBottomToTop ? -1 : 1;

                Buffs[idx].Position = new( xMod * ( BuffNode.WIDTH + 9 ) * position_x, yMod * ( BuffNode.HEIGHT + 7 ) * position_y );
            }
            foreach( var buff in Buffs ) buff.Update();
        }

        protected override void Dispose( bool disposing ) {
            if( disposing ) {
                foreach( var buff in Buffs ) buff.Dispose();
                base.Dispose( disposing );
            }
        }
    }
}
