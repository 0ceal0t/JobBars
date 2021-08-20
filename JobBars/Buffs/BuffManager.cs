using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Buffs {
    public unsafe partial class BuffManager {
        public static BuffManager Manager { get; private set; }
        public static void Dispose() { Manager = null; }

        public List<ActionIds> Icons => AllBuffs.Select(x => x.Icon).ToList();

        private Dictionary<JobIds, Buff[]> JobToBuffs;
        private readonly List<Buff> AllBuffs;

        public BuffManager() {
            Manager = this;

            Init();
            AllBuffs = new List<Buff>();
            foreach (var jobEntry in JobToBuffs) {
                foreach (var buff in jobEntry.Value) AllBuffs.Add(buff);
            }
        }

        public void SetupUI() {
            foreach(var buff in AllBuffs) {
                buff.LoadUI(UIBuilder.Builder.IconToBuff[buff.Icon]);
            }
        }

        public void Reset() {
            AllBuffs.ForEach(buff => buff.Reset());
            UIBuilder.Builder.HideAllBuffs();
            UpdatePositionScale();
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
                if (inCombat) UIBuilder.Builder.ShowBuffs();
                else UIBuilder.Builder.HideBuffs();
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
            UIBuilder.Builder.SetBuffPosition(Configuration.Config.BuffPosition);
            UIBuilder.Builder.SetBuffScale(Configuration.Config.BuffScale);
        }
    }
}
