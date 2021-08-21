using System;
using Dalamud.Plugin;
using JobBars.Data;
using ImGuiNET;
using Dalamud.Interface;
using JobBars.UI;
using JobBars.Helper;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Dalamud.Logging;

namespace JobBars.Utilities {
    public unsafe partial class UtilityManager {
        public static UtilityManager Manager { get; private set; }
        public static void Dispose() { Manager = null; }

        private readonly DalamudPluginInterface PluginInterface;

        public UtilityManager(DalamudPluginInterface pluginInterface) {
            Manager = this;
            PluginInterface = pluginInterface;
        }

        public void Tick() {
            // TODO: check if enabled

            UIBuilder.Builder.SetInnerPercent(0);
            UIBuilder.Builder.SetOuterPercent(0);

            var viewport = ImGuiHelpers.MainViewport;
            var pos = ImGui.GetMousePos() - viewport.Pos;
            var atkStage = AtkStage.GetSingleton();
            var dragging = *((byte*)new IntPtr(atkStage) + 0x137);
            if (pos.X > 0 && pos.Y > 0 && pos.X < viewport.Size.X && pos.Y < viewport.Size.Y && dragging == 1) {
                UIBuilder.Builder.SetCursorPosition(pos);
            }

            // TODO: casting time?

            /*
             * 
             */

            var recast = UIHelper.GetRecastActiveAndTotal((uint)ActionIds.GoringBlade, out var timeElapsed, out var total);
            UIBuilder.Builder.SetOuterPercent(timeElapsed / total);
        }
    }
}