using System;
using System.Runtime.InteropServices;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace JobBars.Helper {
    public unsafe partial class UIHelper {
        private static DalamudPluginInterface PluginInterface;

        public delegate IntPtr GameAllocDelegate(ulong size, IntPtr unk, IntPtr allocator, IntPtr alignment);
        public static GameAllocDelegate GameAlloc { get; private set; }

        public delegate IntPtr GetGameAllocatorDelegate();
        public static GetGameAllocatorDelegate GetGameAllocator { get; private set; }

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

            GameAlloc = Marshal.GetDelegateForFunctionPointer<GameAllocDelegate>(scanner.ScanText("E8 ?? ?? ?? ?? 45 8D 67 23"));
            GetGameAllocator = Marshal.GetDelegateForFunctionPointer<GetGameAllocatorDelegate>(scanner.ScanText("E8 ?? ?? ?? ?? 8B 75 08"));
            PlaySe = Marshal.GetDelegateForFunctionPointer<PlaySeDelegate>(scanner.ScanText("E8 ?? ?? ?? ?? 4D 39 BE ?? ?? ?? ??"));

            var loadAssetsPtr = scanner.ScanText("E8 ?? ?? ?? ?? 48 8B 84 24 ?? ?? ?? ?? 41 B9 ?? ?? ?? ??");
            LoadTex = Marshal.GetDelegateForFunctionPointer<LoadTexDelegate>(loadAssetsPtr);

            var loadTexAllocPtr = scanner.ScanText("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8B 01 49 8B D8 48 8B FA 48 8B F1 FF 50 48");
            LoadTexAlloc = Marshal.GetDelegateForFunctionPointer<LoadTexAllocDelegate>(loadTexAllocPtr);

            var texUnallocPtr = scanner.ScanText("E8 ?? ?? ?? ?? C6 43 10 02");
            TexUnalloc = Marshal.GetDelegateForFunctionPointer<TexUnallocDelegate>(texUnallocPtr);

            Ready = true;
        }

        public static void Dispose() {
            PluginInterface = null;
        }

        public static IntPtr Alloc(ulong size) {
            if (GameAlloc == null || GetGameAllocator == null) return IntPtr.Zero;
            return GameAlloc(size, IntPtr.Zero, GetGameAllocator(), IntPtr.Zero);
        }

        public static IntPtr Alloc(int size) {
            return Alloc((ulong)size);
        }

        public static void PlaySeComplete() {
            PlaySe(78, 0, 0);
        }

        public static AtkUnitBase* Addon => (AtkUnitBase*)PluginInterface?.Framework.Gui.GetUiObjectByName("_ParameterWidget", 1);
    }
}