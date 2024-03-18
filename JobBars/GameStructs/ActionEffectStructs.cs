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
        ApplyStatusTarget = 14,
        ApplyStatusSource = 15,
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
}