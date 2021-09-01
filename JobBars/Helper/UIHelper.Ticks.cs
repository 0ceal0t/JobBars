using System;

namespace JobBars.Helper {
    public unsafe partial class UIHelper {
        private static bool MpTickActive = false;
        private static DateTime MpTime;
        private static uint LastMp = 0;

        private static bool ActorTickActive = false;
        private static DateTime ActorTickTime;

        private static uint DoTTickObjectId = 0;
        private static DateTime DoTTickTime;

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
                ActorTickTime = DateTime.Now;
            }
        }

        public static void ResetTicks() {
            MpTickActive = false;
            ActorTickActive = false;
        }

        public static void UpdateDoTTick(uint objectId) {
            if (objectId == 0) return;
            if (PreviousEnemyTargetId == 0) return;
            if (objectId == PreviousEnemyTargetId && DoTTickObjectId != PreviousEnemyTargetId) {
                DoTTickObjectId = PreviousEnemyTargetId;
                DoTTickTime = DateTime.Now;
            }
        }

        public static float GetDoTTick() {
            if (PreviousEnemyTargetId == 0) return 0;
            if (DoTTickObjectId != PreviousEnemyTargetId) return 0; // has not been updated yet
            return GetTickFrom(DoTTickTime);
        }

        public static float GetActorTick() {
            if (!ActorTickActive) return 0;
            return GetTickFrom(ActorTickTime);
        }

        public static float GetMpTick() {
            if (!MpTickActive) return 0;
            if (LastMp == 10000) return 0; // already max
            return GetTickFrom(MpTime);
        }

        private static float GetTickFrom(DateTime time) {
            var currentTime = DateTime.Now;
            var diff = (currentTime - time).TotalSeconds;
            return (float)(diff % 3.0f / 3.0f);
        }
    }
}
