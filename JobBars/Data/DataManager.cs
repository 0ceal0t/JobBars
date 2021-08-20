using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using JobBars.Gauges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

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

        public static GameObject PreviousEnemyTarget => Manager.GetPreviousEnemyTarget();

        public static JobIds IdToJob(uint job) {
            return job < 19 ? JobIds.OTHER : (JobIds)job;
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

                    buffDict[new Item {
                        Id = status->StatusID,
                        Type = ItemType.Buff
                    }] = new BuffElem {
                        Duration = status->RemainingTime > 0 ? status->RemainingTime : status->RemainingTime * -1,
                        StackCount = status->StackCount
                    };
                }
            }
        }

        // ==================

        private readonly DalamudPluginInterface PluginInterface;

        private readonly HashSet<uint> GCDs = new();
        private readonly Dictionary<uint, uint> ActionToIcon = new();
        private readonly IntPtr TargetAddress;

        private DataManager(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;
            TargetAddress = PluginInterface.TargetModuleScanner.GetStaticAddressFromSig("48 8B 05 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? FF 50 ?? 48 85 DB", 3);
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

        private GameObject GetPreviousEnemyTarget() {
            var actorAddress = Marshal.ReadIntPtr(TargetAddress + 0xF0);
            if (actorAddress == IntPtr.Zero) return null;

            return PluginInterface.ClientState.Objects.CreateObjectReference(actorAddress);
        }
    }
}
