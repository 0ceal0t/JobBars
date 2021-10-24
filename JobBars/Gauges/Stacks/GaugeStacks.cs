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
        public GaugeCompleteSoundType CompletionSound;
        public bool ProgressSound;
    }

    public class GaugeStacks : Gauge {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.Arrow, GaugeVisualType.Bar, GaugeVisualType.Diamond };

        private readonly int MaxStacks;
        private readonly Item[] Triggers;
        private GaugeVisualType Type;
        private ElementColor Color;
        private GaugeCompleteSoundType CompletionSound;
        private bool ProgressSound;

        private int PrevValue = 0; // keep track of state

        public GaugeStacks(string name, GaugeStacksProps props) : base(name) {
            MaxStacks = props.MaxStacks;
            Triggers = props.Triggers;
            Type = JobBars.Config.GaugeType.Get(Name, props.Type);
            Color = JobBars.Config.GaugeColor.Get(Name, props.Color);
            CompletionSound = JobBars.Config.GaugeCompletionSound.Get(Name, props.CompletionSound);
            ProgressSound = JobBars.Config.GaugeProgressSound.Get(Name, props.ProgressSound);
        }

        protected override void LoadUI_() {
            if (UI is UIDiamond diamond) {
                diamond.SetMaxValue(MaxStacks);
            }
            else if (UI is UIArrow arrow) {
                arrow.SetMaxValue(MaxStacks);
            }
            else if (UI is UIBar gauge) {
                gauge.ClearSegments();
                gauge.SetTextColor(UIColor.NoColor);
            }

            PrevValue = 0;
            SetValue(0);
        }

        protected override void ApplyUIVisual_() {
            if (UI is UIDiamond diamond) {
                diamond.SetTextVisible(false);
            }
            else if (UI is UIBar gauge) {
                gauge.SetTextVisible(ShowText);
                gauge.SetTextSwap(SwapText);
            }
            UI.SetColor(Color);
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
                gauge.SetPercent(((float)value) / MaxStacks);
            }
        }

        public override void Tick() {
            int currentValue = 0;
            foreach (var trigger in Triggers) {
                var value = UIHelper.PlayerStatus.TryGetValue(trigger, out var elem) ? elem.StackCount : 0;
                currentValue = value > currentValue ? value : currentValue;
            }
            SetValue(currentValue);

            if (currentValue != PrevValue) {
                if (currentValue == 0) {
                    if (CompletionSound == GaugeCompleteSoundType.When_Empty || CompletionSound == GaugeCompleteSoundType.When_Empty_or_Full)
                        UIHelper.PlaySeComplete();
                }
                else if (currentValue == MaxStacks) {
                    if (CompletionSound == GaugeCompleteSoundType.When_Full || CompletionSound == GaugeCompleteSoundType.When_Empty_or_Full)
                        UIHelper.PlaySeComplete();
                }
                else {
                    if (ProgressSound) UIHelper.PlaySeProgress();
                }
            }
            PrevValue = currentValue;
        }

        protected override bool GetActive() => PrevValue > 0;

        public override void ProcessAction(Item action) { }

        protected override int GetHeight() => UI.GetHeight(0);
        protected override int GetWidth() => UI.GetWidth(MaxStacks);
        public override GaugeVisualType GetVisualType() => Type;

        protected override void DrawGauge(string _ID, JobIds job) {
            if (JobBars.Config.GaugeType.Draw($"Type{_ID}", Name, ValidGaugeVisualType, Type, out var newType)) {
                Type = newType;
                JobBars.GaugeManager.ResetJob(job);
            }

            if (JobBars.Config.GaugeColor.Draw($"Color{_ID}", Name, Color, out var newColor)) {
                Color = newColor;
                ApplyUIVisual();
            }

            if (JobBars.Config.GaugeCompletionSound.Draw($"Completion Sound{_ID}", Name, ValidSoundType, CompletionSound, out var newCompletionSound)) {
                CompletionSound = newCompletionSound;
            }

            if (JobBars.Config.GaugeProgressSound.Draw($"Play Sound On Change{_ID}", Name, ProgressSound, out var newProgressSound)) {
                ProgressSound = newProgressSound;
            }
        }
    }
}
