using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Gauges {
    public struct SubGaugeTimerProps {
        public float MaxDuration;
        public bool HideLowWarning;
        public Item[] Triggers;
        public ElementColor Color;
        public bool NoRefresh;
#nullable enable
        public string? SubName;
        public float? DefaultDuration;
        public ActionIds[]? Icons;
#nullable disable
    }

    public class SubGaugeTimer : SubGauge<GaugeTimer> {
        private SubGaugeTimerProps Props;
        private bool IconEnabled;

        private float TimeLeft;
        private GaugeState State = GaugeState.Inactive;
        private bool InDanger = false;
        private static float LOW_TIME_WARNING => JobBars.Config.GaugeLowTimerWarning;

        private Item LastActiveTrigger;
        private DateTime LastActiveTime;
        private readonly List<uint> Icons;

        public SubGaugeTimer(string name, GaugeTimer gauge, SubGaugeTimerProps props) : base(name, gauge) {
            Props = props;
            Props.Color = JobBars.Config.GaugeColor.Get(Name, Props.Color);
            Props.DefaultDuration = Props.DefaultDuration == null ? Props.MaxDuration : Props.DefaultDuration.Value;
            Icons = Props.Icons == null ? null : new List<ActionIds>(Props.Icons).Select(x => (uint)x).ToList();
            IconEnabled = JobBars.Config.GaugeIconEnabled.Get(Name);
        }

        public void Reset() {
            TimeLeft = 0;
            InDanger = false;
            State = GaugeState.Inactive;
            if(!NoIcon()) JobBars.Icon.Setup(Icons);
        }

        public override void UseSubGauge() {
            UI.SetColor(Props.Color);
            if (UI is UIBar gauge) {
                gauge.SetTextColor(InDanger ? UIColor.Red : UIColor.NoColor);
            }
            SetValue(TimeLeft);
        }

        private void SetIcon(float current, float max) {
            if (NoIcon()) return;
            JobBars.Icon.SetIconProgress(Icons, current, max);
        }

        private void ResetIcon() {
            if (NoIcon()) return;
            JobBars.Icon.SetIconDone(Icons);
        }

        public bool NoIcon() {
            return !IconEnabled || (Icons == null) || !JobBars.Config.GaugeIconReplacement;
        }

        public override void Tick(Dictionary<Item, BuffElem> buffDict) {
            var timeLeft = UIHelper.TimeLeft(Props.DefaultDuration.Value, DateTime.Now, buffDict, LastActiveTrigger, LastActiveTime);
            if (timeLeft > 0 && State == GaugeState.Inactive) { // switching targets with DoTs on them, need to restart the icon, etc.
                State = GaugeState.Active;
            }

            if (State == GaugeState.Active) {
                if (timeLeft <= 0) {
                    timeLeft = 0; // prevent "-1" or something
                    State = GaugeState.Inactive;
                    ResetIcon();
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

                SetValue(timeLeft);
                SetIcon(timeLeft, Props.MaxDuration);
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
                    UseSubGauge();
                }
            }
        }

        private void SetValue(float value) {
            if (UI is UIBar gauge) {
                gauge.SetText(((int)Math.Round(value)).ToString());
                gauge.SetPercent((float)value / Props.MaxDuration);
            }
        }

        public void DrawSubGauge(string _ID, JobIds job) {
            var colorTitle = "Color" + (string.IsNullOrEmpty(Props.SubName) ? "" : $" ({Props.SubName})");

            if (JobBars.Config.GaugeColor.Draw($"{colorTitle}{_ID}", Name, Props.Color, out var newColor)) {
                Props.Color = newColor;
                ParentGauge.RefreshUI();
            }

            if (Props.Icons != null) {
                var iconTitle = "Icon Replacement" + (string.IsNullOrEmpty(Props.SubName) ? "" : $" ({Props.SubName})");

                if (JobBars.Config.GaugeIconEnabled.Draw($"{iconTitle}{_ID}", Name, out var newIconEnabled)) {
                    IconEnabled = newIconEnabled;

                    if (JobBars.GaugeManager.CurrentJob == job && JobBars.Config.GaugeIconReplacement) {
                        if (IconEnabled) JobBars.Icon.Setup(Icons);
                        else JobBars.Icon.Remove(Icons);
                    }
                    ParentGauge.RefreshIconEnabled();
                }
            }
        }
    }
}
