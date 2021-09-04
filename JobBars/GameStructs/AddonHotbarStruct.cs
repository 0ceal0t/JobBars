using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.GameStructs {
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct AddonHotbarStruct {
        [FieldOffset(0x000)] public bool Locked;
        [FieldOffset(0x03C)] public AddonHotbars Hotbars;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct AddonHotbars {
        [FieldOffset(0x400 * 0)] public HotbarStruct HotBar_1;
        [FieldOffset(0x400 * 1)] public HotbarStruct HotBar_2;
        [FieldOffset(0x400 * 2)] public HotbarStruct HotBar_3;
        [FieldOffset(0x400 * 3)] public HotbarStruct HotBar_4;
        [FieldOffset(0x400 * 4)] public HotbarStruct HotBar_5;
        [FieldOffset(0x400 * 5)] public HotbarStruct HotBar_6;
        [FieldOffset(0x400 * 6)] public HotbarStruct HotBar_7;
        [FieldOffset(0x400 * 7)] public HotbarStruct HotBar_8;
        [FieldOffset(0x400 * 8)] public HotbarStruct HotBar_9;
        [FieldOffset(0x400 * 9)] public HotbarStruct HotBar_10;

        public HotbarStruct this[int i] => i switch {
            0 => HotBar_1,
            1 => HotBar_2,
            2 => HotBar_3,
            3 => HotBar_4,
            4 => HotBar_5,
            5 => HotBar_6,
            6 => HotBar_7,
            7 => HotBar_8,
            8 => HotBar_9,
            9 => HotBar_10,
            _ => throw new IndexOutOfRangeException("Index should be between 0 and 9")
        };
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x400)]
    public unsafe struct HotbarStruct {
        [FieldOffset(0x40 * 0 )] public HotbarSlotStruct Slot_1;
        [FieldOffset(0x40 * 1 )] public HotbarSlotStruct Slot_2;
        [FieldOffset(0x40 * 2 )] public HotbarSlotStruct Slot_3;
        [FieldOffset(0x40 * 3 )] public HotbarSlotStruct Slot_4;
        [FieldOffset(0x40 * 4 )] public HotbarSlotStruct Slot_5;
        [FieldOffset(0x40 * 5 )] public HotbarSlotStruct Slot_6;
        [FieldOffset(0x40 * 6 )] public HotbarSlotStruct Slot_7;
        [FieldOffset(0x40 * 7 )] public HotbarSlotStruct Slot_8;
        [FieldOffset(0x40 * 8 )] public HotbarSlotStruct Slot_9;
        [FieldOffset(0x40 * 9 )] public HotbarSlotStruct Slot_10;
        [FieldOffset(0x40 * 10)] public HotbarSlotStruct Slot_11;
        [FieldOffset(0x40 * 11)] public HotbarSlotStruct Slot_12;

        public HotbarSlotStruct this[int i] => i switch {
            0  => Slot_1,
            1  => Slot_2,
            2  => Slot_3,
            3  => Slot_4,
            4  => Slot_5,
            5  => Slot_6,
            6  => Slot_7,
            7  => Slot_8,
            8  => Slot_9,
            9  => Slot_10,
            10 => Slot_11,
            11 => Slot_12,
            _ => throw new IndexOutOfRangeException("Index should be between 0 and 11")
        };
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x40)]
    public unsafe struct HotbarSlotStruct {
        [FieldOffset(0x0C)] public uint ActionId;
        [FieldOffset(0x38)] public bool YellowBorder;
    }
}
