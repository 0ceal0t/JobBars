using JobBars.Data;
using JobBars.UI;
using System.Collections.Generic;
using System.Linq;
using static JobBars.Cooldowns.CooldownTracker;

namespace JobBars.Cooldowns {
    public unsafe class CooldownPartyMember {
        private JobIds PartyMemberCurrentJob = JobIds.OTHER;
        private readonly List<CooldownTracker> Trackers = new();
        private readonly uint ObjectId;

        public CooldownPartyMember(uint objectId) {
            ObjectId = objectId;
        }

        public void Tick(UICooldown ui, CurrentPartyMember partyMember, float percent) {
            if (PartyMemberCurrentJob != partyMember.Job) {
                PartyMemberCurrentJob = partyMember.Job;
                SetupTrackers();
            }

            var trackerIdx = 0;
            foreach (var tracker in Trackers) {
                tracker.Tick(partyMember.BuffDict);

                if (trackerIdx >= (UICooldown.MAX_ITEMS - 1)) break;
                // skip if disabled
                if (!JobBars.Config.CooldownsStateShowDefault && tracker.CurrentState == TrackerState.None || 
                    !JobBars.Config.CooldownsStateShowRunning && tracker.CurrentState == TrackerState.Running || 
                    !JobBars.Config.CooldownsStateShowOnCD && tracker.CurrentState == TrackerState.OnCD || 
                    !JobBars.Config.CooldownsStateShowOffCD && tracker.CurrentState == TrackerState.OffCD
                ) {
                    trackerIdx++;
                    continue;
                }

                var uiIdx = JobBars.Config.CooldownsLeftAligned ? UICooldown.MAX_ITEMS - 1 - trackerIdx : trackerIdx;
                tracker.TickUI(ui.Items[uiIdx], percent);
                trackerIdx++;
            }
            for (int i = trackerIdx; i < UICooldown.MAX_ITEMS; i++) {
                var uiIdx = JobBars.Config.CooldownsLeftAligned ? UICooldown.MAX_ITEMS - 1 - i : i;
                ui.Items[uiIdx].SetVisible(false); // hide unused
            }
        }

        public void ProcessAction(Item action, uint objectId) {
            if (ObjectId != objectId) return;
            foreach (var tracker in Trackers) tracker.ProcessAction(action);
        }

        private void SetupTrackers() {
            Trackers.Clear();

            var trackerProps = JobBars.CooldownManager.GetCooldownConfigs(PartyMemberCurrentJob);
            var count = 0;
            foreach (var prop in trackerProps.OrderBy(x => x.Order)) {
                if (!prop.Enabled) continue;
                count++;
                if (count > UICooldown.MAX_ITEMS) continue;
                Trackers.Add(new CooldownTracker(prop));
            }
        }

        public void Reset() {
            foreach (var item in Trackers) item.Reset();
        }
    }
}