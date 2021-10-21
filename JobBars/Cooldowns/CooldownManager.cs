using JobBars.Data;
using JobBars.Helper;
using System;
using System.Collections.Generic;

namespace JobBars.Cooldowns {
    public struct CooldownPartyMemberStruct {
        public uint ObjectId;
        public JobIds Job;
    }

    public unsafe partial class CooldownManager : PerJobManager<CooldownProps[]> {
        private static readonly int MILLIS_LOOP = 250;
        private Dictionary<uint, CooldownPartyMember> ObjectIdToMember = new();

        public CooldownManager() : base("##JobBars_Cooldowns") {
            Init();

            JobBars.Builder.SetCooldownPosition(JobBars.Config.CooldownPosition);
            if (!JobBars.Config.CooldownsEnabled) JobBars.Builder.HideCooldowns();
        }

        public CooldownProps[] GetCooldownProps(JobIds job) {
            return JobToValue.TryGetValue(job, out var props) ? props : JobToValue[JobIds.OTHER];
        }

        private List<uint> GetPartyMemberOrder() {
            var ret = new List<uint>();

            var partyUI = UIHelper.GetPartyUI();
            if (partyUI == null || partyUI->PartyMemberCount == 0) { // fallback
                ret.Add(JobBars.ClientState.LocalPlayer.ObjectId);
                return ret;
            }

            for (int i = 0; i < partyUI->PartyMemberCount; i++) {
                var member = partyUI->PartyMember[i];
                var objectId = (uint)member.ObjectID;
                ret.Add((objectId == 0xE0000000 || objectId == 0xFFFFFFFF) ? 0 : objectId);
            }

            return ret;
        }

        public void PerformAction(Item action, uint objectId) {
            if (!JobBars.Config.CooldownsEnabled) return;

            foreach (var member in ObjectIdToMember.Values) {
                member.ProcessAction(action, objectId);
            }
        }

        public void Tick(bool inCombat) {
            if (!JobBars.Config.CooldownsEnabled) return;
            if (JobBars.Config.CooldownsHideOutOfCombat) {
                if (inCombat) JobBars.Builder.ShowCooldowns();
                else JobBars.Builder.HideCooldowns();
            }

            var time = DateTime.Now;
            int millis = time.Second * 1000 + time.Millisecond;
            float percent = (float)(millis % MILLIS_LOOP) / MILLIS_LOOP;

            var partyMemberOrder = GetPartyMemberOrder();
            Dictionary<uint, CooldownPartyMember> newObjectIdToMember = new();

            for (int idx = 0; idx < partyMemberOrder.Count; idx++) {
                var objectId = partyMemberOrder[idx];
                if (objectId == 0) {
                    JobBars.Builder.SetCooldownRowVisible(idx, false);
                    continue;
                }

                var partyMember = JobBars.PartyMembers.Find(member => member.ObjectId == objectId);
                if (partyMember == null || partyMember?.ObjectId == 0 || partyMember?.Job == JobIds.OTHER) {
                    JobBars.Builder.SetCooldownRowVisible(idx, false);
                    continue;
                }

                var member = ObjectIdToMember.TryGetValue(partyMember.ObjectId, out var _member) ? _member : new CooldownPartyMember(partyMember.ObjectId);
                member.Tick(JobBars.Builder.Cooldowns[idx], partyMember, percent);
                newObjectIdToMember[partyMember.ObjectId] = member;

                JobBars.Builder.SetCooldownRowVisible(idx, true);
            }
            for (int idx = partyMemberOrder.Count; idx < 8; idx++) { // hide remaining slots
                JobBars.Builder.SetCooldownRowVisible(idx, false);
            }

            ObjectIdToMember = newObjectIdToMember;
        }

        public void ResetUI() {
            ObjectIdToMember.Clear();
        }

        public void ResetTrackers() {
            foreach (var item in ObjectIdToMember.Values) item.Reset();
        }
    }
}