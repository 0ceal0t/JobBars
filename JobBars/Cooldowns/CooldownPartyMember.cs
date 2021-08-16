using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;

namespace JobBars.Cooldowns {
    public unsafe class CooldownPartyMember {
        private JobIds CurrentJob = JobIds.OTHER;
        private UICooldown UI;
        private readonly List<CooldownTracker> Trackers = new();
        private uint ObjectId;

        public CooldownPartyMember(uint objectId) {
            ObjectId = objectId;
        }

        public void Tick(UICooldown ui, JobIds job) {
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
                Trackers[idx].Tick(UI.Items[idx]);
            }
        }

        public void ProcessAction(Item action, uint objectId) {
            if (ObjectId != objectId) return;
            foreach(var tracker in Trackers) tracker.ProcessAction(action);
        }

        public void SetupTrackers() {
            Trackers.Clear();

            var trackerProps = CooldownManager.Manager.JobToCooldowns.TryGetValue(CurrentJob, out var props) ? props :
                CooldownManager.Manager.JobToCooldowns[JobIds.OTHER];
            foreach(var prop in trackerProps) {
                // TODO: check enabled

                Trackers.Add(new CooldownTracker(prop));
            }
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