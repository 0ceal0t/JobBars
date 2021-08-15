using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using static JobBars.UI.UIColor;

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

    public class SubGaugeTimer : SubGauge {
        private SubGaugeTimerProps Props;
        private readonly GaugeTimer ParentGauge;
        private UIElement UI => ParentGauge.UI;

        private float TimeLeft;
        private GaugeState State = GaugeState.Inactive;
        private bool InDanger = false;
        private static float LOW_TIME_WARNING => Configuration.Config.GaugeLowTimerWarning;

        private Item LastActiveTrigger;
        private DateTime LastActiveTime;
        private readonly List<uint> Icons;

        public SubGaugeTimer(string name, GaugeTimer gauge, SubGaugeTimerProps props) : base(name) {
            ParentGauge = gauge;
            Props = props;
            Props.Color = Config.GetColor(Props.Color);
            Props.DefaultDuration = Props.DefaultDuration == null ? Props.MaxDuration : Props.DefaultDuration.Value;
            Icons = Props.Icons == null ? null : new List<ActionIds>(Props.Icons).Select(x => (uint)x).ToList();
        }

        public void Reset() {
            TimeLeft = 0;
            InDanger = false;
            State = GaugeState.Inactive;
            if(!NoIcon()) UIIconManager.Manager.Setup(Icons);
        }

        public void UseSubGauge() {
            UI.SetColor(Props.Color);
            if (UI is UIGauge gauge) {
                gauge.SetTextColor(InDanger ? Red : NoColor);
            }
            SetValue(TimeLeft);
        }

        private void SetIcon(float current, float max) {
            if (NoIcon()) return;
            UIIconManager.Manager.SetIconProgress(Icons, current, max);
        }

        private void ResetIcon() {
            if (NoIcon()) return;
            UIIconManager.Manager.SetIconDone(Icons);
        }

        public bool NoIcon() {
            return !Config.IconEnabled || (Icons == null) || !Configuration.Config.GaugeIconReplacement;
        }

        public void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict) {
            var timeLeft = Gauge.TimeLeft(Props.DefaultDuration.Value, time, buffDict, LastActiveTrigger, LastActiveTime);
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
                    if (Configuration.Config.SeNumber > 0) {
                        UIHelper.PlaySe(Configuration.Config.SeNumber + 36, 0, 0);
                    }
                }
                if (UI is UIGauge gauge) {
                    gauge.SetTextColor(inDanger ? Red : NoColor);
                }

                SetValue(timeLeft);
                SetIcon(timeLeft, Props.MaxDuration);
                InDanger = inDanger;
                TimeLeft = timeLeft;
            }
        }

        public void ProcessAction(Item action) {
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
            if (UI is UIGauge gauge) {
                gauge.SetText(((int)Math.Round(value)).ToString());
                gauge.SetPercent((float)value / Props.MaxDuration);
            }
        }

        public void DrawSubGauge(string _ID, JobIds job) {
            var colorTitle = "Color" + (string.IsNullOrEmpty(Props.SubName) ? "" : $" ({Props.SubName})");

            if (Gauge.DrawColorOptions(_ID, Props.Color, out var newColorString, out var newColor, title: colorTitle)) {
                Props.Color = newColor;
                Config.Color = newColorString;
                Configuration.Config.Save();
                if (ParentGauge.ActiveSubGauge == this && GaugeManager.Manager.CurrentJob == job) {
                    UI?.SetColor(Props.Color);
                }
            }

            if (Props.Icons != null) {
                var iconTitle = "Icon Replacement" + (string.IsNullOrEmpty(Props.SubName) ? "" : $" ({Props.SubName})");
                if (ImGui.Checkbox(iconTitle + _ID + Props.SubName, ref Config.IconEnabled)) {
                    Configuration.Config.Save();
                    if(GaugeManager.Manager.CurrentJob == job && Configuration.Config.GaugeIconReplacement) {
                        if (Config.IconEnabled) UIIconManager.Manager.Setup(Icons);
                        else UIIconManager.Manager.Remove(Icons);
                    }
                    ParentGauge.RefreshIconEnabled();
                }
            }
        }
    }
}
