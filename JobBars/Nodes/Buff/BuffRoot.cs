using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using System.Collections.Generic;
using System.Numerics;

namespace JobBars.Nodes.Buff {
    public class BuffRoot : NodeBase<AtkResNode> {
        public readonly List<BuffNode> Buffs = [];

        public static readonly int MAX_BUFFS = 25;
        public static int BUFFS_HORIZONTAL => JobBars.Configuration.BuffHorizontal;

        public BuffRoot() : base( NodeType.Res ) {
            NodeID = JobBars.NodeId++;
            Size = new( 256, 100 );

            BuffNode lastBuff = null;
            for( var i = 0; i < MAX_BUFFS; i++ ) {
                var newBuff = new BuffNode();
                Buffs.Add( newBuff );
                JobBars.NativeController.AttachToNode(
                    newBuff,
                    lastBuff == null ? this : lastBuff,
                    lastBuff == null ? NodePosition.AsLastChild : NodePosition.AfterTarget
                );
                lastBuff = newBuff;
            }

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

        public void SetPosition( Vector2 v ) => SetPosition( v.X, v.Y );

        public unsafe void SetPosition( float x, float y ) {
            var addon = AtkHelper.BuffGaugeAttachAddon;
            if( addon == null ) return;
            var p = AtkHelper.GetNodePosition( addon->RootNode );
            var scale = AtkHelper.GetNodeScale( addon->RootNode );
            Position = new( ( x - p.X ) / scale.X, ( y - p.Y ) / scale.Y );
        }

        public void SetScale( float v ) => SetScale( v, v );

        public unsafe void SetScale( float x, float y ) {
            var addon = AtkHelper.BuffGaugeAttachAddon;
            if( addon == null ) return;
            var p = AtkHelper.GetNodeScale( addon->RootNode );
            Scale = new( x / p.X, y / p.Y );
        }

        protected override void Dispose( bool disposing ) {
            if( disposing ) {
                foreach( var buff in Buffs ) buff.Dispose();
                base.Dispose( disposing );
            }
        }
    }
}
