using System;
using System.Collections.Generic;
using JobBars.GameStructs;
using JobBars.Data;
using Dalamud.Game.ClientState.Actors.Types;
using Dalamud.Game.ClientState.Actors.Types.NonPlayer;
using Dalamud.Plugin;

namespace JobBars {
    public unsafe partial class JobBars {
        private void ReceiveActionEffect(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail) {
            if (!PlayerExists || !Initialized) {
                receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
                return;
            }

            uint id = *((uint*)effectHeader.ToPointer() + 0x2);
            ushort op = *((ushort*)effectHeader.ToPointer() - 0x7);

            var selfId = (int)PluginInterface.ClientState.LocalPlayer.ActorId;
            var isSelf = sourceId == selfId;
            var isPet = !isSelf && ((GManager?.CurrentJob == JobIds.SMN || GManager?.CurrentJob == JobIds.SCH) && IsPet(sourceId, selfId));
            var isParty = !isSelf && !isPet && IsInParty(sourceId);

            if (!(isSelf || isPet || isParty)) {
                receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
                return;
            }

            var actionItem = new Item {
                Id = id,
                Type = (GCDs.Contains(id) ? ItemType.GCD : ItemType.OGCD)
            };

            if (!isParty) { // don't let party members affect our gauge
                GManager?.PerformAction(actionItem);
            }
            if (isSelf || Configuration.Config.BuffIncludeParty) {
                BManager?.PerformAction(actionItem);
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

                if (entries[i].type == ActionEffectType.Gp_Or_Status || entries[i].type == ActionEffectType.ApplyStatusEffectTarget) {
                    // idk what's up with Gp_Or_Status. sometimes the enum is wrong?

                    var buffItem = new Item {
                        Id = entries[i].value,
                        Type = ItemType.Buff
                    };

                    if (!isParty) { // don't let party members affect our gauge
                        GManager?.PerformAction(buffItem);
                    }

                    // only care about buffs that we are getting
                    // ignore if we don't care about party members' CDs
                    if ((int)tTarget == selfId && Configuration.Config.BuffIncludeParty) {
                        BManager?.PerformAction(buffItem);
                    }
                }
            }

            receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
        }

        private void ActorControlSelf(uint entityId, uint id, uint arg0, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, UInt64 targetId, byte a10) {
            actorControlSelfHook.Original(entityId, id, arg0, arg1, arg2, arg3, arg4, arg5, targetId, a10);
            if (arg1 == 0x40000010) { // WIPE
                Reset();
            }
        }

        private void ZoneChanged(object sender, ushort e) {
            Reset();
        }

        private bool IsPet(int actorId, int ownerId) {
            if (actorId == 0) return false;
            foreach (Actor actor in PluginInterface.ClientState.Actors) {
                if (actor == null) continue;
                if (actor.ActorId == actorId) {
                    if (actor is BattleNpc npc) {
                        if (npc.Address == IntPtr.Zero) return false;
                        return npc.OwnerId == ownerId;
                    }
                    return false;
                }
            }
            return false;
        }

        private bool IsInParty(int actorId) {
            if (actorId == 0) return false;
            foreach (var pMember in Party) {
                if (pMember == null) continue;
                if (pMember.ActorId == 0) continue;
                if (pMember.ActorId == actorId) {
                    return true;
                }
            }
            return false;
        }
    }
}
