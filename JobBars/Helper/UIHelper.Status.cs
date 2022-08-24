using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using JobBars.Data;

namespace JobBars.Helper {
    public unsafe partial class UIHelper {
        public static Dictionary<Item, Status> PlayerStatus { get; private set; }
        public static uint PreviousEnemyTargetId { get; private set; }

        private static readonly List<uint> StatusIgnoreSource = new(new[] { // count these buffs even though they're not coming from us
            (uint)BuffIds.PhysicalAttenuation,
            (uint)BuffIds.AstralAttenuation,
            (uint)BuffIds.UmbralAttenuation
        });

        public static void UpdatePlayerStatus() {
            Dictionary<Item, Status> buffDict = new();

            int ownerId = (int)JobBars.ClientState.LocalPlayer.ObjectId;
            ActorToBuffItems(JobBars.ClientState.LocalPlayer, ownerId, buffDict);

            var prevEnemy = PreviousEnemyTarget;
            if (prevEnemy != null) ActorToBuffItems(prevEnemy, ownerId, buffDict);
            PreviousEnemyTargetId = prevEnemy == null ? 0 : prevEnemy.ObjectId;

            PlayerStatus = buffDict;
        }

        public static bool CheckForTriggers(Dictionary<Item, Status> buffDict, Item[] triggers, out Item newTrigger) {
            newTrigger = default;
            foreach (var trigger in triggers.Where(t => t.Type == ItemType.Buff)) {
                if (buffDict.ContainsKey(trigger) && buffDict[trigger].RemainingTime > 0) {
                    newTrigger = trigger;
                    return true;
                }
            }
            return false;
        }

        public static void StatusToBuffItem(Dictionary<Item, Status> buffDict, Status* status) {
            buffDict[new Item {
                Id = status->StatusID,
                Type = ItemType.Buff
            }] = new Status {
                Param = status->Param,
                RemainingTime = status->RemainingTime > 0 ? status->RemainingTime : status->RemainingTime * -1,
                SourceID = status->SourceID,
                StackCount = status->StackCount,
                StatusID = status->StatusID
            };
        }

        public static void StatusToBuffItem(Dictionary<Item, Status> buffDict, Dalamud.Game.ClientState.Statuses.Status status) {
            buffDict[new Item {
                Id = status.StatusId,
                Type = ItemType.Buff
            }] = new Status {
                Param = (byte)status.Param,
                RemainingTime = status.RemainingTime > 0 ? status.RemainingTime : status.RemainingTime * -1,
                SourceID = status.SourceId,
                StackCount = status.StackCount,
                StatusID = (ushort)status.StatusId
            };
        }

        public static void ActorToBuffItems(GameObject actor, int ownerId, Dictionary<Item, Status> buffDict) {
            if (actor == null) return;
            if (actor is BattleChara charaActor) {
                foreach (var status in charaActor.StatusList) {
                    if (status.SourceId != ownerId && !StatusIgnoreSource.Contains(status.StatusId)) continue;
                    StatusToBuffItem(buffDict, status);
                }
            }
        }
    }
}
