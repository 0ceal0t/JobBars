using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
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
#nullable disable
    }

    public class SubGaugeGCD : SubGauge<GaugeGCD> {

        public SubGaugeGCDProps Props;
        private readonly string SubName;
        private readonly int MaxCounter;
        private readonly float MaxDuration;
        private readonly Item[] Triggers;
        private readonly Item[] Increment;
        private ElementColor Color;
        private bool Invert;
        private bool NoSoundOnFull;

        private int Counter;
        private GaugeState State = GaugeState.Inactive;
        private Item LastActiveTrigger;
        private DateTime LastActiveTime;

        private static readonly int RESET_DELAY = 3;
        private DateTime StopTime;

        public SubGaugeGCD(string name, GaugeGCD gauge, SubGaugeGCDProps props) : base(name, gauge) {
            MaxCounter = props.MaxCounter;
            MaxDuration = props.MaxDuration;
            Triggers = props.Triggers;
            Color = JobBars.Config.GaugeColor.Get(Name, props.Color);
            Invert = JobBars.Config.GaugeInvert.Get(Name, props.Invert);
            NoSoundOnFull = JobBars.Config.GaugeNoSoundOnFull.Get(Name, props.NoSoundOnFull);
            Increment = props.Increment;
            SubName = props.SubName;
        }

        public override void Reset() {
            Counter = 0;
            State = GaugeState.Inactive;
        }

        public override void ApplySubGauge() {
            UI.SetColor(Color);
            if (UI is UIArrow arrows) {
                arrows.SetMaxValue(MaxCounter);
            }
            else if (UI is UIDiamond diamond) {
                diamond.SetMaxValue(MaxCounter);
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
                float timeLeft = UIHelper.TimeLeft(MaxDuration, DateTime.Now, UIHelper.PlayerStatus, LastActiveTrigger, LastActiveTime);
                if (timeLeft < 0) {
                    State = GaugeState.Finished;
                    StopTime = DateTime.Now;
                }

                SetValue(Invert ? MaxCounter - Counter : Counter);
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
            if (Triggers.Contains(action) && !(State == GaugeState.Active)) { // START
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
                ((Increment == null && action.Type == ItemType.GCD) || // just take any gcd
                    (Increment != null && Increment.Contains(action))) // take specific gcds
            ) {
                if (Counter < MaxCounter) Counter++;
                if (Counter == MaxCounter && !NoSoundOnFull) UIHelper.PlaySeComplete(); // play when reached max counter
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
                gauge.SetPercent(((float)value) / MaxCounter);
                gauge.SetText(textValue.ToString());
            }
        }

        public void Draw(string _ID, JobIds _) {
            var suffix = (string.IsNullOrEmpty(SubName) ? "" : $" ({SubName})");

            if (JobBars.Config.GaugeColor.Draw($"Color{suffix}{_ID}", Name, Color, out var newColor)) {
                Color = newColor;
                ParentGauge.ApplyUIConfig();
            }

            if (JobBars.Config.GaugeInvert.Draw($"Invert Counter{suffix}{_ID}", Name, Invert, out var newInvert)) {
                Invert = newInvert;
            }

            if (JobBars.Config.GaugeNoSoundOnFull.Draw($"Don't Play Sound When Full{suffix}{_ID}", Name, NoSoundOnFull, out var newSound)) {
                NoSoundOnFull = newSound;
            }
        }
    }
}
