using JobBars.Atk;
using JobBars.Gauges.Types.Bar;
using JobBars.Gauges.Types.BarDiamondCombo;
using JobBars.Gauges.Types.Diamond;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Gauges.Charges {
    public class GaugeChargesTracker : GaugeTracker, IGaugeBarInterface, IGaugeDiamondInterface, IGaugeBarDiamondComboInterface {
        private readonly GaugeChargesConfig Config;
        private readonly int TotalCharges;
        private readonly List<bool> ChargesActive = [];
        private readonly bool IsCDBar;

        private int ChargesActiveTotal = 0;
        private float BarTextValue = 0;
        private float BarPercentValue = 0;

        public GaugeChargesTracker( GaugeChargesConfig config, int idx ) {
            Config = config;
            TotalCharges = Config.Parts.Where( p => p.Diamond ).Select( d => d.MaxCharges ).Sum();
            IsCDBar = Config.Parts.Where( p => p.Bar ).All( p => p.Triggers.All( t => t.Type != ItemType.Buff ) );
            LoadUI( Config.TypeConfig switch {
                GaugeBarConfig _ => new GaugeBar<GaugeChargesTracker>( this, idx ),
                GaugeDiamondConfig _ => new GaugeDiamond<GaugeChargesTracker>( this, idx ),
                GaugeBarDiamondComboConfig _ => new GaugeBarDiamondCombo<GaugeChargesTracker>( this, idx ),
                _ => new GaugeBarDiamondCombo<GaugeChargesTracker>( this, idx ) // DEFAULT
            } );
        }

        public override GaugeConfig GetConfig() => Config;

        public override bool GetActive() => IsCDBar ? ChargesActiveTotal < TotalCharges : BarPercentValue > 0f; // CD not full : buff active

        public override void ProcessAction( Item action ) { }

        protected override void TickTracker() {
            ChargesActive.Clear();
            var barAssigned = false;
            var currentChargesValue = 0;

            foreach( var part in Config.Parts ) {
                var diamondFound = false;
                foreach( var trigger in part.Triggers ) {
                    if( trigger.Type == ItemType.Buff ) {
                        var buffExists = UiHelper.PlayerStatus.TryGetValue( trigger, out var buff );
                        var buffValue = buffExists ? buff.StackCount : 0;

                        if( part.Bar && !barAssigned && buffExists ) {
                            barAssigned = true;
                            BarPercentValue = buff.RemainingTime / part.Duration;
                            BarTextValue = buff.RemainingTime;
                        }
                        if( part.Diamond ) {
                            currentChargesValue += buffValue;
                            AddToActive( buffValue, part.MaxCharges );
                        }
                        if( buffExists || buffValue > 0 ) {
                            diamondFound = true;
                            break;
                        }
                    }
                    else {
                        var recastActive = UiHelper.GetRecastActive( trigger.Id, out var timeElapsed );
                        var actionValue = recastActive ? ( int )Math.Floor( timeElapsed / part.CD ) : part.MaxCharges;

                        if( part.Bar && !barAssigned && recastActive ) {
                            barAssigned = true;
                            var currentTime = timeElapsed % part.CD;
                            var timeLeft = part.CD - currentTime;

                            BarPercentValue = currentTime / part.CD;
                            BarTextValue = timeLeft;
                        }
                        if( part.Diamond ) {
                            currentChargesValue += actionValue;
                            AddToActive( actionValue, part.MaxCharges );
                        }
                        if( recastActive || actionValue > 0 ) {
                            diamondFound = true;
                            break;
                        }
                    }
                }
                if( !diamondFound ) AddToActive( 0, part.MaxCharges ); // part is empty
            }
            if( !barAssigned ) BarTextValue = BarPercentValue = 0;

            if( currentChargesValue != ChargesActiveTotal ) {
                if( currentChargesValue == 0 ) {
                    if( Config.CompletionSound == GaugeCompleteSoundType.When_Empty || Config.CompletionSound == GaugeCompleteSoundType.When_Empty_or_Full )
                        Config.PlayCompletionSoundEffect();
                }
                else if( currentChargesValue == TotalCharges ) {
                    if( Config.CompletionSound == GaugeCompleteSoundType.When_Full || Config.CompletionSound == GaugeCompleteSoundType.When_Empty_or_Full )
                        Config.PlayCompletionSoundEffect();
                }
                else Config.PlaySoundEffect();
            }
            ChargesActiveTotal = currentChargesValue;
        }

        private void AddToActive( int count, int max ) {
            for( var i = 0; i < count; i++ ) ChargesActive.Add( true );
            for( var i = count; i < max; i++ ) ChargesActive.Add( false );
        }

        public float[] GetBarSegments() => null;

        public bool GetBarTextVisible() => Config.TypeConfig switch {
            GaugeBarConfig barConfig => barConfig.ShowText,
            GaugeBarDiamondComboConfig comboConfig => comboConfig.ShowText,
            _ => false
        };

        public bool GetVertical() => Config.TypeConfig switch {
            GaugeBarConfig barConfig => barConfig.Vertical,
            _ => false
        };

        public bool GetBarTextSwap() => false;

        public ElementColor GetColor() => Config.BarColor;

        public bool GetBarDanger() => false;

        public string GetBarText() => $"{( int )Math.Round( BarTextValue )}";

        public float GetBarPercent() => BarPercentValue;

        public float GetBarIndicatorPercent() => 0;

        public int GetCurrentMaxTicks() => TotalCharges;

        public int GetTotalMaxTicks() => TotalCharges;

        public ElementColor GetTickColor( int idx ) {
            if( Config.SameColor ) return Config.BarColor;

            var startIdx = 0;
            foreach( var part in Config.Parts.Where( x => x.Diamond ) ) {
                var endIdx = startIdx + part.MaxCharges;
                if( idx < endIdx ) return part.Color;
                startIdx = endIdx;
            }
            return ColorConstants.NoColor;
        }

        public bool GetDiamondTextVisible() => false;

        public bool GetTickValue( int idx ) => ChargesActive[idx];

        public string GetDiamondText( int idx ) => "";

        public bool GetReverseFill() => Config.ReverseFill;
    }
}
