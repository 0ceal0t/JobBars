using ImGuiNET;
using JobBars.Data;
using JobBars.UI;
using System.Numerics;
using System.Security.Cryptography;

namespace JobBars.Buffs {
    public struct BuffProps {
        public float Duration;
        public float CD;
        public ActionIds Icon;
        public ElementColor Color;
        public Item[] Triggers;
        public bool IsPlayerOnly;
        public bool ShowPartyText;
    }

    public class BuffConfig {
        public readonly string Name;
        public readonly float Duration;
        public readonly float CD;

        public readonly ActionIds Icon;
        public readonly ElementColor Color;
        public readonly Item[] Triggers;
        public readonly bool IsPlayerOnly;
        public readonly bool ShowPartyText;

        public bool Enabled { get; private set; }
        public bool PartyListHighlight { get; private set; }

        public BuffConfig(string name, BuffProps props) {
            Name = name;
            Duration = props.Duration;
            CD = props.CD;
            Icon = props.Icon;
            Color = props.Color;
            Triggers = props.Triggers;
            IsPlayerOnly = props.IsPlayerOnly;
            ShowPartyText = props.ShowPartyText;

            Enabled = JobBars.Config.BuffEnabled.Get(Name);
            PartyListHighlight = JobBars.Config.BuffPartyListHighlight.Get(Name);
        }

        public void Draw(string _id, ref bool reset) {
            var color = Enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1);

            ImGui.PushStyleColor(ImGuiCol.Text, color);
            if (ImGui.CollapsingHeader($"{Name}{_id}")) {
                ImGui.PopStyleColor();
                ImGui.Indent();

                if (JobBars.Config.BuffEnabled.Draw($"Enabled{_id}{Name}", Name, Enabled, out var newEnabled)) {
                    Enabled = newEnabled;
                    reset = true;
                }

                if (JobBars.Config.BuffPartyListHighlight.Draw($"Highlight party members when active{_id}{Name}", Name, PartyListHighlight, out var newPartyListHighlight)) {
                    PartyListHighlight = newPartyListHighlight;
                }

                ImGui.Unindent();
            }
            else {
                ImGui.PopStyleColor();
            }
        }
    }
}
