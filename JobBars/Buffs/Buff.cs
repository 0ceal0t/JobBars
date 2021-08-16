using ImGuiNET;
using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static JobBars.UI.UIColor;

namespace JobBars.Buffs {
    public enum BuffState {
        Inactive, // hidden
        Active, // bright, show countdown
        OffCD, // bright, no text
        OnCD_Hidden,
        OnCD_Visible // dark, show countdown
    };

    public struct BuffProps {
        public float Duration;
        public float? CD;
        public Item[] Triggers;
        public ElementColor Color;
        public ActionIds Icon;
    }

    public class Buff {
        public string Name;
        public UIBuff UI;
        public bool Enabled;

        private BuffProps Props;
        private DateTime StateTime;

        public ActionIds Icon => Props.Icon;
        public BuffState State = BuffState.Inactive;
        public bool Visible => (State == BuffState.Active || State == BuffState.OffCD || State == BuffState.OnCD_Visible);

        public Buff(string name, BuffProps props) {
            Name = name;
            Props = props;
            Enabled = !Configuration.Config.BuffDisabled.Contains(Name);
            if (Props.CD != null) Props.CD = Props.CD.Value - Props.Duration;
        }

        public void LoadUI(UIBuff ui) {
            UI = ui;
            UI.SetColor(Props.Color);
        }

        public void Reset() {
            State = BuffState.Inactive;
        }

        public void ProcessAction(Item action) {
            if ((State == BuffState.Inactive || State == BuffState.OffCD) && Props.Triggers.Contains(action)) {
                State = BuffState.Active;
                StateTime = DateTime.Now;
                UI.Show();
                UI.SetOffCD();
            }
        }

        public void Tick(DateTime time) {
            if (State == BuffState.Active) {
                var timeleft = Props.Duration - (time - StateTime).TotalSeconds;

                if (timeleft <= 0) { // buff over, either hide or go on cd
                    if (Props.CD == null) {
                        State = BuffState.Inactive;
                        UI.Hide();
                    }
                    else {
                        State = Props.CD.Value > 30 ? BuffState.OnCD_Hidden : BuffState.OnCD_Visible;
                        StateTime = DateTime.Now;
                        UI.SetOnCD();
                        UI.SetText(((int)Props.CD.Value).ToString());
                        UI.SetPercent(1.0f);
                        if (State == BuffState.OnCD_Hidden) {
                            UI.Hide();
                        }
                    }
                }
                else { // buff still active
                    UI.SetPercent(1.0f - (float)(timeleft / Props.Duration));
                    UI.SetText(((int)timeleft).ToString());
                }
            }
            else if (State == BuffState.OnCD_Hidden) { // on CD, but don't show it yet since it's more than 30 seconds away
                var timeleft = Props.CD.Value - (time - StateTime).TotalSeconds;

                if (timeleft < 30) {
                    State = BuffState.OnCD_Visible;
                    UI.Show();
                    UI.SetPercent((float)(timeleft / Props.CD.Value));
                    UI.SetText(((int)timeleft).ToString());
                }
            }
            else if (State == BuffState.OnCD_Visible) { // on CD, now close to being off CD
                var timeleft = Props.CD.Value - (time - StateTime).TotalSeconds;

                if (timeleft <= 0) { // back off CD
                    State = BuffState.OffCD;
                    UI.SetOffCD();
                    UI.SetText("");
                    UI.SetPercent(0);
                }
                else {
                    UI.SetPercent((float)(timeleft / Props.CD.Value));
                    UI.SetText(((int)timeleft).ToString());
                }
            }
        }

        public void Draw(string id) {
            var _ID = id + Name;

            ImGui.TextColored(Enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1), $"{Name}");
            if (ImGui.Checkbox("Enabled" + _ID, ref Enabled)) {
                if (Enabled) {
                    Configuration.Config.BuffDisabled.Remove(Name);
                }
                else {
                    UI.Hide();
                    Reset();
                    Configuration.Config.BuffDisabled.Add(Name);
                }
                Configuration.Config.Save();
            }

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
        }
    }
}
