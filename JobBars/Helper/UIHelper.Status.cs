using System;
using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace JobBars.Helper {
    public struct StatusDuration {
        public float Duration;
        public byte StackCount;
    }

    public unsafe partial class UIHelper {
        public static Dictionary<Item, StatusDuration> PlayerStatus;

        public static void UpdatePlayerStatus() {
            Dictionary<Item, StatusDuration> buffDict = new();

            int ownerId = (int)JobBars.ClientState.LocalPlayer.ObjectId;
            AddStatus(JobBars.ClientState.LocalPlayer, ownerId, buffDict);

            var prevEnemy = PreviousEnemyTarget;
            if (prevEnemy != null) AddStatus(prevEnemy, ownerId, buffDict);

            PlayerStatus = buffDict;
        }

        public static void StatusToDuration(Dictionary<Item, StatusDuration> buffDict, Status* status) {
            buffDict[new Item {
                Id = status->StatusID,
                Type = ItemType.Buff
            }] = new StatusDuration {
                Duration = status->RemainingTime > 0 ? status->RemainingTime : status->RemainingTime * -1,
                StackCount = status->StackCount
            };
        }

        public static void StatusToDuration(Dictionary<Item, StatusDuration> buffDict, Dalamud.Game.ClientState.Statuses.Status status) {
            buffDict[new Item {
                Id = status.StatusId,
                Type = ItemType.Buff
            }] = new StatusDuration {
                Duration = status.RemainingTime > 0 ? status.RemainingTime : status.RemainingTime * -1,
                StackCount = status.StackCount
            };
        }

        public static void AddStatus(GameObject actor, int ownerId, Dictionary<Item, StatusDuration> buffDict) {
            if (actor == null) return;
            if (actor is BattleChara charaActor) {
                foreach (var status in charaActor.StatusList) {
                    if (status.SourceID != ownerId) continue;
                    StatusToDuration(buffDict, status);
                }
            }
        }
    }
}
