using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Gauges {
    public class ActionGaugeGCD : ActionGauge {
        public Item[] Increment;
        public float Duration;
        public int Max;

        public int Value;

        public ActionGaugeGCD(string name, float duration, int max) : base(name, ActionGaugeType.GCDs) {
            Duration = duration;
            Max = max;
            Increment = new Item[0];
        }

        // ===== BUILDER FUNCS =====
        public ActionGaugeGCD WithTriggers(Item[] triggers) {
            Triggers = triggers;
            return this;
        }
        public ActionGaugeGCD WithIncrement(Item[] increment) {
            Increment = increment;
            return this;
        }
        public ActionGaugeGCD WithHide(string gaugeName) {
            HideGauge = true;
            HideGaugeName = gaugeName;
            return this;
        }
        public ActionGaugeGCD NoRefresh() {
            AllowRefresh = false;
            return this;
        }

        // ====================
        public override void Process(Item action, bool add) {
            if (add) {
                if (Triggers.Contains(action) && (!Active || AllowRefresh)) {
                    Start();
                    return;
                }
                if (Increment.Contains(action) && Active) {
                    AddValue();
                }
            }
            else {
                if (Triggers.Contains(action) && Active) {
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

        private void AddValue() {
            Value++;
            // TODO: update gauge
        }

        private void Stop() {
            Active = false;
            Value = 0;
        }
    }
}
