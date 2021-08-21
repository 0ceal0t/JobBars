using Dalamud.Configuration;
using Dalamud.Plugin;
using ImGuiNET;
using JobBars.Gauges;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace JobBars.Data {
    [Serializable]
    public abstract class ValueConfig<T> {
        public Dictionary<string, T> Values = new();

        [NonSerialized]
        protected T Default;
        
        public ValueConfig(T defaultValue) {
            Default = defaultValue;
        }

        public ValueConfig() {
            Default = default;
        }

        public T Get(string name) => Get(name, Default);
        public T Get(string name, T defaultValue) => Values.TryGetValue(name, out var val) ? val : defaultValue;
        public void Set(string name, T value) {
            Values[name] = value;
            Configuration.Config.Save();
        }

        public bool Draw(string id, string name) => Draw(id, name, Default, out var _);
        public bool Draw(string id, string name, T defaultValue) => Draw(id, name, defaultValue, out var _);
        public bool Draw(string id, string name, out T value) => Draw(id, name, Default, out value);
        public abstract bool Draw(string id, string name, T defaultValue, out T value);
    }

    [Serializable]
    public class VectorValueConfig : ValueConfig<Vector2> {
        public VectorValueConfig(Vector2 defaultValue) : base(defaultValue) { }

        public override bool Draw(string id, string name, Vector2 defaultValue, out Vector2 value) {
            value = Get(name, defaultValue);
            if (ImGui.InputFloat2(id, ref value)) {
                Set(name, value);
                return true;
            }
            return false;
        }
    }

    [Serializable]
    public class BoolValueConfig : ValueConfig<bool> {
        public BoolValueConfig(bool defaultValue) : base(defaultValue) { }

        public override bool Draw(string id, string name, bool defaultValue, out bool value) {
            value = Get(name, defaultValue);
            if (ImGui.Checkbox(id, ref value)) {
                Set(name, value);
                return true;
            }

            return false;
        }
    }

    [Serializable]
    public class IntValueConfig : ValueConfig<int> {
        public IntValueConfig(int defaultValue) : base(defaultValue) { }

        public override bool Draw(string id, string name, int defaultValue, out int value) {
            value = Get(name, defaultValue);
            if (ImGui.InputInt(id, ref value)) {
                Set(name, value);
                return true;
            }

            return false;
        }
    }

    [Serializable]
    public class FloatValueConfig : ValueConfig<float> {
        public FloatValueConfig(float defaultValue) : base(defaultValue) { }

        public override bool Draw(string id, string name, float defaultValue, out float value) {
            value = Get(name, defaultValue);
            if (ImGui.InputFloat(id, ref value)) {
                Set(name, value);
                return true;
            }

            return false;
        }
    }

    [Serializable]
    public class TypeValueConfig : ValueConfig<GaugeVisualType> {
        public TypeValueConfig() : base() { }

        public override bool Draw(string id, string name, GaugeVisualType defaultValue, out GaugeVisualType value) { // whatever
            value = default;
            return false;
        }

        public bool Draw(string id, string name, GaugeVisualType[] typeOptions, GaugeVisualType defaultValue, out GaugeVisualType value) {
            value = Get(name, defaultValue);
            if (ImGui.BeginCombo(id, $"{value}")) {
                foreach (GaugeVisualType gType in typeOptions) {
                    if (ImGui.Selectable($"{gType}##Combo", gType == value)) {

                        value = gType;
                        Set(name, value);

                        ImGui.EndCombo();
                        return true;
                    }
                }
                ImGui.EndCombo();
            }
            return false;
        }
    }

    [Serializable]
    public class ColorConfig {
        public Dictionary<string, string> Color = new();

        public ElementColor Get(string name, ElementColor defaultColor) => Color.TryGetValue(name, out var val) ?
            GetColorInternal(val, defaultColor) : defaultColor;

        public void Set(string name, ElementColor color) {
            Color[name] = color.Name;
            Configuration.Config.Save();
        }

        private static ElementColor GetColorInternal(string colorName, ElementColor defaultColor) {
            if (string.IsNullOrEmpty(colorName)) return defaultColor;
            return UIColor.AllColors.TryGetValue(colorName, out var newColor) ? newColor : defaultColor;
        }

        public bool Draw(string id, string name, ElementColor defaultValue, out ElementColor value) {
            value = Get(name, defaultValue);
            if (ImGui.BeginCombo(id, value.Name)) {
                foreach (var entry in UIColor.AllColors) {
                    if (ImGui.Selectable($"{entry.Key}##Combo", value.Name == entry.Key)) {

                        value = entry.Value;
                        Set(name, value);

                        ImGui.EndCombo();
                        return true;
                    }
                }
                ImGui.EndCombo();
            }
            return false;
        }
    }

    [Serializable]
    public class Configuration : IPluginConfiguration {
        public int Version { get; set; } = 1;

        // ==== GAUGES ====

        public float GaugeScale = 1.0f;
        public bool GaugeHorizontal = false;
        public bool GaugeAlignRight = false;
        public bool GaugeBottomToTop = false;
        public bool GaugeSplit = false;
        public Vector2 GaugePosition = new(200, 200);

        public bool GaugesEnabled = true;
        public bool GaugesHideOutOfCombat = false;
        public bool GaugeIconReplacement = true;
        public bool GaugeHideGCDInactive = false;

        public VectorValueConfig GaugeSplitPosition = new VectorValueConfig(new Vector2(200, 200));
        public FloatValueConfig GaugeIndividualScale = new(1.0f);
        public BoolValueConfig GaugeEnabled = new(true);
        public BoolValueConfig GaugeIconEnabled = new(true);
        public IntValueConfig GaugeOrder = new(-1);
        public BoolValueConfig GaugeNoSoundOnFull = new(false);
        public BoolValueConfig GaugeInvert = new(false);
        public ColorConfig GaugeColor = new();
        public TypeValueConfig GaugeType = new();

        public int GaugeSoundEffect = 0;
        public float GaugeLowTimerWarning = 4.0f;

        // ===== BUFFS ======

        public Vector2 BuffPosition = new(200, 200);
        public float BuffScale = 1.0f;

        public bool BuffBarEnabled = true;
        public bool BuffHideOutOfCombat = false;
        public bool BuffIncludeParty = true;

        public BoolValueConfig BuffEnabled = new(true);

        public int BuffHorizontal = 5;
        public bool BuffRightToLeft = false;
        public bool BuffBottomToTop = false;

        // ==== COOLDOWNS ======

        public Vector2 CooldownPosition = new(-40, 40);

        public bool CooldownsEnabled = true;
        public bool CooldownsHideOutOfCombat = false;

        public BoolValueConfig CooldownEnabled = new(true);
        public IntValueConfig CooldownOrder = new(-1);

        [NonSerialized]
        private DalamudPluginInterface PluginInterface;

        public static Configuration Config { get; private set; }

        public void Initialize(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;
            Config = this;
        }

        public void Save() {
            PluginInterface.SavePluginConfig(this);
        }
    }
}
