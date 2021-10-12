using Dalamud.Game.ClientState.Objects.Types;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace JobBars.Gauges {
    public unsafe partial class GaugeManager : PerJobManagerNested<Gauge> {
        public JobIds CurrentJob = JobIds.OTHER;
        private Gauge[] CurrentGauges => JobToValue.TryGetValue(CurrentJob, out var gauges) ? gauges : JobToValue[JobIds.OTHER];

        private static readonly List<BuffIds> GaugeBuffsOnPartyMembers = new(new[] { BuffIds.Excog }); // which buffs on party members do we care about?

        public GaugeManager() : base("##JobBars_Gauges") {
            Init();
            if (!JobBars.Config.GaugesEnabled) JobBars.Builder.HideGauges();
            JobBars.Builder.HideAllGauges();
        }

        public void SetJob(JobIds job) {
            //===== CLEANUP OLD =======
            foreach (var gauge in CurrentGauges) gauge.UnloadUI();
            JobBars.Builder.HideAllGauges();

            //====== SET UP NEW =======
            CurrentJob = job;
            int idx = 0;
            foreach (var gauge in CurrentGauges) {
                gauge.LoadUI(GetUI(idx, gauge.GetVisualType()));
                idx++;
            }
            UpdatePositionScale();
        }

        private Vector2 GetPerJobPosition() => JobBars.Config.GaugePerJobPosition.Get($"{CurrentJob}");

        public void UpdatePositionScale(JobIds job) {
            if (job == CurrentJob) UpdatePositionScale();
        }

        public void UpdatePositionScale() {
            JobBars.Builder.SetGaugePosition(JobBars.Config.GaugePositionType == GaugePositionType.PerJob ? GetPerJobPosition() : JobBars.Config.GaugePositionGlobal);
            JobBars.Builder.SetGaugeScale(JobBars.Config.GaugeScale);

            int totalPosition = 0;
            foreach (var gauge in CurrentGauges.OrderBy(g => g.Order).Where(g => g.Enabled)) {
                if (JobBars.Config.GaugePositionType == GaugePositionType.Split) { // SPLIT
                    gauge.UI.SetSplitPosition(gauge.Position);
                }
                else {
                    if (JobBars.Config.GaugeHorizontal) { // HORIZONTAL
                        gauge.UI.SetPosition(new Vector2(totalPosition, gauge.UI.GetHorizontalYOffset()));
                        totalPosition += gauge.Width;
                    }
                    else { // VERTICAL
                        int xPosition = JobBars.Config.GaugeAlignRight ? 160 - gauge.Width : 0;
                        if (JobBars.Config.GaugeBottomToTop) { // BOTTOM TO TOP
                            gauge.UI.SetPosition(new Vector2(xPosition, totalPosition - gauge.Height));
                            totalPosition -= gauge.Height;
                        }
                        else {
                            gauge.UI.SetPosition(new Vector2(xPosition, totalPosition));
                            totalPosition += gauge.Height;
                        }
                    }
                }
            }
        }

        public void Reset() => SetJob(CurrentJob);

        public void ResetJob(JobIds job) {
            if (job == CurrentJob) Reset();
        }

        public void PerformAction(Item action) {
            if (!JobBars.Config.GaugesEnabled) return;

            foreach (var gauge in CurrentGauges) {
                if (!gauge.Enabled) continue;
                gauge.ProcessAction(action);
            }
        }

        public void Tick(bool inCombat) {
            if (!JobBars.Config.GaugesEnabled) return;

            if (JobBars.Config.GaugesHideOutOfCombat) {
                if (inCombat) JobBars.Builder.ShowGauges();
                else JobBars.Builder.HideGauges();
            }

            if (CurrentJob == JobIds.SCH && inCombat) { // only need this to catch excog for now
                JobBars.SearchForPartyMemberStatus((int)JobBars.ClientState.LocalPlayer.ObjectId, UIHelper.PlayerStatus, GaugeBuffsOnPartyMembers);
            }

            foreach (var gauge in CurrentGauges) {
                if (!gauge.Enabled) continue;
                gauge.Tick();
                gauge.TickActive();
            }
        }

        private static UIGauge GetUI(int idx, GaugeVisualType type) {
            return type switch {
                GaugeVisualType.Arrow => JobBars.Builder.Arrows[idx],
                GaugeVisualType.Bar => JobBars.Builder.Bars[idx],
                GaugeVisualType.Diamond => JobBars.Builder.Diamonds[idx],
                GaugeVisualType.BarDiamondCombo => new UIBarDiamondCombo(
                    JobBars.Builder.Bars[idx],
                    JobBars.Builder.Diamonds[idx]
                ), // kind of scuffed, but oh well
                _ => null,
            };
        }
    }
}
