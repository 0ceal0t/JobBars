using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using JobBars.Data;
using JobBars.GameStructs;

namespace JobBars.Helper {
    public unsafe partial class UIHelper {
        public unsafe static AddonPartyListIntArray* GetPartyUI() {
            var uiModule = Framework.Instance()->GetUiModule();
            if (uiModule == null) return null;
            var numArray = uiModule->RaptureAtkModule.AtkModule.AtkArrayDataHolder.NumberArrays[4];
            return (AddonPartyListIntArray*)numArray->IntArray;
        }

        public unsafe static int GetPartyCount() {
            var groupManager = GroupManager.Instance();
            if (groupManager == null) return 0;
            return groupManager->MemberCount;
        }

        public unsafe static bool IsInParty(uint objectId) {
            if (objectId == 0 || objectId == 0xE0000000 || objectId == 0xFFFFFFFF) return false;

            var groupManager = GroupManager.Instance();
            if (groupManager == null || groupManager->MemberCount == 0) return false;

            for (int i = 0; i < 8; i++) {
                PartyMember* info = (PartyMember*)(new IntPtr(groupManager->PartyMembers) + 0x230 * i);
                if (objectId == info->ObjectID) return true;
            }
            return false;
        }

        public unsafe static void SearchForPartyMemberStatus(int ownerId, Dictionary<Item, StatusDuration> buffDict, List<BuffIds> buffsToSearch) {
            var groupManager = GroupManager.Instance();
            if (groupManager == null || groupManager->MemberCount == 0) return;

            for (int i = 0; i < 8; i++) {
                PartyMember* info = (PartyMember*)(new IntPtr(groupManager->PartyMembers) + 0x230 * i);
                if (info->ObjectID == 0 || info->ObjectID == 0xE0000000 || info->ObjectID == 0xFFFFFFFF) continue;
                if (info->StatusManager.Status == null) continue;

                for (int j = 0; j < 30; j++) {
                    Status* status = (Status*)(new IntPtr(info->StatusManager.Status) + 0xC * j);
                    if (status->SourceID != ownerId) continue;
                    if (!buffsToSearch.Contains((BuffIds)status->StatusID)) continue;
                    StatusToDuration(buffDict, status);
                }
            }
        }
    }
}
