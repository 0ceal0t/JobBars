using JobBars.Buffs.Manager;
using KamiToolKit.Enums;
using KamiToolKit.Overlay.UiOverlay;
using System.Collections.Generic;

namespace JobBars.Nodes.Buff {
    public unsafe class BuffRoot : OverlayNode {
        public override OverlayLayer OverlayLayer => OverlayLayer.AboveUserInterface;

        public readonly List<BuffNode> Buffs = [];

        public static readonly int MAX_BUFFS = 15;
        public static int BUFFS_HORIZONTAL => JobBars.Configuration.BuffHorizontal;

        private readonly BuffManager Manager;

        public BuffRoot( BuffManager manager ) {
            Manager = manager;
            Size = new( 256, 100 );

            for( var i = 0; i < MAX_BUFFS; i++ ) Buffs.Add( new BuffNode() );

            Buffs.ForEach( x => x.AttachNode(this) );
        }

        public void UpdateSettings() {
            for( var idx = 0; idx < Buffs.Count; idx++ ) {
                var position_x = BUFFS_HORIZONTAL == 0 ? 0 : idx % BUFFS_HORIZONTAL;
                var position_y = BUFFS_HORIZONTAL == 0 ? 0 : ( idx - position_x ) / BUFFS_HORIZONTAL;

                var xMod = JobBars.Configuration.BuffRightToLeft ? -1 : 1;
                var yMod = JobBars.Configuration.BuffBottomToTop ? -1 : 1;

                Buffs[idx].Position = new( xMod * ( BuffNode.WIDTH + 9 ) * position_x, yMod * ( BuffNode.HEIGHT + 7 ) * position_y );
            }
            foreach( var buff in Buffs ) buff.UpdateSettings();
        }

        protected override void OnUpdate() => Manager.Tick();
    }
}
