using Dalamud.Logging;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Collections.Generic;

namespace JobBars.Cooldowns.Manager {
    public struct CooldownPartyMemberStruct {
        public uint ObjectId;
        public JobIds Job;
    }

    public unsafe partial class CooldownManager : PerJobManager<CooldownConfig[]> {
        private static readonly int MILLIS_LOOP = 250;
        private Dictionary<uint, CooldownPartyMember> ObjectIdToMember = new();

        public CooldownManager() : base("##JobBars_Cooldowns") {
            JobBars.Builder.SetCooldownPosition(JobBars.Config.CooldownPosition);
            if (!JobBars.Config.CooldownsEnabled) JobBars.Builder.HideCooldowns();
        }

        public CooldownConfig[] GetCooldownConfigs(JobIds job) {
            return JobToValue.TryGetValue(job, out var props) ? props : JobToValue[JobIds.OTHER];
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

            Dictionary<uint, CooldownPartyMember> newObjectIdToMember = new();

            if (JobBars.PartyMembers == null) PluginLog.LogError("PARTYMEMBERS IS NULL");

            for (int idx = 0; idx < JobBars.PartyMembers.Count; idx++) {
                var partyMember = JobBars.PartyMembers[idx];

                if (partyMember == null || partyMember?.ObjectId == 0 || partyMember?.Job == JobIds.OTHER) {
                    JobBars.Builder.SetCooldownRowVisible(idx, false);
                    continue;
                }

                if (!JobBars.Config.CooldownsShowPartyMembers && partyMember.ObjectId != JobBars.ClientState.LocalPlayer.ObjectId) {
                    JobBars.Builder.SetCooldownRowVisible(idx, false);
                    continue;
                }

                var member = ObjectIdToMember.TryGetValue(partyMember.ObjectId, out var _member) ? _member : new CooldownPartyMember(partyMember.ObjectId);
                member.Tick(JobBars.Builder.Cooldowns[idx], partyMember, percent);
                newObjectIdToMember[partyMember.ObjectId] = member;

                JobBars.Builder.SetCooldownRowVisible(idx, true);
            }
            for (int idx = JobBars.PartyMembers.Count; idx < 8; idx++) { // hide remaining slots
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