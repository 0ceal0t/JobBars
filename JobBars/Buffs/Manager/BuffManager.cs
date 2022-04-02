using Dalamud.Logging;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Buffs.Manager {
    public unsafe partial class BuffManager : PerJobManager<BuffConfig[]> {
        private readonly List<BuffConfig> LocalPlayerConfigs = new();
        private Dictionary<uint, BuffPartyMember> ObjectIdToMember = new();

        public BuffManager() : base("##JobBars_Buffs") {
            foreach (var jobEntry in JobToValue) LocalPlayerConfigs.AddRange(jobEntry.Value.Where(b => b.IsPlayerOnly));
            JobBars.Builder.HideAllBuffs();
        }

        public BuffConfig[] GetBuffConfigs(JobIds job, bool isLocalPlayer) {
            var jobBuffs = JobToValue.TryGetValue(job, out var props) ? props : JobToValue[JobIds.OTHER];
            if (!isLocalPlayer) return jobBuffs;

            var combinedProps = new List<BuffConfig>();
            combinedProps.AddRange(LocalPlayerConfigs);
            combinedProps.AddRange(jobBuffs.Where(x => !x.IsPlayerOnly));
            return combinedProps.ToArray();
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
                member.Tick(activeBuffs, partyMember, out var active, out var partyText);
                JobBars.Builder.SetBuffPartyListVisible(idx, JobBars.Config.BuffPartyListEnabled && active);
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
