using System;
using System.Runtime.InteropServices;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace JobBars.Helper {
    public unsafe partial class UIHelper {
        public delegate long PlaySoundEffectDelegate(int a1, long a2, long a3);
        public static PlaySoundEffectDelegate PlaySoundEffect { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe delegate IntPtr TextureLoadPathDelegate(AtkTexture* texture, string path, uint a3);
        public static TextureLoadPathDelegate TextureLoadPath { get; private set; }

        private static IntPtr TargetAddress;

        public static bool Ready { get; private set; } = false;

        public static void Setup() {
            PlaySoundEffect = Marshal.GetDelegateForFunctionPointer<PlaySoundEffectDelegate>(JobBars.SigScanner.ScanText("E8 ?? ?? ?? ?? 4D 39 BE ?? ?? ?? ??"));
            TextureLoadPath = Marshal.GetDelegateForFunctionPointer<TextureLoadPathDelegate>(JobBars.SigScanner.ScanText("E8 ?? ?? ?? ?? 4C 8B 6C 24 ?? 4C 8B 5C 24 ??"));
            TargetAddress = JobBars.SigScanner.GetStaticAddressFromSig("48 8B 05 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? FF 50 ?? 48 85 DB", 3);
            SetupSheets();

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

        public static void PlaySeComplete() {
            PlaySoundEffect(78, 0, 0);
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
            var actorAddress = Marshal.ReadIntPtr(TargetAddress + 0xF0);
            if (actorAddress == IntPtr.Zero) return null;

            return JobBars.Objects.CreateObjectReference(actorAddress);
        }

        public static AtkUnitBase* ChatLogAddon => AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonByName("ChatLog");
        public static AddonPartyList* PartyListAddon => (AddonPartyList*)AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonByName("_PartyList");
    }
}