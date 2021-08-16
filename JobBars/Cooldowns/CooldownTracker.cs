using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void Tick(UICooldownItem ui) {
            var currentTime = DateTime.Now;

            if(State == TrackerState.None) {
                ui.SetOffCD();
                ui.SetText("");
            }
            else if(State == TrackerState.Running) {
                ui.SetOffCD();
                var timeLeft = Props.Duration - (currentTime - LastActiveTime).TotalSeconds;
                ui.SetText(((int)Math.Round(timeLeft)).ToString());
                if (timeLeft <= 0) State = TrackerState.OnCD;
            }
            else if(State == TrackerState.OnCD) {
                ui.SetOnCD();
                var timeLeft = Props.CD - (currentTime - LastActiveTime).TotalSeconds;
                ui.SetText(((int)Math.Round(timeLeft)).ToString());
                if (timeLeft <= 0) State = TrackerState.OffCD;
            }
            else if(State == TrackerState.OffCD) {
                ui.SetOffCD();
                ui.SetText("");
            }
        }

        public void ProcessAction(Item action) {
            if(action.Type != ItemType.Buff && action.Id == (uint) Props.Trigger) {
                LastActiveTime = DateTime.Now;
                State = TrackerState.Running;
            }
        }
    }
}