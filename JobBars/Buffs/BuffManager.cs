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

            foreach (var jobEntry in JobToValue) {
                LocalPlayerBuffs.AddRange(jobEntry.Value.Where(b => b.IsPlayerOnly));
            }

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

            Dictionary<uint, BuffPartyMember> newObjectIdToMember = new();
            HashSet<BuffTracker> activeBuffs = new();

            foreach (var partyMember in JobBars.PartyMembers) {
                if (partyMember.Job == JobIds.OTHER || partyMember.ObjectId == 0) continue;
                if (!JobBars.Config.BuffIncludeParty && partyMember.ObjectId != JobBars.ClientState.LocalPlayer.ObjectId) continue;

                var member = ObjectIdToMember.TryGetValue(partyMember.ObjectId, out var _member) ? _member : new BuffPartyMember(partyMember.ObjectId, partyMember.IsPlayer);
                member.Tick(activeBuffs, partyMember);
                newObjectIdToMember[partyMember.ObjectId] = member;
            }

            var idx = 0;
            foreach(var buff in activeBuffs.OrderBy(b => b.CurrentState)) {
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
}
