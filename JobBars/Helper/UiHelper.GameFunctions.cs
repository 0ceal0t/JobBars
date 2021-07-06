using System;
using System.Runtime.InteropServices;
using Dalamud.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace JobBars.Helper
{
    public unsafe partial class UiHelper {

        private delegate void AtkTextNodeSetText(AtkTextNode* textNode, void* a2);
        private static AtkTextNodeSetText _atkTextNodeSetText;

        public delegate IntPtr GameAlloc(ulong size, IntPtr unk, IntPtr allocator, IntPtr alignment);
        public static GameAlloc _gameAlloc;

        public delegate IntPtr GetGameAllocator();
        public static GetGameAllocator _getGameAllocator;

        public delegate long PlaySeDelegate(int a1, long a2, long a3);
        public static PlaySeDelegate _playSe;
        
        
        public static bool Ready = false;

        public static void Setup(SigScanner scanner) {
            _atkTextNodeSetText = Marshal.GetDelegateForFunctionPointer<AtkTextNodeSetText>(scanner.ScanText("E8 ?? ?? ?? ?? 49 8B FC"));
            _gameAlloc = Marshal.GetDelegateForFunctionPointer<GameAlloc>(scanner.ScanText("E8 ?? ?? ?? ?? 45 8D 67 23"));
            _getGameAllocator = Marshal.GetDelegateForFunctionPointer<GetGameAllocator>(scanner.ScanText("E8 ?? ?? ?? ?? 8B 75 08"));
            _playSe  = Marshal.GetDelegateForFunctionPointer<PlaySeDelegate>(scanner.ScanText("E8 ?? ?? ?? ?? 4D 39 BE ?? ?? ?? ??"));
            Ready = true;
        }

        public static IntPtr Alloc(ulong size) {
            if (_gameAlloc == null || _getGameAllocator == null) return IntPtr.Zero;
            return _gameAlloc(size, IntPtr.Zero, _getGameAllocator(), IntPtr.Zero);
        }

        public static IntPtr Alloc(int size) {
            if (size <= 0) throw new ArgumentException("Allocation size must be positive.");
            return Alloc((ulong) size);
        }

    }
}