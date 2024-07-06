using JobBars.Atk;
using JobBars.Data;
using JobBars.Nodes.Cooldown;
using System.Collections.Generic;
using System.Linq;
using static JobBars.Cooldowns.CooldownTracker;

namespace JobBars.Cooldowns {
    public unsafe class CooldownPartyMember {
        private JobIds PartyMemberCurrentJob = JobIds.OTHER;
        private readonly List<CooldownTracker> Trackers = [];
        private readonly ulong ObjectId;

        public CooldownPartyMember( ulong objectId ) {
            ObjectId = objectId;
        }

        public void Tick( CooldownRow row, CurrentPartyMember partyMember, float percent ) {
            if( PartyMemberCurrentJob != partyMember.Job ) {
                PartyMemberCurrentJob = partyMember.Job;
                SetupTrackers();
            }

            var trackerIdx = 0;
            foreach( var tracker in Trackers ) {
                tracker.Tick( partyMember.BuffDict );

                if( trackerIdx >= ( AtkCooldown.MAX_ITEMS - 1 ) ) break;
                // skip if disabled
                if( !JobBars.Configuration.CooldownsStateShowDefault && tracker.CurrentState == TrackerState.None ||
                    !JobBars.Configuration.CooldownsStateShowRunning && tracker.CurrentState == TrackerState.Running ||
                    !JobBars.Configuration.CooldownsStateShowOnCD && tracker.CurrentState == TrackerState.OnCD ||
                    !JobBars.Configuration.CooldownsStateShowOffCD && tracker.CurrentState == TrackerState.OffCD
                ) {
                    trackerIdx++;
                    continue;
                }

                var uiIdx = JobBars.Configuration.CooldownsLeftAligned ? AtkCooldown.MAX_ITEMS - 1 - trackerIdx : trackerIdx;
                tracker.TickUi( row.Nodes[uiIdx], percent );
                trackerIdx++;
            }
            for( var i = trackerIdx; i < AtkCooldown.MAX_ITEMS; i++ ) {
                var uiIdx = JobBars.Configuration.CooldownsLeftAligned ? AtkCooldown.MAX_ITEMS - 1 - i : i;
                row.Nodes[uiIdx].IsVisible = false;
            }
        }

        public void ProcessAction( Item action, uint objectId ) {
            if( ObjectId != objectId ) return;
            foreach( var tracker in Trackers ) tracker.ProcessAction( action );
        }

        private void SetupTrackers() {
            Trackers.Clear();

            var trackerProps = JobBars.CooldownManager.GetCooldownConfigs( PartyMemberCurrentJob );
            var count = 0;
            foreach( var prop in trackerProps.OrderBy( x => x.Order ) ) {
                if( !prop.Enabled ) continue;
                count++;
                if( count > AtkCooldown.MAX_ITEMS ) continue;
                Trackers.Add( new CooldownTracker( prop ) );
            }
        }

        public void Reset() {
            foreach( var item in Trackers ) item.Reset();
        }
    }
}