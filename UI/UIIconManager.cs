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
        StartRunning, // turn on the recast, turn on the test, turn off the dash
        Running, // good to go
        DoneRunning, // turn off the recast, turn off the text
        Waiting
    }

    public struct IconProgress {
        public float Max;
        public double Current;
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
        public Dictionary<uint, IconState> ActionIdToState = new Dictionary<uint, IconState>();
        HashSet<IntPtr> ToCleanup = new HashSet<IntPtr>();

        HashSet<IntPtr> IconRecastOverride;
        private delegate void SetIconRecastDelegate(IntPtr icon);
        private Hook<SetIconRecastDelegate> setIconRecastHook;

        HashSet<IntPtr> IconComponentOverride;
        private delegate IntPtr SetIconRecastDelegate2(IntPtr icon);
        private Hook<SetIconRecastDelegate2> setIconRecastHook2;

        HashSet<IntPtr> IconTextOverride;
        private delegate void SetIconRecastTextDelegate(IntPtr text, int a2, byte a3, byte a4, byte a5, byte a6);
        private Hook<SetIconRecastTextDelegate> setIconRecastTextHook;

        private delegate void SetIconRecastTextDelegate2(IntPtr text, IntPtr a2);
        private Hook<SetIconRecastTextDelegate2> setIconRecastTextHook2;

        public UIIconManager(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;
            Client = new ClientInterface(pluginInterface.TargetModuleScanner, pluginInterface.Data);

            IconRecastOverride = new HashSet<IntPtr>();
            IconComponentOverride = new HashSet<IntPtr>();
            IconTextOverride = new HashSet<IntPtr>();

            IntPtr setIconRecastPtr = PluginInterface.TargetModuleScanner.ScanText("40 53 48 83 EC 20 48 8B D9 E8 ?? ?? ?? ?? 48 8B 4B 10 48 85 C9 74 23"); // changes recast partId for gcds
            setIconRecastHook = new Hook<SetIconRecastDelegate>(setIconRecastPtr, (SetIconRecastDelegate)SetIconRecast);
            setIconRecastHook.Enable();

            // this is for stuff like thunder during thundercloud procs. god this sig is nasty
            IntPtr setIconRecastPtr2 = PluginInterface.TargetModuleScanner.ScanText("40 53 48 83 EC 20 0F B6 81 ?? ?? ?? ?? 48 8B D9 48 83 C1 08 A8 01 74 1E 48 83 79 ?? ?? 74 17 A8 08 75 0E 48 83 79 ?? ?? 75 07 E8 ?? ?? ?? ?? EB 05 E8 ?? ?? ?? ?? F6 83 ?? ?? ?? ?? ?? 0F 84 ?? ?? ?? ?? 48 8B 93 ?? ?? ?? ??");
            setIconRecastHook2 = new Hook<SetIconRecastDelegate2>(setIconRecastPtr2, (SetIconRecastDelegate2)SetIconRecast2);
            setIconRecastHook2.Enable();

            IntPtr setIconTextPtr = PluginInterface.TargetModuleScanner.ScanText("55 57 48 83 EC 28 0F B6 44 24 ?? 8B EA 48 89 5C 24 ?? 48 8B F9"); // sets icon text for abilities which use mp, like Combust
            setIconRecastTextHook = new Hook<SetIconRecastTextDelegate>(setIconTextPtr, (SetIconRecastTextDelegate)SetIconRecastText);
            setIconRecastTextHook.Enable();

            IntPtr setIconTextPtr2 = PluginInterface.TargetModuleScanner.ScanText("4C 8B DC 53 55 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 49 89 73 18 48 8B EA"); // sets icon text for other abilities, like Goring blade
            setIconRecastTextHook2 = new Hook<SetIconRecastTextDelegate2>(setIconTextPtr2, (SetIconRecastTextDelegate2)SetIconRecastText2);
            setIconRecastTextHook2.Enable();
        }

        public void Reset() {
            foreach(var ptr in ToCleanup) {
                Cleanup(ptr);
            }
            ToCleanup.Clear();

            IconRecastOverride.Clear();
            IconComponentOverride.Clear();
            IconTextOverride.Clear();

            ActionIdToStatus.Clear();
            ActionIdToState.Clear();
        }

        public void Dispose() {
            setIconRecastTextHook.Disable();
            setIconRecastTextHook.Dispose();

            setIconRecastTextHook2.Disable();
            setIconRecastTextHook2.Dispose();

            setIconRecastHook.Disable();
            setIconRecastHook.Dispose();

            setIconRecastHook2.Disable();
            setIconRecastHook2.Dispose();

            Reset();
            Client.Dispose();
        }

        public IntPtr SetIconRecast2(IntPtr icon) {
            if(IconComponentOverride.Contains(icon)) {
                return (IntPtr)0;
            }
            return setIconRecastHook2.Original(icon);
        }
        public void SetIconRecast(IntPtr icon) {
            if (!IconRecastOverride.Contains(icon)) {
                setIconRecastHook.Original(icon);
            }
            return;
        }

        public void SetIconRecastText(IntPtr text, int a2, byte a3, byte a4, byte a5, byte a6) {
            if (!IconTextOverride.Contains(text)) {
                setIconRecastTextHook.Original(text, a2, a3, a4, a5, a6);
            }
            return;
        }
        public void SetIconRecastText2(IntPtr text, IntPtr a2) {
            if (!IconTextOverride.Contains(text) || a2 != IntPtr.Zero) {
                setIconRecastTextHook2.Original(text, a2);
            }
            return;
        }

        // visuals get messed up when icons are dragged around, but there's not much I can do
        static int MILLIS_LOOP = 250;
        public void Update() {
            if (ActionIdToStatus.Count == 0) return;
            var actionManager = Client.ActionManager;
            var hotbarModule = Client.UiModule.RaptureHotbarModule;

            HashSet<uint> TO_BUMP = new HashSet<uint>();

            for (var abIndex = 0; abIndex < AllActionBars.Length; abIndex++) {
                if (actionManager == null || hotbarModule == null) return;
                var actionBar = AllActionBars[abIndex];
                var ab = (AddonActionBarBase*)PluginInterface.Framework.Gui.GetUiObjectByName(actionBar, 1);
                if (ab == null || ab->ActionBarSlotsAction == null) continue;
                var bar = abIndex > 10 ? null : hotbarModule.GetBar(abIndex, HotBarType.Normal);
                for (var i = 0; i < ab->HotbarSlotCount; i++) {
                    var slot = ab->ActionBarSlotsAction[i];
                    var slotStruct = hotbarModule.GetBarSlot(bar, i);
                    if(slotStruct != null && slotStruct->CommandType == HotbarSlotType.Action && ActionIdToStatus.TryGetValue(slotStruct->CommandId, out var iconProgress)) {
                        var state = ActionIdToState[slotStruct->CommandId];

                        var icon = slot.Icon;
                        var cdOverlay = (AtkImageNode*) icon->Component->UldManager.NodeList[5];
                        var dashOverlay = (AtkImageNode*)icon->Component->UldManager.NodeList[9];
                        var iconImage = (AtkImageNode*)icon->Component->UldManager.NodeList[0]; 
                        var bottomLeftText = (AtkTextNode*)icon->Component->UldManager.NodeList[13];

                        if(state == IconState.StartRunning) {
                            ToCleanup.Add((IntPtr)icon);
                            TO_BUMP.Add(slotStruct->CommandId);

                            UiHelper.Show(cdOverlay);
                            UiHelper.Hide(dashOverlay);
                            UiHelper.Show(bottomLeftText);

                            iconImage->AtkResNode.MultiplyBlue = 50;
                            iconImage->AtkResNode.MultiplyBlue_2 = 50;
                            iconImage->AtkResNode.MultiplyRed = 50;
                            iconImage->AtkResNode.MultiplyRed_2 = 50;
                            iconImage->AtkResNode.MultiplyGreen = 50;
                            iconImage->AtkResNode.MultiplyGreen_2 = 50;

                            IconTextOverride.Add((IntPtr)bottomLeftText);
                            IconRecastOverride.Add((IntPtr)cdOverlay);
                            IconComponentOverride.Add((IntPtr)icon->Component);

                            UiHelper.Hide(icon->Component->UldManager.NodeList[14]); // another image overlay :shrug:
                        }
                        else if(state == IconState.Running) {
                            UiHelper.Show(cdOverlay);
                            cdOverlay->PartId = (ushort)(81 - (float)(iconProgress.Current / iconProgress.Max) * 80);
                            UiHelper.SetText(bottomLeftText, ((int)iconProgress.Current).ToString());
                            UiHelper.Show(bottomLeftText);
                        }
                        else if(state == IconState.DoneRunning) {
                            TO_BUMP.Add(slotStruct->CommandId);

                            IconTextOverride.Remove((IntPtr)bottomLeftText);
                            IconRecastOverride.Remove((IntPtr)cdOverlay);
                            IconComponentOverride.Remove((IntPtr)icon->Component);
                            ResetColor(iconImage);

                            UiHelper.Hide(cdOverlay);
                            UiHelper.Show(dashOverlay);
                            UiHelper.Hide(bottomLeftText);
                        }
                        else if(state == IconState.Waiting) {
                            UiHelper.Show(dashOverlay);

                            var time = DateTime.Now;
                            int millis = time.Second * 1000 + time.Millisecond;
                            float percent = (float)(millis % MILLIS_LOOP) / MILLIS_LOOP;
                            dashOverlay->PartId = (ushort)(6 + percent * 7);
                        }
                    }
                }
            }

            foreach(var bump in TO_BUMP) { // necessary because there could be multiple of the same icon :/
                var current = ActionIdToState[bump];
                if(current == IconState.StartRunning) {
                    ActionIdToState[bump] = IconState.Running;
                }
                else if(current == IconState.DoneRunning) {
                    ActionIdToState[bump] = IconState.Waiting;
                }
            }
        }

        public void Cleanup(IntPtr node) {
            Cleanup((AtkComponentNode*)node);
        }
        public void Cleanup(AtkComponentNode* node) {
            var cdOverlay = (AtkImageNode*)node->Component->UldManager.NodeList[5];
            var dashOverlay = (AtkImageNode*)node->Component->UldManager.NodeList[9];
            var iconImage = (AtkImageNode*)node->Component->UldManager.NodeList[0];
            ResetColor(iconImage);
            UiHelper.Hide(cdOverlay);
            UiHelper.Hide(dashOverlay);
        }
        private void ResetColor(AtkImageNode* iconImage) {
            iconImage->AtkResNode.MultiplyBlue = 100;
            iconImage->AtkResNode.MultiplyBlue = 100;
            iconImage->AtkResNode.MultiplyBlue_2 = 100;
            iconImage->AtkResNode.MultiplyRed = 100;
            iconImage->AtkResNode.MultiplyRed_2 = 100;
            iconImage->AtkResNode.MultiplyGreen = 100;
            iconImage->AtkResNode.MultiplyGreen_2 = 100;
        }
    }
}
