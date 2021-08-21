using Dalamud.Plugin;
using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using static JobBars.UI.UIColor;

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

        private GaugeStacksProps Props;

        private bool GaugeFull = true;

        public GaugeStacks(string name, GaugeStacksProps props) : base(name) {
            Props = props;
            Props.Type = Configuration.Config.GaugeType.Get(Name, Props.Type);
            Props.NoSoundOnFull = Configuration.Config.GaugeNoSoundOnFull.Get(Name, Props.NoSoundOnFull);
            Props.Color = Configuration.Config.GaugeColor.Get(Name, Props.Color);
        }

        protected override void LoadUI_Impl() {
            UI.SetColor(Props.Color);
            if (UI is UIDiamond diamond) {
                diamond.SetMaxValue(Props.MaxStacks);
            }
            else if (UI is UIArrow arrow) {
                arrow.SetMaxValue(Props.MaxStacks);
            }
            else if (UI is UIBar gauge) {
                gauge.SetTextColor(NoColor);
            }

            GaugeFull = true;
            SetValue(0);
        }

        protected override void RefreshUI_Impl() {
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

        public override void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict) {
            bool anyTriggerMax = false;
            foreach (var trigger in Props.Triggers) {
                var value = buffDict.TryGetValue(trigger, out var elem) ? elem.StackCount : 0;
                if (value == Props.MaxStacks) anyTriggerMax = true;
                SetValue(value);
            }

            if(anyTriggerMax && !GaugeFull && !Props.NoSoundOnFull) UIHelper.PlaySeComplete(); // play when stacks become full
            GaugeFull = anyTriggerMax;
        }

        public override void ProcessAction(Item action) { }

        protected override int GetHeight() => UI == null ? 0 : UI.GetHeight(0);
        protected override int GetWidth() => UI == null ? 0 : UI.GetWidth(Props.MaxStacks);
        public override GaugeVisualType GetVisualType() => Props.Type;

        protected override void DrawGauge(string _ID, JobIds job) {
            if (Configuration.Config.GaugeColor.Draw($"Color{_ID}", Name, Props.Color, out var newColor)) {
                Props.Color = newColor;
                RefreshUI();
            }

            if (Configuration.Config.GaugeType.Draw($"Type{_ID}", Name, ValidGaugeVisualType, Props.Type, out var newType)) {
                Props.Type = newType;
                GaugeManager.Manager.ResetJob(job);
            }

            if (Configuration.Config.GaugeNoSoundOnFull.Draw($"Don't Play Sound When Full{_ID}", Name, out var newSound)) {
                Props.NoSoundOnFull = newSound;
            }
        }
    }
}
