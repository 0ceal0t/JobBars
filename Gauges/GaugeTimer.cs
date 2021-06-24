﻿using Dalamud.Plugin;
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
    public class GaugeTimer : Gauge {
        public bool IconEnabled = true;
        private UIIconManager Icon;
        public bool ReplaceIcon = false;
        private ActionIds[] ReplaceIconAction;

        private float MaxDuration;
        private float DefaultDuration;
        private Dictionary<Item, float> DurationDict = null;

        private bool ShowLowWarning = true;
        private float LastTimeLeft;
        private static float LowTimerWarning = 4.0f;

        public GaugeTimer(string name, float duration) : base(name) {
            MaxDuration = DefaultDuration = duration;
            DefaultVisual = Visual = new GaugeVisual
            {
                Type = GaugeVisualType.Bar,
                Color = LightBlue
            };
        }

        public override void SetupVisual(bool resetValue = true) {
            UI?.SetColor(Visual.Color);
            if (resetValue) {
                SetValue(0);
                if (UI is UIGauge gauge) {
                    gauge.SetTextColor(NoColor);
                }
            }
        }

        private void StartIcon() {
            if (!IconEnabled || !ReplaceIcon || !Configuration.Config.GaugeIconReplacement) return;
            foreach (var icon in ReplaceIconAction) {
                Icon.ActionIdToState[(uint)icon] = IconState.StartRunning;
            }
        }

        private void SetIcon(double current, float max) {
            if (!IconEnabled || !ReplaceIcon || !Configuration.Config.GaugeIconReplacement) return;
            foreach (var icon in ReplaceIconAction) {
                Icon.ActionIdToStatus[(uint)icon] = new IconProgress
                {
                    Current = current,
                    Max = max
                };
            }
        }

        private void ResetIcon() {
            if (!IconEnabled || !ReplaceIcon || !Configuration.Config.GaugeIconReplacement) return;
            foreach (var icon in ReplaceIconAction) {
                Icon.ActionIdToStatus[(uint)icon] = new IconProgress
                {
                    Current = 0,
                    Max = 1
                };
                Icon.ActionIdToState[(uint)icon] = IconState.DoneRunning;
            }
        }

        public override void Tick(DateTime time, Dictionary<Item, float> buffDict) {
            var timeLeft = TimeLeft(DefaultDuration, time, buffDict);
            if(timeLeft > 0 && State == GaugeState.Inactive) { // switching targets with DoTs on them, need to restart the icon, etc.
                State = GaugeState.Active;
                StartIcon();
            }

            if (State == GaugeState.Active) {
                if (timeLeft <= 0) {
                    timeLeft = 0; // prevent "-1" or something
                    State = GaugeState.Inactive;
                    ResetIcon();
                }

                bool inDanger = LastTimeLeft >= LowTimerWarning && timeLeft < LowTimerWarning && timeLeft != 0 && ShowLowWarning;
                if (inDanger && Configuration.Config.SeNumber > 0) {
                    UiHelper._playSe(Configuration.Config.SeNumber + 36, 0, 0);
                }

                if (UI is UIGauge gauge) {
                    if (inDanger) {
                        gauge.SetTextColor(Red);
                    }
                    else if (LastTimeLeft < LowTimerWarning && timeLeft >= LowTimerWarning) { // duration got refreshed
                        gauge.SetTextColor(NoColor);
                    }
                }
                SetValue(timeLeft);
                SetIcon(timeLeft, MaxDuration);
                LastTimeLeft = timeLeft;
            }
        }

        public override void ProcessAction(Item action) {
            if (Triggers.Contains(action) && (!(State == GaugeState.Active) || AllowRefresh)) { // START
                if(DurationDict != null && DurationDict.TryGetValue(action, out var duration)) {
                    MaxDuration = DefaultDuration = duration;
                }
                SetActive(action);
                StartIcon();
            }
        }

        private void SetValue(float value) {
            if (UI is UIGauge gauge) {
                gauge.SetText(((int)value).ToString());
                gauge.SetPercent((float)value / MaxDuration);
            }
        }

        public override bool DoProcessInput() {
            return Enabled || IconEnabled;
        }

        public override int GetHeight() {
            return UI == null ? 0 : UI.GetHeight(0);
        }

        public override int GetWidth() {
            return UI == null ? 0 : UI.GetWidth(0);
        }

        // ===== BUILDER FUNCS =====
        public GaugeTimer WithTriggers(Item[] triggers) {
            Triggers = triggers;
            return this;
        }

        public GaugeTimer WithStartHidden() {
            StartHidden = true;
            return this;
        }

        public GaugeTimer NoRefresh() {
            AllowRefresh = false;
            return this;
        }

        public GaugeTimer WithReplaceIcon(ActionIds[] action, UIIconManager icon) {
            Icon = icon;
            ReplaceIcon = true;
            ReplaceIconAction = action;
            return this;
        }

        public GaugeTimer WithDefaultDuration(float duration) {
            DefaultDuration = duration;
            return this;
        }

        public GaugeTimer WithDurationDict(Dictionary<Item, float> dict) {
            DurationDict = dict;
            if(dict.Count != Triggers.Length) {
                PluginLog.LogError("WARNING: duration dictionary and triggers do not match!");
            }
            return this;
        }

        public GaugeTimer WithNoLowWarning() {
            ShowLowWarning = false;
            return this;
        }

        public GaugeTimer WithVisual(GaugeVisual visual) {
            DefaultVisual = Visual = visual;
            GetVisualConfig();
            return this;
        }
    }
}
