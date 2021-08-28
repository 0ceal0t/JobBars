using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
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


        private static void SetupSheets() {
            var actionSheet = JobBars.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where(
                x => !string.IsNullOrEmpty(x.Name) && (x.IsPlayerAction || x.ClassJob.Value != null) && !x.IsPvP // weird conditions to catch things like enchanted RDM spells
            );
            foreach (var item in actionSheet) {
                var name = item.Name.ToString();
                var attackType = item.ActionCategory.Value.Name.ToString();
                var actionId = item.ActionCategory.Value.RowId;
                if (actionId == 2 || actionId == 3) { // spell or weaponskill
                    GCDs.Add(item.RowId);
                }

                if (item.Icon != 405 && item.Icon != 0) ActionToIcon[item.RowId] = item.Icon;
            }

            List<StatusNameId> statusList = new();
            var statusSheet = JobBars.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Status>().Where(x => !string.IsNullOrEmpty(x.Name));
            foreach(var item in statusSheet) {
                statusList.Add(new StatusNameId {
                    Name = item.Name,
                    Status = new Item {
                        Id = item.RowId,
                        Type = ItemType.Buff
                    }
                });
            }
            StatusNames = statusList.ToArray();
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

        public static bool GetCurrentCast(out float currentTime, out float totalTime) {
            currentTime = JobBars.ClientState.LocalPlayer.CurrentCastTime;
            totalTime = JobBars.ClientState.LocalPlayer.TotalCastTime;
            return JobBars.ClientState.LocalPlayer.IsCasting;
        }
    }
}
