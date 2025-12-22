using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using JobBars.Data;
using JobBars.Helper;
using System.Collections.Generic;
using System.Linq;

namespace JobBars {
    public class CurrentPartyMember {
        public ulong ObjectId;
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
            PartyMembers = [.. order.Select( objectId => objectId == 0 ? null : members.Find( member => member.ObjectId == objectId ) )];
        }

        private static List<ulong> GetPartyMemberOrder() {
            var ret = new List<ulong>();

            var partyUI = UiHelper.GetPartyUI();
            if( partyUI == null || partyUI->PartyMemberCount == 0 ) { // fallback
                ret.Add( Dalamud.Objects.LocalPlayer.GameObjectId );
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
            var localPlayer = Dalamud.Objects.LocalPlayer;

            var groupManager = GroupManager.Instance()->MainGroup;
            if( groupManager.MemberCount == 0 ) { // fallback
                var localPartyMember = new CurrentPartyMember {
                    IsPlayer = true,
                    ObjectId = localPlayer.GameObjectId,
                    CurrentHP = localPlayer.CurrentHp,
                    MaxHP = localPlayer.MaxHp,
                    Job = UiHelper.IdToJob( localPlayer.ClassJob.RowId ),
                    BuffDict = []
                };

                foreach( var status in localPlayer.StatusList ) {
                    UiHelper.StatusToBuffItem( localPartyMember.BuffDict, status );
                }

                ret.Add( localPartyMember );
                return ret;
            }

            for( var i = 0; i < 8; i++ ) {
                var info = groupManager.PartyMembers[i];
                if( info.EntityId == 0 || info.EntityId == 0xE0000000 || info.EntityId == 0xFFFFFFFF ) continue;

                var partyMember = new CurrentPartyMember {
                    IsPlayer = info.EntityId == localPlayer.GameObjectId,
                    ObjectId = info.EntityId,
                    CurrentHP = info.CurrentHP,
                    MaxHP = info.MaxHP,
                    Job = UiHelper.IdToJob( info.ClassJob ),
                    BuffDict = []
                };

                if( info.StatusManager.Status.IsEmpty ) continue;
                for( var j = 0; j < info.StatusManager.NumValidStatuses; j++ ) {
                    var status = info.StatusManager.Status[j];
                    if( status.StatusId == 0 ) continue;
                    UiHelper.StatusToBuffItem( partyMember.BuffDict, status );
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
                    if( entry.Value.SourceObject.ObjectId != ownerId ) continue;
                    if( !buffsToSearch.Contains( ( BuffIds )entry.Key.Id ) ) continue;
                    buffDict[entry.Key] = entry.Value;
                }
            }
        }
    }
}