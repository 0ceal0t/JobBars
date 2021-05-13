using JobBars.Data;
using JobBars.Gauges;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Buffs {
    public enum BuffState {
        INACTIVE, // hidden
        ACTIVE, // bright, show countdown
        ONCD_HIDDEN, 
        ONCD_VISIBLE, // dark, show countdown
        OFFCD // bright, no text
    };

    public class Buff {
        public string Name;
        public IconIds Icon;
        public Item[] Triggers;
        public UIBuff UI = null;

        public BuffState State = BuffState.INACTIVE;
        DateTime StateTime;
        public bool Enabled = true;

        public float Duration;
        public float CD;
        public bool NoCD = false;

        public bool Visible => (State == BuffState.ACTIVE || State == BuffState.OFFCD || State == BuffState.ONCD_VISIBLE);

        public Buff(string name, IconIds icon, float duration) {
            Name = name;
            Icon = icon;
            Duration = duration;
            Triggers = new Item[0];
        }

        public void ProcessAction(Item action) {
            if ((State == BuffState.INACTIVE || State == BuffState.OFFCD) && Triggers.Contains(action)) {
                State = BuffState.ACTIVE;
                StateTime = DateTime.Now;
                UI.Show();
                UI.SetOffCD();
            }
        }

        public void Tick(DateTime time, float delta) {
            if(State == BuffState.ACTIVE) {
                var timeleft = Duration - (time - StateTime).TotalSeconds;

                if(timeleft <= 0) { // buff over, either hide or go on cd
                    if(NoCD) {
                        State = BuffState.INACTIVE;
                        UI.Hide();
                    }
                    else {
                        State = CD > 30 ? BuffState.ONCD_HIDDEN : BuffState.ONCD_VISIBLE;
                        StateTime = DateTime.Now;
                        UI.SetOnCD();
                        UI.SetText(((int)CD).ToString());
                        UI.SetPercent(1.0f);
                        if(State == BuffState.ONCD_HIDDEN) {
                            UI.Hide();
                        }
                    }
                }
                else { // buff still active
                    UI.SetPercent(1.0f - (float)(timeleft / Duration));
                    UI.SetText(((int)timeleft).ToString());
                }
            }
            else if(State == BuffState.ONCD_HIDDEN) { // on CD, but don't show it yet since it's more than 30 seconds away
                var timeleft = CD - (time - StateTime).TotalSeconds;

                if(timeleft < 30) {
                    State = BuffState.ONCD_VISIBLE;
                    UI.Show();
                    UI.SetPercent((float)(timeleft / CD));
                    UI.SetText(((int)timeleft).ToString());
                }
            }
            else if(State == BuffState.ONCD_VISIBLE) { // on CD, now close to being off CD
                var timeleft = CD - (time - StateTime).TotalSeconds;

                if(timeleft <= 0) { // back off CD
                    State = BuffState.OFFCD;
                    UI.SetOffCD();
                    UI.SetText("");
                    UI.SetPercent(0);
                }
                else {
                    UI.SetPercent((float)(timeleft / CD));
                    UI.SetText(((int)timeleft).ToString());
                }
            }
        }

        // ===== BUILDER FUNCS ========
        public Buff WithTriggers(Item[] triggers) {
            Triggers = triggers;
            return this;
        }
        public Buff WithCD(float cd) {
            CD = (cd - Duration);
            return this;
        }
        public Buff WithNoCD() {
            NoCD = true;
            return this;
        }
    }
}
