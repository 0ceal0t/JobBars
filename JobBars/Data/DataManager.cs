using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Data {
    public class DataManager {
        public static DataManager Manager { get; private set; }

        public static void Initialize(DalamudPluginInterface pluginInterface) {
            Manager = new DataManager(pluginInterface);
        }

        public static void Dispose() {
            Manager = null;
        }

        public static bool IsGCD(ActionIds action) => IsGCD((uint)action);
        public static bool IsGCD(uint action) => Manager.GCDs.Contains(action);

        public static int GetIcon(ActionIds action) => GetIcon((uint)action);
        public static int GetIcon(uint action) => (int)Manager.ActionToIcon[action];

        public static JobIds IdToJob(uint job) {
            return job < 19 ? JobIds.OTHER : (JobIds)job;
        }

        // ==================

        private readonly DalamudPluginInterface PluginInterface;

        private readonly HashSet<uint> GCDs = new();
        private readonly Dictionary<uint, uint> ActionToIcon = new();

        private DataManager(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;
            SetupActions();
        }

        private void SetupActions() {
            var sheet = PluginInterface.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where(
                x => !string.IsNullOrEmpty(x.Name) && (x.IsPlayerAction || x.ClassJob.Value != null) && !x.IsPvP // weird conditions to catch things like enchanted RDM spells
            );
            foreach (var item in sheet) {
                var name = item.Name.ToString();
                var attackType = item.ActionCategory.Value.Name.ToString();
                var actionId = item.ActionCategory.Value.RowId;
                if (actionId == 2 || actionId == 3) { // spell or weaponskill
                    GCDs.Add(item.RowId);
                }

                if (item.Icon != 405) ActionToIcon[item.RowId] = item.Icon;
            }
        }
    }
}
