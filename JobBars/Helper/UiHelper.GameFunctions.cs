using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using System;
using System.Runtime.InteropServices;

namespace JobBars.Helper {
    public unsafe partial class UiHelper {
        public delegate long PlaySoundEffectDelegate( int a1, long a2, long a3, int a4 );
        public static PlaySoundEffectDelegate PlayGameSoundEffect { get; private set; }

        public static bool Ready { get; private set; } = false;

        public static void Setup() {
            PlayGameSoundEffect = Marshal.GetDelegateForFunctionPointer<PlaySoundEffectDelegate>( Dalamud.SigScanner.ScanText( Constants.PlaySoundSig ) );
            SetupSheets();
            Ready = true;
        }

        public static void PlaySoundEffect( int soundEffect ) {
            if( soundEffect == 19 || soundEffect == 21 ) return;
            if( soundEffect <= 0 ) return;
            if( WatchingCutscene ) return;
            PlayGameSoundEffect( soundEffect, 0, 0, 0 );
        }

        public static bool GetCurrentCast( out float currentTime, out float totalTime ) {
            currentTime = Dalamud.ClientState.LocalPlayer.CurrentCastTime;
            totalTime = Dalamud.ClientState.LocalPlayer.TotalCastTime;
            return Dalamud.ClientState.LocalPlayer.IsCasting;
        }

        public static bool GetRecastActive( uint actionId, out float timeElapsed, ActionType actionType = ActionType.Action ) {
            var actionManager = ActionManager.Instance();
            var adjustedId = actionManager->GetAdjustedActionId( actionId );
            timeElapsed = actionManager->GetRecastTimeElapsed( actionType, adjustedId );
            return timeElapsed > 0;
        }

        public static uint GetAdjustedAction( uint actionId ) {
            var actionManager = ActionManager.Instance();
            return actionManager->GetAdjustedActionId( actionId );
        }

        public static bool GetRecastActiveAndTotal( uint actionId, out float timeElapsed, out float timeTotal, ActionType actionType = ActionType.Action ) {
            var actionManager = ActionManager.Instance();
            var adjustedId = actionManager->GetAdjustedActionId( actionId );
            timeElapsed = actionManager->GetRecastTimeElapsed( actionType, adjustedId );
            timeTotal = actionManager->GetRecastTime( actionType, adjustedId );
            return timeElapsed > 0;
        }

        public static IGameObject PreviousEnemyTarget => GetPreviousEnemyTarget();

        private static IGameObject GetPreviousEnemyTarget() {
            var actorAddress = Marshal.ReadIntPtr( new IntPtr( TargetSystem.Instance() ) + Constants.PreviousTargetOffset );
            if( actorAddress == IntPtr.Zero ) return null;

            return Dalamud.Objects.CreateObjectReference( actorAddress );
        }
    }
}