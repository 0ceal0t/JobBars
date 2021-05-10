using Dalamud.Plugin;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Gauges {
    public class ActionGaugeTimer : ActionGauge {
        public UIIconManager Icon;

        public float MaxDuration;
        public bool ReplaceIcon = false;
        public ActionIds[] ReplaceIconAction;

        public float StartValue; // this is needed because sometimes buffs don't start with their maximum value, like storm's eye

        public ActionGaugeTimer(string name, float duration) : base(name, ActionGaugeType.Timer) {
            MaxDuration = duration;
        }

        // ===== BUILDER FUNCS =====
        public ActionGaugeTimer WithTriggers(Item[] triggers) {
            Triggers = triggers;
            return this;
        }
        public ActionGaugeTimer WithHide(string gaugeName) {
            HideGauge = true;
            HideGaugeName = gaugeName;
            return this;
        }
        public ActionGaugeTimer NoRefresh() {
            AllowRefresh = false;
            return this;
        }
        public ActionGaugeTimer WithReplaceIcon(ActionIds[] action, UIIconManager icon) {
            Icon = icon;
            ReplaceIcon = true;
            ReplaceIconAction = action;
            return this;
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
            StartValue = MaxDuration;
        }

        public override void Tick(DateTime time, float delta) {
            if (Active) {
                var timeleft = StartValue - (time - ActiveTime).TotalSeconds;

                if (_UI is UIGauge) {
                    var gauge = (UIGauge)_UI;
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
            if (_UI is UIGauge) {
                var gauge = (UIGauge)_UI;
                gauge.SetText("0");
                gauge.SetPercent(0);
            }
        }

        public override void ProcessDuration(Item buff, float duration) { // primarily used for things like storm's eye
            if(Active && buff == LastActiveTrigger) {
                ActiveTime = DateTime.Now;
                StartValue = duration;
            }
        }

        private void ResetIcon() {
            if (!ReplaceIcon) return;
            foreach(var icon in ReplaceIconAction) {
                Icon.ActionIdToStatus[(uint)icon] = new IconProgress
                {
                    Current = 0,
                    Max = 1,
                    _State = IconState.Waiting
                };
            }
        }

        private void SetIcon(double current, float max) {
            if (!ReplaceIcon) return;
            foreach (var icon in ReplaceIconAction) {
                Icon.ActionIdToStatus[(uint)icon] = new IconProgress
                {
                    Current = current,
                    Max = max,
                    _State = IconState.Running
                };
            }
        }
    }
}
