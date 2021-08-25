using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using JobBars.Data;
using JobBars.GameStructs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Helper {
    public unsafe partial class UIHelper {
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

        public static float TimeLeft(float defaultDuration, DateTime time, Dictionary<Item, BuffElem> buffDict, Item lastActiveTrigger, DateTime lastActiveTime) {
            if (lastActiveTrigger.Type == ItemType.Buff) {
                if (buffDict.TryGetValue(lastActiveTrigger, out var elem)) { // duration exists, use that
                    return elem.Duration;
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

        public unsafe static AddonPartyListIntArray* GetPartyUI() {
            var uiModule = Framework.Instance()->GetUiModule();
            if (uiModule == null) return null;
            var numArray = uiModule->RaptureAtkModule.AtkModule.AtkArrayDataHolder.NumberArrays[4];
            return (AddonPartyListIntArray*)numArray->IntArray;
        }

        public unsafe static int GetPartyCount() {
            var groupManager = GroupManager.Instance();
            if (groupManager == null) return 0;
            return groupManager->MemberCount;
        }

        public unsafe static bool IsInParty(uint objectId) {
            if (objectId == 0 || objectId == 0xE0000000 || objectId == 0xFFFFFFFF) return false;

            var groupManager = GroupManager.Instance();
            if (groupManager == null || groupManager->MemberCount == 0) return false;

            for (int i = 0; i < 8; i++) {
                PartyMember* info = (PartyMember*)(new IntPtr(groupManager->PartyMembers) + 0x230 * i);
                if (objectId == info->ObjectID) return true;
            }
            return false;
        }

        public unsafe static void GetPartyStatus(int ownerId, Dictionary<Item, BuffElem> buffDict) {
            var groupManager = GroupManager.Instance();
            if (groupManager == null || groupManager->MemberCount == 0) return;

            for (int i = 0; i < 8; i++) {
                PartyMember* info = (PartyMember*)(new IntPtr(groupManager->PartyMembers) + 0x230 * i);
                if (info->ObjectID == 0 || info->ObjectID == 0xE0000000 || info->ObjectID == 0xFFFFFFFF) continue;
                if (info->StatusManager.Status == null) continue;

                for(int j = 0; j < 30; j++) {
                    Status* status = (Status*)(new IntPtr(info->StatusManager.Status) + 0xC * j);
                    if (status->SourceID != ownerId) continue;
                    StatusToBuffElem(buffDict, status);
                }
            }
        }

        public static void StatusToBuffElem(Dictionary<Item, BuffElem> buffDict, Status* status) {
            buffDict[new Item {
                Id = status->StatusID,
                Type = ItemType.Buff
            }] = new BuffElem {
                Duration = status->RemainingTime > 0 ? status->RemainingTime : status->RemainingTime * -1,
                StackCount = status->StackCount
            };
        }

        public static void StatusToBuffElem(Dictionary<Item, BuffElem> buffDict, Dalamud.Game.ClientState.Statuses.Status status) {
            buffDict[new Item {
                Id = status.StatusId,
                Type = ItemType.Buff
            }] = new BuffElem {
                Duration = status.RemainingTime > 0 ? status.RemainingTime : status.RemainingTime * -1,
                StackCount = status.StackCount
            };
        }

        // ==================

        private static readonly HashSet<uint> GCDs = new();
        private static readonly Dictionary<uint, uint> ActionToIcon = new();

        private static bool MpTickActive = false;
        private static DateTime MpTime;
        private static uint LastMp = 0;

        private static bool ActorTickActive = false;
        private static DateTime ActorTick;

        public static bool GetCurrentCast(out float currentTime, out float totalTime) {
            currentTime = JobBars.ClientState.LocalPlayer.CurrentCastTime;
            totalTime = JobBars.ClientState.LocalPlayer.TotalCastTime;
            return JobBars.ClientState.LocalPlayer.IsCasting;
        }

        public static void UpdateMp(uint currentMp) {
            if(currentMp != 10000 && currentMp > LastMp && !MpTickActive) {
                MpTickActive = true;
                MpTime = DateTime.Now;
            }
            LastMp = currentMp;
        }

        public static void UpdateActorTick() {
            if(!ActorTickActive) {
                ActorTickActive = true;
                ActorTick = DateTime.Now;
            }
        }

        public static void ResetTicks() {
            MpTickActive = false;
            ActorTickActive = false;
        }

        public static float GetActorTick() {
            if (!ActorTickActive) return 0;
            var currentTime = DateTime.Now;
            var diff = (currentTime - ActorTick).TotalSeconds;
            return (float)(diff % 3.0f / 3.0f);
        }

        public static float GetMpTick() {
            if (!MpTickActive) return 0;
            if (LastMp == 10000) return 0; // already max
            var currentTime = DateTime.Now;
            var diff = (currentTime - MpTime).TotalSeconds;
            return (float)(diff % 3.0f / 3.0f);
        }

        private static void SetupActions() {
            var sheet = JobBars.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where(
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

    public struct BuffElem {
        public float Duration;
        public byte StackCount;
    }
}
