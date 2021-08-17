using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;
using static FFXIVClientStructs.FFXIV.Client.UI.UI3DModule;

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

            var uiModule = Framework.Instance()->GetUiModule()->GetUI3DModule();
            PluginLog.Log($"{uiModule->MemberInfoCount}");
            for(int i = 0; i < 48; i++) {
                MemberInfo* info = (MemberInfo*)(new IntPtr(uiModule->MemberInfoPointerArray) + 0x8 * i);
                if (info->BattleChara == null) continue;
                if ((uint)info->BattleChara == 0xFFFFFFFF) continue;
                
                if(info->BattleChara != null) {
                    PluginLog.Log($"{i} {info->Unk_20}");
                }
            }

            PluginLog.Log("---------------");

            var groupManager = GroupManager.Instance();
            PluginLog.Log($"{groupManager->MemberCount}");
            PluginLog.Log($"{groupManager->IsAlliance}");
            PluginLog.Log($"{groupManager->PartyLeaderIndex}"); // could be FFFFFFFF
            PluginLog.Log($"{groupManager->Unk_3D40} {groupManager->Unk_3D44} {groupManager->Unk_3D48} {groupManager->Unk_3D50} {groupManager->Unk_3D5D}" +
                $" {groupManager->Unk_3D5F} {groupManager->Unk_3D60}");

            for(int i = 0; i < 8; i++) {
                PartyMember* info = (PartyMember*)(new IntPtr(groupManager->PartyMembers) + 0x8 * i);
                PluginLog.Log($"{info->ObjectID} {info->ClassJob}"); // FFFF0000, E0000000
            }

            PluginLog.Log("---------------");

            //var numArray = Framework.Instance()->GetUiModule()->RaptureAtkModule.AtkModule.AtkArrayDataHolder.NumberArrays[19];

            Init();
        }

        public List<CooldownPartyMemberStruct> GetPartyMembers() {
            List<CooldownPartyMemberStruct> partyMembers = new();
            var localPlayer = PluginInterface.ClientState.LocalPlayer;

            var uiModule = Framework.Instance()->GetUiModule()->GetUI3DModule();

            // TODO?

            if (partyMembers.Count == 0) {
                partyMembers.Add(new CooldownPartyMemberStruct {
                    ObjectId = localPlayer.ObjectId,
                    Job = DataManager.IdToJob(localPlayer.ClassJob.Id)
                });
            }
            return partyMembers;
        }

        public void PerformAction(Item action, uint objectId) {
            if (!Configuration.Config.CooldownsEnabled) return;

            foreach (var member in ObjectIdToMember.Values) {
                member.ProcessAction(action, objectId);
            }
        }

        public void Tick() {
            if (!Configuration.Config.CooldownsEnabled) return;

            var time = DateTime.Now;
            int millis = time.Second * 1000 + time.Millisecond;
            float percent = (float)(millis % MILLIS_LOOP) / MILLIS_LOOP;

            // ====== TEMP ========
            List<CooldownPartyMemberStruct> partyMembers = new();
            var localPlayer = PluginInterface.ClientState.LocalPlayer;
            var self = new CooldownPartyMemberStruct {
                ObjectId = localPlayer.ObjectId,
                Job = localPlayer.ClassJob.Id < 19 ? JobIds.OTHER : (JobIds)localPlayer.ClassJob.Id
            };
            if (partyMembers.Count == 0) partyMembers.Add(self);
            //====================

            Dictionary<uint, CooldownPartyMember> newObjectIdToMember = new();

            for(int idx = 0; idx < partyMembers.Count; idx++) {
                var partyMember = partyMembers[idx];
                var member = ObjectIdToMember.TryGetValue(partyMember.ObjectId, out var _member) ? _member : new CooldownPartyMember(partyMember.ObjectId);
                member.Tick(UIBuilder.Builder.Cooldowns[idx], partyMember.Job, percent);
                newObjectIdToMember[partyMember.ObjectId] = member;

                UIBuilder.Builder.SetCooldownRowVisible(idx, true);
            }
            for(int idx = partyMembers.Count; idx < 8; idx++) {
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