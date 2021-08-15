using Dalamud.Plugin;
using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using static JobBars.UI.UIColor;

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

    public class SubGaugeGCD : SubGauge {
        private SubGaugeGCDProps Props;
        private readonly GaugeGCD ParentGauge;
        private UIGaugeElement UI => ParentGauge.UI;

        private int Counter;
        private GaugeState State = GaugeState.Inactive;

        private static readonly int RESET_DELAY = 3;
        private DateTime StopTime;

        private Item LastActiveTrigger;
        private DateTime LastActiveTime;

        public SubGaugeGCD(string name, GaugeGCD gauge, SubGaugeGCDProps props) : base(name) {
            ParentGauge = gauge;
            Props = props;
            Props.Invert = Gauge.GetConfigValue(Config.Invert, Props.Invert).Value;
            Props.NoSoundOnFull = Gauge.GetConfigValue(Config.NoSoundOnFull, Props.NoSoundOnFull).Value;
            Props.Color = Config.GetColor(Props.Color);
        }

        public void Reset() {
            Counter = 0;
            State = GaugeState.Inactive;
        }

        public void UseSubGauge() {
            UI?.SetColor(Props.Color);
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
                if (timeLeft < 0) {
                    State = GaugeState.Finished;
                    StopTime = time;
                }
                SetValue(Props.Invert ? Props.MaxCounter - Counter : Counter);
            }
            else if (State == GaugeState.Finished) {
                if ((time - StopTime).TotalSeconds > RESET_DELAY) { // RESET TO ZERO AFTER A DELAY
                    State = GaugeState.Inactive;
                    Counter = 0;
                    SetValue(0);
                    CheckInactive();
                }
            }
        }

        public void CheckInactive() {
            if (Configuration.Config.GaugeHideGCDInactive) {
                UI?.SetVisible(State != GaugeState.Inactive);
            }
        }

        public void ProcessAction(Item action) {
            if (Props.Triggers.Contains(action) && !(State == GaugeState.Active)) { // START
                LastActiveTrigger = action;
                LastActiveTime = DateTime.Now;
                State = GaugeState.Active;
                Counter = 0;
                CheckInactive();
                if (ParentGauge.ActiveSubGauge != this) {
                    ParentGauge.ActiveSubGauge = this;
                    UseSubGauge();
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

        private void SetValue(int value) {
            if (ParentGauge.ActiveSubGauge != this) return;
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
            var suffix = (string.IsNullOrEmpty(Props.SubName) ? "" : $" ({Props.SubName})");

            if (Gauge.DrawColorOptions(_ID, Props.Color, out var newColorString, out var newColor, title: "Color" + suffix)) {
                Props.Color = newColor;
                Config.Color = newColorString;
                Configuration.Config.Save();

                if (ParentGauge.ActiveSubGauge == this && GaugeManager.Manager.CurrentJob == job) UI?.SetColor(Props.Color);
            }

            if (ImGui.Checkbox($"Invert Counter{suffix}{_ID}{Props.SubName}", ref Props.Invert)) {
                Config.Invert = Props.Invert;
                Configuration.Config.Save();
            }

            if (ImGui.Checkbox($"Don't Play Sound When Full{suffix}{_ID}{Props.SubName}", ref Props.NoSoundOnFull)) {
                Config.Invert = Props.NoSoundOnFull;
                Configuration.Config.Save();
            }
        }
    }
}
