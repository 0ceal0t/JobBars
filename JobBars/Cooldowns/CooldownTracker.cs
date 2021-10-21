using JobBars.Data;
using FFXIVClientStructs.FFXIV.Client.Game;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Cooldowns {
    public class CooldownTracker {
        private enum TrackerState {
            None,
            Running,
            OnCD,
            OffCD
        }

        public readonly ActionIds Icon;

        private readonly float Duration;
        private readonly float CD;
        private readonly Item[] Triggers;

        private TrackerState State = TrackerState.None;
        private DateTime LastActiveTime;
        private Item LastActiveTrigger;
        private float TimeLeft;

        public CooldownTracker(CooldownProps props) {
            Duration = props.Duration;
            CD = props.CD;
            Icon = props.Icon;
            Triggers = props.Triggers;
        }

        public void ProcessAction(Item action) {
            if (Triggers.Contains(action)) SetActive(action);
        }

        private void SetActive(Item trigger) {
            State = Duration == 0 ? TrackerState.OnCD : TrackerState.Running;
            LastActiveTime = DateTime.Now;
            LastActiveTrigger = trigger;
        }

        public void Tick(Dictionary<Item, Status> buffDict) {
            if (State != TrackerState.Running && UIHelper.CheckForTriggers(buffDict, Triggers, out var trigger)) SetActive(trigger);

            if (State == TrackerState.Running) {
                TimeLeft = UIHelper.TimeLeft(Duration, buffDict, LastActiveTrigger, LastActiveTime);
                if(TimeLeft <= 0) {
                    TimeLeft = 0;

                    State = TrackerState.OnCD; // mitigation needs to have a CD
                }
            }
            else if (State == TrackerState.OnCD) {
                TimeLeft = (float)(CD - (DateTime.Now - LastActiveTime).TotalSeconds);

                if (TimeLeft <= 0) {
                    State = TrackerState.OffCD;
                }
            }
        }

        public void TickUI(UICooldownItem ui, float percent) {
            if (State == TrackerState.None) {
                ui.SetOffCD();
                ui.SetNoDash();
                ui.SetText("");
            }
            else if (State == TrackerState.Running) {
                ui.SetOffCD();
                ui.SetDash(percent);
                ui.SetText(((int)Math.Round(TimeLeft)).ToString());
            }
            else if (State == TrackerState.OnCD) {
                ui.SetOnCD();
                ui.SetNoDash();
                ui.SetText(((int)Math.Round(TimeLeft)).ToString());
            }
            else if (State == TrackerState.OffCD) {
                ui.SetOffCD();
                ui.SetNoDash();
                ui.SetText("");
            }
        }

        public void Reset() {
            State = TrackerState.None;
        }
    }
}