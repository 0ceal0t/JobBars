﻿using System.Runtime.InteropServices;
using FFXIVClientStructs.Attributes;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace JobBars.GameStructs {

    [StructLayout(LayoutKind.Explicit, Size = 0x348)]
    [Addon("ActionDetail")]
    public struct AddonActionDetail {
        [FieldOffset(0x000)] public AtkUnitBase AtkUnitBase;

    }
}
