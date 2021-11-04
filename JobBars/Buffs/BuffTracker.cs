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

        private readonly BuffConfig Config;

        private BuffState State = BuffState.None;
        private DateTime LastActiveTime;
        private Item LastActiveTrigger;
        private float TimeLeft;
        private float Percent;

        private UIBuff UI;

        public BuffState CurrentState => State;
        public uint Id => (uint)Config.Icon;
        public bool Enabled => (State == BuffState.Running || State == BuffState.OffCD || State == BuffState.OnCD_Visible);
        public bool Highlighted => State == BuffState.Running;

        public BuffTracker(BuffConfig config) {
            Config = config;
        }

        public void ProcessAction(Item action) {
            if (Config.Triggers.Contains(action)) SetActive(action);
        }

        private void SetActive(Item trigger) {
            State = BuffState.Running;
            LastActiveTime = DateTime.Now;
            LastActiveTrigger = trigger;
        }

        public void Tick(Dictionary<Item, Status> buffDict) {
            if (State != BuffState.Running && UIHelper.CheckForTriggers(buffDict, Config.Triggers, out var trigger)) SetActive(trigger);

            if (State == BuffState.Running) {
                TimeLeft = UIHelper.TimeLeft(Config.Duration, buffDict, LastActiveTrigger, LastActiveTime);
                if (TimeLeft <= 0) { // Buff over
                    Percent = 1f;
                    TimeLeft = 0;

                    State = Config.CD <= 0 ? BuffState.None : // doesn't have a cooldown, just make it invisible
                        Config.CD <= JobBars.Config.BuffDisplayTimer ? BuffState.OnCD_Visible : BuffState.OnCD_Hidden;
                }
                else { // Still running
                    Percent = 1.0f - (float)(TimeLeft / Config.Duration);
                }
            }
            else if (State == BuffState.OnCD_Hidden || State == BuffState.OnCD_Visible) {
                TimeLeft = (float)(Config.CD - (DateTime.Now - LastActiveTime).TotalSeconds);

                if (TimeLeft <= 0) {
                    State = BuffState.OffCD;
                }
                else if (TimeLeft <= JobBars.Config.BuffDisplayTimer) { // CD low enough to be visible
                    State = BuffState.OnCD_Visible;
                    Percent = TimeLeft / Config.CD;
                }
            }
        }

        public void TickUI(UIBuff ui) {
            if (UI != ui || UI?.Iconid != Config.Icon) {
                UI = ui;
                SetupUI();
            }

            UI.Show();

            if (State == BuffState.Running) {
                UI.SetOffCD();
                UI.SetPercent(Percent);
                UI.SetText(((int)Math.Round(TimeLeft)).ToString());
            }
            else if (State == BuffState.OffCD) {
                UI.SetOffCD();
                UI.SetPercent(0);
                UI.SetText("");
            }
            else if (State == BuffState.OnCD_Visible) {
                UI.SetOnCD();
                UI.SetPercent(Percent);
                UI.SetText(((int)Math.Round(TimeLeft)).ToString());
            }
        }

        private void SetupUI() {
            UI.LoadIcon(Config.Icon);
            UI.SetColor(Config.Color);
        }

        public void Reset() {
            State = BuffState.None;
        }
    }
}
