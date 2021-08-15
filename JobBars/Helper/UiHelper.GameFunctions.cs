using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace JobBars.Helper {
    public unsafe partial class UIHelper {
        private static DalamudPluginInterface PluginInterface;

        public delegate long PlaySeDelegate(int a1, long a2, long a3);
        public static PlaySeDelegate PlaySe { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr LoadTexDelegate(IntPtr uldManager, IntPtr allocator, IntPtr texPaths, IntPtr assetMapping, uint assetNumber, byte a6);
        public static LoadTexDelegate LoadTex { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr LoadTexAllocDelegate(IntPtr allocator, Int64 size, UInt64 a3);
        public static LoadTexAllocDelegate LoadTexAlloc { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr TexUnallocDelegate(IntPtr tex);
        public static TexUnallocDelegate TexUnalloc { get; private set; }

        public static bool Ready { get; private set; } = false;

        public static void Setup(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;
            var scanner = pluginInterface.TargetModuleScanner;

            PlaySe = Marshal.GetDelegateForFunctionPointer<PlaySeDelegate>(scanner.ScanText("E8 ?? ?? ?? ?? 4D 39 BE ?? ?? ?? ??"));

            var loadAssetsPtr = scanner.ScanText("E8 ?? ?? ?? ?? 48 8B 84 24 ?? ?? ?? ?? 41 B9 ?? ?? ?? ??");
            LoadTex = Marshal.GetDelegateForFunctionPointer<LoadTexDelegate>(loadAssetsPtr);

            var loadTexAllocPtr = scanner.ScanText("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8B 01 49 8B D8 48 8B FA 48 8B F1 FF 50 48");
            LoadTexAlloc = Marshal.GetDelegateForFunctionPointer<LoadTexAllocDelegate>(loadTexAllocPtr);

            var texUnallocPtr = scanner.ScanText("E8 ?? ?? ?? ?? C6 43 10 02");
            TexUnalloc = Marshal.GetDelegateForFunctionPointer<TexUnallocDelegate>(texUnallocPtr);

            Ready = true;
        }

        public static T* Alloc<T>() where T : unmanaged {
            return (T*)Alloc((ulong)sizeof(T));
        }

        public static void* Alloc(ulong size) {
            return IMemorySpace.GetUISpace()->Malloc(size, 8);
        }

        public static T* CleanAlloc<T>() where T : unmanaged {
            return (T*)CleanAlloc((ulong)sizeof(T));
        }

        public static void* CleanAlloc(ulong size) {
            var alloc = Alloc(size);
            IMemorySpace.Memset(alloc, 0, size);
            return alloc;
        }

        public static T* CloneNode<T>(T* original) where T : unmanaged {
            var allocation = CleanAlloc<T>();

            var bytes = new byte[sizeof(T)];
            Marshal.Copy(new IntPtr(original), bytes, 0, bytes.Length);
            Marshal.Copy(bytes, 0, new IntPtr(allocation), bytes.Length);

            var newNode = (AtkResNode*)allocation;
            newNode->ParentNode = null;
            newNode->ChildNode = null;
            newNode->ChildCount = 0;
            newNode->PrevSiblingNode = null;
            newNode->NextSiblingNode = null;
            return allocation;
        }

        public static void PlaySeComplete() {
            PlaySe(78, 0, 0);
        }

        public static void Dispose() {
            PluginInterface = null;
        }

        public static bool GetRecastActive(uint actionId, out float timeElapsed, ActionType actionType = ActionType.Spell) {
            var actionManager = ActionManager.Instance();
            var adjustedId = actionManager->GetAdjustedActionId(actionId);
            timeElapsed = actionManager->GetRecastTimeElapsed(actionType, adjustedId);
            return timeElapsed > 0;
        }

        public static AtkUnitBase* ParameterAddon => (AtkUnitBase*)PluginInterface?.Framework.Gui.GetUiObjectByName("_ParameterWidget", 1);
    }
}