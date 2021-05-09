using Dalamud.Plugin;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Gauges {
    public class ActionGaugeGCD : ActionGauge {
        public Item[] Increment;
        public float MaxDuration;
        public int MaxCounter;

        public int Counter;
        public float Duration;

        public ActionGaugeGCD(string name, float duration, int max) : base(name, ActionGaugeType.GCDs) {
            MaxDuration = duration;
            MaxCounter = max;
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
        public override void ProcessAction(Item action) {
            if (Triggers.Contains(action) && (!Active || AllowRefresh)) {
                Start(action);
                return;
            }

            if(Increment.Contains(action) && Active) {
                AddValue();
            }
        }

        private void Start(Item action) {
            PluginLog.Log("STARTING");
            SetActive(action);
            Duration = MaxDuration;
            Counter = 0;
        }

        private void AddValue() {
            PluginLog.Log("ADD VALUE");
            Counter++;
        }

        public override void Tick(DateTime time, float delta) {
            if (Active) {
                var timeleft = Duration - (time - ActiveTime).TotalSeconds;
                if(timeleft <= 0) {
                    PluginLog.Log("STOPPING");
                    Active = false;
                }
                // ==================
                if(_UI is UIArrow) {
                    var arrows = (UIArrow)_UI;
                    arrows.SetValue(Counter);
                }
                else {
                    var gauge = (UIGauge)_UI;
                    gauge.SetText(Counter.ToString());
                    gauge.SetPercent(((float)Counter) / MaxCounter);
                }
            }
        }

        public override void Setup() {
            if(_UI is UIArrow) {
                var arrows = (UIArrow)_UI;
                arrows.SetMaxValue(MaxCounter);
                arrows.SetValue(0);
            }
        }

        public override void ProcessDuration(Item buff, float duration) {
            if (Active && buff == LastActiveTrigger) {
                PluginLog.Log("d");
                ActiveTime = DateTime.Now;
                Duration = duration;
            }
        }
    }
}
