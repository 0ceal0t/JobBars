using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;

namespace JobBars.Gauges {
    public struct GaugeStacksProps {
        public int MaxStacks;
        public Item[] Triggers;
        public GaugeVisualType Type;
        public ElementColor Color;
        public bool NoSoundOnFull;
    }

    public class GaugeStacks : Gauge {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Arrow, GaugeVisualType.Bar, GaugeVisualType.Diamond };

        public GaugeStacksProps Props;

        private bool GaugeFull = true;

        public GaugeStacks(string name, GaugeStacksProps props) : base(name) {
            Props = props;
            Props.Type = JobBars.Config.GaugeType.Get(Name, Props.Type);
            Props.NoSoundOnFull = JobBars.Config.GaugeNoSoundOnFull.Get(Name, Props.NoSoundOnFull);
            Props.Color = JobBars.Config.GaugeColor.Get(Name, Props.Color);
        }

        protected override void LoadUI_() {
            if (UI is UIDiamond diamond) {
                diamond.SetMaxValue(Props.MaxStacks);
            }
            else if (UI is UIArrow arrow) {
                arrow.SetMaxValue(Props.MaxStacks);
            }
            else if (UI is UIBar gauge) {
                gauge.ClearSegments();
                gauge.SetTextColor(UIColor.NoColor);
            }

            GaugeFull = true;
            SetValue(0);
        }

        protected override void ApplyUIConfig_() {
            if (UI is UIDiamond diamond) {
                diamond.SetTextVisible(false);
            }
            else if (UI is UIBar gauge) {
                gauge.SetTextVisible(ShowText);
            }
            UI.SetColor(Props.Color);
        }

        private void SetValue(int value) {
            if (UI is UIArrow arrows) {
                arrows.SetValue(value);
            }
            else if (UI is UIDiamond diamond) {
                diamond.SetValue(value);
            }
            else if (UI is UIBar gauge) {
                gauge.SetText(value.ToString());
                gauge.SetPercent(((float)value) / Props.MaxStacks);
            }
        }

        public override void Tick() {
            bool anyTriggerMax = false;
            foreach (var trigger in Props.Triggers) {
                var value = UIHelper.PlayerStatus.TryGetValue(trigger, out var elem) ? elem.Param : 0;
                if (value == Props.MaxStacks) anyTriggerMax = true;
                SetValue(value);
            }

            if(anyTriggerMax && !GaugeFull && !Props.NoSoundOnFull) UIHelper.PlaySeComplete(); // play when stacks become full
            GaugeFull = anyTriggerMax;
        }

        public override void ProcessAction(Item action) { }

        protected override int GetHeight() => UI.GetHeight(0);
        protected override int GetWidth() => UI.GetWidth(Props.MaxStacks);
        public override GaugeVisualType GetVisualType() => Props.Type;

        protected override void DrawGauge(string _ID, JobIds job) {
            if (JobBars.Config.GaugeType.Draw($"Type{_ID}", Name, ValidGaugeVisualType, Props.Type, out var newType)) {
                Props.Type = newType;
                JobBars.GaugeManager.ResetJob(job);
            }

            if (JobBars.Config.GaugeColor.Draw($"Color{_ID}", Name, Props.Color, out var newColor)) {
                Props.Color = newColor;
                ApplyUIConfig();
            }

            if (JobBars.Config.GaugeNoSoundOnFull.Draw($"Don't Play Sound When Full{_ID}", Name, Props.NoSoundOnFull, out var newSound)) {
                Props.NoSoundOnFull = newSound;
            }
        }
    }
}
