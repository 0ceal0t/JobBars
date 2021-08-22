using JobBars.Data;
using JobBars.UI;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Cooldowns {
    public unsafe class CooldownPartyMember {
        private JobIds CurrentJob = JobIds.OTHER;
        private UICooldown UI;
        private readonly List<CooldownTracker> Trackers = new();
        private readonly uint ObjectId;

        public CooldownPartyMember(uint objectId) {
            ObjectId = objectId;
        }

        public void Tick(UICooldown ui, JobIds job, float percent) {
            if(CurrentJob != job) {
                CurrentJob = job;
                UI = ui;
                SetupTrackers();
                SetupUI();
            }
            else if(UI != ui) { // same job, different UI element for some reason
                UI = ui;
                SetupUI();
            }

            for (int idx = 0; idx < Trackers.Count; idx++) {
                Trackers[idx].Tick(UI.Items[idx], percent);
            }
        }

        public void ProcessAction(Item action, uint objectId) {
            if (ObjectId != objectId) return;
            foreach(var tracker in Trackers) tracker.ProcessAction(action);
        }

        public void SetupTrackers() {
            Trackers.Clear();

            var trackerProps = JobBars.CooldownManager.GetCooldownProps(CurrentJob);
            foreach(var prop in trackerProps.OrderBy(x => x.Order)) {
                if (!prop.Enabled) continue;
                Trackers.Add(new CooldownTracker(prop));
            }
        }

        public void Reset() {
            foreach (var item in Trackers) item.Reset();
        }

        public void SetupUI() {
            UI.HideAllItems();

            var idx = 0;
            foreach(var tracker in Trackers) {
                UI.LoadIcon(idx, tracker.Icon);
                UI.SetVisibility(idx, true);
                idx++;
            }
        }
    }
}