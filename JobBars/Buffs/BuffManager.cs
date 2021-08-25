using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Buffs {
    public unsafe partial class BuffManager : JobConfigurationManager<BuffProps[]> {
        private readonly List<BuffProps> LocalPlayerBuffs = new();
        private Dictionary<uint, BuffPartyMember> ObjectIdToMember = new();

        public BuffManager() : base("##JobBars_Buffs") {
            Init();

            foreach (var jobEntry in JobToValue) {
                foreach (var buff in jobEntry.Value) {
                    if (!buff.IsPlayerOnly) continue;
                    LocalPlayerBuffs.Add(buff);
                }
            }

            if (!JobBars.Config.BuffBarEnabled) JobBars.Builder.HideBuffs();
            JobBars.Builder.HideAllBuffs();
        }

        public BuffProps[] GetBuffProps(JobIds job, bool isLocalPlayer) {
            var jobValue = JobToValue.TryGetValue(job, out var props) ? props : JobToValue[JobIds.OTHER];
            if (!isLocalPlayer) return jobValue;
            var combinedProps = new List<BuffProps>();
            combinedProps.AddRange(LocalPlayerBuffs);
            combinedProps.AddRange(jobValue.Where(x => !x.IsPlayerOnly));
            return combinedProps.ToArray();
        }

        private List<BuffPartyMemberStruct> GetPartyMembers() {
            var ret = new List<BuffPartyMemberStruct>();
            var localPlayer = JobBars.ClientState.LocalPlayer;

            var groupManager = GroupManager.Instance();
            if (groupManager == null || groupManager->MemberCount == 0) { // fallback
                var localPlayerStruct = new BuffPartyMemberStruct {
                    IsPlayer = true,
                    ObjectId = localPlayer.ObjectId,
                    Job = UIHelper.IdToJob(localPlayer.ClassJob.Id),
                    BuffDict = new()
                };

                foreach(var status in localPlayer.StatusList) {
                    UIHelper.StatusToBuffElem(localPlayerStruct.BuffDict, status);
                }

                ret.Add(localPlayerStruct);
                return ret;
            }

            for (int i = 0; i < 8; i++) {
                PartyMember* info = (PartyMember*)(new IntPtr(groupManager->PartyMembers) + 0x230 * i);
                if (info->ObjectID == 0 || info->ObjectID == 0xE0000000 || info->ObjectID == 0xFFFFFFFF) continue;
                var playerStruct = new BuffPartyMemberStruct {
                    IsPlayer = info->ObjectID == localPlayer.ObjectId,
                    ObjectId = info->ObjectID,
                    Job = UIHelper.IdToJob(info->ClassJob),
                    BuffDict = new()
                };

                if (info->StatusManager.Status == null) continue;
                for (int j = 0; j < 30; j++) {
                    Status* status = (Status*)(new IntPtr(info->StatusManager.Status) + 0xC * j);
                    if (status->StatusID == 0) continue;
                    UIHelper.StatusToBuffElem(playerStruct.BuffDict, status);
                }

                ret.Add(playerStruct);
            }

            return ret;
        }

        public void PerformAction(Item action, uint objectId) {
            if (!JobBars.Config.BuffBarEnabled) return;
            if (!JobBars.Config.BuffIncludeParty && objectId != JobBars.ClientState.LocalPlayer.ObjectId) return;

            foreach (var member in ObjectIdToMember.Values) {
                member.ProcessAction(action, objectId);
            }
        }

        public void Tick(bool inCombat) {
            if (!JobBars.Config.BuffBarEnabled) return;
            if (JobBars.Config.BuffHideOutOfCombat) {
                if (inCombat) JobBars.Builder.ShowBuffs();
                else JobBars.Builder.HideBuffs();
            }

            var partyMembers = GetPartyMembers();
            Dictionary<uint, BuffPartyMember> newObjectIdToMember = new();
            List<BuffTracker> activeBuffs = new();

            foreach (var partyMember in partyMembers) {
                if (partyMember.Job == JobIds.OTHER || partyMember.ObjectId == 0) continue;
                if (!JobBars.Config.BuffIncludeParty && partyMember.ObjectId != JobBars.ClientState.LocalPlayer.ObjectId) continue;

                var member = ObjectIdToMember.TryGetValue(partyMember.ObjectId, out var _member) ? _member : new BuffPartyMember(partyMember.ObjectId, partyMember.IsPlayer);
                member.Tick(activeBuffs, partyMember);
                newObjectIdToMember[partyMember.ObjectId] = member;
            }

            var idx = 0;
            foreach(var buff in activeBuffs.OrderBy(x => x.CurrentState)) {
                if (idx >= (UIBuilder.MAX_BUFFS - 1)) break;

                buff.TickUI(JobBars.Builder.Buffs[idx]);

                idx++;
            }
            for(int i = idx; i < UIBuilder.MAX_BUFFS; i++) {
                JobBars.Builder.Buffs[i].Hide(); // hide unused
            }

            ObjectIdToMember = newObjectIdToMember;
        }

        public void UpdatePositionScale() {
            JobBars.Builder.SetBuffPosition(JobBars.Config.BuffPosition);
            JobBars.Builder.SetBuffScale(JobBars.Config.BuffScale);
        }

        public void ResetUI() {
            ObjectIdToMember.Clear();
        }

        public void ResetTrackers() {
            foreach (var item in ObjectIdToMember.Values) item.Reset();
        }
    }

    public struct BuffPartyMemberStruct {
        public uint ObjectId;
        public JobIds Job;
        public Dictionary<Item, BuffElem> BuffDict;
        public bool IsPlayer;
    }
}
