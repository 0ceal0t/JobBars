using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.GameStructs;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Atk {
    public unsafe class AtkIconBuilder {
        private readonly string[] AllActionBars = [
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
            "_ActionCross",
            "_ActionDoubleCrossL",
            "_ActionDoubleCrossR"
        ];
        private static readonly int MILLIS_LOOP = 250;

        private readonly Dictionary<uint, UIIconProps> IconConfigs = [];
        private readonly List<AtkIcon> Icons = [];
        private readonly HashSet<IntPtr> IconOverride = [];

        private struct CreateIconStruct {
            public uint Action;
            public uint ActionId;
            public int HotBarIndex;
            public int SlotIndex;
            public AtkComponentNode* Component;
            public UIIconProps Props;
        }

        public AtkIconBuilder() {
        }

        public void Setup( List<uint> triggers, UIIconProps props ) {
            if( triggers == null ) return;

            foreach( var trigger in triggers ) {
                IconConfigs[trigger] = props;
            }
        }

        public void SetProgress( List<uint> triggers, float current, float max ) {
            foreach( var icon in Icons ) {
                if( !triggers.Contains( icon.AdjustedId ) ) continue;
                icon.SetProgress( current, max );
            }
        }

        public void SetDone( List<uint> triggers ) {
            foreach( var icon in Icons ) {
                if( !triggers.Contains( icon.AdjustedId ) ) continue;
                icon.SetDone();
            }
        }

        public void Tick() {
            var time = DateTime.Now;
            var millis = time.Second * 1000 + time.Millisecond;
            var percent = ( float )( millis % MILLIS_LOOP ) / MILLIS_LOOP;

            var hotbarData = AtkHelper.GetHotbarUI();
            if( hotbarData == null ) return;

            HashSet<AtkIcon> foundIcons = [];
            HashSet<CreateIconStruct> createIcons = [];

            for( var hotbarIndex = 0; hotbarIndex < AllActionBars.Length; hotbarIndex++ ) {
                var actionBar = ( AddonActionBarBase* )AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonByName( AllActionBars[hotbarIndex] );
                if( actionBar == null || actionBar->ActionBarSlotsAction == null || actionBar->AtkUnitBase.IsVisible == false ) continue;

                if( hotbarIndex == 10 ) { // cross
                    ProcessCrossHotbar( hotbarIndex, actionBar, hotbarData, foundIcons, createIcons, percent );
                }
                else if( hotbarIndex == 11 ) {
                    ProcessDoubleCrossHotbar( hotbarIndex, actionBar, AtkHelper.GetLeftDoubleCrossBar(), true, hotbarData, foundIcons, createIcons, percent );
                }
                else if( hotbarIndex == 12 ) {
                    ProcessDoubleCrossHotbar( hotbarIndex, actionBar, AtkHelper.GetRightDoubleCrossBar(), false, hotbarData, foundIcons, createIcons, percent );
                }
                else {
                    ProcessNormalHotbar( hotbarIndex, actionBar, hotbarData, foundIcons, createIcons, percent );
                }
            }

            // remove unused
            foreach( var icon in Icons.Where( x => !foundIcons.Contains( x ) ) ) {
                icon.Dispose();
            }
            Icons.RemoveAll( x => !foundIcons.Contains( x ) );

            // create new
            foreach( var create in createIcons ) {
                AtkIcon newIcon = create.Props.IsTimer ?
                    new AtkIconTimer( create.Action, create.ActionId, create.HotBarIndex, create.SlotIndex, create.Component, create.Props ) :
                    new AtkIconBuff( create.Action, create.ActionId, create.HotBarIndex, create.SlotIndex, create.Component, create.Props );
                Icons.Add( newIcon );
            }
        }

        private void ProcessCrossHotbar( int hotbarIndex, AddonActionBarBase* actionBar, AddonHotbarNumberArray* hotbarData, HashSet<AtkIcon> foundIcons, HashSet<CreateIconStruct> createIcons, float percent ) {
            var crossBar = AtkHelper.GetCrossBar();
            if( crossBar == null ) return;

            if( crossBar->LeftCompactFlags != 0 || crossBar->RightCompactFlags != 0 ) {
                ProcessCompactCrossHotbar( hotbarIndex, actionBar, crossBar, hotbarData, foundIcons, createIcons, percent );
                return;
            }

            var hotbar = hotbarData->Hotbars[10 + crossBar->CrossBarSet];

            for( var slotIndex = 0; slotIndex < actionBar->HotbarSlotCount; slotIndex++ ) {
                ProcessIcon( hotbarIndex, slotIndex, hotbar[slotIndex], actionBar->ActionBarSlotsAction[slotIndex], foundIcons, createIcons, percent );
            }
        }

        private void ProcessCompactCrossHotbar( int hotbarIndex, AddonActionBarBase* actionBar, AddonActionBarCross* crossBar, AddonHotbarNumberArray* hotbarData, HashSet<AtkIcon> foundIcons, HashSet<CreateIconStruct> createIcons, float percent ) {
            crossBar->GetCompact( out var compactSet, out var compactLeft );

            var set = 10 + compactSet - 1;
            var hotbar = hotbarData->Hotbars[set];

            for( var idx = 0; idx < 8; idx++ ) {
                var slotIndex = idx + 4;
                var dataIndex = idx + ( compactLeft ? 0 : 8 );

                ProcessIcon( hotbarIndex, slotIndex, hotbar[dataIndex], actionBar->ActionBarSlotsAction[slotIndex], foundIcons, createIcons, percent );
            }
        }

        private void ProcessDoubleCrossHotbar( int hotbarIndex, AddonActionBarBase* actionBar, AddonActionBarDoubleCross* doubleCrossBar, bool left, AddonHotbarNumberArray* hotbarData, HashSet<AtkIcon> foundIcons, HashSet<CreateIconStruct> createIcons, float percent ) {
            var hotbar = hotbarData->Hotbars[doubleCrossBar->HotbarIndex];
            var setLeft = doubleCrossBar->Left == 1;
            var large = doubleCrossBar->LargeDoubleCross == 1;

            for( var idx = 0; idx < ( large ? 8 : 4 ); idx++ ) {
                var slotIndex = idx;
                var dataIndex = idx + ( setLeft ? 0 : 8 );
                if( !large ) {
                    slotIndex += 4;
                    dataIndex += 4;
                }

                ProcessIcon( hotbarIndex, slotIndex, hotbar[dataIndex], actionBar->ActionBarSlotsAction[slotIndex], foundIcons, createIcons, percent );
            }
        }

        private void ProcessNormalHotbar( int hotbarIndex, AddonActionBarBase* actionBar, AddonHotbarNumberArray* hotbarData, HashSet<AtkIcon> foundIcons, HashSet<CreateIconStruct> createIcons, float percent ) {
            var hotbar = hotbarData->Hotbars[hotbarIndex];

            for( var slotIndex = 0; slotIndex < actionBar->HotbarSlotCount; slotIndex++ ) {
                ProcessIcon( hotbarIndex, slotIndex, hotbar[slotIndex], actionBar->ActionBarSlotsAction[slotIndex], foundIcons, createIcons, percent );
            }
        }

        private void ProcessIcon( int hotbarIndex, int slotIndex, HotbarSlotStruct slotData, ActionBarSlotAction slot, HashSet<AtkIcon> foundIcons, HashSet<CreateIconStruct> createIcons, float percent ) {
            if( slotData.Type != HotbarSlotStructType.Action ) return;

            var action = AtkHelper.GetAdjustedAction( slotData.ActionId );

            if( !IconConfigs.TryGetValue( action, out var props ) ) return; // not looking for this action id

            var found = false;
            foreach( var icon in Icons ) {
                if( icon.HotbarIdx != hotbarIndex || icon.SlotIdx != slotIndex ) continue;
                if( slotData.ActionId != icon.SlotId ) break; // action changed, just ignore it

                // found existing icon which matches
                found = true;
                foundIcons.Add( icon );
                icon.Tick( percent, slotData.YellowBorder );

                break;
            }
            if( found ) return; // already found an icon, don't need to create a new one

            createIcons.Add( new CreateIconStruct {
                Action = action,
                ActionId = slotData.ActionId,
                HotBarIndex = hotbarIndex,
                SlotIndex = slotIndex,
                Component = slot.Icon,
                Props = props
            } );
        }

        public void AddIconOverride( IntPtr icon ) {
            IconOverride.Add( icon );
        }

        public void RemoveIconOverride( IntPtr icon ) {
            IconOverride.Remove( icon );
        }

        public void ProcessIconOverride( IntPtr icon ) {
            if( icon == IntPtr.Zero ) return;
            if( IconOverride.Contains( icon ) ) {
                var image = ( AtkImageNode* )icon;
                AtkIconTimer.SetDimmed( image, true );
            }
        }

        public void RefreshVisuals() {
            foreach( var icon in Icons ) icon.RefreshVisuals();
        }

        public void Reset() {
            IconConfigs.Clear();
            Icons.ForEach( x => x.Dispose() );
            Icons.Clear();
        }

        public void Dispose() {
            IconOverride.Clear();
            Reset();
        }
    }
}
