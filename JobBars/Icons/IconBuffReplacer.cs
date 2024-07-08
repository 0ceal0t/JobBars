using JobBars.Data;
using JobBars.GameStructs;
using JobBars.Helper;
using System;

namespace JobBars.Icons {
    public struct IconBuffProps {
        public bool IsTimer;
        public ActionIds[] Icons;
        public IconBuffTriggerStruct[] Triggers;
    }

    public struct IconBuffTriggerStruct {
        public Item Trigger;
        public float Duration;
    }

    public class IconBuffReplacer : IconReplacer {
        public enum IconBuffState {
            Inactive,
            Active,
            Done
        }

        private readonly IconBuffTriggerStruct[] Triggers;

        private IconBuffState State = IconBuffState.Inactive;
        private float MaxDuration;
        private float TimeLeft;

        public IconBuffReplacer( string name, IconBuffProps props ) : base( name, props.IsTimer, props.Icons ) {
            Triggers = props.Triggers;
        }

        public override void ProcessAction( Item action ) { }

        public override void Tick() {
            TimeLeft = -1f;
            MaxDuration = 1f;
            foreach( var trigger in Triggers ) {
                if( UiHelper.PlayerStatus.TryGetValue( trigger.Trigger, out var value ) ) {
                    TimeLeft = value.RemainingTime - Offset;
                    MaxDuration = trigger.Duration - Offset;
                    break;
                }
            }

            if( TimeLeft > 0 && State != IconBuffState.Active ) State = IconBuffState.Active;

            if( State == IconBuffState.Active ) {
                if( TimeLeft <= 0 ) {
                    TimeLeft = 0;
                    State = IconBuffState.Done;
                }
            }
        }

        public override unsafe void UpdateIcon( HotbarSlotStruct* data ) {
            if( State == IconBuffState.Active ) {
                if( IsTimer ) { // Timer
                    data->YellowBorder = CalcShowBorder( false, data->YellowBorder );
                    data->TextColor = 0;
                    data->Usable = false;
                    data->CdText = ( uint )Math.Round( TimeLeft );
                    data->InRange = true;

                    if( JobBars.Configuration.IconTimerLarge ) data->TextStyle = 5;

                    if( ShowRing ) {
                        data->CdPercent = ( uint )Math.Round( 100f * ( 1f - ( TimeLeft / MaxDuration ) ) );
                        if( data->CdPercent == 100 ) data->CdPercent = 0;
                        data->UseRing = true;
                    }
                }
                else { // Buff
                    data->YellowBorder = CalcShowBorder( true, data->YellowBorder );
                    data->TextColor = 0;
                    data->CdText = ( uint )Math.Round( TimeLeft );
                    data->GcdSwingPercent = 0;
                    data->CdPercent = 0;
                    data->Usable = true;
                    data->UseRing = false;
                    data->InRange = true;

                    if( JobBars.Configuration.IconBuffLarge ) data->TextStyle = 5;
                }
            }
            else if( State == IconBuffState.Done && IsTimer ) {
                data->YellowBorder = CalcShowBorder( true, data->YellowBorder );
            }
        }

        protected bool CalcShowBorder( bool active, bool border ) => ComboType switch {
            IconComboType.Only_When_Combo => border,
            IconComboType.Only_When_Active => active,
            IconComboType.Combo_Or_Active => border || active,
            IconComboType.Combo_And_Active => border && active,
            IconComboType.Never => false,
            _ => false
        };
    }
}
