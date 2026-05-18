using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.GameStructs;
using KamiToolKit.Extensions;
using System;

namespace JobBars.Helper {
    public unsafe partial class UiHelper {
        public static unsafe AddonPartyListIntArray* GetPartyUI() {
            var uiModule = Framework.Instance()->UIModule;
            if( uiModule == null ) return null;
            return ( AddonPartyListIntArray* )uiModule->GetRaptureAtkModule()->AtkModule.AtkArrayDataHolder.NumberArrays[4]->IntArray;
        }

        public static AddonHotbarNumberArray* GetHotbarUI() {
            var uiModule = Framework.Instance()->UIModule;
            if( uiModule == null ) return null;
            return ( AddonHotbarNumberArray* )uiModule->GetRaptureAtkModule()->AtkModule.AtkArrayDataHolder.NumberArrays[6]->IntArray;
        }

        public static AtkUnitBase* GetAddon( string name ) => AtkStage.Instance()->RaptureAtkUnitManager->GetAddonByName( name );

        public static float PartyListOffset() {
            var partyList = ( AddonPartyList* )GetAddon( "_PartyList" );
            if( partyList == null ) return 0;
            if( partyList->AtkUnitBase.UldManager.NodeList == null ) return 0;

            var offsetNode = partyList->AtkUnitBase.UldManager.SearchNodeById( 2 );
            if( offsetNode == null ) return 0;
            return offsetNode->Y;
        }
    }
}
