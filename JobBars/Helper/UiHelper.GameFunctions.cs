using System;
using System.Runtime.InteropServices;
using Dalamud.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace JobBars.Helper {
    public unsafe partial class UiHelper {

        public delegate IntPtr GameAllocDelegate(ulong size, IntPtr unk, IntPtr allocator, IntPtr alignment);
        public static GameAllocDelegate GameAlloc { get; private set; }

        public delegate IntPtr GetGameAllocatorDelegate();
        public static GetGameAllocatorDelegate GetGameAllocator { get; private set; }

        public delegate long PlaySeDelegate(int a1, long a2, long a3);
        public static PlaySeDelegate PlaySe { get; private set; }


        public static bool Ready { get; private set; } = false;

        public static void Setup(SigScanner scanner) {
            GameAlloc = Marshal.GetDelegateForFunctionPointer<GameAllocDelegate>(scanner.ScanText("E8 ?? ?? ?? ?? 45 8D 67 23"));
            GetGameAllocator = Marshal.GetDelegateForFunctionPointer<GetGameAllocatorDelegate>(scanner.ScanText("E8 ?? ?? ?? ?? 8B 75 08"));
            PlaySe = Marshal.GetDelegateForFunctionPointer<PlaySeDelegate>(scanner.ScanText("E8 ?? ?? ?? ?? 4D 39 BE ?? ?? ?? ??"));
            Ready = true;
        }

        public static IntPtr Alloc(ulong size) {
            if (GameAlloc == null || GetGameAllocator == null) return IntPtr.Zero;
            return GameAlloc(size, IntPtr.Zero, GetGameAllocator(), IntPtr.Zero);
        }

        public static IntPtr Alloc(int size) {
            return Alloc((ulong)size);
        }
    }
}