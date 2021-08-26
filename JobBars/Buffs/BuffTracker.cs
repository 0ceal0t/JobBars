using Dalamud.Logging;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Buffs {
    public class BuffTracker {
        public enum BuffState {
            None, // hidden
            Running, // bright, show countdown
            OffCD, // bright, no text
            OnCD_Hidden,
            OnCD_Visible // dark, show countdown
        }

        public BuffProps Props;
        private BuffState State = BuffState.None;
        private DateTime LastActiveTime;
        private Item LastActiveTrigger;
        private float TimeLeft;
        private float Percent;

        private UIBuff UI;

        public BuffState CurrentState => State;
        public bool Enabled => (State == BuffState.Running || State == BuffState.OffCD || State == BuffState.OnCD_Visible);

        public BuffTracker(BuffProps props) {
            Props = props;
        }

        public void ProcessAction(Item action) {
            if (Props.Triggers.Contains(action)) SetActive(action);
        }

        public void Tick(Dictionary<Item, StatusDuration> buffDict) {
            if (State != BuffState.Running) { // check for buff triggers
                foreach (var trigger in Props.Triggers) {
                    if (trigger.Type != ItemType.Buff) continue;
                    if (buffDict.ContainsKey(trigger) && buffDict[trigger].Duration > 0) SetActive(trigger);
                }
            }

            if(State == BuffState.Running) {
                TimeLeft = UIHelper.TimeLeft(Props.Duration, DateTime.Now, buffDict, LastActiveTrigger, LastActiveTime);
                if(TimeLeft <= 0) { // Buff over
                    Percent = 1f;
                    TimeLeft = 0;

                    State = Props.CD <= 0 ? BuffState.None : // no CD, inactive
                        Props.CD <= 30 ? BuffState.OnCD_Visible : BuffState.OnCD_Hidden;
                }
                else { // Still running
                    Percent = 1.0f - (float)(TimeLeft / Props.Duration);
                }
            }
            else if(State == BuffState.OnCD_Hidden || State == BuffState.OnCD_Visible) {
                TimeLeft = (float)(Props.CD - (DateTime.Now - LastActiveTime).TotalSeconds);
                if(TimeLeft <= 0) { // Off CD
                    State = BuffState.OffCD;
                }
                else if(TimeLeft <= 30) { // Visible
                    State = BuffState.OnCD_Visible;
                    Percent = TimeLeft / Props.CD;
                }
            }
        }

        private void SetActive(Item trigger) {
            State = BuffState.Running;
            LastActiveTime = DateTime.Now;
            LastActiveTrigger = trigger;
        }

        public void TickUI(UIBuff ui) {
            if(UI != ui) {
                UI = ui;
                SetupUI();
            }
            else if(UI.Iconid != Props.Icon) SetupUI();

            UI.Show();

            if(State == BuffState.Running) {
                UI.SetOffCD();
                UI.SetPercent(Percent);
                UI.SetText(((int)Math.Round(TimeLeft)).ToString());
            }
            else if(State == BuffState.OffCD) {
                UI.SetOffCD();
                UI.SetPercent(0);
                UI.SetText("");
            }
            else if(State == BuffState.OnCD_Visible) {
                UI.SetOnCD();
                UI.SetPercent(Percent);
                UI.SetText(((int)Math.Round(TimeLeft)).ToString());
            }
        }

        private void SetupUI() {
            UI.LoadIcon(Props.Icon);
            UI.SetColor(Props.Color);
        }

        public void Reset() {
            State = BuffState.None;
        }
    }
}
