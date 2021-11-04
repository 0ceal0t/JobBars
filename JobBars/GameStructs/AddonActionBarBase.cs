using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace JobBars.GameStructs {
    [StructLayout(LayoutKind.Explicit, Size = 0x702)]
    public unsafe struct AddonActionBarCross {
        [FieldOffset(0x000)] public AddonActionBarBase ActionBarBase;
        [FieldOffset(0x190)] public int CrossBarSet;
        [FieldOffset(0x6E8)] public uint LeftCompactFlags;
        [FieldOffset(0x6EC)] public uint RightCompactFlags;
        [FieldOffset(0x701)] public byte LeftHeld;
        [FieldOffset(0x702)] public byte RightHeld;

        public bool GetCompact(out int set, out bool left) {
            set = -1;
            left = false;

            if (GetCompactLeft(out var setL, out var leftL)) {
                set = setL;
                left = leftL;
                return true;
            }

            if (GetCompactRight(out var setR, out var leftR)) {
                set = setR;
                left = leftR;
                return true;
            }

            return false;
        }
        public bool GetCompactLeft(out int set, out bool left) => GetSelectedCompact(LeftCompactFlags, out set, out left);
        public bool GetCompactRight(out int set, out bool left) => GetSelectedCompact(RightCompactFlags, out set, out left);

        public static bool GetSelectedCompact(uint flag, out int set, out bool left) { // 1 = 0/left, 2 = 0/right, 3 = 1/left, etc.
            set = 0;
            left = false;

            if (flag == 0) return false;

            var leftOver = flag % 2;
            left = leftOver == 1;
            set = (int)((flag + leftOver) / 2);

            return true;
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x330)]
    public unsafe struct AddonActionBarDoubleCross {
        [FieldOffset(0x000)] public AddonActionBarBase ActionBarBase;
        [FieldOffset(0x2E1)] public byte LargeDoubleCross;
        [FieldOffset(0x2E8)] public int HotbarIndex; // set 0 = 10, set 7 = 17, etc.
        [FieldOffset(0x2EC)] public byte Left;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x2B8)]
    public unsafe struct AddonActionBarMain {
        [FieldOffset(0x000)] public AddonActionBarBase ActionBarBase;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x248)]
    public unsafe struct AddonActionBarBase {
        [FieldOffset(0x000)] public AtkUnitBase AtkUnitBase;
        [FieldOffset(0x220)] public ActionBarSlotAction* ActionBarSlotsAction;
        [FieldOffset(0x228)] public void* UnknownPtr228; // Field of 0s
        [FieldOffset(0x230)] public void* UnknownPtr230; // Points to same location as +0x228 ??
        [FieldOffset(0x238)] public int UnknownInt238;
        [FieldOffset(0x23C)] public byte HotbarID;
        [FieldOffset(0x23D)] public byte HotbarIDOther;
        [FieldOffset(0x23E)] public byte HotbarSlotCount;
        [FieldOffset(0x23F)] public int UnknownInt23F;
        [FieldOffset(0x243)] public int UnknownInt243; // Flags of some kind
    }

    [StructLayout(LayoutKind.Explicit, Size = 0xC8)]
    public unsafe struct ActionBarSlotAction {
        [FieldOffset(0x04)] public int ActionId;       // Not cleared when slot is emptied
        [FieldOffset(0x18)] public void* UnknownPtr;   // Points 34 bytes ahead ??
        [FieldOffset(0x90)] public AtkComponentNode* Icon;
        [FieldOffset(0x98)] public AtkTextNode* ControlHintTextNode;
        [FieldOffset(0xA0)] public AtkResNode* IconFrame;
        [FieldOffset(0xA8)] public AtkImageNode* ChargeIcon;
        [FieldOffset(0xB0)] public AtkResNode* RecastOverlayContainer;
        [FieldOffset(0xB8)] public byte* PopUpHelpTextPtr; // Null when slot is empty
    }

}
