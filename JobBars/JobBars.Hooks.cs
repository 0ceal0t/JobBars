using System;
using System.Collections.Generic;
using JobBars.GameStructs;
using JobBars.Data;
using Dalamud.Game.ClientState.Objects.Types;
using JobBars.Helper;
using Dalamud.Logging;

namespace JobBars {
    public unsafe partial class JobBars {
        private void ReceiveActionEffect(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail) {
            if (!PlayerExists) {
                ReceiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
                return;
            }

            uint id = *((uint*)effectHeader.ToPointer() + 0x2);
            byte type = *((byte*)effectHeader.ToPointer() + 0x1F); // 1 = action

            var selfId = (int)ClientState.LocalPlayer.ObjectId;
            var isSelf = sourceId == selfId;
            var isPet = !isSelf && (GaugeManager?.CurrentJob == JobIds.SMN || GaugeManager?.CurrentJob == JobIds.SCH) && IsPet(sourceId, selfId);
            var isParty = !isSelf && !isPet && UIHelper.IsInParty((uint)sourceId);

            if (type != 1 || !(isSelf || isPet || isParty)) {
                ReceiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
                return;
            }

            var actionItem = new Item {
                Id = id,
                Type = (UIHelper.IsGCD(id) ? ItemType.GCD : ItemType.OGCD)
            };

            if (!isParty) { // don't let party members affect our gauge
                GaugeManager?.PerformAction(actionItem);
            }
            if (!isPet) {
                BuffManager?.PerformAction(actionItem, (uint) sourceId);
                CooldownManager?.PerformAction(actionItem, (uint)sourceId);
            }

            byte targetCount = *(byte*)(effectHeader + 0x21);

            int effectsEntries = 0;
            int targetEntries = 1;
            if (targetCount == 0) {
                effectsEntries = 0;
                targetEntries = 1;
            }
            else if (targetCount == 1) {
                effectsEntries = 8;
                targetEntries = 1;
            }
            else if (targetCount <= 8) {
                effectsEntries = 64;
                targetEntries = 8;
            }
            else if (targetCount <= 16) {
                effectsEntries = 128;
                targetEntries = 16;
            }
            else if (targetCount <= 24) {
                effectsEntries = 192;
                targetEntries = 24;
            }
            else if (targetCount <= 32) {
                effectsEntries = 256;
                targetEntries = 32;
            }

            List<EffectEntry> entries = new(effectsEntries);
            for (int i = 0; i < effectsEntries; i++) {
                entries.Add(*(EffectEntry*)(effectArray + i * 8));
            }

            ulong[] targets = new ulong[targetEntries];
            for (int i = 0; i < targetCount; i++) {
                targets[i] = *(ulong*)(effectTrail + i * 8);
            }

            for (int i = 0; i < entries.Count; i++) {
                ulong tTarget = targets[i / 8];

                if (entries[i].type == ActionEffectType.ApplyStatusTarget || entries[i].type == ActionEffectType.ApplyStatusSource) {

                    var buffItem = new Item {
                        Id = entries[i].value,
                        Type = ItemType.Buff
                    };

                    if (!isParty) {
                        GaugeManager?.PerformAction(buffItem); // more accurate than using the status list
                    }
                }
            }

            ReceiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
        }

        private void ActorControlSelf(uint entityId, uint id, uint arg0, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, UInt64 targetId, byte a10) {
            ActorControlSelfHook.Original(entityId, id, arg0, arg1, arg2, arg3, arg4, arg5, targetId, a10);
            if (entityId > 0 && entityId == ClientState?.LocalPlayer?.ObjectId && id == 23) {
                UIHelper.UpdateActorTick();
            }

            if (arg1 == 0x40000010) {
                GaugeManager?.Reset();
                BuffManager?.ResetTrackers();
                CooldownManager?.ResetTrackers();
                UIHelper.ResetTicks();
            }
        }

        private void ZoneChanged(object sender, ushort e) {
            GaugeManager?.Reset();
            BuffManager?.ResetTrackers();
            // don't reset CDs on zone change
            UIHelper.ResetTicks();
        }

        private bool IsPet(int objectId, int ownerId) {
            if (objectId == 0) return false;
            foreach (var actor in Objects) {
                if (actor == null) continue;
                if (actor.ObjectId == objectId) {
                    if (actor is BattleNpc npc) {
                        if (npc.Address == IntPtr.Zero) return false;
                        return npc.OwnerId == ownerId;
                    }
                    return false;
                }
            }
            return false;
        }
    }
}
