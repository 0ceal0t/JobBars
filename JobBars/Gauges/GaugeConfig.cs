using JobBars.Data;
using System;
using System.Linq;
using System.Numerics;

using JobBars.Gauges.Types;
using JobBars.Gauges.Types.Bar;
using JobBars.Gauges.Types.Arrow;
using JobBars.Gauges.Types.Diamond;
using JobBars.Gauges.Types.BarDiamondCombo;

namespace JobBars.Gauges {
    public abstract class GaugeConfig {
        public readonly string Name;
        public GaugeVisualType Type { get; private set; }
        public GaugeTypeConfig TypeConfig { get; private set; }

        public bool Enabled { get; private set; }
        public int Order { get; private set; }
        public float Scale { get; private set; }
        public bool HideWhenInactive { get; private set; }
        public Vector2 Position => JobBars.Config.GaugeSplitPosition.Get(Name);

        public static readonly GaugeCompleteSoundType[] ValidSoundType = (GaugeCompleteSoundType[])Enum.GetValues(typeof(GaugeCompleteSoundType));

        public GaugeConfig(string name, GaugeVisualType type) {
            Name = name;
            Enabled = JobBars.Config.GaugeEnabled.Get(Name);
            Order = JobBars.Config.GaugeOrder.Get(Name);
            Scale = JobBars.Config.GaugeIndividualScale.Get(Name);
            HideWhenInactive = JobBars.Config.GaugeHideInactive.Get(Name);
            SetType(type);
        }

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

            if (JobBars.Config.GaugeEnabled.Draw($"Enabled{id}", Name, out var newEnabled)) {
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

            TypeConfig.Draw(id, out var newPos_Type, out var newVisual_Type, out var reset_Type);

            DrawConfig(id, out var newPos_Config, out var newVisual_Config, out var reset_Config);

            newPos = newPos || newPos_Type || newPos_Config;
            newVisual = newVisual || newVisual_Type || newVisual_Config;
            reset = reset || reset_Type || reset_Config;
        }

        public void DrawPositionBox() {
            if (JobBars.DrawPositionView(Name + "##GaugePosition", Position, out var pos)) {
                JobBars.Config.GaugeSplitPosition.Set(Name, pos);
                SetSplitPosition(pos);
                JobBars.GaugeManager.UpdatePositionScale();
            }
        }

        protected abstract GaugeVisualType[] GetValidGaugeTypes();

        protected abstract void DrawConfig(string id, out bool newPos, out bool newVisual, out bool reset);

        private void SetSplitPosition(Vector2 pos) {
            JobBars.SetWindowPosition(Name + "##GaugePosition", pos);
        }
    }
}
