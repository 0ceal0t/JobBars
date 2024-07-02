using JobBars.Data;
using System.Collections.Generic;

namespace JobBars.Buffs {
    public class BuffPartyMember {
        private JobIds PartyMemberCurrentJob = JobIds.OTHER;
        private readonly List<BuffTracker> Trackers = [];
        private readonly ulong ObjectId;
        private readonly bool IsPlayer;

        public BuffPartyMember( ulong objectId, bool isPlayer ) {
            ObjectId = objectId;
            IsPlayer = isPlayer;
        }

        public void Tick( HashSet<BuffTracker> trackers, CurrentPartyMember partyMember, out bool highlight, out string partyText ) {
            highlight = false;
            partyText = "";

            if( PartyMemberCurrentJob != partyMember.Job ) {
                PartyMemberCurrentJob = partyMember.Job;
                SetupTrackers();
            }

            foreach( var tracker in Trackers ) {
                tracker.Tick( partyMember.BuffDict );
                // add the icon if it's active and either a personal buff or on yourself
                if( tracker.Enabled && ( !tracker.ApplyToTarget || IsPlayer ) ) trackers.Add( tracker );
                if( tracker.Highlight ) {
                    highlight = true;
                }
                if( tracker.Active && tracker.ShowPartyText ) {
                    partyText = tracker.Text;
                }
            }
        }

        public void ProcessAction( Item action, uint objectId ) {
            if( ObjectId != objectId ) return;
            foreach( var tracker in Trackers ) tracker.ProcessAction( action );
        }

        public void SetupTrackers() {
            Trackers.Clear();

            var trackerProps = JobBars.BuffManager.GetBuffConfigs( PartyMemberCurrentJob );
            foreach( var prop in trackerProps ) {
                if( !prop.Enabled ) continue;
                Trackers.Add( new BuffTracker( prop ) );
            }
        }

        public void Reset() {
            foreach( var item in Trackers ) item.Reset();
        }
    }
}
