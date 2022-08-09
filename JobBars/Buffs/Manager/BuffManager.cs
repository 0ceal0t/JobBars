using Dalamud.Logging;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Buffs.Manager {
    public unsafe partial class BuffManager : PerJobManager<BuffConfig[]> {
        private Dictionary<uint, BuffPartyMember> ObjectIdToMember = new();
        private readonly List<BuffConfig> ApplyToTargetBuffs = new();

        private readonly Dictionary<JobIds, List<BuffConfig>> CustomBuffs = new();
        private List<BuffConfig> ApplyToTargetCustomBuffs => CustomBuffs.Values.SelectMany(x => x.Where(y => y.ApplyToTarget)).ToList();

        public BuffManager() : base("##JobBars_Buffs", true) {
            ApplyToTargetBuffs.AddRange(JobToValue.Values.SelectMany(x => x.Where(y => y.ApplyToTarget)).ToList());
            JobBars.Builder.HideAllBuffs();
        }

        public BuffConfig[] GetBuffConfigs(JobIds job) {
            List<BuffConfig> configs = new();

            configs.AddRange(ApplyToTargetBuffs);
            if (JobToValue.TryGetValue(job, out var props)) configs.AddRange(props.Where(x => !x.ApplyToTarget)); // avoid double-adding

            configs.AddRange(ApplyToTargetCustomBuffs);
            if (CustomBuffs.TryGetValue(job, out var customProps)) configs.AddRange(customProps.Where(x => !x.ApplyToTarget));

            return configs.ToArray();
        }

        public void PerformAction(Item action, uint objectId) {
            if (!JobBars.Config.BuffBarEnabled) return;
            if (!JobBars.Config.BuffIncludeParty && objectId != JobBars.ClientState.LocalPlayer.ObjectId) return;

            foreach (var member in ObjectIdToMember.Values) member.ProcessAction(action, objectId);
        }

        public void Tick() {
            if (UIHelper.CalcDoHide(JobBars.Config.BuffBarEnabled, JobBars.Config.BuffHideOutOfCombat, JobBars.Config.BuffHideWeaponSheathed)) {
                JobBars.Builder.HideBuffs();
                return;
            }
            else {
                JobBars.Builder.ShowBuffs();
            }

            // ============================

            Dictionary<uint, BuffPartyMember> newObjectIdToMember = new();
            HashSet<BuffTracker> activeBuffs = new();

            if (JobBars.PartyMembers == null) PluginLog.LogError("PartyMembers is NULL");

            for (int idx = 0; idx < JobBars.PartyMembers.Count; idx++) {
                var partyMember = JobBars.PartyMembers[idx];

                if (partyMember == null || partyMember?.Job == JobIds.OTHER || partyMember?.ObjectId == 0) continue;
                if (!JobBars.Config.BuffIncludeParty && partyMember.ObjectId != JobBars.ClientState.LocalPlayer.ObjectId) continue;

                var member = ObjectIdToMember.TryGetValue(partyMember.ObjectId, out var _member) ? _member : new BuffPartyMember(partyMember.ObjectId, partyMember.IsPlayer);
                member.Tick(activeBuffs, partyMember, out var highlight, out var partyText);
                JobBars.Builder.SetBuffPartyListVisible(idx, highlight);
                JobBars.Builder.SetBuffPartyListText(idx, (JobBars.Config.BuffPartyListASTText && JobBars.CurrentJob == JobIds.AST) ? partyText : "");
                newObjectIdToMember[partyMember.ObjectId] = member;
            }
            for (int idx = JobBars.PartyMembers.Count; idx < 8; idx++) {
                JobBars.Builder.SetBuffPartyListVisible(idx, false);
                JobBars.Builder.SetBuffPartyListText(idx, "");
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
