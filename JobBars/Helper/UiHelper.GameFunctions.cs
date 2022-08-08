using System;
using System.Runtime.InteropServices;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.GameStructs;

namespace JobBars.Helper {
    public unsafe partial class UIHelper {
        public delegate long PlaySoundEffectDelegate(int a1, long a2, long a3,  int a4);
        public static PlaySoundEffectDelegate PlayGameSoundEffect { get; private set; }

        private static Crc32 Crc32;

        public static bool Ready { get; private set; } = false;

        public static void Setup() {
            PlayGameSoundEffect = Marshal.GetDelegateForFunctionPointer<PlaySoundEffectDelegate>(JobBars.SigScanner.ScanText("E8 ?? ?? ?? ?? 4D 39 BE ?? ?? ?? ??"));
            SetupSheets();

            Crc32 = new Crc32();

            Ready = true;
        }

        public static T* Alloc<T>() where T : unmanaged {
            return (T*)Alloc((ulong)sizeof(T));
        }

        public static void* Alloc(ulong size) {
            return IMemorySpace.GetUISpace()->Malloc(size, 8);
        }

        public static T* CleanAlloc<T>() where T : unmanaged {
            return (T*)CleanAlloc((ulong)sizeof(T));
        }

        public static void* CleanAlloc(ulong size) {
            var alloc = Alloc(size);
            IMemorySpace.Memset(alloc, 0, size);
            return alloc;
        }

        public static T* CloneNode<T>(T* original) where T : unmanaged {
            var allocation = CleanAlloc<T>();

            var bytes = new byte[sizeof(T)];
            Marshal.Copy(new IntPtr(original), bytes, 0, bytes.Length);
            Marshal.Copy(bytes, 0, new IntPtr(allocation), bytes.Length);

            var newNode = (AtkResNode*)allocation;
            newNode->ParentNode = null;
            newNode->ChildNode = null;
            newNode->ChildCount = 0;
            newNode->PrevSiblingNode = null;
            newNode->NextSiblingNode = null;
            return allocation;
        }

        public static void PlaySoundEffect(int soundEffect) {
            if (soundEffect == 19 || soundEffect == 21) return;
            if (soundEffect <= 0) return;
            if (WatchingCutscene) return;
            PlayGameSoundEffect(soundEffect, 0, 0, 0);
        }

        public static bool GetCurrentCast(out float currentTime, out float totalTime) {
            currentTime = JobBars.ClientState.LocalPlayer.CurrentCastTime;
            totalTime = JobBars.ClientState.LocalPlayer.TotalCastTime;
            return JobBars.ClientState.LocalPlayer.IsCasting;
        }

        public static bool GetRecastActive(uint actionId, out float timeElapsed, ActionType actionType = ActionType.Spell) {
            var actionManager = ActionManager.Instance();
            var adjustedId = actionManager->GetAdjustedActionId(actionId);
            timeElapsed = actionManager->GetRecastTimeElapsed(actionType, adjustedId);
            return timeElapsed > 0;
        }

        public static uint GetAdjustedAction(uint actionId) {
            var actionManager = ActionManager.Instance();
            return actionManager->GetAdjustedActionId(actionId);
        }

        public static bool GetRecastActiveAndTotal(uint actionId, out float timeElapsed, out float timeTotal, ActionType actionType = ActionType.Spell) {
            var actionManager = ActionManager.Instance();
            var adjustedId = actionManager->GetAdjustedActionId(actionId);
            timeElapsed = actionManager->GetRecastTimeElapsed(actionType, adjustedId);
            timeTotal = actionManager->GetRecastTime(actionType, adjustedId);
            return timeElapsed > 0;
        }

        public static GameObject PreviousEnemyTarget => GetPreviousEnemyTarget();

        private static GameObject GetPreviousEnemyTarget() {
            var actorAddress = Marshal.ReadIntPtr(new IntPtr(TargetSystem.Instance()) + 0xF0);
            if (actorAddress == IntPtr.Zero) return null;

            return JobBars.Objects.CreateObjectReference(actorAddress);
        }
    }
}