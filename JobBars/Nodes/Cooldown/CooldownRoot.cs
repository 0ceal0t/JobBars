using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Nodes.Cooldown {
    public unsafe class CooldownRoot : NodeBase<AtkResNode> {
        public readonly List<CooldownRow> Rows = [];

        public static readonly int MAX_BUFFS = 25;
        public static int BUFFS_HORIZONTAL => JobBars.Configuration.BuffHorizontal;

        public CooldownRoot() : base( NodeType.Res ) {
            NodeID = JobBars.NodeId++;
            Size = new( 100, 100 );
            Position = new( -40, -40 );
            NodeFlags = NodeFlags.Visible;

            for( var i = 0; i < 8; i++ ) Rows.Add( new CooldownRow() );
            JobBars.NativeController.AttachToNode( Rows.Select( x => ( NodeBase )x ).ToList(), this, NodePosition.AsLastChild );

            Update();
        }

        public void Update() {
            for( var i = 0; i < 8; i++ ) Rows[i].Position = new( 0, JobBars.Configuration.CooldownsSpacing * i );
        }

        public void SetCooldownRowVisible( int idx, bool visible ) => Rows[idx].IsVisible = visible;

        protected override void Dispose( bool disposing ) {
            if( disposing ) {
                foreach( var buff in Rows ) buff.Dispose();
                base.Dispose( disposing );
            }
        }
    }
}
