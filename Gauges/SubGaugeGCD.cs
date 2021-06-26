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
    public struct SubGaugeGCDProps {
        public int MaxCounter;
        public float MaxDuration;
        public Item[] Triggers;
        public ElementColor Color;
#nullable enable
        public string? SubName;
        public Item[]? Increment;
#nullable disable
    }

    public class SubGaugeGCD {
        private SubGaugeGCDProps Props;
        private GaugeGCD Gauge;
        private string Id;

        private int Counter;
        private GaugeState State = GaugeState.Inactive;

        private static int RESET_DELAY = 3;
        private DateTime StopTime;

        private Item LastActiveTrigger;
        private DateTime LastActiveTime;
        private UIElement UI => Gauge.UI;

        public SubGaugeGCD(string id, GaugeGCD gauge, SubGaugeGCDProps props) {
            Id = id;
            Gauge = gauge;
            Props = props;
            Props.Color = Configuration.Config.GetColorOverride(Id, out var newColor) ? newColor : Props.Color;
        }

        public void Reset() {
            Counter = 0;
            State = GaugeState.Inactive;
        }

        public void UseSubGauge() {
            UI.SetColor(Props.Color);
            if (UI is UIArrow arrows) {
                arrows.SetMaxValue(Props.MaxCounter);
            }
            else if (UI is UIDiamond diamond) {
                diamond.SetMaxValue(Props.MaxCounter);
            }
            else if (UI is UIGauge gauge) {
                gauge.SetTextColor(NoColor);
            }
            SetValue(Counter);
        }

        public void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict) {
            if (State == GaugeState.Active) {
                float timeLeft = Gauge.TimeLeft(Props.MaxDuration, time, buffDict, LastActiveTrigger, LastActiveTime);
                if(timeLeft < 0) {
                    State = GaugeState.Finished;
                    StopTime = time;
                }
                SetValue(Counter);
            }
            else if(State == GaugeState.Finished) {
                if ((time - StopTime).TotalSeconds > RESET_DELAY) { // RESET TO ZERO AFTER A DELAY
                    State = GaugeState.Inactive;
                    Counter = 0;
                    SetValue(0);
                    CheckInactive();
                }
            }
        }

        public void CheckInactive() {
            if(Configuration.Config.GaugeHideGCDInactive) {
                if(State == GaugeState.Inactive) {
                    UI.Hide();
                }
                else {
                    UI.Show();
                }
            }
        }

        public void ProcessAction(Item action) {
            if (Props.Triggers.Contains(action) && !(State == GaugeState.Active)) { // START
                LastActiveTrigger = action;
                LastActiveTime = DateTime.Now;
                State = GaugeState.Active;
                Counter = 0;
                CheckInactive();
                if (Gauge.ActiveSubGauge != this) {
                    Gauge.ActiveSubGauge = this;
                    UseSubGauge();
                }
            }

            if (
                (State == GaugeState.Active) &&
                (   (Props.Increment == null && action.Type == ItemType.GCD) || // just take any gcd
                    (Props.Increment != null && Props.Increment.Contains(action))   ) // take specific gcds
            ) {
                if (Counter < Props.MaxCounter) {
                    Counter++;
                }
            }
        }

        private void SetValue(int value) {
            if (Gauge.ActiveSubGauge != this) return;
            if (UI is UIArrow arrows) {
                arrows.SetValue(value);
            }
            else if (UI is UIDiamond diamond) {
                diamond.SetValue(value);
            }
            else if (UI is UIGauge gauge) {
                gauge.SetText(value.ToString());
                gauge.SetPercent(((float)value) / Props.MaxCounter);
            }
        }

        public void DrawSubGauge(string _ID, JobIds job) {
            //============ COLOR ===================
            var colorTitle = "Color" + (string.IsNullOrEmpty(Props.SubName) ? "" : $" ({Props.SubName})");
            if(Gauge.DrawColorOptions(_ID + Props.SubName, Id, Props.Color, out var newColor, title: colorTitle)) {
                Props.Color = newColor;
                if(Gauge.ActiveSubGauge == this && GaugeManager.Manager.CurrentJob == job) {
                    UI?.SetColor(Props.Color);
                }
            }   
        }
    }
}
