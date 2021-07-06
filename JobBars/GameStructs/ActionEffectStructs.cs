using Dalamud.Game.ClientState.Structs;
using System.Runtime.InteropServices;

namespace JobBars.GameStructs {
    public enum ActionEffectType : byte {
        Nothing = 0,
        Miss = 1,
        FullResist = 2,
        Damage = 3,
        Heal = 4,
        BlockedDamage = 5,
        ParriedDamage = 6,
        Invulnerable = 7,
        NoEffectText = 8,
        Unknown_0 = 9,
        MpLoss = 10,
        MpGain = 11,
        TpLoss = 12,
        TpGain = 13,
        Gp_Or_Status = 14,
        ApplyStatusEffectTarget = 15,
        ApplyStatusEffectSource = 16,
        StatusNoEffect = 20,
        StartActionCombo = 27,
        ComboSucceed = 28,
        Knockback = 33,
        Mount = 40,
        VFX = 59,
    };

    public struct EffectEntry {
        public ActionEffectType type;
        public byte param0;
        public byte param1;
        public byte param2;
        public byte mult;
        public byte flags;
        public ushort value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct StatusItem {
        public uint Unk1;
        public uint CurrentHP;
        public uint MapHP;
        public uint CurrentMP;
        public uint MaxMP;
        public uint Unk_Shields;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)] public StatusEffect[] Status;
    }
}