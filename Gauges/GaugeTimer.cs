using Dalamud.Plugin;
using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars.Gauges {
    public class GaugeTimer : Gauge {
        public UIIconManager Icon;

        public float MaxDuration;
        public float DefaultDuration;
        public bool ReplaceIcon = false;
        public ActionIds[] ReplaceIconAction;

        public float Duration;

        public GaugeTimer(string name, float duration) : base(name) {
            MaxDuration = duration;
            DefaultDuration = MaxDuration;
            DefaultVisual = Visual = new GaugeVisual
            {
                Type = GaugeVisualType.Bar,
                Color = LightBlue
            };
        }
        public override void Setup() {
            SetColor();
            if (UI is UIGauge gauge) {
                gauge.SetText("0");
                gauge.SetPercent(0);
            }
        }
        private void Start(Item action) {
            PluginLog.Log("STARTING");
            SetActive(action);
            Duration = DefaultDuration;
            StartIcon();
        }
        public override void SetColor() {
            if (UI == null) return;
            if (UI is UIGauge gauge) {
                gauge.SetColor(Visual.Color);
            }
        }

        private void StartIcon() {
            if (!ReplaceIcon || !Configuration.Config.GaugeIconReplacement) return;
            foreach (var icon in ReplaceIconAction) {
                Icon.ActionIdToState[(uint)icon] = IconState.START_RUNNING;
            }
        }
        private void SetIcon(double current, float max) {
            if (!ReplaceIcon || !Configuration.Config.GaugeIconReplacement) return;
            foreach (var icon in ReplaceIconAction) {
                Icon.ActionIdToStatus[(uint)icon] = new IconProgress
                {
                    Current = current,
                    Max = max
                };
            }
        }
        private void ResetIcon() {
            if (!ReplaceIcon || !Configuration.Config.GaugeIconReplacement) return;
            foreach (var icon in ReplaceIconAction) {
                Icon.ActionIdToStatus[(uint)icon] = new IconProgress
                {
                    Current = 0,
                    Max = 1
                };
                Icon.ActionIdToState[(uint)icon] = IconState.DONE_RUNNING;
            }
        }

        // ===== UPDATE ============
        public override void Tick(DateTime time, Dictionary<Item, float> buffDict) {
            if (State == GaugeState.ACTIVE) {
                var timeleft = TimeLeft(Duration, time, buffDict);
                if (timeleft < 0) {
                    PluginLog.Log("STOPPING");
                    State = GaugeState.INACTIVE;
                    ResetIcon();
                }

                if (UI is UIGauge gauge) {
                    gauge.SetText(((int)timeleft).ToString());
                    gauge.SetPercent((float)timeleft / MaxDuration);
                    SetIcon(timeleft, MaxDuration);
                }
            }
        }
        public override void ProcessAction(Item action) {
            if (Triggers.Contains(action) && (!(State == GaugeState.ACTIVE) || AllowRefresh)) {
                Start(action);
            }
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
        public GaugeTimer WithVisual(GaugeVisual visual) {
            DefaultVisual = Visual = visual;
            if (Configuration.Config.GetColorOverride(Name, out var color)) {
                Visual.Color = color;
            }
            if (Configuration.Config.GaugeTypeOverride.TryGetValue(Name, out var type)) {
                Visual.Type = type;
            }
            return this;
        }
    }
}
