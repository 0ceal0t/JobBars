using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace JobBars {
    public class CurrentPartyMember {
        public uint ObjectId;
        public JobIds Job;
        public uint CurrentHP;
        public uint MaxHP;
        public Dictionary<Item, Status> BuffDict;
        public bool IsPlayer;
    }

    public unsafe partial class JobBars {
        public static List<CurrentPartyMember> PartyMembers { get; private set; } = [];

        public static void UpdatePartyMembers() {
            var order = GetPartyMemberOrder();
            var members = GetPartyMembers();
            PartyMembers = order.Select( objectId => objectId == 0 ? null : members.Find( member => member.ObjectId == objectId ) ).ToList();
        }

        private static List<uint> GetPartyMemberOrder() {
            var ret = new List<uint>();

            var partyUI = AtkHelper.GetPartyUI();
            if( partyUI == null || partyUI->PartyMemberCount == 0 ) { // fallback
                ret.Add( Dalamud.ClientState.LocalPlayer.ObjectId );
                return ret;
            }

            for( var i = 0; i < partyUI->PartyMemberCount; i++ ) {
                var member = partyUI->PartyMember[i];
                var objectId = ( uint )member.ObjectID;
                ret.Add( ( objectId == 0xE0000000 || objectId == 0xFFFFFFFF ) ? 0 : objectId );
            }

            return ret;
        }

        private static List<CurrentPartyMember> GetPartyMembers() {
            var ret = new List<CurrentPartyMember>();
            var localPlayer = Dalamud.ClientState.LocalPlayer;

            var groupManager = GroupManager.Instance();
            if( groupManager == null || groupManager->MemberCount == 0 ) { // fallback
                var localPartyMember = new CurrentPartyMember {
                    IsPlayer = true,
                    ObjectId = localPlayer.ObjectId,
                    CurrentHP = localPlayer.CurrentHp,
                    MaxHP = localPlayer.MaxHp,
                    Job = AtkHelper.IdToJob( localPlayer.ClassJob.Id ),
                    BuffDict = []
                };

                foreach( var status in localPlayer.StatusList ) {
                    AtkHelper.StatusToBuffItem( localPartyMember.BuffDict, status );
                }

                ret.Add( localPartyMember );
                return ret;
            }

            for( var i = 0; i < 8; i++ ) {
                var info = ( PartyMember* )( new IntPtr( groupManager->PartyMembers ) + Marshal.SizeOf( typeof( PartyMember ) ) * i );
                if( info->ObjectID == 0 || info->ObjectID == 0xE0000000 || info->ObjectID == 0xFFFFFFFF ) continue;

                var partyMember = new CurrentPartyMember {
                    IsPlayer = info->ObjectID == localPlayer.ObjectId,
                    ObjectId = info->ObjectID,
                    CurrentHP = info->CurrentHP,
                    MaxHP = info->MaxHP,
                    Job = AtkHelper.IdToJob( info->ClassJob ),
                    BuffDict = []
                };

                if( info->StatusManager.Status == null ) continue;
                for( var j = 0; j < info->StatusManager.NumValidStatuses; j++ ) {
                    var status = ( Status* )( new IntPtr( info->StatusManager.Status ) + 0xC * j );
                    if( status->StatusID == 0 ) continue;
                    AtkHelper.StatusToBuffItem( partyMember.BuffDict, status );
                }

                ret.Add( partyMember );
            }

            return ret;
        }

        public static unsafe bool IsInParty( uint objectId ) {
            if( objectId == 0 || objectId == 0xE0000000 || objectId == 0xFFFFFFFF ) return false;

            if( PartyMembers == null ) return false;
            foreach( var member in PartyMembers ) {
                if( member == null ) continue;
                if( member.ObjectId == objectId ) return true;
            }
            return false;
        }

        public static unsafe void SearchForPartyMemberStatus( int ownerId, Dictionary<Item, Status> buffDict, List<BuffIds> buffsToSearch ) {
            if( PartyMembers == null ) return;
            foreach( var member in PartyMembers ) {
                if( member == null ) continue;
                foreach( var entry in member.BuffDict ) {
                    if( entry.Value.SourceID != ownerId ) continue;
                    if( !buffsToSearch.Contains( ( BuffIds )entry.Key.Id ) ) continue;
                    buffDict[entry.Key] = entry.Value;
                }
            }
        }
    }
}