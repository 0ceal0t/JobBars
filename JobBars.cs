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
using FFXIVClientStructs;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Runtime.InteropServices;
using JobBars.UI;

#pragma warning disable CS0659
namespace JobBars {
    public unsafe class JobBars : IDalamudPlugin {
        public string Name => "JobBars";
        public DalamudPluginInterface PluginInterface { get; private set; }
        public string AssemblyLocation { get; private set; } = Assembly.GetExecutingAssembly().Location;

        public UIBuilder _UI;

        public void Initialize(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;
            UiHelper.Setup(pluginInterface.TargetModuleScanner);

            _UI = new UIBuilder(pluginInterface);

            PluginInterface.UiBuilder.OnBuildUi += BuildUI;
            PluginInterface.Framework.OnUpdateEvent += FrameworkOnUpdate;
            SetupCommands();
        }

        public void Dispose() {
            PluginInterface.UiBuilder.OnBuildUi -= BuildUI;
            PluginInterface.Framework.OnUpdateEvent -= FrameworkOnUpdate;

            _UI.Dispose();

            RemoveCommands();
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

        private void FrameworkOnUpdate(Framework framework) {
            try {
                if(FrameCount == 0) { // idk lmao
                    _UI.SetupTex();
                    FrameCount++;
                }
                else if(FrameCount == 1) {
                    _UI.Init();
                    FrameCount++;
                }
            }
            catch (Exception ex) {
                PluginLog.Log(ex.ToString());
            }
        }
    }
}
