using JobBars.Data;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace JobBars.Cooldowns.Manager {
    public struct CooldownPartyMemberStruct {
        public uint ObjectId;
        public JobIds Job;
    }

    public unsafe partial class CooldownManager : PerJobManager<CooldownConfig[]> {
        private static readonly int MILLIS_LOOP = 250;
        private Dictionary<ulong, CooldownPartyMember> ObjectIdToMember = [];
        private readonly Dictionary<JobIds, List<CooldownConfig>> CustomCooldowns = [];

        public CooldownManager() : base( "##JobBars_Cooldowns" ) {
            // Initialize custom cooldowns, remove duplicates
            foreach( var custom in JobBars.Configuration.CustomCooldown.GroupBy( x => x.GetNameId() ).Select( x => x.First() ).ToList() ) {
                if( !CustomCooldowns.ContainsKey( custom.Job ) ) CustomCooldowns[custom.Job] = [];
                CustomCooldowns[custom.Job].Add( new CooldownConfig( custom.Name, custom.GetNameId(), custom.Props ) );
            }
        }

        public CooldownConfig[] GetCooldownConfigs( JobIds job ) {
            List<CooldownConfig> configs = [];
            if( JobToValue.TryGetValue( job, out var props ) ) configs.AddRange( props );
            if( CustomCooldowns.TryGetValue( job, out var customProps ) ) configs.AddRange( customProps );
            return [.. configs];
        }

        public void PerformAction( Item action, uint objectId ) {
            if( !JobBars.Configuration.CooldownsEnabled ) return;

            foreach( var member in ObjectIdToMember.Values ) {
                member.ProcessAction( action, objectId );
            }
        }

        public void Tick() {
            if( UiHelper.CalcDoHide( JobBars.Configuration.CooldownsEnabled, JobBars.Configuration.CooldownsHideOutOfCombat, JobBars.Configuration.CooldownsHideWeaponSheathed ) ) {
                JobBars.NodeBuilder.CooldownRoot.IsVisible = false;
                return;
            }
            else {
                JobBars.NodeBuilder.CooldownRoot.IsVisible = true;
            }

            // ============================

            var time = DateTime.Now;
            var millis = time.Second * 1000 + time.Millisecond;
            var percent = ( float )( millis % MILLIS_LOOP ) / MILLIS_LOOP;

            Dictionary<ulong, CooldownPartyMember> newObjectIdToMember = [];

            if( JobBars.PartyMembers == null ) Service.Error( "PartyMembers is null" );

            for( var idx = 0; idx < JobBars.PartyMembers.Count; idx++ ) {
                var partyMember = JobBars.PartyMembers[idx];

                if( partyMember == null || partyMember?.ObjectId == 0 || partyMember?.Job == JobIds.OTHER ) {
                    JobBars.NodeBuilder.CooldownRoot.SetCooldownRowVisible( idx, false );
                    continue;
                }

                if( !JobBars.Configuration.CooldownsShowPartyMembers && partyMember.ObjectId != Service.ClientState.LocalPlayer.GameObjectId ) {
                    JobBars.NodeBuilder.CooldownRoot.SetCooldownRowVisible( idx, false );
                    continue;
                }

                var member = ObjectIdToMember.TryGetValue( partyMember.ObjectId, out var _member ) ? _member : new CooldownPartyMember( partyMember.ObjectId );
                member.Tick( JobBars.NodeBuilder.CooldownRoot.Rows[idx], partyMember, percent );
                newObjectIdToMember[partyMember.ObjectId] = member;

                JobBars.NodeBuilder.CooldownRoot.SetCooldownRowVisible( idx, true );
            }

            for( var idx = JobBars.PartyMembers.Count; idx < 8; idx++ ) { // hide remaining slots
                JobBars.NodeBuilder.CooldownRoot.SetCooldownRowVisible( idx, false );
            }

            ObjectIdToMember = newObjectIdToMember;
        }

        public static void UpdatePositionScale() {
            JobBars.NodeBuilder.CooldownRoot.Position = JobBars.Configuration.CooldownPosition + new Vector2( 0, UiHelper.PartyListOffset() );
            JobBars.NodeBuilder.CooldownRoot.Scale = new( JobBars.Configuration.CooldownScale, JobBars.Configuration.CooldownScale );
            JobBars.NodeBuilder.CooldownRoot.Update();
        }

        public void ResetUi() => ObjectIdToMember.Clear();

        public void ResetTrackers() {
            foreach( var item in ObjectIdToMember.Values ) item.Reset();
        }

        public void AddCustomCooldown( JobIds job, string name, CooldownProps props ) {
            if( !CustomCooldowns.ContainsKey( job ) ) CustomCooldowns[job] = [];
            var newCustom = JobBars.Configuration.AddCustomCooldown( name, job, props );
            CustomCooldowns[job].Add( new CooldownConfig( name, newCustom.GetNameId(), props ) );
        }

        public void DeleteCustomCooldown( JobIds job, CooldownConfig custom ) {
            CustomCooldowns[job].Remove( custom );
            JobBars.Configuration.RemoveCustomCooldown( custom.NameId );
        }
    }
}