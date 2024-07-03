using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;

namespace JobBars.Atk {
    public unsafe partial class AtkBuilder {
        private static readonly uint NODE_IDX_START = 89990001;
        private static uint NodeIdx = NODE_IDX_START;

        public AtkBuilder() {
            NodeIdx = NODE_IDX_START;
            InitGauges();
            InitBuffs();
            InitCooldowns();
            InitCursor();

            AtkHelper.Link( GaugeRoot, BuffRoot );
            AtkHelper.Link( BuffRoot, CursorRoot );
        }

        public void Dispose() {
            AtkHelper.Detach( GaugeRoot );
            AtkHelper.Detach( CooldownRoot );

            DisposeCooldowns();
            DisposeGauges();
            DisposeBuffs();
            DisposeCursor();

            var attachAddon = AtkHelper.BuffGaugeAttachAddon;
            if( attachAddon != null ) attachAddon->UldManager.UpdateDrawNodeList();

            var cooldownAddon = AtkHelper.CooldownAttachAddon;
            if( cooldownAddon != null ) cooldownAddon->UldManager.UpdateDrawNodeList();

            var partyListAddon = AtkHelper.PartyListAddon;
            if( partyListAddon != null ) partyListAddon->AtkUnitBase.UldManager.UpdateDrawNodeList();
        }

        public void Attach() {
            var buffGaugeAddon = AtkHelper.BuffGaugeAttachAddon;
            var cooldownAddon = AtkHelper.CooldownAttachAddon;
            var partyListAddon = AtkHelper.PartyListAddon;

            Dalamud.Log( $"Gauges={buffGaugeAddon != null} PartyList={partyListAddon != null} Cooldowns={cooldownAddon != null}" );

            // ===== CONTAINERS =========

            GaugeRoot->ParentNode = buffGaugeAddon->RootNode;
            BuffRoot->ParentNode = buffGaugeAddon->RootNode;
            CursorRoot->ParentNode = buffGaugeAddon->RootNode;

            AtkHelper.Attach( buffGaugeAddon, GaugeRoot );

            Dalamud.Log( "Attached Gauges" );

            // ===== BUFF PARTYLIST ======

            for( var i = 0; i < PartyListBuffs.Count; i++ ) {
                var partyMember = partyListAddon->PartyMembers[i];
                PartyListBuffs[i].AttachTo( partyMember.TargetGlowContainer );
                partyMember.PartyMemberComponent->UldManager.UpdateDrawNodeList();
            }

            Dalamud.Log( "Attached PartyList" );

            // ===== COOLDOWNS =========

            CooldownRoot->ParentNode = cooldownAddon->RootNode;

            AtkHelper.Attach( cooldownAddon, CooldownRoot );

            Dalamud.Log( "Attached Cooldowns" );

            // ======================

            buffGaugeAddon->UldManager.UpdateDrawNodeList();

            Dalamud.Log( "Updated Gauges" );

            cooldownAddon->UldManager.UpdateDrawNodeList();

            Dalamud.Log( "Updated PartyList" );

            partyListAddon->AtkUnitBase.UldManager.UpdateDrawNodeList();

            Dalamud.Log( "Updated Cooldowns" );
        }

        public void Tick( float percent ) {
            Arrows.ForEach( x => x.Tick( percent ) );
            Diamonds.ForEach( x => x.Tick( percent ) );
        }

        // ==== HELPER FUNCTIONS ============

        private void SetPosition( AtkResNode* node, float X, float Y ) {
            var addon = AtkHelper.BuffGaugeAttachAddon;
            if( addon == null ) return;
            var p = AtkHelper.GetNodePosition( addon->RootNode );
            var pScale = AtkHelper.GetNodeScale( addon->RootNode );
            AtkHelper.SetPosition( node, ( X - p.X ) / pScale.X, ( Y - p.Y ) / pScale.Y );
        }

        private void SetScale( AtkResNode* node, float X, float Y ) {
            var addon = AtkHelper.BuffGaugeAttachAddon;
            if( addon == null ) return;
            var p = AtkHelper.GetNodeScale( addon->RootNode );
            AtkHelper.SetScale( node, X / p.X, Y / p.Y );
        }
    }
}
