using JobBars.Atk;
using JobBars.Gauges.Types.Diamond;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Gauges.Procs {
    public class GaugeProcsTracker : GaugeTracker, IGaugeDiamondInterface {
        private class Proc {
            public readonly ProcConfig Config;
            public bool Active = true;
            public float RemainingTime = 0;

            public Proc( ProcConfig config ) {
                Config = config;
            }
        }

        // =============================

        private readonly GaugeProcsConfig Config;
        private readonly List<Proc> Procs;
        private GaugeState State = GaugeState.Inactive;

        public GaugeProcsTracker( GaugeProcsConfig config, int idx ) {
            Config = config;
            Procs = [.. Config.Procs.Select( p => new Proc( p ) ).OrderBy( proc => proc.Config.Order )];
            LoadUI( Config.TypeConfig switch {
                GaugeDiamondConfig _ => new GaugeDiamond<GaugeProcsTracker>( this, idx ),
                _ => new GaugeDiamond<GaugeProcsTracker>( this, idx ) // DEFAULT
            } );
        }

        public override GaugeConfig GetConfig() => Config;

        public override bool GetActive() => State != GaugeState.Inactive;

        public override void ProcessAction( Item action ) { }

        protected override void TickTracker() {
            var playSound = false;
            var procActiveCount = 0;
            foreach( var proc in Procs ) {
                var procActive = false;
                proc.RemainingTime = 0;

                foreach( var trigger in proc.Config.Triggers ) {
                    if( trigger.Type == ItemType.Buff ) {
                        if( UiHelper.PlayerStatus.TryGetValue( trigger, out var buff ) ) {
                            procActive = true;
                            proc.RemainingTime = Math.Max( 0, buff.RemainingTime );
                        }
                    }
                    else {
                        if( !UiHelper.GetRecastActive( trigger.Id, out _ ) ) {
                            procActive = true;
                        }
                    }

                    if( procActive ) break;
                }

                if( procActive ) procActiveCount++;
                if( procActive != proc.Active ) {
                    if( procActive && ( Config.ProcSound == GaugeCompleteSoundType.When_Full || Config.ProcSound == GaugeCompleteSoundType.When_Empty_or_Full ) )
                        playSound = true;
                    else if( !procActive && ( Config.ProcSound == GaugeCompleteSoundType.When_Empty || Config.ProcSound == GaugeCompleteSoundType.When_Empty_or_Full ) )
                        playSound = true;
                }
                proc.Active = procActive;
            }

            if( playSound ) Config.PlaySoundEffect();
            State = procActiveCount == 0 ? GaugeState.Inactive : GaugeState.Active;
        }

        public int GetCurrentMaxTicks() => Procs.Count;

        public int GetTotalMaxTicks() => Procs.Count;

        public ElementColor GetTickColor( int idx ) => Procs[idx].Config.Color;

        public bool GetDiamondTextVisible() => Config.ProcsShowText;

        public bool GetTickValue( int idx ) => Procs[idx].Active;

        public string GetDiamondText( int idx ) {
            var proc = Procs[idx];
            return proc.RemainingTime >= 0 ? ( ( int )Math.Round( proc.RemainingTime ) ).ToString() : "";
        }

        public bool GetReverseFill() => false;
    }
}
