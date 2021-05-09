using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Gauges {
    public class ActionGaugeTimer : ActionGauge {
        public float Duration;
        public bool ReplaceIcon = false;
        public ActionIds ReplaceIconAction;

        public float Value;

        public ActionGaugeTimer(string name, float duration) : base(name, ActionGaugeType.Timer) {
            Duration = duration;
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
        public ActionGaugeTimer WithReplaceIcon(ActionIds action) {
            ReplaceIcon = true;
            ReplaceIconAction = action;
            return this;
        }

        // ====================
        public override void Process(Item action, bool add) {
            if (add) {
                if (Triggers.Contains(action) && (!Active || AllowRefresh)) {
                    Start();
                    return;
                }
            }
            else {
                if(Triggers.Contains(action) && Active) {
                    Stop();
                }
            }
        }

        private void Start() {
            Active = true;
            // TODO: hide
            Value = 0;
            // TODO: update gauge
        }

        private void Stop() {
            Active = false;
            Value = 0;
        }
    }
}
