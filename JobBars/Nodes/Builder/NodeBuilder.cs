using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.GameStructs;
using JobBars.Helper;
using JobBars.Nodes.Buff;
using JobBars.Nodes.Cooldown;
using JobBars.Nodes.Cursor;
using JobBars.Nodes.Gauge;
using JobBars.Nodes.Highlight;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using System.Collections.Generic;
using System.Numerics;

namespace JobBars.Nodes.Builder {
    public unsafe partial class NodeBuilder {
        public static readonly List<string> AllActionBars = [
            "_ActionBar",
            "_ActionBar01",
            "_ActionBar02",
            "_ActionBar03",
            "_ActionBar04",
            "_ActionBar05",
            "_ActionBar06",
            "_ActionBar07",
            "_ActionBar08",
            "_ActionBar09",
            "_ActionCross", // 10
            "_ActionDoubleCrossL",
            "_ActionDoubleCrossR"
        ];


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

            Dalamud.AddonLifecycle.RegisterListener( AddonEvent.PreRequestedUpdate, AllActionBars, ActionBarUpdate );
        }

        private void ActionBarUpdate( AddonEvent _, AddonArgs args ) {
            var addon = ( AddonActionBarBase* )args.Addon.Address;
            if( args is AddonRequestedUpdateArgs updateArgs ) {
                var data = ( ( NumberArrayData** )updateArgs.NumberArrayData )[7];
                var addonData = ( AddonHotbarNumberArray* )data->IntArray;
                var idx = AllActionBars.IndexOf( args.AddonName );

                if( idx >= 10 ) {
                    for( var hotbarIdx = 10; hotbarIdx < 18; hotbarIdx++ ) ActionBarUpdate( addonData, hotbarIdx );
                }
                else ActionBarUpdate( addonData, idx );
            }
        }

        private void ActionBarUpdate( AddonHotbarNumberArray* addonData, int hotbarIdx ) {
            var hotbarData = ( HotbarSlotStruct* )( ( nint )addonData + 0x3C + sizeof( HotbarStruct ) * hotbarIdx );

            for( var i = 0; i < 16; i++ ) {
                var slotData = ( HotbarSlotStruct* )( ( nint )hotbarData + sizeof( HotbarSlotStruct ) * i );
                JobBars.IconManager?.UpdateIcon( slotData );
            }
        }

        public void Unload() {
            Dalamud.AddonLifecycle.UnregisterListener( OnPartyListSetup );
            Dalamud.AddonLifecycle.UnregisterListener( OnPartyListFinalize );

            Dalamud.AddonLifecycle.UnregisterListener( OnChatLogSetup );
            Dalamud.AddonLifecycle.UnregisterListener( OnChatLogFinalize );

            Dalamud.AddonLifecycle.UnregisterListener( OnParametersSetup );
            Dalamud.AddonLifecycle.UnregisterListener( OnParametersFinalize );

            Dalamud.AddonLifecycle.UnregisterListener( ActionBarUpdate );

            if( UiHelper.PartyListAddon is not null ) DetachFromNative( ( AtkUnitBase* )UiHelper.PartyListAddon, "_PartyList" );
            if( UiHelper.ChatLogAddon is not null ) DetachFromNative( UiHelper.ChatLogAddon, "ChatLog" );
            if( UiHelper.ParameterAddon is not null ) DetachFromNative( UiHelper.ParameterAddon, "_ParameterWidget" );
        }

        public void Dispose() {
            Unload();
        }

        private void OnPartyListSetup( AddonEvent _, AddonArgs args ) => AttachToNative( ( AtkUnitBase* )args.Addon.Address, "_PartyList" );
        private void OnPartyListFinalize( AddonEvent _, AddonArgs args ) => DetachFromNative( ( AtkUnitBase* )args.Addon.Address, "_PartyList" );

        private void OnChatLogSetup( AddonEvent _, AddonArgs args ) => AttachToNative( ( AtkUnitBase* )args.Addon.Address, "ChatLog" );
        private void OnChatLogFinalize( AddonEvent _, AddonArgs args ) => DetachFromNative( ( AtkUnitBase* )args.Addon.Address, "ChatLog" );

        private void OnParametersSetup( AddonEvent _, AddonArgs args ) => AttachToNative( ( AtkUnitBase* )args.Addon.Address, "_ParameterWidget" );
        private void OnParametersFinalize( AddonEvent _, AddonArgs args ) => DetachFromNative( ( AtkUnitBase* )args.Addon.Address, "_ParameterWidget" );

        private void AttachToNative( AtkUnitBase* addon, string name ) {
            Dalamud.Framework.RunOnFrameworkThread( () => {
                if( IsAttached.Contains( name ) ) return;

                if( name == UiHelper.BuffGaugeAttachAddonName ) {
                    BuffRoot = new();
                    CursorRoot = new();
                    GaugeRoot = new();

                    JobBars.GaugeManager?.Reset();

                    BuffRoot.AttachNode( addon->RootNode );
                    CursorRoot.AttachNode( addon->RootNode );
                    GaugeRoot.AttachNode( addon->RootNode );
                }

                if( name == UiHelper.CooldownAttachAddonName ) {
                    CooldownRoot = new();
                    CooldownRoot.AttachNode( addon->RootNode );
                }

                if( name == "_PartyList" ) {
                    HighlightRoot = new();
                    HighlightRoot.AttachNode( addon->GetNodeById( 20 ) );
                }

                IsAttached.Add( name );
            } );
        }

        private void DetachFromNative( AtkUnitBase* addon, string name ) {
            Dalamud.Framework.RunOnFrameworkThread( () => {
                if( !IsAttached.Contains( name ) ) return;

                if( name == UiHelper.BuffGaugeAttachAddonName ) {
                    if( BuffRoot != null ) {
                        BuffRoot.Dispose();
                        BuffRoot = null;
                    }
                    if( CursorRoot != null ) {
                        CursorRoot.Dispose();
                        CursorRoot = null;
                    }
                    if( GaugeRoot != null ) {
                        GaugeRoot.Dispose();
                        GaugeRoot = null;
                    }
                }

                if( name == UiHelper.CooldownAttachAddonName ) {
                    if( CooldownRoot != null ) {
                        CooldownRoot.Dispose();
                        CooldownRoot = null;
                    }
                }

                if( name == "_PartyList" ) {
                    if( HighlightRoot != null ) {
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
