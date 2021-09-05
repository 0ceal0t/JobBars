using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Status = FFXIVClientStructs.FFXIV.Client.Game.Status;
using Lumina.Excel.GeneratedSheets;
using JobBars.Data;

namespace JobBars.Helper {
    public struct StatusNameId {
        public string Name;
        public Item Status;

        public override string ToString() {
            return Name;
        }

        public override bool Equals(object obj) {
            return obj is StatusNameId overrides && Equals(overrides);
        }

        public bool Equals(StatusNameId other) {
            return Status.Id == other.Status.Id;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Name, Status);
        }

        public static bool operator ==(StatusNameId left, StatusNameId right) {
            return left.Equals(right);
        }

        public static bool operator !=(StatusNameId left, StatusNameId right) {
            return !(left == right);
        }
    }

    public unsafe partial class UIHelper {
        private static readonly HashSet<uint> GCDs = new();
        private static readonly Dictionary<uint, uint> ActionToIcon = new();

        public static StatusNameId[] StatusNames { get; private set; }

        // ===============

        public static bool IsGCD(ActionIds action) => IsGCD((uint)action);
        public static bool IsGCD(uint action) => GCDs.Contains(action);

        public static int GetIcon(ActionIds action) => GetIcon((uint)action);
        public static int GetIcon(uint action) => (int)ActionToIcon[action];

        public static JobIds IdToJob(uint job) => job < 19 ? JobIds.OTHER : (JobIds)job;
        public static JobIds IconToJob(uint icon) => icon switch {
            062119 => JobIds.PLD,
            062120 => JobIds.MNK,
            062121 => JobIds.WAR,
            062122 => JobIds.DRG,
            062123 => JobIds.BRD,
            062124 => JobIds.WHM,
            062125 => JobIds.BLM,
            062127 => JobIds.SMN,
            062128 => JobIds.SCH,
            062130 => JobIds.NIN,
            062131 => JobIds.MCH,
            062132 => JobIds.DRK,
            062133 => JobIds.AST,
            062134 => JobIds.SAM,
            062135 => JobIds.RDM,
            062136 => JobIds.BLU,
            062137 => JobIds.GNB,
            062138 => JobIds.DNC,
            _ => JobIds.OTHER
        };

        private static IEnumerable<ClassJob> JobSheet;
        private static IEnumerable<Lumina.Excel.GeneratedSheets.Action> ActionSheet;
        private static IEnumerable<Lumina.Excel.GeneratedSheets.Status> StatusSheet;

        // Cache converted strings
        private static Dictionary<JobIds, string> JobToString;
        private static Dictionary<Item, string> ItemToString;

        public static string Localize(JobIds job) {
            if (JobToString.TryGetValue(job, out var jobString)) return jobString;
            else {
                var convertedJob = ConvertJobToString(job);
                JobToString[job] = convertedJob;
                return convertedJob;
            }
        }

        public static string Localize(ActionIds action) => Localize(new Item(action));
        public static string Localize(BuffIds buff) => Localize(new Item(buff));
        public static string Localize(Item item) {
            if (ItemToString.TryGetValue(item, out var itemString)) return itemString;
            else {
                var convertedItem = ConvertItemToString(item);
                ItemToString[item] = convertedItem;
                return convertedItem;
            }
        }

        private static string ConvertJobToString(JobIds job) {
            foreach (var classJob in JobSheet) {
                if (classJob.RowId == (uint)job) return ToTitleCase(classJob.Name);
            }
            return "ERROR";
        }

        private static string ConvertItemToString(Item item) {
            if (item.Type == ItemType.Buff) {
                var buff = StatusSheet.Where(x => x.RowId == item.Id);
                return ToTitleCase(buff.First().Name);
            }
            else {
                var action = ActionSheet.Where(x => x.RowId == item.Id);
                return ToTitleCase(action.First().Name);
            }
        }

        private static string ToTitleCase(this string title) {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }

        private static void SetupSheets() {
            JobToString = new();
            ItemToString = new();

            ActionSheet = JobBars.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where(
                x => !string.IsNullOrEmpty(x.Name) && (x.IsPlayerAction || x.ClassJob.Value != null) && !x.IsPvP // weird conditions to catch things like enchanted RDM spells
            );
            foreach (var item in ActionSheet) {
                var name = item.Name.ToString();
                var attackType = item.ActionCategory.Value.Name.ToString();
                var actionId = item.ActionCategory.Value.RowId;
                if (actionId == 2 || actionId == 3) { // spell or weaponskill
                    if (item.CooldownGroup != 58 && item.AdditionalCooldownGroup != 58) continue; // not actually a gcd
                    GCDs.Add(item.RowId);
                }

                if (item.Icon != 405 && item.Icon != 0) ActionToIcon[item.RowId] = item.Icon;
            }

            List<StatusNameId> statusList = new();
            StatusSheet = JobBars.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Status>().Where(x => !string.IsNullOrEmpty(x.Name));
            foreach(var item in StatusSheet) {
                statusList.Add(new StatusNameId {
                    Name = item.Name,
                    Status = new Item {
                        Id = item.RowId,
                        Type = ItemType.Buff
                    }
                });
            }
            StatusNames = statusList.ToArray();

            JobSheet = JobBars.DataManager.GetExcelSheet<ClassJob>().Where(x => x.Name != null);
        }

        public static float TimeLeft(float defaultDuration, DateTime time, Dictionary<Item, Status> buffDict, Item lastActiveTrigger, DateTime lastActiveTime) {
            if (lastActiveTrigger.Type == ItemType.Buff) {
                if (buffDict.TryGetValue(lastActiveTrigger, out var elem)) { // duration exists, use that
                    return elem.RemainingTime;
                }
                else { // time isn't there, are we just waiting on it?
                    var timeSinceActive = (time - lastActiveTime).TotalSeconds;
                    if (timeSinceActive <= 2) { // hasn't been enough time for it to show up in the buff list
                        return defaultDuration;
                    }
                    return -1; // yeah lmao it's gone
                }
            }
            else {
                return (float)(defaultDuration - (time - lastActiveTime).TotalSeconds); // triggered by an action, just calculate the time
            }
        }
    }
}
