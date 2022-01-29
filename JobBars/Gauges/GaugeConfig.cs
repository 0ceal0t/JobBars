using JobBars.Data;
using System;
using System.Linq;
using System.Numerics;

using JobBars.Gauges.Types;
using JobBars.Gauges.Types.Bar;
using JobBars.Gauges.Types.Arrow;
using JobBars.Gauges.Types.Diamond;
using JobBars.Gauges.Types.BarDiamondCombo;
using ImGuiNET;

namespace JobBars.Gauges {
    public abstract class GaugeConfig {
        public readonly string Name;
        public GaugeVisualType Type { get; private set; }
        public GaugeTypeConfig TypeConfig { get; private set; }

        public bool Enabled { get; protected set; }
        public int Order { get; private set; }
        public float Scale { get; private set; }
        public bool HideWhenInactive { get; private set; }
        public int SoundEffect { get; private set; }
        public int CompletionSoundEffect { get; private set; }
        public Vector2 Position => JobBars.Config.GaugeSplitPosition.Get(Name);

        public static readonly GaugeCompleteSoundType[] ValidSoundType = (GaugeCompleteSoundType[])Enum.GetValues(typeof(GaugeCompleteSoundType));

        public GaugeConfig(string name, GaugeVisualType type) {
            Name = name;
            Enabled = JobBars.Config.GaugeEnabled.Get(Name);
            Order = JobBars.Config.GaugeOrder.Get(Name);
            Scale = JobBars.Config.GaugeIndividualScale.Get(Name);
            HideWhenInactive = JobBars.Config.GaugeHideInactive.Get(Name);
            SoundEffect = JobBars.Config.GaugeSoundEffect_2.Get(Name);
            CompletionSoundEffect = JobBars.Config.GaugeCompletionSoundEffect_2.Get(Name);
            SetType(JobBars.Config.GaugeType.Get(Name, type));
        }

        public abstract GaugeTracker GetTracker(int idx);

        private void SetType(GaugeVisualType type) {
            var validTypes = GetValidGaugeTypes();
            Type = validTypes.Contains(type) ? type : validTypes[0];
            TypeConfig = Type switch {
                GaugeVisualType.Bar => new GaugeBarConfig(Name),
                GaugeVisualType.Diamond => new GaugeDiamondConfig(Name),
                GaugeVisualType.Arrow => new GaugeArrowConfig(Name),
                GaugeVisualType.BarDiamondCombo => new GaugeBarDiamondComboConfig(Name),
                _ => null
            };
        }

        public void Draw(string id, out bool newPos, out bool newVisual, out bool reset) {
            newPos = newVisual = reset = false;

            if (JobBars.Config.GaugeEnabled.Draw($"Enabled{id}", Name, Enabled, out var newEnabled)) {
                Enabled = newEnabled;
                newVisual = true;
                newPos = true;
            }

            if (JobBars.Config.GaugeHideInactive.Draw($"Hide When Inactive{id}", Name, HideWhenInactive, out var newHideWhenInactive)) {
                HideWhenInactive = newHideWhenInactive;
            }

            if (JobBars.Config.GaugeIndividualScale.Draw($"Scale{id}", Name, out var newScale)) {
                Scale = Math.Max(0.1f, newScale);
                newVisual = true;
                newPos = true;
            }

            if (JobBars.Config.GaugePositionType == GaugePositionType.Split) {
                if (JobBars.Config.GaugeSplitPosition.Draw($"Split Position{id}", Name, out var newPosition)) {
                    newPos = true;
                    SetSplitPosition(newPosition);
                }
            }
            else {
                if (JobBars.Config.GaugeOrder.Draw($"Order{id}", Name, Order, out var newOrder)) {
                    Order = newOrder;
                    newPos = true;
                }
            }

            var validTypes = GetValidGaugeTypes();
            if (validTypes.Length > 1) {
                if (JobBars.Config.GaugeType.Draw($"Type{id}", Name, validTypes, Type, out var newType)) {
                    SetType(newType);
                    reset = true;
                }
            }

            TypeConfig.Draw(id, ref newPos, ref newVisual, ref reset);

            DrawConfig(id, ref newPos, ref newVisual, ref reset);
        }

        protected void DrawSoundEffect(string label = "Progress Sound Effect") {
            if (ImGui.Button("Test##SoundEffect")) Helper.UIHelper.PlaySoundEffect(SoundEffect);
            ImGui.SameLine();

            ImGui.SetNextItemWidth(200f);
            if (JobBars.Config.GaugeSoundEffect_2.Draw($"{label} (0 = off)", Name, SoundEffect, out var newSoundEffect)) {
                SoundEffect = newSoundEffect;
            }
            ImGui.SameLine();
            HelpMarker("For macro sound effects, add 36. For example, <se.6> would be 6+36=42");
        }

        public void PlaySoundEffect() => Helper.UIHelper.PlaySoundEffect(SoundEffect);

        protected void DrawCompletionSoundEffect() {
            if (ImGui.Button("Test##CompletionSoundEffect")) Helper.UIHelper.PlaySoundEffect(CompletionSoundEffect);
            ImGui.SameLine();

            ImGui.SetNextItemWidth(200f);
            if (JobBars.Config.GaugeCompletionSoundEffect_2.Draw($"Completion Sound Effect (0 = off)", Name, CompletionSoundEffect, out var newSoundEffect)) {
                CompletionSoundEffect = newSoundEffect;
            }
            ImGui.SameLine();
            HelpMarker("For macro sound effects, add 36. For example, <se.6> would be 6+36=42");
        }

        public void PlayCompletionSoundEffect() => Helper.UIHelper.PlaySoundEffect(CompletionSoundEffect);

        public static void HelpMarker(string text) {
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 5);
            ImGui.TextDisabled("(?)");
            if (ImGui.IsItemHovered()) {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
                ImGui.TextUnformatted(text);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }

        public void DrawPositionBox() {
            if (JobBars.DrawPositionView(Name + "##GaugePosition", Position, out var pos)) {
                JobBars.Config.GaugeSplitPosition.Set(Name, pos);
                SetSplitPosition(pos);
                JobBars.GaugeManager.UpdatePositionScale();
            }
        }

        protected abstract GaugeVisualType[] GetValidGaugeTypes();

        protected abstract void DrawConfig(string id, ref bool newPos, ref bool newVisual, ref bool reset);

        private void SetSplitPosition(Vector2 pos) {
            JobBars.SetWindowPosition(Name + "##GaugePosition", pos);
        }
    }
}
