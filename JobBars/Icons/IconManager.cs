using JobBars.Data;
using System;
using System.Linq;

namespace JobBars.Icons {
    public unsafe partial class IconManager : PerJobManager<IconReplacer[]> {
        public JobIds CurrentJob = JobIds.OTHER;
        private IconReplacer[] CurrentIcons => JobToValue.TryGetValue(CurrentJob, out var gauges) ? gauges : JobToValue[JobIds.OTHER];

        public IconManager() : base("##JobBars_Icons") {
            Init();
        }

        public void SetJob(JobIds job) {
            JobBars.IconBuilder.Reset();
            CurrentJob = job;

            if (!JobBars.Config.IconsEnabled) return;
            foreach (var icon in CurrentIcons) icon.Setup();
        }

        public void Reset() => SetJob(CurrentJob);

        public void ResetJob(JobIds job) {
            if (job == CurrentJob) Reset();
        }

        public void Tick() {
            if (!JobBars.Config.IconsEnabled) return;
            foreach(var icon in CurrentIcons.Where(i => i.Enabled)) {
                icon.Tick();
            }
            JobBars.IconBuilder.Tick();
        }
    }
}
