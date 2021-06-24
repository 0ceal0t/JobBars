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

        private int Counter;
        private int MaxCounter;

        private float Duration;
        private float MaxDuration;

        private static int RESET_DELAY = 3;
        private DateTime StopTime;

        public static GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Arrow, GaugeVisualType.Bar, GaugeVisualType.Diamond };

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

        public override void SetupVisual(bool resetValue = true) {
            UI?.SetColor(Visual.Color);
            if(resetValue) {
                if (UI is UIArrow arrows) {
                    arrows.SetMaxValue(MaxCounter);
                }
                else if(UI is UIDiamond diamond) {
                    diamond.SetMaxValue(MaxCounter);
                }
                else if(UI is UIGauge gauge) {
                    gauge.SetTextColor(UIColor.NoColor);
                }
                GCDInactive();
            }
        }

        public override void Tick(DateTime time, Dictionary<Item, float> buffDict) {
            if (State == GaugeState.Active) {
                float timeLeft = TimeLeft(Duration, time, buffDict);
                if(timeLeft < 0) {
                    State = GaugeState.Finished;
                    StopTime = time;
                }
                SetValue(Counter);
            }
            else if(State == GaugeState.Finished) {
                if ((time - StopTime).TotalSeconds > RESET_DELAY) { // RESET TO ZERO AFTER A DELAY
                    State = GaugeState.Inactive;
                    GCDInactive();
                }
            }
        }

        public override void ProcessAction(Item action) {
            if (Triggers.Contains(action) && (!(State == GaugeState.Active) || AllowRefresh)) { // START
                SetActive(action);
                Duration = MaxDuration;
                Counter = 0;
                GCDActive();
            }

            if (
                (State == GaugeState.Active) &&
                ((Increment.Length == 0 && action.Type == ItemType.GCD) || // just take any gcd
                (Increment.Length > 0 && Increment.Contains(action))) // take specific gcds
            ) {
                if (Counter < MaxCounter) {
                    Counter++;
                }
            }
        }

        private void GCDInactive() {
            SetValue(0);
            if (Configuration.Config.GaugeHideGCDInactive) {
                UI?.Hide();
            }
        }

        private void GCDActive() {
            if (Configuration.Config.GaugeHideGCDInactive) {
                UI?.Show();
            }
        }

        private void SetValue(int value) {
            if (UI is UIArrow arrows) {
                arrows.SetValue(value);
            }
            else if (UI is UIDiamond diamond) {
                diamond.SetValue(value);
            }
            else if (UI is UIGauge gauge) {
                gauge.SetText(value.ToString());
                gauge.SetPercent(((float)value) / MaxCounter);
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
            GetVisualConfig();
            return this;
        }
    }
}
