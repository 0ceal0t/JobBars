using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Cooldowns.Manager;
using KamiToolKit.Enums;
using KamiToolKit.Overlay.UiOverlay;
using System.Collections.Generic;

namespace JobBars.Nodes.Cooldown {
    public unsafe class CooldownRoot : OverlayNode {
        public override OverlayLayer OverlayLayer => OverlayLayer.AboveUserInterface;

        public readonly List<CooldownRow> Rows = [];

        public static readonly int MAX_BUFFS = 25;
        public static int BUFFS_HORIZONTAL => JobBars.Configuration.BuffHorizontal;

        private readonly CooldownManager Manager;

        public CooldownRoot( CooldownManager manager ) {
            Manager = manager;
            Size = new( 100, 100 );

            Position = JobBars.Configuration.CooldownPosition;
            NodeFlags = NodeFlags.Visible;

            for( var i = 0; i < 8; i++ ) Rows.Add( new CooldownRow() );

            Rows.ForEach( x => x.AttachNode(this) );

            UpdateSpacing();
        }

        public void UpdateSpacing() {
            for( var i = 0; i < 8; i++ ) Rows[i].Position = new( 0, JobBars.Configuration.CooldownsSpacing * i );
        }

        public void SetCooldownRowVisible( int idx, bool visible ) => Rows[idx].IsVisible = visible;

        protected override void OnUpdate() => Manager.Tick();
    }
}
