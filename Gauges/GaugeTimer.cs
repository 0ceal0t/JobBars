using Dalamud.Plugin;
using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars.Gauges {
    public class GaugeTimer : Gauge {
        public UIIconManager Icon;

        public float MaxDuration;
        public float DefaultDuration;
        public bool ReplaceIcon = false;
        public ActionIds[] ReplaceIconAction;

        public float Duration;

        public GaugeTimer(string name, float duration) : base(name) {
            MaxDuration = duration;
            DefaultDuration = MaxDuration;
            DefaultVisual = Visual = new GaugeVisual
            {
                Type = GaugeVisualType.Bar,
                Color = LightBlue
            };
        }

        // ===== BUILDER FUNCS =====
        public GaugeTimer WithTriggers(Item[] triggers) {
            Triggers = triggers;
            return this;
        }
        public GaugeTimer WithStartHidden() {
            StartHidden = true;
            return this;
        }
        public GaugeTimer NoRefresh() {
            AllowRefresh = false;
            return this;
        }
        public GaugeTimer WithReplaceIcon(ActionIds[] action, UIIconManager icon) {
            Icon = icon;
            ReplaceIcon = true;
            ReplaceIconAction = action;
            return this;
        }
        public GaugeTimer WithDefaultDuration(float duration) {
            DefaultDuration = duration;
            return this;
        }
        public GaugeTimer WithVisual(GaugeVisual visual) {
            DefaultVisual = Visual = visual;
            return this;
        }

        // ====================
        public bool GetReplaceIcon() {
            return ReplaceIcon;
        }

        // ====================
        public override void ProcessAction(Item action) {
            if (Triggers.Contains(action) && (!Active || AllowRefresh)) {
                Start(action);
            }
        }

        private void Start(Item action) {
            PluginLog.Log("STARTING");
            SetActive(action);
            Duration = DefaultDuration;
            StartIcon();
        }

        public override void Tick(DateTime time, float delta) {
            if (Active) {
                var timeleft = Duration - (time - ActiveTime).TotalSeconds;

                if (UI is UIGauge gauge) {
                    gauge.SetText(((int)timeleft).ToString());
                    gauge.SetPercent((float)timeleft / MaxDuration);
                    SetIcon(timeleft, MaxDuration);
                }
                // ==========
                if (timeleft <= 0) {
                    PluginLog.Log("STOPPING");
                    Active = false;
                    ResetIcon();
                }
            }
        }

        public override void Setup() {
            if (UI is UIGauge gauge) {
                gauge.SetColor(Visual.Color);
                gauge.SetText("0");
                gauge.SetPercent(0);
            }
        }

        public override void ProcessDuration(Item buff, float duration, bool isRefresh) { // primarily used for things like storm's eye
            if(Active && buff == LastActiveTrigger && (!isRefresh || AllowRefresh)) {
                ActiveTime = DateTime.Now;
                Duration = duration;
            }
        }

        private void StartIcon() {
            if (!ReplaceIcon) return;
            foreach (var icon in ReplaceIconAction) {
                Icon.ActionIdToState[(uint)icon] = IconState.START_RUNNING;
            }
        }

        private void ResetIcon() {
            if (!ReplaceIcon) return;
            foreach(var icon in ReplaceIconAction) {
                Icon.ActionIdToStatus[(uint)icon] = new IconProgress
                {
                    Current = 0,
                    Max = 1
                };
                Icon.ActionIdToState[(uint)icon] = IconState.DONE_RUNNING;
            }
        }

        private void SetIcon(double current, float max) {
            if (!ReplaceIcon) return;
            foreach (var icon in ReplaceIconAction) {
                Icon.ActionIdToStatus[(uint)icon] = new IconProgress
                {
                    Current = current,
                    Max = max
                };
            }
        }
    }
}
