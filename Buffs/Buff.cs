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
        Inactive, // hidden
        Active, // bright, show countdown
        OnCD_Hidden, 
        OnCD_Visible, // dark, show countdown
        OffCD // bright, no text
    };

    public class Buff {
        public string Name;
        public IconIds Icon;
        public UIBuff UI;
        public BuffState State = BuffState.Inactive;
        public bool Enabled = true;

        private Item[] Triggers;
        private DateTime StateTime;

        private float Duration;
        private float CD;
        private bool NoCD = false;

        public bool Visible => (State == BuffState.Active || State == BuffState.OffCD || State == BuffState.OnCD_Visible);

        public Buff(string name, IconIds icon, float duration) {
            Name = name;
            Icon = icon;
            Duration = duration;
            Triggers = new Item[0];
        }

        public void ProcessAction(Item action) {
            if ((State == BuffState.Inactive || State == BuffState.OffCD) && Triggers.Contains(action)) {
                State = BuffState.Active;
                StateTime = DateTime.Now;
                UI.Show();
                UI.SetOffCD();
            }
        }

        public void Tick(DateTime time) {
            if(State == BuffState.Active) {
                var timeleft = Duration - (time - StateTime).TotalSeconds;

                if(timeleft <= 0) { // buff over, either hide or go on cd
                    if(NoCD) {
                        State = BuffState.Inactive;
                        UI.Hide();
                    }
                    else {
                        State = CD > 30 ? BuffState.OnCD_Hidden : BuffState.OnCD_Visible;
                        StateTime = DateTime.Now;
                        UI.SetOnCD();
                        UI.SetText(((int)CD).ToString());
                        UI.SetPercent(1.0f);
                        if(State == BuffState.OnCD_Hidden) {
                            UI.Hide();
                        }
                    }
                }
                else { // buff still active
                    UI.SetPercent(1.0f - (float)(timeleft / Duration));
                    UI.SetText(((int)timeleft).ToString());
                }
            }
            else if(State == BuffState.OnCD_Hidden) { // on CD, but don't show it yet since it's more than 30 seconds away
                var timeleft = CD - (time - StateTime).TotalSeconds;

                if(timeleft < 30) {
                    State = BuffState.OnCD_Visible;
                    UI.Show();
                    UI.SetPercent((float)(timeleft / CD));
                    UI.SetText(((int)timeleft).ToString());
                }
            }
            else if(State == BuffState.OnCD_Visible) { // on CD, now close to being off CD
                var timeleft = CD - (time - StateTime).TotalSeconds;

                if(timeleft <= 0) { // back off CD
                    State = BuffState.OffCD;
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
