using Dalamud.Logging;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Buffs {
    public unsafe partial class BuffManager : PerJobManager<BuffProps[]> {
        private readonly List<BuffProps> LocalPlayerBuffs = new();
        private Dictionary<uint, BuffPartyMember> ObjectIdToMember = new();

        public BuffManager() : base("##JobBars_Buffs") {
            Init();

            foreach (var jobEntry in JobToValue)
                LocalPlayerBuffs.AddRange(jobEntry.Value.Where(b => b.IsPlayerOnly));

            if (!JobBars.Config.BuffBarEnabled) JobBars.Builder.HideBuffs();
            JobBars.Builder.HideAllBuffs();
        }

        public BuffProps[] GetBuffProps(JobIds job, bool isLocalPlayer) {
            var jobBuffs = JobToValue.TryGetValue(job, out var props) ? props : JobToValue[JobIds.OTHER];
            if (!isLocalPlayer) return jobBuffs;

            var combinedProps = new List<BuffProps>();
            combinedProps.AddRange(LocalPlayerBuffs);
            combinedProps.AddRange(jobBuffs.Where(x => !x.IsPlayerOnly));
            return combinedProps.ToArray();
        }

        public void PerformAction(Item action, uint objectId) {
            if (!JobBars.Config.BuffBarEnabled) return;
            if (!JobBars.Config.BuffIncludeParty && objectId != JobBars.ClientState.LocalPlayer.ObjectId) return;

            foreach (var member in ObjectIdToMember.Values) member.ProcessAction(action, objectId);
        }

        public void Tick(bool inCombat) {
            if (!JobBars.Config.BuffBarEnabled) return;
            if (JobBars.Config.BuffHideOutOfCombat) {
                if (inCombat) JobBars.Builder.ShowBuffs();
                else JobBars.Builder.HideBuffs();
            }

            Dictionary<uint, BuffPartyMember> newObjectIdToMember = new();
            HashSet<BuffTracker> activeBuffs = new();

            if (JobBars.PartyMembers == null) PluginLog.LogError("PARTYMEMBERS IS NULL");

            for (int idx = 0; idx < JobBars.PartyMembers.Count; idx++) {
                var partyMember = JobBars.PartyMembers[idx];

                if (partyMember == null || partyMember?.Job == JobIds.OTHER || partyMember?.ObjectId == 0) continue;
                if (!JobBars.Config.BuffIncludeParty && partyMember.ObjectId != JobBars.ClientState.LocalPlayer.ObjectId) continue;

                var member = ObjectIdToMember.TryGetValue(partyMember.ObjectId, out var _member) ? _member : new BuffPartyMember(partyMember.ObjectId, partyMember.IsPlayer);
                var highlightMember = member.Tick(activeBuffs, partyMember);
                JobBars.Builder.SetBuffPartyListVisible(idx, JobBars.Config.BuffPartyListEnabled && highlightMember);
                newObjectIdToMember[partyMember.ObjectId] = member;
            }
            for (int idx = JobBars.PartyMembers.Count; idx < 8; idx++) {
                JobBars.Builder.SetBuffPartyListVisible(idx, false);
            }

            var buffIdx = 0;
            foreach (var buff in JobBars.Config.BuffOrderByActive ?
                activeBuffs.OrderBy(b => b.CurrentState) :
                activeBuffs.OrderBy(b => b.Id)
            ) {
                if (buffIdx >= (UIBuilder.MAX_BUFFS - 1)) break;
                buff.TickUI(JobBars.Builder.Buffs[buffIdx]);
                buffIdx++;
            }
            for (int i = buffIdx; i < UIBuilder.MAX_BUFFS; i++) {
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
}
