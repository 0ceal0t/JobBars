using System;
using System.Collections.Generic;
using System.Linq;

using JobBars.Helper;
using JobBars.UI;
using JobBars.Gauges.Types.Bar;

namespace JobBars.Gauges.Timer {
    public class GaugeTimerTracker : GaugeTracker, IGaugeBarInterface {
        private class GaugeSubTimer {
            private readonly GaugeTimerConfig.GaugeSubTimerConfig Config;

            private GaugeState State = GaugeState.Inactive;
            private bool InDanger = false;
            private Item LastActiveTrigger;
            private DateTime LastActiveTime;

            private float TimeLeft;
            private float PercentRemaining;

            private static float LOW_TIME_WARNING => JobBars.Config.GaugeLowTimerWarning;
            private float OffsetMaxDuration => Config.Offset == Config.MaxDuration ? 1f : Config.MaxDuration - Config.Offset;

            public GaugeSubTimer(GaugeTimerConfig.GaugeSubTimerConfig config) {
                Config = config;
            }

            public bool ProcessAction(Item action) {
                if (Config.Triggers.Contains(action) && (!(State == GaugeState.Active) || !Config.NoRefresh)) { // START
                    LastActiveTrigger = action;
                    LastActiveTime = DateTime.Now;
                    State = GaugeState.Active;

                    return true;
                }
                return false;
            }

            public void Tick() {
                var currentTimeLeft = UIHelper.TimeLeft(Config.DefaultDuration, UIHelper.PlayerStatus, LastActiveTrigger, LastActiveTime) - Config.Offset;
                if (currentTimeLeft > 0 && State == GaugeState.Inactive) { // switching targets with DoTs on them, need to restart the icon, etc.
                    State = GaugeState.Active;
                }

                if (State == GaugeState.Active) {
                    if (currentTimeLeft <= 0) {
                        currentTimeLeft = 0; // prevent "-1" or something
                        State = GaugeState.Inactive;
                    }

                    bool currentDanger = currentTimeLeft < LOW_TIME_WARNING && LOW_TIME_WARNING > 0 && !Config.HideLowWarning && currentTimeLeft > 0;
                    bool beforeOk = TimeLeft >= LOW_TIME_WARNING;
                    if (currentDanger && beforeOk && Config.LowWarningSound) UIHelper.PlaySeProgress();
                    InDanger = currentDanger;

                    var barTimeLeft = Config.Invert ? (currentTimeLeft == 0 ? 0 : OffsetMaxDuration - currentTimeLeft) : currentTimeLeft;

                    PercentRemaining = barTimeLeft / OffsetMaxDuration;
                    TimeLeft = currentTimeLeft;
                }
                else {
                    TimeLeft = PercentRemaining = 0;
                }
            }

            public ElementColor GetColor() => Config.Color;

            public bool GetDanger() => InDanger;

            public string GetText() => ((int)Math.Round(TimeLeft)).ToString();

            public float GetBarPercent() => PercentRemaining;

            public bool GetActive() => State != GaugeState.Inactive;
        }

        // ======================

        private readonly GaugeTimerConfig Config;
        private readonly List<GaugeSubTimer> SubTimers;
        private GaugeSubTimer ActiveSubTimer;

        public GaugeTimerTracker(GaugeTimerConfig config, int idx) {
            Config = config;
            SubTimers = Config.SubTimers.Select(subTimer => new GaugeSubTimer(subTimer)).ToList();
            ActiveSubTimer = SubTimers[0];
            LoadUI(Config.TypeConfig switch {
                GaugeBarConfig _ => new GaugeBar<GaugeTimerTracker>(this, idx),
                _ => null
            });
        }

        public override GaugeConfig GetConfig() => Config;

        public override bool GetActive() => ActiveSubTimer.GetActive();

        public override void ProcessAction(Item action) {
            var refreshVisuals = false;
            foreach (var subTimer in SubTimers) {
                if (subTimer.ProcessAction(action) && subTimer != ActiveSubTimer) {
                    ActiveSubTimer = subTimer;
                    refreshVisuals = true;
                }
            }
            if (refreshVisuals) UI.UpdateVisual();
        }

        protected override void TickTracker() {
            foreach (var subTimer in SubTimers) subTimer.Tick();
        }

        public float[] GetBarSegments() => null;

        public bool GetBarTextSwap() => Config.TypeConfig switch {
            GaugeBarConfig barConfig => barConfig.SwapText,
            _ => false
        };

        public bool GetBarTextVisible() => Config.TypeConfig switch {
            GaugeBarConfig barConfig => barConfig.ShowText,
            _ => false
        };

        public ElementColor GetColor() => ActiveSubTimer.GetColor();

        public bool GetBarDanger() => ActiveSubTimer.GetDanger();

        public string GetBarText() => ActiveSubTimer.GetText();

        public float GetBarPercent() => ActiveSubTimer.GetBarPercent();
    }
}
