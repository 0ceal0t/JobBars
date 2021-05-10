using Dalamud.Hooking;
using Dalamud.Plugin;
using FFXIVClientInterface;
using FFXIVClientInterface.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.GameStructs;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.UI {
    public enum IconState {
        Running,
        Waiting,
        Remove
    }

    public struct IconProgress {
        public float Max;
        public double Current;
        public IconState _State;
    }

    public unsafe class UIIconManager {
        public DalamudPluginInterface PluginInterface;
        public ClientInterface Client;
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
            "_ActionCross",
            "_ActionDoubleCrossL",
            "_ActionDoubleCrossR",
        };
        public Dictionary<uint, IconProgress> ActionIdToStatus = new Dictionary<uint, IconProgress>();
        HashSet<IntPtr> ToCleanup = new HashSet<IntPtr>();

        List<IntPtr> IconRecastOverride = new List<IntPtr>();
        private delegate void SetIconRecastDelegate(IntPtr icon);
        private Hook<SetIconRecastDelegate> setIconRecastHook;

        public UIIconManager(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;
            Client = new ClientInterface(pluginInterface.TargetModuleScanner, pluginInterface.Data);

            IntPtr setIconRecastPtr = PluginInterface.TargetModuleScanner.ScanText("40 53 48 83 EC 20 48 8B D9 E8 ?? ?? ?? ?? 48 8B 4B 10 48 85 C9 74 23 BA ?? ?? ?? ?? ");
            setIconRecastHook = new Hook<SetIconRecastDelegate>(setIconRecastPtr, (SetIconRecastDelegate)SetIconRecast);
            setIconRecastHook.Enable();
        }

        public void Reset() {
            foreach(var ptr in ToCleanup) {
                Cleanup(ptr);
            }
            ToCleanup.Clear();
            ActionIdToStatus.Clear();
        }

        public void Dispose() {
            setIconRecastHook.Disable();
            setIconRecastHook.Dispose();

            Reset();
            Client.Dispose();
        }

        public void SetIconRecast(IntPtr icon) {
            if (!IconRecastOverride.Contains(icon)) {
                setIconRecastHook.Original(icon);
            }
            return;
        }

        static int MILLIS_LOOP = 250;

        public void Update() {
            if (ActionIdToStatus.Count == 0) return;
            var actionManager = Client.ActionManager;
            var hotbarModule = Client.UiModule.RaptureHotbarModule;
            List<IntPtr> newRecastOverride = new List<IntPtr>();

            for (var abIndex = 0; abIndex < AllActionBars.Length; abIndex++) {
                if (actionManager == null || hotbarModule == null) return;
                var actionBar = AllActionBars[abIndex];
                var ab = (AddonActionBarBase*)PluginInterface.Framework.Gui.GetUiObjectByName(actionBar, 1);
                if (ab == null || ab->ActionBarSlotsAction == null) continue;
                var bar = abIndex > 10 ? null : hotbarModule.GetBar(abIndex, HotBarType.Normal);
                for (var i = 0; i < ab->HotbarSlotCount; i++) {
                    var slot = ab->ActionBarSlotsAction[i];
                    var slotStruct = hotbarModule.GetBarSlot(bar, i);
                    if(slotStruct != null && ActionIdToStatus.TryGetValue(slotStruct->CommandId, out var iconProgress)) {
                        var icon = slot.Icon;
                        var cdOverlay = (AtkImageNode*) icon->Component->UldManager.NodeList[5];
                        var dashOverlay = (AtkImageNode*)icon->Component->UldManager.NodeList[9];
                        var iconImage = (AtkImageNode*)icon->Component->UldManager.NodeList[0];

                        if (iconProgress._State == IconState.Remove) { // WHEN DONE WITH AN ICON, SUCH AS CHANGING JOBS
                            ActionIdToStatus.Remove(slotStruct->CommandId);
                            Cleanup(icon);
                            ToCleanup.Remove((IntPtr)icon);
                        }
                        else if(iconProgress._State == IconState.Waiting) { // WHEN WAITING TO BE RECAST
                            ToCleanup.Add((IntPtr)icon);
                            Cleanup(icon);

                            var time = DateTime.Now;
                            int millis = time.Second * 1000 + time.Millisecond;
                            float percent = (float)(millis % MILLIS_LOOP) / MILLIS_LOOP;
                            dashOverlay->PartId = (ushort)(6 + percent * 7);
                            UiHelper.Show(dashOverlay);
                        }
                        else {
                            ToCleanup.Add((IntPtr)icon);
                            newRecastOverride.Add((IntPtr)cdOverlay);
                            iconImage->AtkResNode.MultiplyBlue = 50;
                            iconImage->AtkResNode.MultiplyBlue_2 = 50;
                            iconImage->AtkResNode.MultiplyRed = 50;
                            iconImage->AtkResNode.MultiplyRed_2 = 50;
                            iconImage->AtkResNode.MultiplyGreen = 50;
                            iconImage->AtkResNode.MultiplyGreen_2 = 50;
                            cdOverlay->PartId = (ushort)(81 - (float)(iconProgress.Current / iconProgress.Max) * 80);
                            UiHelper.Show(cdOverlay);
                            UiHelper.Hide(dashOverlay);
                        }
                    }
                }
            }

            IconRecastOverride = newRecastOverride;
        }

        public void Cleanup(IntPtr node) {
            Cleanup((AtkComponentNode*)node);
        }
        public void Cleanup(AtkComponentNode* node) {
            var cdOverlay = (AtkImageNode*)node->Component->UldManager.NodeList[5];
            var dashOverlay = (AtkImageNode*)node->Component->UldManager.NodeList[9];
            var iconImage = (AtkImageNode*)node->Component->UldManager.NodeList[0];
            iconImage->AtkResNode.MultiplyBlue = 100;
            iconImage->AtkResNode.MultiplyBlue_2 = 100;
            iconImage->AtkResNode.MultiplyRed = 100;
            iconImage->AtkResNode.MultiplyRed_2 = 100;
            iconImage->AtkResNode.MultiplyGreen = 100;
            iconImage->AtkResNode.MultiplyGreen_2 = 100;
            UiHelper.Hide(cdOverlay);
            UiHelper.Hide(dashOverlay);
        }
    }
}
