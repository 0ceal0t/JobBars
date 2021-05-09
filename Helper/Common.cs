using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Dalamud.Game;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;

namespace JobBars.Helper {
    internal unsafe class Common {
        public static DalamudPluginInterface PluginInterface { get; private set; }

        public static SigScanner Scanner => PluginInterface.TargetModuleScanner;

        public Common(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;
        }

        public static HookWrapper<T> Hook<T>(string signature, T detour, bool enable = true) where T : Delegate {
            var addr = Common.Scanner.ScanText(signature);
            var h = new Hook<T>(addr, detour);
            var wh = new HookWrapper<T>(h);
            if (enable) wh.Enable();
            HookList.Add(wh);
            return wh;
        }

        public static HookWrapper<T> Hook<T>(IntPtr addr, T detour, bool enable = true) where T : Delegate {
            var h = new Hook<T>(addr, detour);
            var wh = new HookWrapper<T>(h);
            if (enable) wh.Enable();
            HookList.Add(wh);
            return wh;
        }

        public static List<IHookWrapper> HookList = new();

    }
}
