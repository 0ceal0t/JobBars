using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Buffs {
    public unsafe partial class BuffManager {
        public static BuffManager Manager { get; private set; }

        public Dictionary<JobIds, Buff[]> JobToBuffs;
        public Buff[] CurrentBuffs => JobToBuffs.TryGetValue(CurrentJob, out var gauges) ? gauges : JobToBuffs[JobIds.OTHER];

        private readonly UIBuilder UI;
        private readonly List<Buff> AllBuffs;
        public JobIds CurrentJob = JobIds.OTHER;

        public BuffManager(UIBuilder ui) {
            Manager = this;
            UI = ui;
            if (!Configuration.Config.BuffBarEnabled) {
                UI.HideBuffs();
            }
            // ===== SETUP =========
            Init();
            AllBuffs = new List<Buff>();
            foreach (var jobEntry in JobToBuffs) {
                foreach (var buff in jobEntry.Value) {
                    buff.UI = UI.IconToBuff[buff.Icon];
                    buff.Setup();
                    AllBuffs.Add(buff);
                }
            }
        }

        public void SetJob(JobIds job) {
            CurrentJob = job;
            AllBuffs.ForEach(buff => buff.Reset());
            UI.HideAllBuffs();
            SetPositionScale();
        }

        public void SetPositionScale() {
            UI.SetBuffPosition(Configuration.Config.BuffPosition);
            UI.SetBuffScale(Configuration.Config.BuffScale);
        }

        public void Reset() {
            SetJob(CurrentJob);
        }

        public void PerformAction(Item action) {
            if (!Configuration.Config.BuffBarEnabled) return;

            foreach (var buff in AllBuffs) {
                if (!buff.Enabled) { continue; }
                buff.ProcessAction(action);
            }
        }

        public void Tick(bool inCombat) {
            if (!Configuration.Config.BuffBarEnabled) return;
            if (Configuration.Config.BuffHideOutOfCombat) {
                if (inCombat) UI.ShowBuffs();
                else UI.HideBuffs();
            }

            var idx = 0;
            var currentTime = DateTime.Now;
            foreach (var buff in AllBuffs.OrderBy(b => b.State)) {
                if (!buff.Enabled) { continue; }
                buff.Tick(currentTime);
                if (buff.Visible) {
                    buff.UI.SetPosition(idx);
                    idx++;
                }
            }
        }
    }
}
