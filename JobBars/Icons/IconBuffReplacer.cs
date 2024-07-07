using JobBars.Data;
using JobBars.Helper;

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
        private readonly IconBuffTriggerStruct[] Triggers;

        public IconBuffReplacer( string name, IconBuffProps props ) : base( name, props.IsTimer, props.Icons ) {
            Triggers = props.Triggers;
        }

        public override void ProcessAction( Item action ) { }

        public override void Tick() {
            var timeLeft = -1f;
            var maxDuration = 1f;
            foreach( var trigger in Triggers ) {
                if( UiHelper.PlayerStatus.TryGetValue( trigger.Trigger, out var value ) ) {
                    timeLeft = value.RemainingTime - Offset;
                    maxDuration = trigger.Duration - Offset;
                    break;
                }
            }

            if( timeLeft > 0 && State == IconState.Inactive ) {
                State = IconState.Active;
            }

            if( State == IconState.Active ) {
                if( timeLeft <= 0 ) {
                    timeLeft = 0;
                    State = IconState.Inactive;
                }

                if( timeLeft == 0 ) ResetIcon();
                else SetIcon( timeLeft, maxDuration );
            }
        }
    }
}
