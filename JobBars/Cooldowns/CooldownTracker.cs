using JobBars.Data;
using JobBars.UI;
using System;
using System.Linq;

namespace JobBars.Cooldowns {
    public class CooldownTracker {
        private enum TrackerState {
            None,
            Running,
            OnCD,
            OffCD
        }

        public ActionIds Icon => Trigger;

        private readonly ActionIds Trigger;
        private readonly ActionIds[] AdditionalTriggers;
        private readonly float Duration;
        private readonly float CD;

        private TrackerState State = TrackerState.None;
        private DateTime LastActiveTime;

        public CooldownTracker(CooldownProps props) {
            Trigger = props.Trigger;
            AdditionalTriggers = props.AdditionalTriggers;
            Duration = props.Duration;
            CD = props.CD;
        }

        public void Tick(UICooldownItem ui, float percent) {
            var currentTime = DateTime.Now;

            if(State == TrackerState.None) {
                ui.SetOffCD();
                ui.SetNoDash();
                ui.SetText("");
            }
            else if(State == TrackerState.Running) {
                ui.SetOffCD();
                ui.SetDash(percent);
                var timeLeft = Duration - (currentTime - LastActiveTime).TotalSeconds;
                ui.SetText(((int)Math.Round(timeLeft)).ToString());
                if (timeLeft <= 0) State = TrackerState.OnCD;
            }
            else if(State == TrackerState.OnCD) {
                ui.SetOnCD();
                ui.SetNoDash();
                var timeLeft = CD - (currentTime - LastActiveTime).TotalSeconds;
                ui.SetText(((int)Math.Round(timeLeft)).ToString());
                if (timeLeft <= 0) State = TrackerState.OffCD;
            }
            else if(State == TrackerState.OffCD) {
                ui.SetOffCD();
                ui.SetNoDash();
                ui.SetText("");
            }
        }

        public void ProcessAction(Item action) {
            if(action.Type != ItemType.Buff && 
                (action.Id == (uint) Trigger ||
                    (
                        AdditionalTriggers != null && AdditionalTriggers.Contains((ActionIds)action.Id)
                    )
                )
             ){
                LastActiveTime = DateTime.Now;
                State = Duration == 0 ? TrackerState.OnCD : TrackerState.Running;
            }
        }

        public void Reset() {
            State = TrackerState.None;
        }
    }
}