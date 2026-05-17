using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.GameStructs;
using KamiToolKit.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Icons.Manager {
    public unsafe partial class IconManager : PerJobManager<IconReplacer[]> {
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

        public JobIds CurrentJob = JobIds.OTHER;
        private IconReplacer[] CurrentIcons => JobToValue.TryGetValue( CurrentJob, out var gauges ) ? gauges : JobToValue[JobIds.OTHER];

        public IconManager() : base( "##JobBars_Icons" ) {
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
                UpdateIcon( slotData );
            }
        }

        public void Dispose() {
            Dalamud.AddonLifecycle.UnregisterListener( ActionBarUpdate );
        }

        public void SetJob( JobIds job ) {
            CurrentJob = job;
        }

        public void Reset() {
            foreach( var item in CurrentIcons ) item.Reset();
        }

        public void ResetJob( JobIds job ) {
            if( job == CurrentJob ) Reset();
        }

        public void PerformAction( Item action ) {
            if( !JobBars.Configuration.IconsEnabled ) return;
            foreach( var icon in CurrentIcons.Where( i => i.Enabled ) ) icon.ProcessAction( action );
        }

        public void Tick() {
            if( !JobBars.Configuration.IconsEnabled ) return;
            foreach( var icon in CurrentIcons.Where( i => i.Enabled ) ) icon.Tick();
        }

        public void UpdateIcon( HotbarSlotStruct* data ) {
            if( !JobBars.Configuration.IconsEnabled ) return;
            CurrentIcons.FirstOrDefault( i => i.AppliesTo( data->ActionId ) )?.UpdateIcon( data );
        }
    }
}
