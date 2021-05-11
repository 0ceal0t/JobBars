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
            Visual = new GaugeVisual
            {
                Type = GaugeVisualType.Arrow,
                Color = UIColor.LightBlue
            };
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
        public ActionGaugeGCD WithVisual(GaugeVisual visual) {
            Visual = visual;
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
            Counter++;
        }

        static int RESET_DELAY = 5;
        DateTime StopTime;
        public override void Tick(DateTime time, float delta) {
            if (Active) {
                var timeleft = Duration - (time - ActiveTime).TotalSeconds;
                if(timeleft <= 0) {
                    PluginLog.Log("STOPPING");
                    Active = false;
                    StopTime = time;
                }
                // ==================
                if(_UI is UIArrow arrows) {
                    arrows.SetValue(Counter);
                }
                else if(_UI is UIGauge gauge) {
                    gauge.SetText(Counter.ToString());
                    gauge.SetPercent(((float)Counter) / MaxCounter);
                }
            }
            else if((time - StopTime).TotalSeconds > RESET_DELAY) { // RESET AFTER A DELAY
                if (_UI is UIArrow arrows) {
                    arrows.SetValue(0);
                }
                else if(_UI is UIGauge gauge) {
                    gauge.SetText("0");
                    gauge.SetPercent(0);
                }
            }
        }

        public override void Setup() {
            _UI.SetColor(Visual.Color);
            if(_UI is UIArrow arrows) {
                arrows.SetMaxValue(MaxCounter);
                arrows.SetValue(0);
            }
            else if(_UI is UIGauge gauge) {
                gauge.SetText("0");
                gauge.SetPercent(0);
            }
        }

        public override void ProcessDuration(Item buff, float duration) {
            if (Active && buff == LastActiveTrigger) {
                ActiveTime = DateTime.Now;
                Duration = duration;
            }
        }
    }
}
