using ImGuiNET;
using JobBars.Data;
using JobBars.GameStructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace JobBars.Icons {
    public abstract class IconReplacer {
        public enum IconComboType {
            Combo_Or_Active,
            Combo_And_Active,
            Only_When_Combo,
            Only_When_Active,
            Never
        }

        public static readonly IconComboType[] ValidComboTypes = ( IconComboType[] )Enum.GetValues( typeof( IconComboType ) );

        public bool Enabled;

        public readonly string Name;
        public readonly bool IsTimer;
        public readonly List<uint> Icons;
        protected IconComboType ComboType;
        protected float Offset;
        protected bool ShowRing;

        public IconReplacer( string name, bool isTimer, ActionIds[] icons ) {
            Name = name;
            IsTimer = isTimer;
            Icons = new List<ActionIds>( icons ).Select( x => ( uint )x ).ToList();
            Enabled = JobBars.Configuration.IconEnabled.Get( Name );
            ComboType = JobBars.Configuration.IconComboType.Get( Name );
            Offset = JobBars.Configuration.IconTimerOffset.Get( Name );
            ShowRing = JobBars.Configuration.IconTimerRing.Get( Name );
        }

        public abstract void Tick();

        public abstract void ProcessAction( Item action );

        public void Draw( string id, JobIds _ ) {
            var _ID = id + Name;
            var type = IsTimer ? "TIMER" : "BUFF";
            var color = Enabled ? new Vector4( 0, 1, 0, 1 ) : new Vector4( 1, 0, 0, 1 );

            ImGui.PushStyleColor( ImGuiCol.Text, color );
            if( ImGui.CollapsingHeader( $"{Name} [{type}]{_ID}" ) ) {
                ImGui.PopStyleColor();
                ImGui.Indent();

                if( JobBars.Configuration.IconEnabled.Draw( $"Enabled{_ID}", Name, Enabled, out var newEnabled ) ) Enabled = newEnabled;
                if( JobBars.Configuration.IconComboType.Draw( $"Dash border{_ID}", Name, ValidComboTypes, ComboType, out var newComboType ) ) ComboType = newComboType;

                if( IsTimer ) {
                    if( JobBars.Configuration.IconTimerOffset.Draw( $"Time offset{_ID}", Name, Offset, out var newOffset ) ) Offset = newOffset;
                    if( JobBars.Configuration.IconTimerRing.Draw( $"Display ring{_ID}", Name, Enabled, out var newRing ) ) ShowRing = newRing;
                }

                ImGui.Unindent();
            }
            else {
                ImGui.PopStyleColor();
            }
        }

        public bool AppliesTo( uint actionId ) => Enabled && Icons.Contains( actionId );

        public abstract unsafe void UpdateIcon( HotbarSlotStruct* data );
    }
}
