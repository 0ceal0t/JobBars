using Dalamud.Plugin;
using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Gauges {
    public class GaugeGCD : Gauge {
        public Item[] Increment;
        public int MaxCounter;
        public float MaxDuration;

        public int Counter;
        public float Duration;

        public GaugeGCD(string name, float duration, int max) : base(name) {
            MaxDuration = duration;
            MaxCounter = max;
            Increment = new Item[0];
            DefaultVisual = Visual = new GaugeVisual
            {
                Type = GaugeVisualType.Arrow,
                Color = UIColor.LightBlue
            };
        }

        public override void Setup() {
            SetColor();
            if (UI is UIArrow arrows) {
                arrows.SetMaxValue(MaxCounter);
                arrows.SetValue(0);
            }
            else if (UI is UIGauge gauge) {
                gauge.SetText("0");
                gauge.SetPercent(0);
            }
        }
        public override void SetColor() {
            if (UI == null) return;
            if (UI is UIArrow arrows) {
                arrows.SetColor(Visual.Color);
            }
            else if (UI is UIGauge gauge) {
                gauge.SetColor(Visual.Color);
            }
        }
        private void Start(Item action) {
            PluginLog.Log("STARTING");
            SetActive(action);
            Duration = MaxDuration;
            Counter = 0;
        }
        private void AddValue() {
            if (Counter < MaxCounter) {
                Counter++;
            }
        }

        // ========= UPDATE ========
        static int RESET_DELAY = 3;
        DateTime StopTime;
        public override void Tick(DateTime time, Dictionary<Item, float> buffDict) {
            if (State == GaugeState.ACTIVE) {
                float timeLeft = TimeLeft(Duration, time, buffDict);
                if(timeLeft < 0) {
                    PluginLog.Log("STOPPING");
                    State = GaugeState.FINISHED;
                     StopTime = time;
                }

                if(UI is UIArrow arrows) {
                    arrows.SetValue(Counter);
                }
                else if(UI is UIGauge gauge) {
                    gauge.SetText(Counter.ToString());
                    gauge.SetPercent(((float)Counter) / MaxCounter);
                }
            }
            else if(State == GaugeState.FINISHED) {
                if ((time - StopTime).TotalSeconds > RESET_DELAY) { // RESET TO ZERO AFTER A DELAY
                    State = GaugeState.INACTIVE;
                    if (UI is UIArrow arrows) {
                        arrows.SetValue(0);
                    }
                    else if (UI is UIGauge gauge) {
                        gauge.SetText("0");
                        gauge.SetPercent(0);
                    }
                }
            }
        }
        public override void ProcessAction(Item action) {
            if (Triggers.Contains(action) && (!(State == GaugeState.ACTIVE) || AllowRefresh)) {
                Start(action);
                return;
            }

            if (
                (State == GaugeState.ACTIVE) &&
                (
                    (Increment.Length == 0 && action.Type == ItemType.GCD) || // just take any gcd
                    (Increment.Length > 0 && Increment.Contains(action)) // take specific gcds
                )
            ) {
                AddValue();
            }
        }

        public override int GetHeight() {
            return UI == null ? 0 : UI.GetHeight(0);
        }
        public override int GetWidth() {
            return UI == null ? 0 : UI.GetWidth(MaxCounter);
        }

        // ===== BUILDER FUNCS =====
        public GaugeGCD WithTriggers(Item[] triggers) {
            Triggers = triggers;
            return this;
        }
        public GaugeGCD WithSpecificIncrement(Item[] increment) {
            Increment = increment;
            return this;
        }
        public GaugeGCD WithStartHidden() {
            StartHidden = true;
            return this;
        }
        public GaugeGCD WithNoRefresh() {
            AllowRefresh = false;
            return this;
        }
        public GaugeGCD WithVisual(GaugeVisual visual) {
            DefaultVisual = Visual = visual;
            if (Configuration.Config.GetColorOverride(Name, out var color)) {
                Visual.Color = color;
            }
            if (Configuration.Config.GaugeTypeOverride.TryGetValue(Name, out var type)) {
                Visual.Type = type;
            }
            return this;
        }
    }
}
