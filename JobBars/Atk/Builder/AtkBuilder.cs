using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using JobBars.Nodes.Buff;
using JobBars.Nodes.Cursor;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using System.Numerics;

namespace JobBars.Atk {
    public unsafe partial class AtkBuilder {
        private static readonly uint NODE_IDX_START = 89995001;
        private static uint NodeIdx = NODE_IDX_START;

        public BuffRoot BuffRoot { get; private set; }
        public CursorRoot CursorRoot { get; private set; }

        public AtkBuilder() {
            NodeIdx = NODE_IDX_START;

            InitGauges();
            InitBuffs();
            InitCooldowns();

            BuffRoot = new();
            CursorRoot = new();
        }

        public void Dispose() {
            AtkHelper.Detach( GaugeRoot );
            AtkHelper.Detach( CooldownRoot );

            JobBars.NativeController.DetachFromAddon( BuffRoot, AtkHelper.BuffGaugeAttachAddon );
            JobBars.NativeController.DetachFromAddon( CursorRoot, AtkHelper.BuffGaugeAttachAddon );

            DisposeCooldowns();
            DisposeGauges();

            BuffRoot.Dispose();
            CursorRoot.Dispose();

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
            //BuffRoot->ParentNode = buffGaugeAddon->RootNode;
            //CursorRoot->ParentNode = buffGaugeAddon->RootNode;

            GaugeRoot->Timeline = buffGaugeAddon->RootNode->Timeline;
            // BuffRoot->Timeline = buffGaugeAddon->RootNode->Timeline;
            //CursorRoot->Timeline = buffGaugeAddon->RootNode->Timeline;
            AtkHelper.Attach( buffGaugeAddon, GaugeRoot );

            JobBars.NativeController.AttachToAddon( BuffRoot, buffGaugeAddon, buffGaugeAddon->RootNode, NodePosition.AsLastChild );
            JobBars.NativeController.AttachToAddon( CursorRoot, buffGaugeAddon, buffGaugeAddon->RootNode, NodePosition.AsLastChild );

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
            CooldownRoot->Timeline = cooldownAddon->RootNode->Timeline;
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

        public static void SetPosition( NodeBase node, Vector2 v ) => SetPosition( node, v.X, v.Y );

        public static unsafe void SetPosition( NodeBase node, float x, float y ) {
            var addon = AtkHelper.BuffGaugeAttachAddon;
            if( addon == null ) return;
            var p = AtkHelper.GetNodePosition( addon->RootNode );
            var scale = AtkHelper.GetNodeScale( addon->RootNode );
            node.Position = new( ( x - p.X ) / scale.X, ( y - p.Y ) / scale.Y );
        }

        public static void SetScale( NodeBase node, float v ) => SetScale( node, v, v );

        public static unsafe void SetScale( NodeBase node, float x, float y ) {
            var addon = AtkHelper.BuffGaugeAttachAddon;
            if( addon == null ) return;
            var p = AtkHelper.GetNodeScale( addon->RootNode );
            node.Scale = new( x / p.X, y / p.Y );
        }
    }
}
