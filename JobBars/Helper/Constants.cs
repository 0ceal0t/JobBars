namespace JobBars.Helper {
    public static class Constants {
        public static readonly string ReceiveActionEffectSig = "40 55 53 57 41 54 41 55 41 56 41 57 48 8D AC 24 ?? ?? ?? ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 70";
        public static readonly string ActorControlSig = "E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64";
        public static readonly string IconDimmedSig = "E8 ?? ?? ?? ?? 49 8D 4D 10 FF C6";

        public static readonly string PlaySoundSig = "E8 ?? ?? ?? ?? 4D 39 BE ?? ?? ?? ??";

        public static readonly int PreviousTargetOffset = 0xF0;

        public static readonly int ActorControlSelfId = 1539;
        public static readonly int ActorControlOtherId = 1540;
        public static readonly int WipeArg1 = 0x4000000F;
    }
}
