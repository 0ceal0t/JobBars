using Dalamud.Configuration;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using JobBars.Gauges;
using JobBars.UI;
using JobBars.Cursors;

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
            JobBars.Config.Save();
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
    public class ComboValueConfig<T> : ValueConfig<T> {
        [NonSerialized]
        private readonly bool ShowSearch;
        [NonSerialized]
        private string SearchInput = "";

        public ComboValueConfig(bool showSearch=false) : base() {
            ShowSearch = showSearch;
        }

        public override bool Draw(string id, string name, T defaultValue, out T value) { // whatever
            value = default;
            return false;
        }

        public bool Draw(string id, string name, T[] comboOptions, T defaultValue, out T value) {
            value = Get(name, defaultValue);
            if(DrawCombo(id, comboOptions, value, out value)) {
                Set(name, value);
                return true;
            }
            return false;
        }

        private bool DrawCombo(string id, T[] comboOptions, T currentValue, out T value) {
            value = currentValue;
            if (ImGui.BeginCombo(id, $"{currentValue}", ImGuiComboFlags.HeightLargest)) {
                if (ShowSearch) {
                    ImGui.SetNextItemWidth(ImGui.GetWindowContentRegionWidth() - 50);
                    ImGui.InputText("Search##Combo", ref SearchInput, 256);
                }

                if(ShowSearch) ImGui.BeginChild("Child##Combo", new Vector2(ImGui.GetWindowContentRegionWidth(), 200), true);

                var idx = 0;
                foreach (T option in comboOptions) {
                    if (ShowSearch && !string.IsNullOrEmpty(SearchInput)) {
                        var optionString = option.ToString();
                        if (!optionString.ToLower().Contains(SearchInput.ToLower())) continue;
                    }

                    if (ImGui.Selectable($"{option}##Combo{idx}", option.Equals(currentValue))) {
                        value = option;

                        if (ShowSearch) ImGui.EndChild();
                        ImGui.EndCombo();
                        return true;
                    }
                    idx++;
                }

                if (ShowSearch) ImGui.EndChild();
                ImGui.EndCombo();
            }
            return false;
        }
    }

    [Serializable]
    public class ColorConfig {
        public Dictionary<string, string> Color = new();

        public ElementColor Get(string name, ElementColor defaultColor) => Color.TryGetValue(name, out var val) ?
            UIColor.GetColor(val, defaultColor) : defaultColor;

        public void Set(string name, ElementColor color) {
            Color[name] = color.Name;
            JobBars.Config.Save();
        }

        public bool Draw(string id, string name, ElementColor defaultValue, out ElementColor value) {
            value = Get(name, defaultValue);
            if(Configuration.DrawColor(id, value, out value)) {
                Set(name, value);
                return true;
            }
            return false;
        }
    }

    [Serializable]
    public class Configuration : IPluginConfiguration {
        public int Version { get; set; } = 1;

        // ====== GAUGES ======

        public float GaugeScale = 1.0f;
        public bool GaugeHorizontal = false;
        public bool GaugeAlignRight = false;
        public bool GaugeBottomToTop = false;
        public bool GaugeSplit = false;
        public Vector2 GaugePosition = new(200, 200);

        public bool GaugesEnabled = true;
        public bool GaugesHideOutOfCombat = false;
        public bool GaugeHideGCDInactive = false;
        public bool GaugeGCDTextVisible = true;

        public VectorValueConfig GaugeSplitPosition = new(new Vector2(200, 200));
        public FloatValueConfig GaugeIndividualScale = new(1.0f);
        public BoolValueConfig GaugeEnabled = new(true);
        public IntValueConfig GaugeOrder = new(-1);
        public BoolValueConfig GaugeNoSoundOnFull = new(false);
        public BoolValueConfig GaugeInvert = new(false);
        public ColorConfig GaugeColor = new();
        public ComboValueConfig<GaugeVisualType> GaugeType = new();

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

        // ===== COOLDOWNS ======

        public Vector2 CooldownPosition = new(-40, 40);

        public bool CooldownsEnabled = true;
        public bool CooldownsHideOutOfCombat = false;

        public BoolValueConfig CooldownEnabled = new(true);
        public IntValueConfig CooldownOrder = new(-1);

        // ===== CURSOR =======

        public bool CursorsEnabled = true;
        public bool CursorHideWhenHeld = false;
        public float CursorInnerScale = 1.5f;
        public float CursorOuterScale = 1.2f;
        public string CursorInnerColor = UIColor.MpPink.Name;
        public string CursorOuterColor = UIColor.HealthGreen.Name;

        public ComboValueConfig<CursorType> CursorType = new();
        public ComboValueConfig<Helper.StatusNameId> CursorStatus = new(true);
        public FloatValueConfig CursorStatusDuration = new(5f);

        // ===== ICONS ===========

        public bool IconsEnabled = true;
        public BoolValueConfig IconEnabled = new(true);

        // =====================

        public void Save() {
            JobBars.PluginInterface.SavePluginConfig(this);
        }

        public static bool DrawColor(string id, ElementColor currentValue, out ElementColor value) {
            value = currentValue;
            if (ImGui.BeginCombo(id, value.Name)) {
                foreach (var entry in UIColor.AllColors) {
                    if (ImGui.Selectable($"{entry.Key}##Combo", value.Name == entry.Key)) {
                        value = entry.Value;
                        ImGui.EndCombo();
                        return true;
                    }
                }
                ImGui.EndCombo();
            }
            return false;
        }
    }
}
