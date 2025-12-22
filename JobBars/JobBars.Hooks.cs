using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using JobBars.Data;
using JobBars.GameStructs;
using JobBars.Helper;
using System;
using System.Collections.Generic;

namespace JobBars {
    public unsafe partial class JobBars {
        private delegate void ReceiveActionEffectDelegate( int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail );
        private readonly Hook<ReceiveActionEffectDelegate> ReceiveActionEffectHook;

        private delegate void ActorControlSelfDelegate( uint entityId, uint id, uint arg0, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, uint arg6, uint arg7, ulong targetId, byte a10 );
        private readonly Hook<ActorControlSelfDelegate> ActorControlSelfHook;

        private void ReceiveActionEffect( int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail ) {
            if( !NodeBuilder.IsLoaded || !Dalamud.ClientState.IsLoggedIn ) {
                ReceiveActionEffectHook.Original( sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail );
                return;
            }

            var id = *( ( uint* )effectHeader.ToPointer() + 0x2 );
            var type = *( ( byte* )effectHeader.ToPointer() + 0x1F ); // 1 = action

            var selfId = ( int )Dalamud.Objects.LocalPlayer.GameObjectId;
            var isSelf = sourceId == selfId;
            var isPet = !isSelf && ( GaugeManager?.CurrentJob == JobIds.SMN || GaugeManager?.CurrentJob == JobIds.SCH ) && IsPet( ( ulong )sourceId, selfId );
            var isParty = !isSelf && !isPet && IsInParty( ( uint )sourceId );

            if( type != 1 || !( isSelf || isPet || isParty ) ) {
                ReceiveActionEffectHook.Original( sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail );
                return;
            }

            var actionItem = new Item {
                Id = id,
                Type = ( UiHelper.IsGcd( id ) ? ItemType.GCD : ItemType.OGCD )
            };

            if( !isParty ) { // don't let party members affect our gauge
                GaugeManager?.PerformAction( actionItem );
                IconManager?.PerformAction( actionItem );
            }
            if( !isPet ) {
                BuffManager?.PerformAction( actionItem, ( uint )sourceId );
                CooldownManager?.PerformAction( actionItem, ( uint )sourceId );
            }

            var targetCount = *( byte* )( effectHeader + 0x21 );

            var effectsEntries = 0;
            var targetEntries = 1;
            if( targetCount == 0 ) {
                effectsEntries = 0;
                targetEntries = 1;
            }
            else if( targetCount == 1 ) {
                effectsEntries = 8;
                targetEntries = 1;
            }
            else if( targetCount <= 8 ) {
                effectsEntries = 64;
                targetEntries = 8;
            }
            else if( targetCount <= 16 ) {
                effectsEntries = 128;
                targetEntries = 16;
            }
            else if( targetCount <= 24 ) {
                effectsEntries = 192;
                targetEntries = 24;
            }
            else if( targetCount <= 32 ) {
                effectsEntries = 256;
                targetEntries = 32;
            }

            List<EffectEntry> entries = new( effectsEntries );
            for( var i = 0; i < effectsEntries; i++ ) {
                entries.Add( *( EffectEntry* )( effectArray + i * 8 ) );
            }

            var targets = new ulong[targetEntries];
            for( var i = 0; i < targetCount; i++ ) {
                targets[i] = *( ulong* )( effectTrail + i * 8 );
            }

            for( var i = 0; i < entries.Count; i++ ) {
                var entryTarget = targets[i / 8];

                if( entries[i].type == ActionEffectType.ApplyStatusTarget || entries[i].type == ActionEffectType.ApplyStatusSource ) {
                    var buffItem = new Item {
                        Id = entries[i].value,
                        Type = ItemType.Buff
                    };

                    if( !isParty ) { // more accurate than using the status list
                        GaugeManager?.PerformAction( buffItem );
                        IconManager?.PerformAction( buffItem );
                    }
                }
            }

            ReceiveActionEffectHook.Original( sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail );
        }

        private void ActorControlSelf( uint entityId, uint id, uint arg0, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, uint arg6, uint arg7, ulong targetId, byte a10 ) {


            ActorControlSelfHook.Original( entityId, id, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, targetId, a10 );
            if( !NodeBuilder.IsLoaded ) return;

            if( entityId > 0 && id == Constants.ActorControlSelfId && entityId == Dalamud.Objects.LocalPlayer?.GameObjectId ) {
                UiHelper.UpdateActorTick();
            }
            else if( entityId > 0 && id == Constants.ActorControlOtherId ) {
                UiHelper.UpdateDoTTick( entityId );
            }

            if( arg1 == Constants.WipeArg1 ) {
                GaugeManager?.Reset();
                IconManager?.Reset();
                BuffManager?.Reset();
                CooldownManager?.ResetTrackers();
                UiHelper.ResetTicks();
            }
        }

        private static bool IsPet( ulong objectId, int ownerId ) {
            if( objectId == 0 ) return false;
            foreach( var actor in Dalamud.Objects ) {
                if( actor == null ) continue;
                if( actor.GameObjectId == objectId ) {
                    if( actor is IBattleNpc npc ) {
                        if( npc.Address == IntPtr.Zero ) return false;
                        return npc.OwnerId == ownerId;
                    }
                    return false;
                }
            }
            return false;
        }
    }
}
