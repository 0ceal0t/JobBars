using ImGuiNET;
using JobBars.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UIIconComboType = JobBars.Atk.UIIconComboType;
using UIIconProps = JobBars.Atk.UIIconProps;

namespace JobBars.Icons {
    public abstract class IconReplacer {
        public static readonly UIIconComboType[] ValidComboTypes = ( UIIconComboType[] )Enum.GetValues( typeof( UIIconComboType ) );

        public enum IconState {
            Inactive,
            Active
        }

        public bool Enabled;

        public readonly string Name;
        protected readonly bool IsTimer;
        protected readonly List<uint> Icons;
        protected IconState State = IconState.Inactive;
        protected UIIconComboType ComboType;
        protected UIIconProps IconProps;
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
            CreateIconProps();
        }

        private void CreateIconProps() {
            IconProps = new UIIconProps {
                IsTimer = IsTimer,
                ComboType = ComboType,
                ShowRing = ShowRing
            };
        }

        public void Setup() {
            State = IconState.Inactive;
            if( !Enabled ) return;
            JobBars.IconBuilder.Setup( Icons, IconProps );
        }

        public abstract void Tick();

        public abstract void ProcessAction( Item action );

        protected void SetIcon( float current, float duration ) => JobBars.IconBuilder.SetProgress( Icons, current, duration );

        protected void ResetIcon() => JobBars.IconBuilder.SetDone( Icons );

        public void Draw( string id, JobIds _ ) {
            var _ID = id + Name;
            var type = IsTimer ? "TIMER" : "BUFF";
            var color = Enabled ? new Vector4( 0, 1, 0, 1 ) : new Vector4( 1, 0, 0, 1 );

            ImGui.PushStyleColor( ImGuiCol.Text, color );
            if( ImGui.CollapsingHeader( $"{Name} [{type}]{_ID}" ) ) {
                ImGui.PopStyleColor();
                ImGui.Indent();

                if( JobBars.Configuration.IconEnabled.Draw( $"Enabled{_ID}", Name, Enabled, out var newEnabled ) ) {
                    Enabled = newEnabled;
                    JobBars.IconManager.Reset();
                }

                if( JobBars.Configuration.IconComboType.Draw( $"Dash border{_ID}", Name, ValidComboTypes, ComboType, out var newComboType ) ) {
                    ComboType = newComboType;
                    CreateIconProps();
                    JobBars.IconManager.Reset();
                }

                if( IsTimer ) {
                    if( JobBars.Configuration.IconTimerOffset.Draw( $"Time offset{_ID}", Name, Offset, out var newOffset ) ) {
                        Offset = newOffset;
                    }

                    if( JobBars.Configuration.IconTimerRing.Draw( $"Display ring{_ID}", Name, Enabled, out var newRing ) ) {
                        ShowRing = newRing;
                        CreateIconProps();
                        JobBars.IconManager.Reset();
                    }
                }

                ImGui.Unindent();
            }
            else {
                ImGui.PopStyleColor();
            }
        }
    }
}
