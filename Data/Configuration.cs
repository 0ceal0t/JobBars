using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Data {
    [Serializable]
    public class Configuration : IPluginConfiguration {
        public int Version { get; set; } = 0;
        public Vector2 GaugePosition { get; set; } = new Vector2(200, 200);
        public float GaugeScale = 1.0f;
        public Vector2 BuffPosition { get; set; } = new Vector2(300, 300);
        public float BuffScale = 1.0f;
        public HashSet<string> GaugeHidden = new HashSet<string>();

        [NonSerialized]
        private DalamudPluginInterface _pluginInterface;
        [NonSerialized]
        public static Configuration Config;

        public void Initialize(DalamudPluginInterface pluginInterface) {
            _pluginInterface = pluginInterface;
            Config = this;
        }

        public void Save() {
            _pluginInterface.SavePluginConfig(this);
        }
    }
}
