using JobBars.Data;
using JobBars.Helper;
using JobBars.Nodes.Buff;
using JobBars.Nodes.Builder;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Buffs.Manager {
    public unsafe partial class BuffManager : PerJobManager<BuffConfig[]> {
        private Dictionary<ulong, BuffPartyMember> ObjectIdToMember = [];
        private readonly List<BuffConfig> ApplyToTargetBuffs = [];

        private readonly Dictionary<JobIds, List<BuffConfig>> CustomBuffs = [];
        private List<BuffConfig> ApplyToTargetCustomBuffs => [.. CustomBuffs.Values.SelectMany( x => x.Where( y => y.ApplyToTarget ) )];

        public BuffManager() : base( "##JobBars_Buffs" ) {
            ApplyToTargetBuffs.AddRange( [.. JobToValue.Values.SelectMany( x => x.Where( y => y.ApplyToTarget ) )] );
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
            if( UiHelper.CalcDoHide( JobBars.Configuration.BuffBarEnabled, JobBars.Configuration.BuffHideOutOfCombat, JobBars.Configuration.BuffHideWeaponSheathed ) ) {
                JobBars.NodeBuilder.HighlightRoot.HideAll();
                JobBars.NodeBuilder.BuffRoot.IsVisible = false;
                return;
            }
            else {
                JobBars.NodeBuilder.BuffRoot.IsVisible = true;
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
                JobBars.NodeBuilder.HighlightRoot.Highlights[idx].IsVisible = highlight;
                newObjectIdToMember[partyMember.ObjectId] = member;
            }

            for( var idx = JobBars.PartyMembers.Count; idx < 8; idx++ ) {
                JobBars.NodeBuilder.HighlightRoot.Highlights[idx].IsVisible = false;
            }

            var buffIdx = 0;
            foreach( var buff in JobBars.Configuration.BuffOrderByActive ?
                activeBuffs.OrderBy( b => b.CurrentState ) :
                activeBuffs.OrderBy( b => b.Id )
            ) {
                if( buffIdx >= ( BuffRoot.MAX_BUFFS - 1 ) ) break;
                buff.TickUi( JobBars.NodeBuilder.BuffRoot.Buffs[buffIdx] );
                buffIdx++;
            }
            for( var i = buffIdx; i < BuffRoot.MAX_BUFFS; i++ ) {
                JobBars.NodeBuilder.BuffRoot.Buffs[i].IsVisible = false;
            }

            ObjectIdToMember = newObjectIdToMember;
        }

        public static void UpdatePositionScale() {
            NodeBuilder.SetPositionGlobal( JobBars.NodeBuilder.BuffRoot, JobBars.Configuration.BuffPosition );
            NodeBuilder.SetScaleGlobal( JobBars.NodeBuilder.BuffRoot, JobBars.Configuration.BuffScale );
        }

        public void ResetUi() => ObjectIdToMember.Clear();

        public void Reset() {
            foreach( var item in ObjectIdToMember.Values ) item.Reset();
        }
    }
}
