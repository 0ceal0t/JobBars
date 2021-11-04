using System;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.GameStructs;

namespace JobBars.Helper {
    public unsafe partial class UIHelper {
        public unsafe static AddonPartyListIntArray* GetPartyUI() {
            var uiModule = Framework.Instance()->GetUiModule();
            if (uiModule == null) return null;
            var numArray = uiModule->RaptureAtkModule.AtkModule.AtkArrayDataHolder.NumberArrays[4];
            return (AddonPartyListIntArray*)numArray->IntArray;
        }

        public static AddonHotbarNumberArray* GetHotbarUI() {
            var uiModule = Framework.Instance()->GetUiModule();
            return (AddonHotbarNumberArray*)uiModule->RaptureAtkModule.AtkModule.AtkArrayDataHolder.NumberArrays[6]->IntArray;
        }

        public static AtkUnitBase* GetAddon(string name) => AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonByName(name);

        public static AddonActionBarCross* GetCrossBar() => (AddonActionBarCross*)GetAddon("_ActionCross");

        public static AddonActionBarDoubleCross* GetLeftDoubleCrossBar() => (AddonActionBarDoubleCross*)GetAddon("_ActionDoubleCrossL");

        public static AddonActionBarDoubleCross* GetRightDoubleCrossBar() => (AddonActionBarDoubleCross*)GetAddon("_ActionDoubleCrossR");

        public static AtkUnitBase* ChatLogAddon => GetAddon("ChatLog");

        public static AtkUnitBase* ParameterAddon => GetAddon("_ParameterWidget");

        public static AddonPartyList* PartyListAddon => (AddonPartyList*)GetAddon("_PartyList");

        public static AtkUnitBase* AttachAddon => JobBars.AttachAddon switch {
            Data.AttachAddon.Chatbox => ChatLogAddon,
            Data.AttachAddon.HP_MP_Bars => ParameterAddon,
            _ => null
        };
    }
}
