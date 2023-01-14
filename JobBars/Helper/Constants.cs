using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Helper {
    public static class Constants {
        public static readonly string ReceiveActionEffectSig = "4C 89 44 24 ?? 55 56 41 54 41 55 41 56";
        public static readonly string ActorControlSig = "E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64";
        public static readonly string IconDimmedSig = "E8 ?? ?? ?? ?? 49 8D 4D 10 FF C6";

        public static readonly string PlaySoundSig = "E8 ?? ?? ?? ?? 4D 39 BE ?? ?? ?? ??";

        public static readonly int PreviousTargetOffset = 0xF0;

        public static readonly int ActorControlSelfId = 1539;
        public static readonly int ActorControlOtherId = 1540;
        public static readonly int WipeArg1 = 0x4000000F;
    }
}
