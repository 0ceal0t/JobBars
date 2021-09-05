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
        public SubGaugeTimerProps Props;

        private float TimeLeft;
        private GaugeState State = GaugeState.Inactive;

        private bool InDanger = false;
        private static float LOW_TIME_WARNING => JobBars.Config.GaugeLowTimerWarning;

        private Item LastActiveTrigger;
        private DateTime LastActiveTime;

        public SubGaugeTimer(string name, GaugeTimer gauge, SubGaugeTimerProps props) : base(name, gauge) {
            Props = props;
            Props.Color = JobBars.Config.GaugeColor.Get(Name, Props.Color);
            Props.DefaultDuration = Props.DefaultDuration == null ? Props.MaxDuration : Props.DefaultDuration.Value;
            Props.Invert = JobBars.Config.GaugeInvert.Get(Name, Props.Invert);
        }

        public override void Reset() {
            TimeLeft = 0;
            InDanger = false;
            State = GaugeState.Inactive;
        }

        public override void ApplySubGauge() {
            UI.SetColor(Props.Color);
            if (UI is UIBar gauge) {
                gauge.ClearSegments();
                gauge.SetTextColor(InDanger ? UIColor.Red : UIColor.NoColor);
            }
            SetValue(TimeLeft);
        }

        public override void Tick() {
            var timeLeft = UIHelper.TimeLeft(Props.DefaultDuration.Value, DateTime.Now, UIHelper.PlayerStatus, LastActiveTrigger, LastActiveTime);
            if (timeLeft > 0 && State == GaugeState.Inactive) { // switching targets with DoTs on them, need to restart the icon, etc.
                State = GaugeState.Active;
            }

            if (State == GaugeState.Active) {
                if (timeLeft <= 0) {
                    timeLeft = 0; // prevent "-1" or something
                    State = GaugeState.Inactive;
                }

                bool inDanger = timeLeft < LOW_TIME_WARNING && LOW_TIME_WARNING > 0 && !Props.HideLowWarning && timeLeft > 0;
                bool beforeOk = TimeLeft >= LOW_TIME_WARNING;
                if (inDanger && beforeOk) {
                    if (JobBars.Config.GaugeSoundEffect > 0) {
                        UIHelper.PlaySoundEffect(JobBars.Config.GaugeSoundEffect + 36, 0, 0);
                    }
                }
                if (UI is UIBar gauge) {
                    gauge.SetTextColor(inDanger ? UIColor.Red : UIColor.NoColor);
                }

                var barTimeLeft = (Props.Invert ?
                    (timeLeft == 0 ? 0 : Props.MaxDuration - timeLeft)
                    : timeLeft
                );
                SetValue(barTimeLeft, timeLeft);
                InDanger = inDanger;
                TimeLeft = timeLeft;
            }
        }

        public override void ProcessAction(Item action) {
            if (Props.Triggers.Contains(action) && (!(State == GaugeState.Active) || !Props.NoRefresh)) { // START
                LastActiveTrigger = action;
                LastActiveTime = DateTime.Now;
                State = GaugeState.Active;
                TimeLeft = Props.DefaultDuration.Value;
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
                gauge.SetPercent((float)value / Props.MaxDuration);
                gauge.SetText(((int)Math.Round(textValue)).ToString());
            }
        }

        public void Draw(string _ID, JobIds job) {
            var suffix = (string.IsNullOrEmpty(Props.SubName) ? "" : $" ({Props.SubName})");

            if (JobBars.Config.GaugeColor.Draw($"Color{suffix}{_ID}", Name, Props.Color, out var newColor)) {
                Props.Color = newColor;
                ParentGauge.ApplyUIConfig();
            }

            if (JobBars.Config.GaugeInvert.Draw($"Invert{suffix}{_ID}", Name, Props.Invert, out var newInvert)) {
                Props.Invert = newInvert;
            }
        }
    }
}
