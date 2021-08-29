using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Gauges {
    public abstract class IconSubGauge<T> where T : Gauge {
        public readonly string Name;

        protected readonly T ParentGauge;
        protected UIGaugeElement UI => ParentGauge.UI;

        protected readonly List<uint> Icons;
        protected bool IconEnabled;
        protected bool DoTTypeIcon;

        public bool NoIcon => !IconEnabled || (Icons == null) || !JobBars.Config.GaugeIconReplacement;

        public IconSubGauge(string name, T parentGauge, ActionIds[] icons, bool dotTypeIcon) {
            Name = name;
            ParentGauge = parentGauge;
            Icons = icons == null ? null : new List<ActionIds>(icons).Select(x => (uint)x).ToList();
            IconEnabled = JobBars.Config.GaugeIconEnabled.Get(Name);
            DoTTypeIcon = dotTypeIcon;
        }

        public void Reset() {
            if (!NoIcon) JobBars.Icon.Setup(Icons, DoTTypeIcon);
        }
        protected abstract void ResetSubGauge();

        protected void SetIcon(float current) => SetIcon(current, current);
        protected void SetIcon(float current, float max) {
            if (NoIcon) return;
            if (DoTTypeIcon)
                JobBars.Icon.SetTimerProgress(Icons, current, max);
            else
                JobBars.Icon.SetBuffProgress(Icons, current);
        }

        protected void ResetIcon() {
            if (NoIcon) return;
            if (DoTTypeIcon)
                JobBars.Icon.SetTimerDone(Icons);
            else
                JobBars.Icon.SetBuffDone(Icons);
        }

        public abstract void ApplySubGauge();
        public abstract void Tick();
        public abstract void ProcessAction(Item action);

        protected bool DrawIconReplacement(string _ID, JobIds job, string suffix) {
            if (Icons == null) return false;
            if (JobBars.Config.GaugeIconEnabled.Draw($"Icon Replacement{suffix}{_ID}", Name, out var newIconEnabled)) {
                IconEnabled = newIconEnabled;

                if (JobBars.GaugeManager.CurrentJob == job && JobBars.Config.GaugeIconReplacement) {
                    if (IconEnabled) JobBars.Icon.Setup(Icons, DoTTypeIcon);
                    else JobBars.Icon.Remove(Icons);
                }
                return true;
            }
            return false;
        }
    }
}
