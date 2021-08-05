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

        public float GaugeScale = 1.0f;
        public bool GaugeHorizontal = false;
        public bool GaugeAlignRight = false;
        public bool GaugeSplit = false;
        public Vector2 GaugePosition { get; set; } = new Vector2(0, 0);
        public Dictionary<string, Vector2> GaugeSplitPosition = new();
        public Dictionary<string, float> GaugeIndividualScale = new();

        public bool GaugesEnabled = true;
        public bool GaugesHideOutOfCombat = false;
        public bool GaugeIconReplacement = true;
        public bool GaugeHideGCDInactive = false;
        public HashSet<string> GaugeDisabled = new();
        public HashSet<string> GaugeIconDisabled = new();
        public HashSet<string> GaugeInvert = new();
        public float GaugeLowTimerWarning = 4.0f;

        public Dictionary<string, string> GaugeColorOverride = new();
        public Dictionary<string, GaugeVisualType> GaugeTypeOverride = new();
        public Dictionary<string, int> GaugeOrderOverride = new();

        public int SeNumber = 0;

        public Vector2 BuffPosition { get; set; } = new Vector2(0, 0);
        public float BuffScale = 1.0f;

        public bool BuffBarEnabled = true;
        public bool BuffHideOutOfCombat = false;
        public bool BuffIncludeParty = true;
        public HashSet<string> BuffDisabled = new();

        public int BuffHorizontal = 5;
        public bool BuffRightToLeft = false;
        public bool BuffBottomToTop = false;


        [NonSerialized]
        private DalamudPluginInterface PluginInterface;

        public static Configuration Config { get; private set; }

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
            return GaugeSplitPosition.TryGetValue(gaugeName, out var value) ? value : new Vector2(0, 0);
        }

        public void Initialize(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;
            Config = this;
        }

        public void Save() {
            PluginInterface.SavePluginConfig(this);
        }
    }
}
