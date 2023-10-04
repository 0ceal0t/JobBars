using JobBars.Data;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace JobBars.Cooldowns.Manager {
    public struct CooldownPartyMemberStruct {
        public uint ObjectId;
        public JobIds Job;
    }

    public unsafe partial class CooldownManager : PerJobManager<CooldownConfig[]> {
        private static readonly int MILLIS_LOOP = 250;
        private Dictionary<uint, CooldownPartyMember> ObjectIdToMember = new();
        private readonly Dictionary<JobIds, List<CooldownConfig>> CustomCooldowns = new();

        public CooldownManager() : base("##JobBars_Cooldowns") {
            JobBars.Builder.SetCooldownPosition(JobBars.Configuration.CooldownPosition);

            // initialize custom cooldowns
            foreach (var custom in JobBars.Configuration.CustomCooldown) {
                if (!CustomCooldowns.ContainsKey(custom.Job)) CustomCooldowns[custom.Job] = new();
                CustomCooldowns[custom.Job].Add(new CooldownConfig(custom.Name, custom.Props));
            }
        }

        public CooldownConfig[] GetCooldownConfigs(JobIds job) {
            List<CooldownConfig> configs = new();
            if (JobToValue.TryGetValue(job, out var props)) configs.AddRange(props);
            if (CustomCooldowns.TryGetValue(job, out var customProps)) configs.AddRange(customProps);
            return configs.ToArray();
        }

        public void PerformAction(Item action, uint objectId) {
            if (!JobBars.Configuration.CooldownsEnabled) return;

            foreach (var member in ObjectIdToMember.Values) {
                member.ProcessAction(action, objectId);
            }
        }

        public void Tick() {
            if (AtkHelper.CalcDoHide(JobBars.Configuration.CooldownsEnabled, JobBars.Configuration.CooldownsHideOutOfCombat, JobBars.Configuration.CooldownsHideWeaponSheathed)) {
                JobBars.Builder.HideCooldowns();
                return;
            }
            else {
                JobBars.Builder.ShowCooldowns();
            }

            // ============================

            var time = DateTime.Now;
            int millis = time.Second * 1000 + time.Millisecond;
            float percent = (float)(millis % MILLIS_LOOP) / MILLIS_LOOP;

            Dictionary<uint, CooldownPartyMember> newObjectIdToMember = new();

            if (JobBars.PartyMembers == null) Dalamud.LogError("PartyMembers is null");

            for (int idx = 0; idx < JobBars.PartyMembers.Count; idx++) {
                var partyMember = JobBars.PartyMembers[idx];

                if (partyMember == null || partyMember?.ObjectId == 0 || partyMember?.Job == JobIds.OTHER) {
                    JobBars.Builder.SetCooldownRowVisible(idx, false);
                    continue;
                }

                if (!JobBars.Configuration.CooldownsShowPartyMembers && partyMember.ObjectId != Dalamud.ClientState.LocalPlayer.ObjectId) {
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

        public void UpdatePositionScale() {
            JobBars.Builder.SetCooldownPosition(JobBars.Configuration.CooldownPosition + new Vector2(0, AtkHelper.PartyListOffset()));
            JobBars.Builder.SetCooldownScale(JobBars.Configuration.CooldownScale);
            JobBars.Builder.RefreshCooldownsLayout();
        }

        public void ResetUi() => ObjectIdToMember.Clear();

        public void ResetTrackers() {
            foreach (var item in ObjectIdToMember.Values) item.Reset();
        }

        public void AddCustomCooldown(JobIds job, string name, CooldownProps props) {
            if (!CustomCooldowns.ContainsKey(job)) CustomCooldowns[job] = new();
            CustomCooldowns[job].Add(new CooldownConfig(name, props));
            JobBars.Configuration.AddCustomCooldown(name, job, props);
        }

        public void DeleteCustomCooldown(JobIds job, CooldownConfig custom) {
            CustomCooldowns[job].Remove(custom);
            JobBars.Configuration.RemoveCustomCooldown(custom.Name);
        }
    }
}