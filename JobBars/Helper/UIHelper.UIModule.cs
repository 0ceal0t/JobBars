using System;
using Dalamud.Logging;
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

        public static int GetCrossBarSet() {
            var addon = (AddonActionBarCross*)AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonByName("_ActionCross");
            if (addon == null) return -1;
            return addon->CrossBarSet;
        }

        public static AtkUnitBase* ChatLogAddon => AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonByName("ChatLog");
        public static AtkUnitBase* ParameterAddon => AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonByName("_ParameterWidget");
        public static AddonPartyList* PartyListAddon => (AddonPartyList*)AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonByName("_PartyList");
    }
}
