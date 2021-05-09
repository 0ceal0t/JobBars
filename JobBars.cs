using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Newtonsoft.Json.Linq;
using JobBars.Helper;
using Dalamud.Game.Internal;
using JobBars.UI;
using FFXIVClientInterface;
using JobBars.GameStructs;
using FFXIVClientInterface.Client.UI.Misc;
using Dalamud.Hooking;
using JobBars.Gauges;

#pragma warning disable CS0659
namespace JobBars {
    public unsafe class JobBars : IDalamudPlugin {
        public string Name => "JobBars";
        public DalamudPluginInterface PluginInterface { get; private set; }
        public string AssemblyLocation { get; private set; } = Assembly.GetExecutingAssembly().Location;

        public UIBuilder _UI;
        public ActionGaugeManager _GManager;
        public static ClientInterface Client;

        private delegate void ReceiveActionEffectDelegate(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private Hook<ReceiveActionEffectDelegate> receiveActionEffectHook;
        private delegate byte AddStatusDelegate(IntPtr statusEffectList, uint slot, UInt16 statusId, float duration, UInt16 a5, uint sourceActorId, byte newStatus);
        private Hook<AddStatusDelegate> addStatusHook;
        private delegate IntPtr RemoveStatusDelegate(IntPtr a1, uint statusId, IntPtr a3, uint sourceActorId, byte a5, byte a6);
        private Hook<RemoveStatusDelegate> removeStatusHook;

        private bool _Ready => (PluginInterface.ClientState != null && PluginInterface.ClientState.LocalPlayer != null);

        public void Initialize(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;
            UiHelper.Setup(pluginInterface.TargetModuleScanner);
            Client = new ClientInterface(pluginInterface.TargetModuleScanner, pluginInterface.Data);
            _UI = new UIBuilder(pluginInterface);

            IntPtr receiveActionEffectFuncPtr = PluginInterface.TargetModuleScanner.ScanText("4C 89 44 24 18 53 56 57 41 54 41 57 48 81 EC ?? 00 00 00 8B F9");
            receiveActionEffectHook = new Hook<ReceiveActionEffectDelegate>(receiveActionEffectFuncPtr, (ReceiveActionEffectDelegate)ReceiveActionEffect);
            receiveActionEffectHook.Enable();
            IntPtr addStatusFuncPtr = PluginInterface.TargetModuleScanner.ScanText("40 53 55 56 48 83 EC 60 40 32 ED 41 0F B7 F0 48 8B D9");
            addStatusHook = new Hook<AddStatusDelegate>(addStatusFuncPtr, (AddStatusDelegate)AddStatus);
            addStatusHook.Enable();
            IntPtr removeStatusFuncPtr = PluginInterface.TargetModuleScanner.ScanText("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 30 80 7C 24 ?? ?? 41 8B D9 8B FA");
            removeStatusHook = new Hook<RemoveStatusDelegate>(removeStatusFuncPtr, (RemoveStatusDelegate)RemoveStatus);
            removeStatusHook.Enable();

            PluginInterface.UiBuilder.OnBuildUi += BuildUI;
            PluginInterface.Framework.OnUpdateEvent += FrameworkOnUpdate;
            SetupCommands();
        }

        public void Dispose() {
            receiveActionEffectHook?.Disable();
            receiveActionEffectHook?.Dispose();

            addStatusHook?.Disable();
            addStatusHook?.Dispose();

            removeStatusHook?.Disable();
            removeStatusHook?.Dispose();

            PluginInterface.UiBuilder.OnBuildUi -= BuildUI;
            PluginInterface.Framework.OnUpdateEvent -= FrameworkOnUpdate;
            Client.Dispose();
            _UI.Dispose();

            RemoveCommands();
        }

        private byte AddStatus(IntPtr statusEffectList, uint slot, UInt16 statusId, float duration, UInt16 a5, uint sourceActorId, byte newStatus) {
            if(_Ready && sourceActorId == PluginInterface.ClientState.LocalPlayer.ActorId) {
                PluginLog.Log("ADD STATUS");
            }
            return addStatusHook.Original(statusEffectList, slot, statusId, duration, a5, sourceActorId, newStatus);
        }

        private IntPtr RemoveStatus(IntPtr a1, uint statusId, IntPtr a3, uint sourceActorId, byte a5, byte a6) {
            if (_Ready && sourceActorId == PluginInterface.ClientState.LocalPlayer.ActorId) {
                PluginLog.Log("REMOVE STATUS");
            }
            return removeStatusHook.Original(a1, statusId, a3, sourceActorId, a5, a6);
        }

        private void ReceiveActionEffect(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail) {
            uint id = *((uint*)effectHeader.ToPointer() + 0x2);
            if(_Ready && IntPtr.Equals(sourceCharacter, PluginInterface.ClientState.LocalPlayer.Address)) {
                PluginLog.Log("ACTION");
            }
            receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
        }

        public void SetupCommands() {
            PluginInterface.CommandManager.AddHandler("/jobbars", new Dalamud.Game.Command.CommandInfo(OnConfigCommandHandler) {
                HelpMessage = $"Open config window for {this.Name}",
                ShowInHelp = true
            });
        }

        public void OnConfigCommandHandler(object command, object args) {
        }

        public void RemoveCommands() {
            PluginInterface.CommandManager.RemoveHandler("/jobbars");
        }

        private bool Visible = true;
        private int FrameCount = 0;

        private void BuildUI() {
            ImGui.SetNextWindowSize(new Vector2(500, 500));
            if(ImGui.Begin("Settings", ref Visible)) {
                ImGui.End();
            }
        }

        private readonly string[] allActionBars = {
            "_ActionBar"/*,
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
            "_ActionDoubleCrossR",*/
        };

        private void FrameworkOnUpdate(Framework framework) {
            if (!_Ready) return;
            if(FrameCount == 0) { // idk lmao
                _UI.SetupTex();
                FrameCount++;
                return;

                // DO STUFF HERE

                /*
                    * icon component
                    *  frame res
                    *      combo border res->image (6-12)
                    *      recast (part 1-80)
                    * 
                    */

                /*var a = Client.ActionManager;
                var hotbarModule = Client.UiModule.RaptureHotbarModule;
                for (var abIndex = 0; abIndex < allActionBars.Length; abIndex++) {
                    var actionBar = allActionBars[abIndex];
                    var ab = (AddonActionBarBase*)PluginInterface.Framework.Gui.GetUiObjectByName(actionBar, 1);
                    if (ab == null || ab->ActionBarSlotsAction == null) continue;
                    var bar = abIndex > 10 ? null : hotbarModule.GetBar(abIndex, HotBarType.Normal);
                    for (var i = 0; i < ab->HotbarSlotCount; i++) {
                        var slot = ab->ActionBarSlotsAction[i];
                        var slotStruct = hotbarModule.GetBarSlot(bar, i);
                        if ((slot.PopUpHelpTextPtr != null) && slot.Icon != null) {
                            var adjustedActionId = Client.ActionManager.GetAdjustedActionId(slotStruct->CommandId);
                            var recastGroup = (int)Client.ActionManager.GetRecastGroup((byte)slotStruct->CommandType, adjustedActionId) + 1;
                            PluginLog.Log($"{slot.ActionId} {adjustedActionId} {recastGroup} {slotStruct->CommandType} {slotStruct->CommandId}");
                            if(recastGroup != 0) {
                                var recastTimer = Client.ActionManager.GetGroupRecastTime(recastGroup);
                                if (adjustedActionId == 16554) {
                                    PluginLog.Log($"{recastTimer->IsActive} {recastTimer->Elapsed} {recastTimer->Total}");
                                    recastTimer->IsActive = 1;
                                    recastTimer->Elapsed = 10;
                                    recastTimer->Total = 30;
                                }
                            }
                        }
                    }
                }*/
            }
            else if(FrameCount == 1) {
                _UI.Init();
                _GManager = new ActionGaugeManager();
                FrameCount++;
                return;
            }

            _GManager.SetJob(PluginInterface.ClientState.LocalPlayer.ClassJob);
        }
    }
}
