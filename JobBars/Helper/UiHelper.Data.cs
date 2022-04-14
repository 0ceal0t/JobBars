using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Status = FFXIVClientStructs.FFXIV.Client.Game.Status;
using Lumina.Excel.GeneratedSheets;
using JobBars.Data;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Logging;

namespace JobBars.Helper {
    public struct ItemData {
        public string Name;
        public Item Data;
        public ushort Icon;

        public override string ToString() {
            return Name;
        }

        public override bool Equals(object obj) {
            return obj is ItemData overrides && Equals(overrides);
        }

        public bool Equals(ItemData other) {
            return Data.Id == other.Data.Id;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Name, Data);
        }

        public static bool operator ==(ItemData left, ItemData right) {
            return left.Equals(right);
        }

        public static bool operator !=(ItemData left, ItemData right) {
            return !(left == right);
        }
    }

    public unsafe partial class UIHelper {
        public static bool OutOfCombat => !JobBars.Condition[ConditionFlag.InCombat];
        public static bool WeaponSheathed => JobBars.ClientState.LocalPlayer != null && !JobBars.ClientState.LocalPlayer.StatusFlags.HasFlag(StatusFlags.WeaponOut);
        public static bool WatchingCutscene => JobBars.Condition[ConditionFlag.OccupiedInCutSceneEvent] || JobBars.Condition[ConditionFlag.WatchingCutscene78] || JobBars.Condition[ConditionFlag.BetweenAreas] || JobBars.Condition[ConditionFlag.BetweenAreas51];
        public static bool InPvP { get; private set; } = false;

        public static bool CalcDoHide( bool enabled, bool hideOutOfCombat, bool hideWeaponSheathed) {
            if (!enabled) return true;
            if (OutOfCombat && hideOutOfCombat) return true;
            if (WeaponSheathed && hideWeaponSheathed) return true;
            if (WatchingCutscene) return true;
            if (InPvP) return true;
            return false;
        }

        public static void ZoneChanged(ushort e) {
            InPvP = JobBars.DataManager.GetExcelSheet<TerritoryType>().GetRow(e)?.IsPvpZone ?? false;
        }

        private static readonly HashSet<uint> GCDs = new();
        private static readonly Dictionary<uint, uint> ActionToIcon = new();

        public static List<ItemData> StatusList { get; private set; } = new();
        public static List<ItemData> ActionList { get; private set; } = new();

        public static bool IsGCD(ActionIds action) => IsGCD((uint)action);
        public static bool IsGCD(uint action) => GCDs.Contains(action);

        public static int GetIcon(ActionIds action) => GetIcon((uint)action);
        public static int GetIcon(uint action) => (int)ActionToIcon[action];

        public static JobIds IdToJob(uint job) => job < 19 ? JobIds.OTHER : (JobIds)job;

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

        public static string ProcText => JobBars.ClientState.ClientLanguage switch {
            Dalamud.ClientLanguage.Japanese => "Procs",
            Dalamud.ClientLanguage.English => "Procs",
            Dalamud.ClientLanguage.German => "Procs",
            Dalamud.ClientLanguage.French => "Procs",
            _ => "触发"
        };

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
                return !buff.Any() ? "Unknown" : ToTitleCase(buff.First().Name);
            }
            else {
                var action = ActionSheet.Where(x => x.RowId == item.Id);
                return !action.Any() ? "Unknown" : ToTitleCase(action.First().Name);
            }
        }

        private static string ToTitleCase(this string title) {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title);
        }

        private static void SetupSheets() {
            JobToString = new();
            ItemToString = new();
            ActionList.Clear();
            StatusList.Clear();

            ActionSheet = JobBars.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where(
                x => !string.IsNullOrEmpty(x.Name) && (x.IsPlayerAction || x.ClassJob.Value != null) && !x.IsPvP // weird conditions to catch things like enchanted RDM spells
            );
            foreach (var item in ActionSheet) {
                var name = item.Name.ToString();
                var attackType = item.ActionCategory.Value.Name.ToString();
                var actionId = item.ActionCategory.Value.RowId;
                if (item.Icon != 405 && item.Icon != 0) ActionToIcon[item.RowId] = item.Icon;

                if (actionId == 2 || actionId == 3) { // spell or weaponskill
                    if (item.CooldownGroup == 58 || item.AdditionalCooldownGroup == 58) GCDs.Add(item.RowId); // not actually a gcd
                }

                ActionList.Add(new ItemData {
                    Name = item.Name,
                    Icon = item.Icon,
                    Data = new Item {
                        Id = item.RowId,
                        Type = ItemType.Action
                    }
                });
            }

            StatusSheet = JobBars.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Status>().Where(x => !string.IsNullOrEmpty(x.Name));
            foreach (var item in StatusSheet) {
                StatusList.Add(new ItemData {
                    Name = item.Name,
                    Icon = item.Icon,
                    Data = new Item {
                        Id = item.RowId,
                        Type = ItemType.Buff
                    }
                });
            }

            JobSheet = JobBars.DataManager.GetExcelSheet<ClassJob>().Where(x => x.Name != null);
        }

        public static float TimeLeft(float defaultDuration, Dictionary<Item, Status> buffDict, Item lastActiveTrigger, DateTime lastActiveTime) {
            if (lastActiveTrigger.Type == ItemType.Buff) {
                if (buffDict.TryGetValue(lastActiveTrigger, out var elem)) { // duration exists, use that
                    return elem.RemainingTime;
                }
                else { // time isn't there, are we just waiting on it?
                    var timeSinceActive = (DateTime.Now - lastActiveTime).TotalSeconds;
                    if (timeSinceActive <= 2) { // hasn't been enough time for it to show up in the buff list
                        return defaultDuration;
                    }
                    return -1; // yeah lmao it's gone
                }
            }
            else {
                return (float)(defaultDuration - (DateTime.Now - lastActiveTime).TotalSeconds); // triggered by an action, just calculate the time
            }
        }
    }
}
