using JobBars.Atk;
using JobBars.Data;
using JobBars.Helper;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Buffs.Manager {
    public unsafe partial class BuffManager : PerJobManager<BuffConfig[]> {
        private Dictionary<ulong, BuffPartyMember> ObjectIdToMember = [];
        private readonly List<BuffConfig> ApplyToTargetBuffs = [];

        private readonly Dictionary<JobIds, List<BuffConfig>> CustomBuffs = [];
        private List<BuffConfig> ApplyToTargetCustomBuffs => CustomBuffs.Values.SelectMany( x => x.Where( y => y.ApplyToTarget ) ).ToList();

        public BuffManager() : base( "##JobBars_Buffs" ) {
            ApplyToTargetBuffs.AddRange( JobToValue.Values.SelectMany( x => x.Where( y => y.ApplyToTarget ) ).ToList() );
            JobBars.Builder.HideAllBuffPartyList();
            JobBars.Builder.HideAllBuffs();
        }

        public BuffConfig[] GetBuffConfigs( JobIds job ) {
            List<BuffConfig> configs = [.. ApplyToTargetBuffs];
            if( JobToValue.TryGetValue( job, out var props ) ) configs.AddRange( props.Where( x => !x.ApplyToTarget ) ); // avoid double-adding

            configs.AddRange( ApplyToTargetCustomBuffs );
            if( CustomBuffs.TryGetValue( job, out var customProps ) ) configs.AddRange( customProps.Where( x => !x.ApplyToTarget ) );

            return [.. configs];
        }

        public void PerformAction( Item action, uint objectId ) {
            if( !JobBars.Configuration.BuffBarEnabled ) return;
            if( !JobBars.Configuration.BuffIncludeParty && objectId != Dalamud.ClientState.LocalPlayer.GameObjectId ) return;

            foreach( var member in ObjectIdToMember.Values ) member.ProcessAction( action, objectId );
        }

        public void Tick() {
            if( AtkHelper.CalcDoHide( JobBars.Configuration.BuffBarEnabled, JobBars.Configuration.BuffHideOutOfCombat, JobBars.Configuration.BuffHideWeaponSheathed ) ) {
                JobBars.Builder.HideAllBuffPartyList();
                JobBars.Builder.HideBuffs();
                return;
            }
            else {
                JobBars.Builder.ShowBuffs();
            }

            // ============================

            Dictionary<ulong, BuffPartyMember> newObjectIdToMember = [];
            HashSet<BuffTracker> activeBuffs = [];

            if( JobBars.PartyMembers == null ) Dalamud.Error( "PartyMembers is NULL" );

            for( var idx = 0; idx < JobBars.PartyMembers.Count; idx++ ) {
                var partyMember = JobBars.PartyMembers[idx];

                if( partyMember == null || partyMember?.Job == JobIds.OTHER || partyMember?.ObjectId == 0 ) continue;
                if( !JobBars.Configuration.BuffIncludeParty && partyMember.ObjectId != Dalamud.ClientState.LocalPlayer.GameObjectId ) continue;

                var member = ObjectIdToMember.TryGetValue( partyMember.ObjectId, out var _member ) ? _member : new BuffPartyMember( partyMember.ObjectId, partyMember.IsPlayer );
                member.Tick( activeBuffs, partyMember, out var highlight, out var partyText );
                JobBars.Builder.SetBuffPartyListVisible( idx, highlight );
                JobBars.Builder.SetBuffPartyListText( idx, ( JobBars.Configuration.BuffPartyListASTText && JobBars.CurrentJob == JobIds.AST ) ? partyText : "" );
                newObjectIdToMember[partyMember.ObjectId] = member;
            }

            for( var idx = JobBars.PartyMembers.Count; idx < 8; idx++ ) {
                JobBars.Builder.SetBuffPartyListVisible( idx, false );
                JobBars.Builder.SetBuffPartyListText( idx, "" );
            }

            var buffIdx = 0;
            foreach( var buff in JobBars.Configuration.BuffOrderByActive ?
                activeBuffs.OrderBy( b => b.CurrentState ) :
                activeBuffs.OrderBy( b => b.Id )
            ) {
                if( buffIdx >= ( AtkBuilder.MAX_BUFFS - 1 ) ) break;
                buff.TickUI( JobBars.Builder.Buffs[buffIdx] );
                buffIdx++;
            }
            for( var i = buffIdx; i < AtkBuilder.MAX_BUFFS; i++ ) {
                JobBars.Builder.Buffs[i].Hide(); // hide unused
            }

            ObjectIdToMember = newObjectIdToMember;
        }

        public static void UpdatePositionScale() {
            JobBars.Builder.SetBuffPosition( JobBars.Configuration.BuffPosition );
            JobBars.Builder.SetBuffScale( JobBars.Configuration.BuffScale );
        }

        public void ResetUI() {
            ObjectIdToMember.Clear();
        }

        public void ResetTrackers() {
            foreach( var item in ObjectIdToMember.Values ) item.Reset();
        }
    }
}
