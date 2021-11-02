using JobBars.Helper;
using JobBars.UI;
using JobBars.Gauges.Types.Diamond;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Gauges.Procs {
    public class GaugeProcsTracker : GaugeTracker, IGaugeDiamondInterface {
        private class Proc {
            public readonly ProcConfig Config;
            public bool Active = true;
            public float RemainingTime = 0;

            public Proc(ProcConfig config) {
                Config = config;
            }
        }

        // =============================

        private readonly GaugeProcsConfig Config;
        private readonly List<Proc> Procs;
        private GaugeState State = GaugeState.Inactive;

        public GaugeProcsTracker(GaugeProcsConfig config, int idx) {
            Config = config;
            Procs = Config.Procs.Select(p => new Proc(p)).OrderBy(proc => proc.Config.Order).ToList();
            LoadUI(Config.TypeConfig switch {
                GaugeDiamondConfig _ => new GaugeDiamond<GaugeProcsTracker>(this, idx),
                _ => null
            });
        }

        public override GaugeConfig GetConfig() => Config;

        public override bool GetActive() => State != GaugeState.Inactive;

        public override void ProcessAction(Item action) { }

        protected override void TickTracker() {
            var playSound = false;
            var procActiveCount = 0;
            foreach (var proc in Procs) {
                bool procActive;

                if (proc.Config.Trigger.Type == ItemType.Buff) {
                    procActive = UIHelper.PlayerStatus.TryGetValue(proc.Config.Trigger, out var buff);
                    proc.RemainingTime = procActive ? Math.Max(0, buff.RemainingTime) : 0;
                }
                else {
                    procActive = !UIHelper.GetRecastActive(proc.Config.Trigger.Id, out _);
                    proc.RemainingTime = 0;
                }

                if (procActive && !proc.Active) playSound = true;
                if (procActive) procActiveCount++;
                proc.Active = procActive;
            }

            if (playSound && Config.ProcSound) UIHelper.PlaySeComplete();
            State = procActiveCount == 0 ? GaugeState.Inactive : GaugeState.Active;
        }

        public int GetCurrentMaxTicks() => Procs.Count;

        public int GetTotalMaxTicks() => Procs.Count;

        public ElementColor GetTickColor(int idx) => Procs[idx].Config.Color;

        public bool GetDiamondTextVisible() => Config.ProcsShowText;

        public bool GetTickValue(int idx) => Procs[idx].Active;

        public string GetDiamondText(int idx) {
            var proc = Procs[idx];
            return proc.RemainingTime >= 0 ? ((int)Math.Round(proc.RemainingTime)).ToString() : "";
        }

        public bool GetReverseFill() => false;
    }
}
