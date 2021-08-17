using JobBars.Data;
using JobBars.UI;
using System;

namespace JobBars.Cooldowns {
    public class CooldownTracker {
        private enum TrackerState {
            None,
            Running,
            OnCD,
            OffCD
        }

        public ActionIds Icon => Props.Trigger;

        private CooldownProps Props;
        private TrackerState State = TrackerState.None;
        private DateTime LastActiveTime;

        public CooldownTracker(CooldownProps props) {
            Props = props;
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
                var timeLeft = Props.Duration - (currentTime - LastActiveTime).TotalSeconds;
                ui.SetText(((int)Math.Round(timeLeft)).ToString());
                if (timeLeft <= 0) State = TrackerState.OnCD;
            }
            else if(State == TrackerState.OnCD) {
                ui.SetOnCD();
                ui.SetNoDash();
                var timeLeft = Props.CD - (currentTime - LastActiveTime).TotalSeconds;
                ui.SetText(((int)Math.Round(timeLeft)).ToString());
                if (timeLeft <= 0) State = TrackerState.OffCD;
            }
            else if(State == TrackerState.OffCD) {
                ui.SetOffCD();
                ui.SetNoDash();
                ui.SetText("");
            }
        }

        public void Reset() {
            State = TrackerState.None;
        }

        public void ProcessAction(Item action) {
            if(action.Type != ItemType.Buff && action.Id == (uint) Props.Trigger) {
                LastActiveTime = DateTime.Now;
                State = Props.CD == 0 ? TrackerState.OnCD : TrackerState.Running;
            }
        }
    }
}