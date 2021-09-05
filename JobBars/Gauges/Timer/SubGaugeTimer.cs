using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Gauges {
    public struct SubGaugeTimerProps {
        public float MaxDuration;
        public bool NoRefresh;
        public bool HideLowWarning;
        public Item[] Triggers;
        public ElementColor Color;
        public bool Invert;
#nullable enable
        public string? SubName;
        public float? DefaultDuration;
#nullable disable
    }

    public class SubGaugeTimer : SubGauge<GaugeTimer> {
        private readonly string SubName;
        private readonly float MaxDuration;
        private readonly float DefaultDuration;
        private readonly bool NoRefresh;
        private readonly Item[] Triggers;
        private readonly bool HideLowWarning;
        private ElementColor Color;
        private bool Invert;

        private float TimeLeft;
        private GaugeState State = GaugeState.Inactive;
        private Item LastActiveTrigger;
        private DateTime LastActiveTime;

        private bool InDanger = false;
        private static float LOW_TIME_WARNING => JobBars.Config.GaugeLowTimerWarning;

        public SubGaugeTimer(string name, GaugeTimer gauge, SubGaugeTimerProps props) : base(name, gauge) {
            SubName = props.SubName;
            MaxDuration = props.MaxDuration;
            DefaultDuration = props.DefaultDuration == null ? props.MaxDuration : props.DefaultDuration.Value;
            NoRefresh = props.NoRefresh;
            Triggers = props.Triggers;
            HideLowWarning = props.HideLowWarning;
            Color = JobBars.Config.GaugeColor.Get(Name, props.Color);
            Invert = JobBars.Config.GaugeInvert.Get(Name, props.Invert);
        }

        public override void Reset() {
            TimeLeft = 0;
            InDanger = false;
            State = GaugeState.Inactive;
        }

        public override void ApplySubGauge() {
            UI.SetColor(Color);
            if (UI is UIBar gauge) {
                gauge.ClearSegments();
                gauge.SetTextColor(InDanger ? UIColor.Red : UIColor.NoColor);
            }
            SetValue(TimeLeft);
        }

        public override void Tick() {
            var timeLeft = UIHelper.TimeLeft(DefaultDuration, DateTime.Now, UIHelper.PlayerStatus, LastActiveTrigger, LastActiveTime);
            if (timeLeft > 0 && State == GaugeState.Inactive) { // switching targets with DoTs on them, need to restart the icon, etc.
                State = GaugeState.Active;
            }

            if (State == GaugeState.Active) {
                if (timeLeft <= 0) {
                    timeLeft = 0; // prevent "-1" or something
                    State = GaugeState.Inactive;
                }

                bool inDanger = timeLeft < LOW_TIME_WARNING && LOW_TIME_WARNING > 0 && !HideLowWarning && timeLeft > 0;
                bool beforeOk = TimeLeft >= LOW_TIME_WARNING;
                if (inDanger && beforeOk) {
                    if (JobBars.Config.GaugeSoundEffect > 0) {
                        UIHelper.PlaySoundEffect(JobBars.Config.GaugeSoundEffect + 36, 0, 0);
                    }
                }
                if (UI is UIBar gauge) {
                    gauge.SetTextColor(inDanger ? UIColor.Red : UIColor.NoColor);
                }

                var barTimeLeft = (Invert ?
                    (timeLeft == 0 ? 0 : MaxDuration - timeLeft)
                    : timeLeft
                );
                SetValue(barTimeLeft, timeLeft);
                InDanger = inDanger;
                TimeLeft = timeLeft;
            }
        }

        public override void ProcessAction(Item action) {
            if (Triggers.Contains(action) && (!(State == GaugeState.Active) || !NoRefresh)) { // START
                LastActiveTrigger = action;
                LastActiveTime = DateTime.Now;
                State = GaugeState.Active;
                TimeLeft = DefaultDuration;
                InDanger = false;

                if (ParentGauge.ActiveSubGauge != this) {
                    ParentGauge.ActiveSubGauge = this;
                    ApplySubGauge();
                }
            }
        }

        private void SetValue(float value) => SetValue(value, value);
        private void SetValue(float value, float textValue) {
            if (ParentGauge.ActiveSubGauge != this) return;
            if (UI is UIBar gauge) {
                gauge.SetPercent((float)value / MaxDuration);
                gauge.SetText(((int)Math.Round(textValue)).ToString());
            }
        }

        public void Draw(string _ID, JobIds _) {
            var suffix = string.IsNullOrEmpty(SubName) ? "" : $" ({SubName})";

            if (JobBars.Config.GaugeColor.Draw($"Color{suffix}{_ID}", Name, Color, out var newColor)) {
                Color = newColor;
                ParentGauge.ApplyUIConfig();
            }

            if (JobBars.Config.GaugeInvert.Draw($"Invert{suffix}{_ID}", Name, Invert, out var newInvert)) {
                Invert = newInvert;
            }
        }
    }
}
