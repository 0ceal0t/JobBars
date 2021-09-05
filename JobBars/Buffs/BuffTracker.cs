using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using JobBars.Data;
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

        private readonly float Duration;
        private readonly float CD;
        private readonly ActionIds Icon;
        private readonly ElementColor Color;
        private readonly Item[] Triggers;

        private BuffState State = BuffState.None;
        private DateTime LastActiveTime;
        private Item LastActiveTrigger;
        private float TimeLeft;
        private float Percent;

        private UIBuff UI;

        public BuffState CurrentState => State;
        public bool Enabled => (State == BuffState.Running || State == BuffState.OffCD || State == BuffState.OnCD_Visible);

        public BuffTracker(BuffProps props) {
            Duration = props.Duration;
            CD = props.CD;
            Icon = props.Icon;
            Color = props.Color;
            Triggers = props.Triggers;
        }

        public void ProcessAction(Item action) {
            if (Triggers.Contains(action)) SetActive(action);
        }

        public void Tick(Dictionary<Item, Status> buffDict) {
            if (State != BuffState.Running) { // check for buff triggers
                foreach (var trigger in Triggers.Where(t => t.Type == ItemType.Buff)) {
                    if (buffDict.ContainsKey(trigger) && buffDict[trigger].RemainingTime > 0) SetActive(trigger);
                }
            }

            if(State == BuffState.Running) {
                TimeLeft = UIHelper.TimeLeft(Duration, DateTime.Now, buffDict, LastActiveTrigger, LastActiveTime);
                if(TimeLeft <= 0) { // Buff over
                    Percent = 1f;
                    TimeLeft = 0;

                    State = CD <= 0 ? BuffState.None : // no CD, inactive
                        CD <= JobBars.Config.BuffDisplayTimer ? BuffState.OnCD_Visible : BuffState.OnCD_Hidden;
                }
                else { // Still running
                    Percent = 1.0f - (float)(TimeLeft / Duration);
                }
            }
            else if(State == BuffState.OnCD_Hidden || State == BuffState.OnCD_Visible) {
                TimeLeft = (float)(CD - (DateTime.Now - LastActiveTime).TotalSeconds);
                if(TimeLeft <= 0) { // Off CD
                    State = BuffState.OffCD;
                }
                else if(TimeLeft <= JobBars.Config.BuffDisplayTimer) { // Visible
                    State = BuffState.OnCD_Visible;
                    Percent = TimeLeft / CD;
                }
            }
        }

        private void SetActive(Item trigger) {
            State = BuffState.Running;
            LastActiveTime = DateTime.Now;
            LastActiveTrigger = trigger;
        }

        public void TickUI(UIBuff ui) {
            if(UI != ui || UI?.Iconid != Icon) {
                UI = ui;
                SetupUI();
            }

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
            UI.LoadIcon(Icon);
            UI.SetColor(Color);
        }

        public void Reset() {
            State = BuffState.None;
        }
    }
}
