using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using JobBars.Data;
using System.Collections.Generic;
using System.Linq;
using DalamudStatus = Dalamud.Game.ClientState.Statuses.Status;

namespace JobBars.Helper {
    public unsafe partial class UiHelper {
        public static Dictionary<Item, Status> PlayerStatus { get; private set; }
        public static ulong PreviousEnemyTargetId { get; private set; }

        private static readonly List<uint> StatusIgnoreSource = new( [ // count these buffs even though they're not coming from us
            (uint)BuffIds.PhysicalAttenuation,
            (uint)BuffIds.AstralAttenuation,
            (uint)BuffIds.UmbralAttenuation
        ] );

        public static void UpdatePlayerStatus() {
            Dictionary<Item, Status> buffDict = [];

            var ownerId = ( int )Dalamud.ClientState.LocalPlayer.GameObjectId;
            ActorToBuffItems( Dalamud.ClientState.LocalPlayer, ownerId, buffDict );

            var prevEnemy = PreviousEnemyTarget;
            if( prevEnemy != null ) ActorToBuffItems( prevEnemy, ownerId, buffDict );
            PreviousEnemyTargetId = prevEnemy == null ? 0 : prevEnemy.GameObjectId;

            PlayerStatus = buffDict;
        }

        public static bool CheckForTriggers( Dictionary<Item, Status> buffDict, Item[] triggers, out Item newTrigger ) {
            newTrigger = default;
            foreach( var trigger in triggers.Where( t => t.Type == ItemType.Buff ) ) {
                if( buffDict.TryGetValue( trigger, out var value ) && value.RemainingTime > 0 ) {
                    newTrigger = trigger;
                    return true;
                }
            }
            return false;
        }

        public static void StatusToBuffItem( Dictionary<Item, Status> buffDict, Status status ) {
            buffDict[new Item {
                Id = status.StatusId,
                Type = ItemType.Buff
            }] = new Status {
                Param = status.Param,
                RemainingTime = status.RemainingTime > 0 ? status.RemainingTime : status.RemainingTime * -1,
                SourceId = status.SourceId,
                StatusId = status.StatusId
            };
        }

        public static void StatusToBuffItem( Dictionary<Item, Status> buffDict, DalamudStatus status ) {
            buffDict[new Item {
                Id = status.StatusId,
                Type = ItemType.Buff
            }] = new Status {
                Param = ( byte )status.Param,
                RemainingTime = status.RemainingTime > 0 ? status.RemainingTime : status.RemainingTime * -1,
                SourceId = status.SourceId,
                StatusId = ( ushort )status.StatusId
            };
        }

        public static void ActorToBuffItems( IGameObject actor, int ownerId, Dictionary<Item, Status> buffDict ) {
            if( actor == null ) return;
            if( actor is IBattleChara charaActor ) {
                foreach( var status in charaActor.StatusList ) {
                    if( status.SourceId != ownerId && !StatusIgnoreSource.Contains( status.StatusId ) ) continue;
                    StatusToBuffItem( buffDict, status );
                }
            }
        }
    }
}
