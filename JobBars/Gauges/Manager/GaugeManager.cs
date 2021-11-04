using JobBars.Data;
using JobBars.Helper;

using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace JobBars.Gauges.Manager {
    public partial class GaugeManager : PerJobManagerNested<GaugeConfig> {
        public JobIds CurrentJob = JobIds.OTHER;
        private GaugeConfig[] CurrentConfigs => JobToValue.TryGetValue(CurrentJob, out var configs) ? configs : JobToValue[JobIds.OTHER];
        private readonly List<GaugeTracker> CurrentGauges = new();

        private static readonly List<BuffIds> GaugeBuffsOnPartyMembers = new(new[] { BuffIds.Excog }); // which buffs on party members do we care about?

        public GaugeManager() : base("##JobBars_Gauges") {
            if (!JobBars.Config.GaugesEnabled) JobBars.Builder.HideGauges();
            JobBars.Builder.HideAllGauges();
        }

        public void SetJob(JobIds job) {
            foreach (var gauge in CurrentGauges) gauge.Cleanup();
            CurrentGauges.Clear();
            JobBars.Builder.HideAllGauges();


            CurrentJob = job;
            for (var idx = 0; idx < CurrentConfigs.Length; idx++) {
                CurrentGauges.Add(CurrentConfigs[idx].GetTracker(idx));
            }
            UpdatePositionScale();
        }

        public void PerformAction(Item action) {
            if (!JobBars.Config.GaugesEnabled) return;

            foreach (var gauge in CurrentGauges.Where(g => g.Enabled && !g.Disposed)) gauge.ProcessAction(action);
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

            foreach (var gauge in CurrentGauges.Where(g => g.Enabled && !g.Disposed)) gauge.Tick();
        }

        private Vector2 GetPerJobPosition() => JobBars.Config.GaugePerJobPosition.Get($"{CurrentJob}");

        public void UpdatePositionScale() {
            JobBars.Builder.SetGaugePosition(JobBars.Config.GaugePositionType == GaugePositionType.PerJob ? GetPerJobPosition() : JobBars.Config.GaugePositionGlobal);
            JobBars.Builder.SetGaugeScale(JobBars.Config.GaugeScale);

            var position = 0;
            foreach (var gauge in CurrentGauges.OrderBy(g => g.Order).Where(g => g.Enabled)) {
                if (JobBars.Config.GaugePositionType == GaugePositionType.Split) {
                    gauge.UpdateSplitPosition();
                }
                else {
                    var x = JobBars.Config.GaugeHorizontal ? position :
                        (JobBars.Config.GaugeAlignRight ? 160 - gauge.Width : 0);

                    var y = JobBars.Config.GaugeHorizontal ? gauge.YOffset :
                        (JobBars.Config.GaugeBottomToTop ? position - gauge.Height : position);

                    var posChange = JobBars.Config.GaugeHorizontal ? gauge.Width :
                        (JobBars.Config.GaugeBottomToTop ? -1 * gauge.Height : gauge.Height);

                    gauge.SetPosition(new Vector2(x, y));
                    position += posChange;
                }
            }
        }

        public void UpdateVisuals() {
            foreach (var gauge in CurrentGauges) gauge.UpdateVisual();
        }

        public void Reset() => SetJob(CurrentJob);
    }
}
