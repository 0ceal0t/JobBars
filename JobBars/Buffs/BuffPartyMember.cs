using Dalamud.Logging;
using JobBars.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Buffs {
    public class BuffPartyMember {
        private JobIds CurrentJob = JobIds.OTHER;
        private readonly List<BuffTracker> Trackers = new();
        private readonly uint ObjectId;
        private readonly bool IsPlayer;

        public BuffPartyMember(uint objectId, bool isPlayer) {
            ObjectId = objectId;
            IsPlayer = isPlayer;
        }

        public void Tick(List<BuffTracker> trackers, BuffPartyMemberStruct partyMember) {
            if(CurrentJob != partyMember.Job) {
                CurrentJob = partyMember.Job;
                SetupTrackers();
            }

            foreach(var tracker in Trackers) {
                tracker.Tick(partyMember.BuffDict);
                if (tracker.Enabled) trackers.Add(tracker);
            }
        }

        public void ProcessAction(Item action, uint objectId) {
            if (ObjectId != objectId) return;
            foreach (var tracker in Trackers) tracker.ProcessAction(action);
        }

        public void SetupTrackers() {
            Trackers.Clear();

            var trackerProps = JobBars.BuffManager.GetBuffProps(CurrentJob, IsPlayer);
            foreach(var prop in trackerProps) {
                if (!prop.Enabled) continue;
                Trackers.Add(new BuffTracker(prop));
            }
        }

        public void Reset() {
            foreach (var item in Trackers) item.Reset();
        }
    }
}
