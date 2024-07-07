using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
using JobBars.GameStructs;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace JobBars.Nodes.Icon {
    public unsafe class IconBuilder {
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

        public IconBuilder() { }

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

            var hotbarData = UiHelper.GetHotbarUI();
            if( hotbarData == null ) return;

            HashSet<AtkIcon> foundIcons = [];
            HashSet<CreateIconStruct> createIcons = [];

            for( var hotbarIndex = 0; hotbarIndex < AllActionBars.Length; hotbarIndex++ ) {
                var actionBar = ( AddonActionBarBase* )AtkStage.Instance()->RaptureAtkUnitManager->GetAddonByName( AllActionBars[hotbarIndex] );
                if( actionBar == null || actionBar->AtkUnitBase.IsVisible == false ) continue;

                if( hotbarIndex == 10 ) { // cross
                    ProcessCrossHotbar( hotbarIndex, actionBar, hotbarData, foundIcons, createIcons, percent );
                }
                else if( hotbarIndex == 11 ) {
                    ProcessDoubleCrossHotbar( hotbarIndex, actionBar, UiHelper.GetLeftDoubleCrossBar(), true, hotbarData, foundIcons, createIcons, percent );
                }
                else if( hotbarIndex == 12 ) {
                    ProcessDoubleCrossHotbar( hotbarIndex, actionBar, UiHelper.GetRightDoubleCrossBar(), false, hotbarData, foundIcons, createIcons, percent );
                }
                else {
                    ProcessNormalHotbar( hotbarIndex, actionBar, hotbarData, foundIcons, createIcons, percent );
                }
            }

            // remove unused
            foreach( var icon in Icons.Where( x => !foundIcons.Contains( x ) ) ) icon.Dispose();
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
            var crossBar = UiHelper.GetCrossBar();
            if( crossBar == null ) return;

            if( crossBar->ExpandedHoldMapValueLR != 0 || crossBar->ExpandedHoldMapValueRL != 0 ) {
                ProcessCompactCrossHotbar( hotbarIndex, actionBar, crossBar, hotbarData, foundIcons, createIcons, percent );
                return;
            }


            var hotbar = hotbarData->Hotbars[10 + Marshal.ReadInt32( ( nint )crossBar + 0x1A0 )];
            for( var slotIndex = 0; slotIndex < actionBar->SlotCount; slotIndex++ ) {
                ProcessIcon( hotbarIndex, slotIndex, hotbar[slotIndex], actionBar->ActionBarSlotVector[slotIndex], foundIcons, createIcons, percent );
            }
        }

        private void ProcessCompactCrossHotbar( int hotbarIndex, AddonActionBarBase* actionBar, AddonActionCross* crossBar, AddonHotbarNumberArray* hotbarData, HashSet<AtkIcon> foundIcons, HashSet<CreateIconStruct> createIcons, float percent ) {
            GetCompact( crossBar, out var compactSet, out var compactLeft );

            var set = 10 + compactSet - 1;
            var hotbar = hotbarData->Hotbars[set];

            for( var idx = 0; idx < 8; idx++ ) {
                var slotIndex = idx + 4;
                var dataIndex = idx + ( compactLeft ? 0 : 8 );

                ProcessIcon( hotbarIndex, slotIndex, hotbar[dataIndex], actionBar->ActionBarSlotVector[slotIndex], foundIcons, createIcons, percent );
            }
        }

        public bool GetCompact( AddonActionCross* crossBar, out int set, out bool left ) {
            set = -1;
            left = false;

            if( GetCompactLeft( crossBar, out var setL, out var leftL ) ) {
                set = setL;
                left = leftL;
                return true;
            }

            if( GetCompactRight( crossBar, out var setR, out var leftR ) ) {
                set = setR;
                left = leftR;
                return true;
            }

            return false;
        }

        public bool GetCompactLeft( AddonActionCross* crossBar, out int set, out bool left ) => GetSelectedCompact( crossBar->ExpandedHoldMapValueLR, out set, out left );

        public bool GetCompactRight( AddonActionCross* crossBar, out int set, out bool left ) => GetSelectedCompact( crossBar->ExpandedHoldMapValueRL, out set, out left );

        public static bool GetSelectedCompact( uint flag, out int set, out bool left ) { // 1 = 0/left, 2 = 0/right, 3 = 1/left, etc.
            set = 0;
            left = false;

            if( flag == 0 ) return false;

            var leftOver = flag % 2;
            left = leftOver == 1;
            set = ( int )( ( flag + leftOver ) / 2 );

            return true;
        }

        private void ProcessDoubleCrossHotbar( int hotbarIndex, AddonActionBarBase* actionBar, AddonActionDoubleCrossBase* doubleCrossBar, bool left, AddonHotbarNumberArray* hotbarData, HashSet<AtkIcon> foundIcons, HashSet<CreateIconStruct> createIcons, float percent ) {
            var hotbar = hotbarData->Hotbars[doubleCrossBar->BarTarget];
            var setLeft = doubleCrossBar->UseLeftSide == 1;
            var large = doubleCrossBar->ShowDPadSlots == 1;

            for( var idx = 0; idx < ( large ? 8 : 4 ); idx++ ) {
                var slotIndex = idx;
                var dataIndex = idx + ( setLeft ? 0 : 8 );
                if( !large ) {
                    slotIndex += 4;
                    dataIndex += 4;
                }

                ProcessIcon( hotbarIndex, slotIndex, hotbar[dataIndex], actionBar->ActionBarSlotVector[slotIndex], foundIcons, createIcons, percent );
            }
        }

        private void ProcessNormalHotbar( int hotbarIndex, AddonActionBarBase* actionBar, AddonHotbarNumberArray* hotbarData, HashSet<AtkIcon> foundIcons, HashSet<CreateIconStruct> createIcons, float percent ) {
            var hotbar = hotbarData->Hotbars[hotbarIndex];

            for( var slotIndex = 0; slotIndex < actionBar->SlotCount; slotIndex++ ) {
                ProcessIcon( hotbarIndex, slotIndex, hotbar[slotIndex], actionBar->ActionBarSlotVector[slotIndex], foundIcons, createIcons, percent );
            }
        }

        private void ProcessIcon( int hotbarIndex, int slotIndex, HotbarSlotStruct slotData, ActionBarSlot slot, HashSet<AtkIcon> foundIcons, HashSet<CreateIconStruct> createIcons, float percent ) {
            if( slotData.Type != HotbarSlotStructType.Action ) return;

            var action = UiHelper.GetAdjustedAction( slotData.ActionId );

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
