using Dalamud.Plugin;
using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public class SubGaugeTimer {
        private SubGaugeTimerProps Props;
        private readonly GaugeTimer Gauge;
        private readonly string Id;

        private bool IconEnabled;

        private readonly float DefaultDuration;
        private float TimeLeft;
        private GaugeState State = GaugeState.Inactive;
        private bool InDanger = false;
        private static float LOW_TIME_WARNING => Configuration.Config.GaugeLowTimerWarning;

        private Item LastActiveTrigger;
        private DateTime LastActiveTime;
        private UIElement UI => Gauge.UI;

        public SubGaugeTimer(string id, GaugeTimer gauge, SubGaugeTimerProps props) {
            Id = id;
            Gauge = gauge;
            Props = props;
            Props.Color = Configuration.Config.GetColorOverride(Id, out var newColor) ? newColor : Props.Color;
            DefaultDuration = Props.DefaultDuration == null ? Props.MaxDuration : Props.DefaultDuration.Value;
            IconEnabled = !Configuration.Config.GaugeIconDisabled.Contains(Id);
        }

        public void Reset() {
            TimeLeft = 0;
            InDanger = false;
            State = GaugeState.Inactive;
        }

        public void UseSubGauge() {
            UI.SetColor(Props.Color);
            if (UI is UIGauge gauge) {
                gauge.SetTextColor(InDanger ? Red : NoColor);
            }
            SetValue(TimeLeft);
        }

        private void StartIcon() {
            if (NoIcon()) return;
            foreach (var icon in Props.Icons) {
                UIIconManager.Manager.ActionIdToState[(uint)icon] = IconState.StartRunning;
            }
        }

        private void SetIcon(double current, float max) {
            if (NoIcon()) return;
            foreach (var icon in Props.Icons) {
                UIIconManager.Manager.ActionIdToStatus[(uint)icon] = new IconProgress {
                    Current = current,
                    Max = max
                };
            }
        }

        private void ResetIcon() {
            if (NoIcon()) return;
            foreach (var icon in Props.Icons) {
                UIIconManager.Manager.ActionIdToStatus[(uint)icon] = new IconProgress {
                    Current = 0,
                    Max = 1
                };
                UIIconManager.Manager.ActionIdToState[(uint)icon] = IconState.DoneRunning;
            }
        }

        public bool NoIcon() {
            return !IconEnabled || (Props.Icons == null) || !Configuration.Config.GaugeIconReplacement;
        }

        public void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict) {
            var timeLeft = Gauges.Gauge.TimeLeft(DefaultDuration, time, buffDict, LastActiveTrigger, LastActiveTime);
            if (timeLeft > 0 && State == GaugeState.Inactive) { // switching targets with DoTs on them, need to restart the icon, etc.
                State = GaugeState.Active;
                StartIcon();
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
                        UiHelper.PlaySe(Configuration.Config.SeNumber + 36, 0, 0);
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
                TimeLeft = DefaultDuration;
                InDanger = false;

                StartIcon();
                if (Gauge.ActiveSubGauge != this) {
                    Gauge.ActiveSubGauge = this;
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
            //============ COLOR ===============
            var colorTitle = "Color" + (string.IsNullOrEmpty(Props.SubName) ? "" : $" ({Props.SubName})");
            if (Gauges.Gauge.DrawColorOptions(_ID + Props.SubName, Id, Props.Color, out var newColor, title: colorTitle)) {
                Props.Color = newColor;
                if (Gauge.ActiveSubGauge == this && GaugeManager.Manager.CurrentJob == job) {
                    UI?.SetColor(Props.Color);
                }
            }

            //============ ICON =============
            if (Props.Icons != null) {
                var iconTitle = "Icon Replacement" + (string.IsNullOrEmpty(Props.SubName) ? "" : $" ({Props.SubName})");
                if (ImGui.Checkbox(iconTitle + _ID + Props.SubName, ref IconEnabled)) {
                    if (IconEnabled) {
                        Configuration.Config.GaugeIconDisabled.Remove(Id);
                    }
                    else {
                        Configuration.Config.GaugeIconDisabled.Add(Id);
                        if (GaugeManager.Manager.CurrentJob == job) {
                            UIIconManager.Manager.Reset();
                        }
                    }
                    Gauge.RefreshIconEnabled();
                    UIIconManager.Manager.Reset();
                    Configuration.Config.Save();
                }
            }
        }
    }
}
