using Dalamud.Configuration;
using Dalamud.Plugin;
using JobBars.Gauges;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Numerics;
using static JobBars.UI.UIColor;

namespace JobBars.Data {
    [Serializable]
    public class Configuration : IPluginConfiguration {
        public int Version { get; set; } = 0;

        public Vector2 GaugePosition { get; set; } = new Vector2(200, 200);
        public float GaugeScale = 1.0f;
        public bool GaugeHorizontal = false;
        public bool GaugeAlignRight = false;
        public bool GaugeIconReplacement = true;
        public HashSet<string> GaugeDisabled = new HashSet<string>();
        public Dictionary<string, string> GaugeColorOverride = new Dictionary<string, string>();
        public Dictionary<string, GaugeVisualType> GaugeTypeOverride = new Dictionary<string, GaugeVisualType>();
        public Dictionary<string, int> GaugeOrderOverride = new Dictionary<string, int>();
        public int SeNumber = 0;
        public bool GaugeSplit = false;
        public Dictionary<string, Vector2> GaugeSplitPosition = new Dictionary<string, Vector2>();

        public Vector2 BuffPosition { get; set; } = new Vector2(300, 300);
        public float BuffScale = 1.0f;
        public HashSet<string> BuffDisabled = new HashSet<string>();

        [NonSerialized]
        private DalamudPluginInterface _pluginInterface;
        [NonSerialized]
        public static Configuration Config;

        public bool GetColorOverride(string gaugeName, out ElementColor color) {
            color = new ElementColor();
            if(!GaugeColorOverride.TryGetValue(gaugeName, out string colorName)) {
                return false;
            }
            if (!AllColors.TryGetValue(colorName, out var newColor)) {
                return false;
            }
            color = newColor;
            return true;
        }

        public Vector2 GetGaugeSplitPosition(string gaugeName) {
            return GaugeSplitPosition.TryGetValue(gaugeName, out var value) ? value : new Vector2(200, 200);
        }

        public void Initialize(DalamudPluginInterface pluginInterface) {
            _pluginInterface = pluginInterface;
            Config = this;
        }

        public void Save() {
            _pluginInterface.SavePluginConfig(this);
        }
    }
}
