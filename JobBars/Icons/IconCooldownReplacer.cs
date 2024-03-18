using JobBars.Data;
using System;
using System.Linq;

namespace JobBars.Icons {
    public struct IconCooldownProps {
        public ActionIds[] Icons;
        public float Cooldown;
        public Item[] Triggers;
    }

    public class IconCooldownReplacer : IconReplacer {
        private readonly Item[] Triggers;
        private readonly float Cooldown;

        private DateTime LastActiveTime;

        public IconCooldownReplacer( string name, IconCooldownProps props ) : base( name, true, props.Icons ) {
            Triggers = props.Triggers;
            Cooldown = props.Cooldown;
        }

        public override void ProcessAction( Item action ) {
            if( Triggers.Contains( action ) ) SetActive();
        }

        private void SetActive() {
            State = IconState.Active;
            LastActiveTime = DateTime.Now;
        }

        public override void Tick() {
            if( State == IconState.Active ) {
                var timeLeft = Cooldown - ( float )( DateTime.Now - LastActiveTime ).TotalSeconds;
                if( timeLeft <= 0 ) {
                    timeLeft = 0;
                    State = IconState.Inactive;
                }

                if( timeLeft == 0 ) ResetIcon();
                else SetIcon( timeLeft, Cooldown );
            }
        }
    }
}
