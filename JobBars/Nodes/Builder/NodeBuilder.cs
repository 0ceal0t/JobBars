using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using JobBars.Nodes.Buff;
using JobBars.Nodes.Cooldown;
using JobBars.Nodes.Cursor;
using JobBars.Nodes.Gauge;
using JobBars.Nodes.Highlight;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using System.Collections.Generic;
using System.Numerics;

namespace JobBars.Nodes.Builder {
    public unsafe partial class NodeBuilder {
        private readonly HashSet<string> IsAttached = [];
        public bool IsLoaded => IsAttached.Count > 0;

        public BuffRoot BuffRoot { get; private set; }
        public CursorRoot CursorRoot { get; private set; }
        public CooldownRoot CooldownRoot { get; private set; }
        public GaugeRoot GaugeRoot { get; private set; }
        public HighlightRoot HighlightRoot { get; private set; }

        public NodeBuilder() { }

        public void Load() {
            Dalamud.AddonLifecycle.RegisterListener( AddonEvent.PostSetup, "_PartyList", OnPartyListSetup );
            Dalamud.AddonLifecycle.RegisterListener( AddonEvent.PreFinalize, "_PartyList", OnPartyListFinalize );

            Dalamud.AddonLifecycle.RegisterListener( AddonEvent.PostSetup, "ChatLog", OnChatLogSetup );
            Dalamud.AddonLifecycle.RegisterListener( AddonEvent.PreFinalize, "ChatLog", OnChatLogFinalize );

            Dalamud.AddonLifecycle.RegisterListener( AddonEvent.PostSetup, "_ParameterWidget", OnParametersSetup );
            Dalamud.AddonLifecycle.RegisterListener( AddonEvent.PreFinalize, "_ParameterWidget", OnParametersFinalize );

            if( UiHelper.PartyListAddon is not null ) AttachToNative( ( AtkUnitBase* )UiHelper.PartyListAddon, "_PartyList" );
            if( UiHelper.ChatLogAddon is not null ) AttachToNative( UiHelper.ChatLogAddon, "ChatLog" );
            if( UiHelper.ParameterAddon is not null ) AttachToNative( UiHelper.ParameterAddon, "_ParameterWidget" );
        }

        public void Unload() {
            Dalamud.AddonLifecycle.UnregisterListener( OnPartyListSetup );
            Dalamud.AddonLifecycle.UnregisterListener( OnPartyListFinalize );

            Dalamud.AddonLifecycle.UnregisterListener( OnChatLogSetup );
            Dalamud.AddonLifecycle.UnregisterListener( OnChatLogFinalize );

            Dalamud.AddonLifecycle.UnregisterListener( OnParametersSetup );
            Dalamud.AddonLifecycle.UnregisterListener( OnParametersFinalize );

            if( UiHelper.PartyListAddon is not null ) DetachFromNative( ( AtkUnitBase* )UiHelper.PartyListAddon, "_PartyList" );
            if( UiHelper.ChatLogAddon is not null ) DetachFromNative( UiHelper.ChatLogAddon, "ChatLog" );
            if( UiHelper.ParameterAddon is not null ) DetachFromNative( UiHelper.ParameterAddon, "_ParameterWidget" );
        }

        public void Dispose() {
            Unload();
        }

        private void OnPartyListSetup( AddonEvent _, AddonArgs args ) => AttachToNative( ( AtkUnitBase* )args.Addon, "_PartyList" );
        private void OnPartyListFinalize( AddonEvent _, AddonArgs args ) => DetachFromNative( ( AtkUnitBase* )args.Addon, "_PartyList" );

        private void OnChatLogSetup( AddonEvent _, AddonArgs args ) => AttachToNative( ( AtkUnitBase* )args.Addon, "ChatLog" );
        private void OnChatLogFinalize( AddonEvent _, AddonArgs args ) => DetachFromNative( ( AtkUnitBase* )args.Addon, "ChatLog" );

        private void OnParametersSetup( AddonEvent _, AddonArgs args ) => AttachToNative( ( AtkUnitBase* )args.Addon, "_ParameterWidget" );
        private void OnParametersFinalize( AddonEvent _, AddonArgs args ) => DetachFromNative( ( AtkUnitBase* )args.Addon, "_ParameterWidget" );

        private void AttachToNative( AtkUnitBase* addon, string name ) {
            Dalamud.Framework.RunOnFrameworkThread( () => {
                if( IsAttached.Contains( name ) ) return;

                if( name == UiHelper.BuffGaugeAttachAddonName ) {
                    BuffRoot = new();
                    CursorRoot = new();
                    GaugeRoot = new();

                    JobBars.GaugeManager?.Reset();

                    JobBars.NativeController.AttachToAddon( BuffRoot, addon, addon->RootNode, NodePosition.AsLastChild, false, false );
                    JobBars.NativeController.AttachToAddon( CursorRoot, addon, addon->RootNode, NodePosition.AsLastChild, false, false );
                    JobBars.NativeController.AttachToAddon( GaugeRoot, addon, addon->RootNode, NodePosition.AsLastChild, false, false );
                }

                if( name == UiHelper.CooldownAttachAddonName ) {
                    CooldownRoot = new();

                    JobBars.NativeController.AttachToAddon( CooldownRoot, addon, addon->RootNode, NodePosition.AsLastChild, false, false );
                }

                if( name == "_PartyList" ) {
                    HighlightRoot = new();

                    JobBars.NativeController.AttachToAddon( HighlightRoot, addon, addon->GetNodeById( 20 ), NodePosition.AfterTarget, false, false );
                }

                IsAttached.Add( name );
            } );
        }

        private void DetachFromNative( AtkUnitBase* addon, string name ) {
            Dalamud.Framework.RunOnFrameworkThread( () => {
                if( !IsAttached.Contains( name ) ) return;

                if( name == UiHelper.BuffGaugeAttachAddonName ) {
                    if( BuffRoot != null ) {
                        JobBars.NativeController.DetachFromAddon( BuffRoot, addon, false, false );
                        BuffRoot.Dispose();
                        BuffRoot = null;
                    }
                    if( CursorRoot != null ) {
                        JobBars.NativeController.DetachFromAddon( CursorRoot, addon, false, false );
                        CursorRoot.Dispose();
                        CursorRoot = null;
                    }
                    if( GaugeRoot != null ) {
                        JobBars.NativeController.DetachFromAddon( GaugeRoot, addon, false, false );
                        GaugeRoot.Dispose();
                        GaugeRoot = null;
                    }
                }

                if( name == UiHelper.CooldownAttachAddonName ) {
                    if( CooldownRoot != null ) {
                        JobBars.NativeController.DetachFromAddon( CooldownRoot, addon, false, false );
                        CooldownRoot.Dispose();
                        CooldownRoot = null;
                    }
                }

                if( name == "_PartyList" ) {
                    if( HighlightRoot != null ) {
                        JobBars.NativeController.DetachFromAddon( HighlightRoot, addon, false, false );
                        HighlightRoot.Dispose();
                        HighlightRoot = null;
                    }
                }

                IsAttached.Remove( name );
            } );
        }

        public void Tick( float percent ) {
            GaugeRoot?.Arrows.ForEach( x => x.Tick( percent ) );
            GaugeRoot?.Diamonds.ForEach( x => x.Tick( percent ) );
        }

        // ==== HELPER FUNCTIONS ============

        public static void SetPositionGlobal( NodeBase node, Vector2 v ) => SetPosition( node, v.X, v.Y );

        public static unsafe void SetPosition( NodeBase node, float x, float y ) {
            var addon = UiHelper.BuffGaugeAttachAddon;
            if( addon == null ) return;
            var p = UiHelper.GetGlobalPosition( addon->RootNode );
            var scale = UiHelper.GetGlobalScale( addon->RootNode );
            node.Position = new( ( x - p.X ) / scale.X, ( y - p.Y ) / scale.Y );
        }

        public static void SetScaleGlobal( NodeBase node, float v ) => SetScale( node, v, v );

        public static unsafe void SetScale( NodeBase node, float x, float y ) {
            var addon = UiHelper.BuffGaugeAttachAddon;
            if( addon == null ) return;
            var p = UiHelper.GetGlobalScale( addon->RootNode );
            node.Scale = new( x / p.X, y / p.Y );
        }
    }
}
