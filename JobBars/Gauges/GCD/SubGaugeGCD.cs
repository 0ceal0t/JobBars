using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Gauges {
    public struct SubGaugeGCDProps {
        public int MaxCounter;
        public float MaxDuration;
        public Item[] Triggers;
        public ElementColor Color;
        public bool Invert;
        public bool NoSoundOnFull;
#nullable enable
        public string? SubName;
        public Item[]? Increment;
        public ActionIds[]? Icons;
#nullable disable
    }

    public class SubGaugeGCD : IconSubGauge<GaugeGCD> {
        private SubGaugeGCDProps Props;

        private int Counter;
        private GaugeState State = GaugeState.Inactive;

        private static readonly int RESET_DELAY = 3;
        private DateTime StopTime;

        private Item LastActiveTrigger;
        private DateTime LastActiveTime;

        public SubGaugeGCD(string name, GaugeGCD gauge, SubGaugeGCDProps props) : base(name, gauge, props.Icons, false) {
            Props = props;
            Props.Color = JobBars.Config.GaugeColor.Get(Name, Props.Color);
            Props.Invert = JobBars.Config.GaugeInvert.Get(Name, Props.Invert);
            Props.NoSoundOnFull = JobBars.Config.GaugeNoSoundOnFull.Get(Name, Props.NoSoundOnFull);
        }

        protected override void ResetSubGauge() {
            Counter = 0;
            State = GaugeState.Inactive;
        }

        public override void ApplySubGauge() {
            UI.SetColor(Props.Color);
            if (UI is UIArrow arrows) {
                arrows.SetMaxValue(Props.MaxCounter);
            }
            else if (UI is UIDiamond diamond) {
                diamond.SetMaxValue(Props.MaxCounter);
            }
            else if (UI is UIBar gauge) {
                gauge.ClearSegments();
                gauge.SetTextColor(UIColor.NoColor);
            }
            SetValue(Counter);
            CheckInactive();
        }

        public override void Tick() {
            if (State == GaugeState.Active) {
                float timeLeft = UIHelper.TimeLeft(Props.MaxDuration, DateTime.Now, UIHelper.PlayerStatus, LastActiveTrigger, LastActiveTime);
                if (timeLeft < 0) {
                    timeLeft = 0;
                    State = GaugeState.Finished;
                    StopTime = DateTime.Now;
                }

                SetValue(Props.Invert ? Props.MaxCounter - Counter : Counter);

                if (timeLeft == 0) ResetIcon();
                else SetIcon(timeLeft);
            }
            else if (State == GaugeState.Finished) {
                if ((DateTime.Now - StopTime).TotalSeconds > RESET_DELAY) { // RESET TO ZERO AFTER A DELAY
                    State = GaugeState.Inactive;
                    Counter = 0;
                    SetValue(0);
                    CheckInactive();
                }
            }
        }

        private void CheckInactive() {
            if (JobBars.Config.GaugeHideGCDInactive) UI.SetVisible(State != GaugeState.Inactive);
        }

        public override void ProcessAction(Item action) {
            if (Props.Triggers.Contains(action) && !(State == GaugeState.Active)) { // START
                LastActiveTrigger = action;
                LastActiveTime = DateTime.Now;
                State = GaugeState.Active;
                Counter = 0;
                CheckInactive();

                if (ParentGauge.ActiveSubGauge != this) {
                    ParentGauge.ActiveSubGauge = this;
                    ApplySubGauge();
                }
            }

            if (
                (State == GaugeState.Active) &&
                ((Props.Increment == null && action.Type == ItemType.GCD) || // just take any gcd
                    (Props.Increment != null && Props.Increment.Contains(action))) // take specific gcds
            ) {
                if (Counter < Props.MaxCounter) Counter++;
                if (Counter == Props.MaxCounter && !Props.NoSoundOnFull) UIHelper.PlaySeComplete(); // play when reached max counter
            }
        }

        private void SetValue(int value) => SetValue(value, value);
        private void SetValue(int value, int textValue) {
            if (ParentGauge.ActiveSubGauge != this) return;
            if (UI is UIArrow arrows) {
                arrows.SetValue(value);
            }
            else if (UI is UIDiamond diamond) {
                diamond.SetValue(value);
            }
            else if (UI is UIBar gauge) {
                gauge.SetPercent(((float)value) / Props.MaxCounter);
                gauge.SetText(textValue.ToString());
            }
        }

        public void Draw(string _ID, JobIds job) {
            var suffix = (string.IsNullOrEmpty(Props.SubName) ? "" : $" ({Props.SubName})");

            if (JobBars.Config.GaugeColor.Draw($"Color{suffix}{_ID}", Name, Props.Color, out var newColor)) {
                Props.Color = newColor;
                ParentGauge.ApplyUIConfig();
            }

            if (JobBars.Config.GaugeInvert.Draw($"Invert Counter{suffix}{_ID}", Name, out var newInvert)) {
                Props.Invert = newInvert;
            }

            if (JobBars.Config.GaugeNoSoundOnFull.Draw($"Don't Play Sound When Full{suffix}{_ID}", Name, out var newSound)) {
                Props.NoSoundOnFull = newSound;
            }

            if (DrawIconReplacement(_ID, job, suffix)) {
                ParentGauge.RefreshIconEnabled();
            }
        }
    }
}
