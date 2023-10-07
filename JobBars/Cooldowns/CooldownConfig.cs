using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using JobBars.Data;
using System.Numerics;

namespace JobBars.Cooldowns {
    public struct CooldownProps {
        public ActionIds Icon;
        public Item[] Triggers;
        public float Duration;
        public float CD;
    }

    public class CooldownConfig {
        public readonly string Name;
        public readonly string NameId;
        public readonly ActionIds Icon;
        public readonly Item[] Triggers;
        public readonly float Duration;
        public readonly float CD;

        public bool Enabled { get; private set; }
        public int Order { get; private set; }
        public bool ShowBorderWhenActive { get; private set; }
        public bool ShowBorderWhenOffCD { get; private set; }

        public CooldownConfig( string name, CooldownProps props ) : this( name, name, props ) { }

        public CooldownConfig( string name, string id, CooldownProps props ) {
            Name = name;
            NameId = id;
            Icon = props.Icon;
            Triggers = props.Triggers;
            Duration = props.Duration;
            CD = props.CD;
            Enabled = JobBars.Configuration.CooldownEnabled.Get( NameId );
            Order = JobBars.Configuration.CooldownOrder.Get( NameId );
            ShowBorderWhenActive = JobBars.Configuration.CooldownShowBorderWhenActive.Get( NameId );
            ShowBorderWhenOffCD = JobBars.Configuration.CooldownShowBorderWhenOffCD.Get( NameId );
        }

        public bool Draw( string _id, bool isCustom, ref bool reset ) {
            var deleteCustom = false;
            var color = Enabled ? new Vector4( 0, 1, 0, 1 ) : new Vector4( 1, 0, 0, 1 );

            using var _ = ImRaii.PushId( $"{_id}{NameId}" );

            ImGui.PushStyleColor( ImGuiCol.Text, color );
            if( ImGui.CollapsingHeader( Name ) ) {
                ImGui.PopStyleColor();
                ImGui.Indent();

                if( isCustom ) {
                    if( JobBars.RemoveButton( $"Delete", true ) ) deleteCustom = true;
                }

                if( JobBars.Configuration.CooldownEnabled.Draw( "Enabled", NameId, Enabled, out var newEnabled ) ) {
                    Enabled = newEnabled;
                    reset = true;
                }

                if( JobBars.Configuration.CooldownOrder.Draw( "Order", NameId, Order, out var newOrder ) ) {
                    Order = newOrder;
                    reset = true;
                }

                if( JobBars.Configuration.CooldownShowBorderWhenActive.Draw( "Show border when active", NameId, ShowBorderWhenActive, out var newShowBorderWhenActive ) ) {
                    ShowBorderWhenActive = newShowBorderWhenActive;
                }

                if( JobBars.Configuration.CooldownShowBorderWhenOffCD.Draw( "Show border when off CD", NameId, ShowBorderWhenOffCD, out var newShowBorderWhenOffCD ) ) {
                    ShowBorderWhenOffCD = newShowBorderWhenOffCD;
                }

                ImGui.Unindent();
            }
            else {
                ImGui.PopStyleColor();
            }

            return deleteCustom;
        }
    }
}
