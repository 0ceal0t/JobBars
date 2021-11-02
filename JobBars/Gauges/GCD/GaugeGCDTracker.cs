using System;
using System.Collections.Generic;
using System.Linq;
using JobBars.Helper;
using JobBars.UI;
using JobBars.Gauges.Types.Arrow;
using JobBars.Gauges.Types.Bar;
using JobBars.Gauges.Types.Diamond;
using Dalamud.Logging;

namespace JobBars.Gauges.GCD {
    public class GaugeGCDTracker : GaugeTracker, IGaugeBarInterface, IGaugeDiamondInterface, IGaugeArrowInterface {
        private class GaugeSubGCD {
            private readonly GaugeGCDConfig.GaugeSubGCDConfig Config;

            private int Counter;
            private GaugeState State = GaugeState.Inactive;
            private Item LastActiveTrigger;
            private DateTime LastActiveTime;

            private static readonly int RESET_DELAY = 3;
            private DateTime StopTime;

            private int Value;

            public GaugeSubGCD(GaugeGCDConfig.GaugeSubGCDConfig config) {
                Config = config;
            }

            public bool ProcessAction(Item action) {
                var ret = false;

                if (Config.Triggers.Contains(action) && !(State == GaugeState.Active)) { // START
                    LastActiveTrigger = action;
                    LastActiveTime = DateTime.Now;
                    State = GaugeState.Active;
                    Counter = 0;

                    if (Config.CompletionSound == GaugeCompleteSoundType.When_Empty || Config.CompletionSound == GaugeCompleteSoundType.When_Empty_or_Full)
                        UIHelper.PlaySeComplete();

                    ret = true;
                }

                // active and (any gcd) or (looking for specific gcd)
                if ((State == GaugeState.Active) && (Config.Increment == null ? (action.Type == ItemType.GCD) : Config.Increment.Contains(action))) {
                    if (Counter < Config.MaxCounter) {
                        Counter++;

                        if (Config.ProgressSound) UIHelper.PlaySeProgress();
                        if (Counter == Config.MaxCounter && (Config.CompletionSound == GaugeCompleteSoundType.When_Full || Config.CompletionSound == GaugeCompleteSoundType.When_Empty_or_Full))
                            UIHelper.PlaySeComplete();
                    }
                }

                return ret;
            }

            public void Tick() {
                if (State == GaugeState.Active) {
                    float timeLeft = UIHelper.TimeLeft(Config.MaxDuration, UIHelper.PlayerStatus, LastActiveTrigger, LastActiveTime);
                    if (timeLeft < 0) {
                        State = GaugeState.Finished;
                        StopTime = DateTime.Now;
                    }

                    Value = Config.Invert ? Config.MaxCounter - Counter : Counter;
                }
                else if (State == GaugeState.Finished) {
                    if ((DateTime.Now - StopTime).TotalSeconds > RESET_DELAY) { // RESET TO ZERO AFTER A DELAY
                        State = GaugeState.Inactive;
                        Counter = 0;
                        Value = 0;
                    }
                }
                else {
                    Value = 0;
                }
            }

            public ElementColor GetColor() => Config.Color;

            public ElementColor GetTickColor(int _) => Config.Color;

            public bool GetActive() => State != GaugeState.Inactive;

            public int GetMaxTicks() => Config.MaxCounter;

            public bool GetTickValue(int idx) => idx < Value;

            public float GetBarPercent() => ((float)Value) / Config.MaxCounter;

            public string GetText() => Value.ToString();

            public bool GetReverseFill() => Config.ReverseFill;
        }

        // =========================

        private readonly GaugeGCDConfig Config;
        private readonly List<GaugeSubGCD> SubGCDs;
        private GaugeSubGCD ActiveSubGCD;

        private readonly int MaxWidth;

        public GaugeGCDTracker(GaugeGCDConfig config, int idx) {
            Config = config;
            SubGCDs = Config.SubGCDs.Select(subGCD => new GaugeSubGCD(subGCD)).ToList();
            MaxWidth = Config.SubGCDs.Select(subGCD => subGCD.MaxCounter).Max();
            ActiveSubGCD = SubGCDs[0];
            LoadUI(Config.TypeConfig switch {
                GaugeBarConfig _ => new GaugeBar<GaugeGCDTracker>(this, idx),
                GaugeArrowConfig _ => new GaugeArrow<GaugeGCDTracker>(this, idx),
                GaugeDiamondConfig _ => new GaugeDiamond<GaugeGCDTracker>(this, idx),
                _ => null
            });
        }

        public override GaugeConfig GetConfig() => Config;

        public override bool GetActive() => ActiveSubGCD.GetActive();

        public override void ProcessAction(Item action) {
            var refreshVisuals = false;
            foreach (var subGCD in SubGCDs) {
                if (subGCD.ProcessAction(action) && subGCD != ActiveSubGCD) {
                    ActiveSubGCD = subGCD;
                    refreshVisuals = true;
                }
            }
            if (refreshVisuals) UI.UpdateVisual();
        }

        protected override void TickTracker() {
            foreach (var subGCD in SubGCDs) subGCD.Tick();
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

        public ElementColor GetColor() => ActiveSubGCD.GetColor();

        public bool GetBarDanger() => false;

        public string GetBarText() => ActiveSubGCD.GetText();

        public float GetBarPercent() => ActiveSubGCD.GetBarPercent();

        public int GetTotalMaxTicks() => MaxWidth;

        public int GetCurrentMaxTicks() => ActiveSubGCD.GetMaxTicks();

        public ElementColor GetTickColor(int idx) => ActiveSubGCD.GetTickColor(idx);

        public bool GetDiamondTextVisible() => false;

        public bool GetTickValue(int idx) => ActiveSubGCD.GetTickValue(idx);

        public string GetDiamondText(int idx) => "";

        public bool GetReverseFill() => ActiveSubGCD.GetReverseFill();
    }
}
