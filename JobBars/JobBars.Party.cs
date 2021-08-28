using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Collections.Generic;

namespace JobBars {

    public class CurrentPartyMember {
        public uint ObjectId;
        public JobIds Job;
        public uint CurrentHP;
        public uint MaxHP;
        public Dictionary<Item, Status> BuffDict;
        public bool IsPlayer;
    }

    public unsafe partial class JobBars {
        public static List<CurrentPartyMember> PartyMembers { get; private set; } = new();

        private static void UpdatePartyMembers() {
            var ret = new List<CurrentPartyMember>();
            var localPlayer = ClientState.LocalPlayer;

            var groupManager = GroupManager.Instance();
            if (groupManager == null || groupManager->MemberCount == 0) { // fallback
                var localPartyMember = new CurrentPartyMember {
                    IsPlayer = true,
                    ObjectId = localPlayer.ObjectId,
                    CurrentHP = localPlayer.CurrentHp,
                    MaxHP = localPlayer.MaxHp,
                    Job = UIHelper.IdToJob(localPlayer.ClassJob.Id),
                    BuffDict = new()
                };

                foreach (var status in localPlayer.StatusList) {
                    UIHelper.StatusToBuffItem(localPartyMember.BuffDict, status);
                }

                ret.Add(localPartyMember);
                PartyMembers = ret;
                return;
            }

            for (int i = 0; i < 8; i++) {
                PartyMember* info = (PartyMember*)(new IntPtr(groupManager->PartyMembers) + 0x230 * i);
                if (info->ObjectID == 0 || info->ObjectID == 0xE0000000 || info->ObjectID == 0xFFFFFFFF) continue;
                var partyMember = new CurrentPartyMember {
                    IsPlayer = info->ObjectID == localPlayer.ObjectId,
                    ObjectId = info->ObjectID,
                    CurrentHP = info->CurrentHP,
                    MaxHP = info->MaxHP,
                    Job = UIHelper.IdToJob(info->ClassJob),
                    BuffDict = new()
                };

                if (info->StatusManager.Status == null) continue;
                for (int j = 0; j < 30; j++) {
                    Status* status = (Status*)(new IntPtr(info->StatusManager.Status) + 0xC * j);
                    if (status->StatusID == 0) continue;
                    UIHelper.StatusToBuffItem(partyMember.BuffDict, status);
                }

                ret.Add(partyMember);
            }

            PartyMembers = ret;
        }

        public unsafe static bool IsInParty(uint objectId) {
            if (objectId == 0 || objectId == 0xE0000000 || objectId == 0xFFFFFFFF) return false;

            foreach (var member in JobBars.PartyMembers) {
                if (member.ObjectId == objectId) return true;
            }
            return false;
        }

        public unsafe static void SearchForPartyMemberStatus(int ownerId, Dictionary<Item, Status> buffDict, List<BuffIds> buffsToSearch) {
            foreach (var member in PartyMembers) {
                foreach (var entry in member.BuffDict) {
                    if (entry.Value.SourceID != ownerId) continue;
                    if (!buffsToSearch.Contains((BuffIds)entry.Value.StatusID)) continue;
                    buffDict[entry.Key] = entry.Value;
                }
            }
        }
    }
}