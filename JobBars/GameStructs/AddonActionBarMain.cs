using System.Runtime.InteropServices;

namespace JobBars.GameStructs {

    [StructLayout(LayoutKind.Explicit, Size = 0x2B8)]
    public unsafe struct AddonActionBarMain {
        [FieldOffset(0x000)] public AddonActionBarBase ActionBarBase;
    }
}
