using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System.Collections.Generic;
using System.Numerics;

namespace JobBars.Atk {
    public unsafe partial class AtkBuilder {
        public List<AtkCooldown> Cooldowns = [];
        private AtkResNode* CooldownRoot = null;

        private void InitCooldowns() {
            CooldownRoot = CreateResNode();
            CooldownRoot->Width = 100;
            CooldownRoot->Height = 100;
            CooldownRoot->NodeFlags = NodeFlags.Visible | NodeFlags.Fill | NodeFlags.AnchorLeft | NodeFlags.AnchorTop | NodeFlags.Enabled | NodeFlags.EmitsEvents;
            CooldownRoot->DrawFlags = 0;
            AtkHelper.SetPosition( CooldownRoot, -40, 40 );

            AtkCooldown lastCooldown = null;
            for( var i = 0; i < 8; i++ ) {
                var newItem = new AtkCooldown();

                Cooldowns.Add( newItem );
                newItem.RootRes->ParentNode = CooldownRoot;

                if( lastCooldown != null ) AtkHelper.Link( lastCooldown.RootRes, newItem.RootRes );
                lastCooldown = newItem;
            }

            CooldownRoot->ChildCount = ( ushort )( ( 1 + Cooldowns[0].RootRes->ChildCount ) * Cooldowns.Count );
            CooldownRoot->ChildNode = Cooldowns[0].RootRes;

            RefreshCooldownsLayout();
        }

        public void RefreshCooldownsLayout() {
            for( var i = 0; i < Cooldowns.Count; i++ ) {
                AtkHelper.SetPosition( Cooldowns[i].RootRes, 0, JobBars.Configuration.CooldownsSpacing * i );
            }
        }

        private void DisposeCooldowns() {
            foreach( var cd in Cooldowns ) cd.Dispose();
            Cooldowns = null;

            CooldownRoot->Destroy( true );
            CooldownRoot = null;
        }

        public void SetCooldownPosition( Vector2 pos ) => AtkHelper.SetPosition( CooldownRoot, pos.X, pos.Y );
        public void SetCooldownScale( float scale ) => AtkHelper.SetScale( CooldownRoot, scale, scale );
        public void SetCooldownRowVisible( int idx, bool visible ) => AtkHelper.SetVisibility( Cooldowns[idx].RootRes, visible );
        public void ShowCooldowns() => AtkHelper.Show( CooldownRoot );
        public void HideCooldowns() => AtkHelper.Hide( CooldownRoot );
    }
}
