using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.GameStructs;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.UI {
    public unsafe class UIIconManager {
        private readonly string[] AllActionBars = {
            "_ActionBar",
            "_ActionBar01",
            "_ActionBar02",
            "_ActionBar03",
            "_ActionBar04",
            "_ActionBar05",
            "_ActionBar06",
            "_ActionBar07",
            "_ActionBar08",
            "_ActionBar09",
            "_ActionCross"
        };
        private static readonly int MILLIS_LOOP = 250;

        private readonly Dictionary<uint, UIIconProps> IconConfigs = new();
        private readonly List<UIIcon> Icons = new();
        private readonly HashSet<IntPtr> IconOverride = new();

        public UIIconManager() {
        }

        public void Setup(List<uint> triggers, UIIconProps props) {
            if (triggers == null) return;

            foreach (var trigger in triggers) {
                IconConfigs[trigger] = props;
            }
        }

        public void SetProgress(List<uint> triggers, float current, float max) {
            foreach (var icon in Icons) {
                if (!triggers.Contains(icon.AdjustedId)) continue;
                icon.SetProgress(current, max);
            }
        }

        public void SetDone(List<uint> triggers) {
            foreach (var icon in Icons) {
                if (!triggers.Contains(icon.AdjustedId)) continue;
                icon.SetDone();
            }
        }

        public void Tick() {
            var time = DateTime.Now;
            var millis = time.Second * 1000 + time.Millisecond;
            var percent = (float)(millis % MILLIS_LOOP) / MILLIS_LOOP;

            var hotbarData = UIHelper.GetHotbarUI();
            if (hotbarData == null) return;

            HashSet<UIIcon> FoundIcons = new();

            for (var hotbarIndex = 0; hotbarIndex < AllActionBars.Length; hotbarIndex++) {
                var isCross = hotbarIndex > 9;
                var crossBarSet = isCross ? UIHelper.GetCrossBarSet() : -1;
                if (isCross && (crossBarSet == -1 || crossBarSet > 7)) continue;

                var hotbar = hotbarData->Hotbars[isCross ? 10 + crossBarSet : hotbarIndex];

                var actionBar = (AddonActionBarBase*)AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonByName(AllActionBars[hotbarIndex]);
                if (actionBar == null || actionBar->ActionBarSlotsAction == null) continue;

                for (var slotIndex = 0; slotIndex < actionBar->HotbarSlotCount; slotIndex++) {
                    var slotData = hotbar[slotIndex];
                    if (slotData.Type != HotbarSlotStructType.Action) continue;

                    var action = UIHelper.GetAdjustedAction(slotData.ActionId);

                    if (!IconConfigs.TryGetValue(action, out var props)) continue; // not looking for this action id

                    var found = false;
                    foreach (var icon in Icons) {
                        if (icon.HotbarIdx != hotbarIndex || icon.SlotIdx != slotIndex) continue;
                        if (slotData.ActionId != icon.SlotId) break; // action changed, just ignore it

                        // found existing icon which matches
                        found = true;
                        FoundIcons.Add(icon);
                        icon.Tick(percent, slotData.YellowBorder);

                        break;
                    }
                    if (found) continue; // already found an icon, don't need to create a new one

                    var slot = actionBar->ActionBarSlotsAction[slotIndex];
                    UIIcon newIcon = props.IsTimer ?
                        new UIIconTimer(action, slotData.ActionId, hotbarIndex, slotIndex, slot.Icon, props) :
                        new UIIconBuff(action, slotData.ActionId, hotbarIndex, slotIndex, slot.Icon, props);
                    FoundIcons.Add(newIcon);
                    Icons.Add(newIcon);
                }
            }

            foreach (var icon in Icons.Where(x => !FoundIcons.Contains(x))) icon.Dispose();
            Icons.RemoveAll(x => !FoundIcons.Contains(x));
        }

        public void AddIconOverride(IntPtr icon) {
            IconOverride.Add(icon);
        }

        public void RemoveIconOverride(IntPtr icon) {
            IconOverride.Remove(icon);
        }

        public void ProcessIconOverride(IntPtr icon) {
            if (icon == IntPtr.Zero) return;
            if (IconOverride.Contains(icon)) {
                var image = (AtkImageNode*)icon;
                UIIconTimer.SetDimmed(image, true);
            }
        }

        public void Reset() {
            IconConfigs.Clear();
            Icons.ForEach(x => x.Dispose());
            Icons.Clear();
        }

        public void Dispose() {
            IconOverride.Clear();
            Reset();
        }
    }
}
