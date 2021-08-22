using JobBars.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Buffs {
    public unsafe partial class BuffManager : JobConfigurationManager<Buff[]> {
        public List<ActionIds> Icons => AllBuffs.Select(x => x.Icon).ToList();
        private readonly List<Buff> AllBuffs;

        public BuffManager() : base("##JobBars_Buffs") {
            Init();
            AllBuffs = new List<Buff>();
            foreach (var jobEntry in JobToValue) {
                foreach (var buff in jobEntry.Value) AllBuffs.Add(buff);
            }
        }

        public void SetupUI() {
            foreach(var buff in AllBuffs) {
                buff.LoadUI(JobBars.Builder.IconToBuff[buff.Icon]);
            }
        }

        public void Reset() {
            AllBuffs.ForEach(buff => buff.Reset());
            JobBars.Builder.HideAllBuffs();
            UpdatePositionScale();
        }

        public void PerformAction(Item action) {
            if (!JobBars.Config.BuffBarEnabled) return;

            foreach (var buff in AllBuffs) {
                if (!buff.Enabled) { continue; }
                buff.ProcessAction(action);
            }
        }

        public void Tick(bool inCombat) {
            if (!JobBars.Config.BuffBarEnabled) return;

            if (JobBars.Config.BuffHideOutOfCombat) {
                if (inCombat) JobBars.Builder.ShowBuffs();
                else JobBars.Builder.HideBuffs();
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

        public void UpdatePositionScale() {
            JobBars.Builder.SetBuffPosition(JobBars.Config.BuffPosition);
            JobBars.Builder.SetBuffScale(JobBars.Config.BuffScale);
        }
    }
}
