using Dalamud.Logging;
using Dalamud.Plugin;
using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;

namespace JobBars.Cooldowns {
    public unsafe partial class CooldownManager {
        public static CooldownManager Manager { get; private set; }
        public static void Dispose() { Manager = null; }
        private static readonly int MILLIS_LOOP = 250;

        private Dictionary<uint, CooldownPartyMember> ObjectIdToMember = new();

        private readonly DalamudPluginInterface PluginInterface;

        public CooldownManager(DalamudPluginInterface pluginInterface) {
            Manager = this;
            PluginInterface = pluginInterface;
            Init();
        }

        public void SetupUI() {
            UIBuilder.Builder.SetCooldownPosition(Configuration.Config.CooldownPosition);
        }

        public List<CooldownPartyMemberStruct> GetPartyMembers() {
            var ret = new List<CooldownPartyMemberStruct>();

            var partyUI = DataManager.GetPartyUI();
            if(partyUI == null || partyUI->PartyMemberCount == 0) { // fallback
                var localPlayer = PluginInterface.ClientState.LocalPlayer;
                var self = new CooldownPartyMemberStruct {
                    ObjectId = localPlayer.ObjectId,
                    Job = DataManager.IdToJob(localPlayer.ClassJob.Id)
                };
                ret.Add(self);
                return ret;
            }

            for(int i = 0; i < partyUI->PartyMemberCount; i++) {
                var member = partyUI->PartyMember[i];
                var objectId = (uint)member.ObjectID;
                ret.Add(new CooldownPartyMemberStruct {
                    ObjectId = (objectId == 0xE0000000 || objectId == 0xFFFFFFFF) ? 0 : objectId,
                    Job = DataManager.IconToJob((uint)member.ClassJobIcon)
                }); ;
            }

            return ret;
        }

        public void PerformAction(Item action, uint objectId) {
            if (!Configuration.Config.CooldownsEnabled) return;

            foreach (var member in ObjectIdToMember.Values) {
                member.ProcessAction(action, objectId);
            }
        }

        public void Tick(bool inCombat) {
            if (!Configuration.Config.CooldownsEnabled) return;

            if (Configuration.Config.CooldownsHideOutOfCombat) {
                if (inCombat) UIBuilder.Builder.ShowCooldowns();
                else UIBuilder.Builder.HideCooldowns();
            }

            var time = DateTime.Now;
            int millis = time.Second * 1000 + time.Millisecond;
            float percent = (float)(millis % MILLIS_LOOP) / MILLIS_LOOP;

            List<CooldownPartyMemberStruct> partyMembers = GetPartyMembers();

            Dictionary<uint, CooldownPartyMember> newObjectIdToMember = new();

            for(int idx = 0; idx < partyMembers.Count; idx++) {
                var partyMember = partyMembers[idx];
                if(partyMember.Job == JobIds.OTHER || partyMember.ObjectId == 0) { // skip it
                    UIBuilder.Builder.SetCooldownRowVisible(idx, false);
                    continue;
                }

                var member = ObjectIdToMember.TryGetValue(partyMember.ObjectId, out var _member) ? _member : new CooldownPartyMember(partyMember.ObjectId);
                member.Tick(UIBuilder.Builder.Cooldowns[idx], partyMember.Job, percent);
                newObjectIdToMember[partyMember.ObjectId] = member;

                UIBuilder.Builder.SetCooldownRowVisible(idx, true);
            }
            for(int idx = partyMembers.Count; idx < 8; idx++) { // hide remaining slots
                UIBuilder.Builder.SetCooldownRowVisible(idx, false);
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

    public struct CooldownPartyMemberStruct {
        public uint ObjectId;
        public JobIds Job;
    }
}