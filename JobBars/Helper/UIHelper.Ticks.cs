using System;

namespace JobBars.Helper {
    public unsafe partial class UIHelper {
        private static bool MpTickActive = false;
        private static DateTime MpTime;
        private static uint LastMp = 0;

        private static bool ActorTickActive = false;
        private static DateTime ActorTick;

        public static void UpdateMp(uint currentMp) {
            if (currentMp != 10000 && currentMp > LastMp && !MpTickActive) {
                MpTickActive = true;
                MpTime = DateTime.Now;
            }
            LastMp = currentMp;
        }

        public static void UpdateActorTick() {
            if (!ActorTickActive) {
                ActorTickActive = true;
                ActorTick = DateTime.Now;
            }
        }

        public static void ResetTicks() {
            MpTickActive = false;
            ActorTickActive = false;
        }

        public static float GetActorTick() {
            if (!ActorTickActive) return 0;
            var currentTime = DateTime.Now;
            var diff = (currentTime - ActorTick).TotalSeconds;
            return (float)(diff % 3.0f / 3.0f);
        }

        public static float GetMpTick() {
            if (!MpTickActive) return 0;
            if (LastMp == 10000) return 0; // already max
            var currentTime = DateTime.Now;
            var diff = (currentTime - MpTime).TotalSeconds;
            return (float)(diff % 3.0f / 3.0f);
        }
    }
}
