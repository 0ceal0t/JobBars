using Dalamud.Logging;
using Dalamud.Plugin;
using JobBars.Data;
using JobBars.UI;
using System.Collections.Generic;

namespace JobBars.Cooldowns {
    public unsafe partial class CooldownManager {
        public static CooldownManager Manager { get; private set; }
        public static void Dispose() { Manager = null; }

        private Dictionary<uint, CooldownPartyMember> ObjectIdToMember = new();

        private readonly DalamudPluginInterface PluginInterface;

        public CooldownManager(DalamudPluginInterface pluginInterface) {
            Manager = this;
            PluginInterface = pluginInterface;

            //var groupManager = GroupManager.Instance();
            //var uiModule = Framework.Instance()->GetUiModule()->GetUI3DModule();
            //PluginLog.Log($"{uiModule->MemberInfoCount}");

            Init();
        }

        public void PerformAction(Item action, uint objectId) {
            foreach(var member in ObjectIdToMember.Values) {
                member.ProcessAction(action, objectId);
            }
        }

        public void Tick() {
            List<CooldownPartyMemberStruct> partyMembers = new();

            var localPlayer = PluginInterface.ClientState.LocalPlayer;
            var self = new CooldownPartyMemberStruct {
                ObjectId = localPlayer.ObjectId,
                Job = localPlayer.ClassJob.Id < 19 ? JobIds.OTHER : (JobIds)localPlayer.ClassJob.Id
            };

            // TODO?

            if (partyMembers.Count == 0) partyMembers.Add(self);

            Dictionary<uint, CooldownPartyMember> newObjectIdToMember = new();

            for(int idx = 0; idx < partyMembers.Count; idx++) {
                var partyMember = partyMembers[idx];
                var member = ObjectIdToMember.TryGetValue(partyMember.ObjectId, out var _member) ? _member : new CooldownPartyMember(partyMember.ObjectId);
                member.Tick(UIBuilder.Builder.Cooldowns[idx], partyMember.Job);
                newObjectIdToMember[partyMember.ObjectId] = member;

                UIBuilder.Builder.SetCooldownRowVisible(idx, true);
            }
            for(int idx = partyMembers.Count; idx < 8; idx++) {
                UIBuilder.Builder.SetCooldownRowVisible(idx, false);
            }

            ObjectIdToMember = newObjectIdToMember;
        }

        public void Reset() {
            ObjectIdToMember.Clear();
        }
    }

    public struct CooldownPartyMemberStruct {
        public uint ObjectId;
        public JobIds Job;
    }
}