using System;
using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace JobBars.Helper {
    public unsafe partial class UIHelper {
        public static Dictionary<Item, Status> PlayerStatus { get; private set; }
        public static uint PreviousEnemyTargetId { get; private set; }

        public static void UpdatePlayerStatus() {
            Dictionary<Item, Status> buffDict = new();

            int ownerId = (int)JobBars.ClientState.LocalPlayer.ObjectId;
            ActorToBuffItems(JobBars.ClientState.LocalPlayer, ownerId, buffDict);

            var prevEnemy = PreviousEnemyTarget;
            if (prevEnemy != null) ActorToBuffItems(prevEnemy, ownerId, buffDict);
            PreviousEnemyTargetId = prevEnemy == null ? 0 : prevEnemy.ObjectId;

            PlayerStatus = buffDict;
        }

        public static void StatusToBuffItem(Dictionary<Item, Status> buffDict, Status* status) {
            buffDict[new Item {
                Id = status->StatusID,
                Type = ItemType.Buff
            }] = new Status {
                Param = status->Param,
                RemainingTime = status->RemainingTime > 0 ? status->RemainingTime : status->RemainingTime * -1,
                SourceID = status->SourceID,
                StackCount = status->StackCount
            };
        }

        public static void StatusToBuffItem(Dictionary<Item, Status> buffDict, Dalamud.Game.ClientState.Statuses.Status status) {
            buffDict[new Item {
                Id = status.StatusId,
                Type = ItemType.Buff
            }] = new Status {
                Param = status.Param,
                RemainingTime = status.RemainingTime > 0 ? status.RemainingTime : status.RemainingTime * -1,
                SourceID = status.SourceID,
                StackCount = status.StackCount
            };
        }

        public static void ActorToBuffItems(GameObject actor, int ownerId, Dictionary<Item, Status> buffDict) {
            if (actor == null) return;
            if (actor is BattleChara charaActor) {
                foreach (var status in charaActor.StatusList) {
                    if (status.SourceID != ownerId) continue;
                    StatusToBuffItem(buffDict, status);
                }
            }
        }
    }
}
