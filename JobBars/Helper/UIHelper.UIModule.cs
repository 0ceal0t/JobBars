using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.GameStructs;

namespace JobBars.Helper {
    public unsafe partial class AtkHelper {
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

        public static AddonActionCross* GetCrossBar() => ( AddonActionCross* )GetAddon( "_ActionCross" );

        public static AddonActionDoubleCrossBase* GetLeftDoubleCrossBar() => ( AddonActionDoubleCrossBase* )GetAddon( "_ActionDoubleCrossL" );

        public static AddonActionDoubleCrossBase* GetRightDoubleCrossBar() => ( AddonActionDoubleCrossBase* )GetAddon( "_ActionDoubleCrossR" );

        public static AtkUnitBase* ChatLogAddon => GetAddon( "ChatLog" );

        public static AtkUnitBase* ParameterAddon => GetAddon( "_ParameterWidget" );

        public static AddonPartyList* PartyListAddon => ( AddonPartyList* )GetAddon( "_PartyList" );

        public static float PartyListOffset() {
            var partyList = PartyListAddon;
            if( partyList == null ) return 0;
            if( partyList->AtkUnitBase.UldManager.NodeList == null ) return 0;
            if( partyList->AtkUnitBase.UldManager.NodeList[3] == null ) return 0;

            return partyList->AtkUnitBase.UldManager.NodeList[3]->Y;
        }

        public static AtkUnitBase* BuffGaugeAttachAddon => GetAddon( JobBars.AttachAddon );

        public static AtkUnitBase* CooldownAttachAddon => GetAddon( JobBars.CooldownAttachAddon );

        public static AtkUnitBase* GetAddon( Data.AttachAddon addon ) => addon switch {
            Data.AttachAddon.Chatbox => ChatLogAddon,
            Data.AttachAddon.HP_MP_Bars => ParameterAddon,
            Data.AttachAddon.PartyList => ( AtkUnitBase* )PartyListAddon,
            _ => null
        };
    }
}
