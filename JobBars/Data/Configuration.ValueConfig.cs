using ImGuiNET;
using JobBars.Atk;
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
            JobBars.Configuration.Save();
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

        public ComboValueConfig(bool showSearch = false) : base() {
            ShowSearch = showSearch;
        }

        public override bool Draw(string id, string name, T defaultValue, out T value) { // whatever
            value = default;
            return false;
        }
        public bool Draw(string id, string name, List<T> comboOptions, T defaultValue, out T value) => Draw(id ,name, comboOptions.ToArray(), defaultValue, out value);
        public bool Draw(string id, string name, T[] comboOptions, T defaultValue, out T value) {
            value = Get(name, defaultValue);
            if (DrawCombo(id, comboOptions, value, out value)) {
                Set(name, value);
                return true;
            }
            return false;
        }

        private bool DrawCombo(string id, T[] comboOptions, T currentValue, out T value) {
            value = currentValue;
            if (ImGui.BeginCombo(id, $"{currentValue}", ImGuiComboFlags.HeightLargest)) {
                if (ShowSearch) {
                    ImGui.SetNextItemWidth(ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X - 50);
                    ImGui.InputText("Search##Combo", ref SearchInput, 256);
                }

                if (ShowSearch) ImGui.BeginChild("Child##Combo", new Vector2(ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X, 200), true);

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
            AtkColor.GetColor(val, defaultColor) : defaultColor;

        public void Set(string name, ElementColor color) {
            Color[name] = color.Name;
            JobBars.Configuration.Save();
        }

        public bool Draw(string id, string name, ElementColor defaultValue, out ElementColor value) {
            value = Get(name, defaultValue);
            if (Configuration.DrawColor(id, value, out value)) {
                Set(name, value);
                return true;
            }
            return false;
        }
    }
}
