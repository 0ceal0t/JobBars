using JobBars.Data;
using System.Collections.Generic;

namespace JobBars.Buffs {
    public class BuffPartyMember {
        private JobIds PartyMemberCurrentJob = JobIds.OTHER;
        private readonly List<BuffTracker> Trackers = new();
        private readonly uint ObjectId;
        private readonly bool IsPlayer;

        public BuffPartyMember(uint objectId, bool isPlayer) {
            ObjectId = objectId;
            IsPlayer = isPlayer;
        }

        public void Tick(HashSet<BuffTracker> trackers, CurrentPartyMember partyMember, out bool highlight, out string partyText) {
            highlight = false;
            partyText = "";

            if (PartyMemberCurrentJob != partyMember.Job) {
                PartyMemberCurrentJob = partyMember.Job;
                SetupTrackers();
            }

            foreach (var tracker in Trackers) {
                tracker.Tick(partyMember.BuffDict);
                if (tracker.Enabled) trackers.Add(tracker);
                if (tracker.Highlight) {
                    highlight = true;
                }
                if (tracker.Active && tracker.ShowPartyText) {
                    partyText = tracker.Text;
                }
            }
        }

        public void ProcessAction(Item action, uint objectId) {
            if (ObjectId != objectId) return;
            foreach (var tracker in Trackers) tracker.ProcessAction(action);
        }

        public void SetupTrackers() {
            Trackers.Clear();

            var trackerProps = JobBars.BuffManager.GetBuffConfigs(PartyMemberCurrentJob, IsPlayer);
            foreach (var prop in trackerProps) {
                if (!prop.Enabled) continue;
                Trackers.Add(new BuffTracker(prop));
            }
        }

        public void Reset() {
            foreach (var item in Trackers) item.Reset();
        }
    }
}
