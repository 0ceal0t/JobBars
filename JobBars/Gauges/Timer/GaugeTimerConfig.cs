using ImGuiNET;
using JobBars.Atk;

namespace JobBars.Gauges.Timer {
    public struct GaugeSubTimerProps {
        public string SubName;
        public float MaxDuration;
        public bool NoRefresh;
        public bool HideLowWarning;
        public Item[] Triggers;
        public ElementColor Color;
        public bool Invert;
        public float DefaultDuration;
    }

    public struct GaugeTimerProps {
        public GaugeSubTimerProps[] SubTimers;
    }

    public class GaugeTimerConfig : GaugeConfig {
        public class GaugeSubTimerConfig {
            public readonly string Name;

            public readonly string SubName;
            public readonly float MaxDuration;
            public readonly float DefaultDuration;
            public readonly bool NoRefresh;
            public readonly Item[] Triggers;
            public readonly bool HideLowWarning; // Visual low warning
            public ElementColor Color;
            public bool Invert;
            public float Offset;
            public float LowWarningTime;

            public GaugeSubTimerConfig( string name, GaugeSubTimerProps props ) {
                Name = name;

                SubName = props.SubName;
                MaxDuration = props.MaxDuration;
                DefaultDuration = props.DefaultDuration == 0 ? props.MaxDuration : props.DefaultDuration;
                NoRefresh = props.NoRefresh;
                Triggers = props.Triggers;
                HideLowWarning = props.HideLowWarning;
                Color = JobBars.Configuration.GaugeColor.Get( Name, props.Color );
                Invert = JobBars.Configuration.GaugeInvert.Get( Name, props.Invert );
                Offset = JobBars.Configuration.GaugeTimerOffset.Get( Name );
                LowWarningTime = JobBars.Configuration.GaugeLowTimerWarning_2.Get( Name );
            }
        }

        // ===============================

        private static readonly GaugeVisualType[] ValidGaugeVisualType = [GaugeVisualType.Bar, GaugeVisualType.Diamond];
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public GaugeSubTimerConfig[] SubTimers { get; private set; }

        public GaugeTimerConfig( string name, GaugeVisualType type, GaugeSubTimerProps subConfig ) : this( name, type, new GaugeTimerProps {
            SubTimers = [subConfig]
        } ) { }

        public GaugeTimerConfig( string name, GaugeVisualType type, GaugeTimerProps props ) : base( name, type ) {
            SubTimers = new GaugeSubTimerConfig[props.SubTimers.Length];
            for( var i = 0; i < SubTimers.Length; i++ ) {
                var id = string.IsNullOrEmpty( props.SubTimers[i].SubName ) ? Name : Name + "/" + props.SubTimers[i].SubName;
                SubTimers[i] = new GaugeSubTimerConfig( id, props.SubTimers[i] );
            }
        }

        public override GaugeTracker GetTracker( int idx ) => new GaugeTimerTracker( this, idx );

        protected override void DrawConfig( string id, ref bool newVisual, ref bool reset ) {
            DrawSoundEffect( "Low warning sound effect" );

            foreach( var subTimer in SubTimers ) {
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 10 );

                var suffix = string.IsNullOrEmpty( subTimer.SubName ) ? "" : $" ({subTimer.SubName})";

                if( JobBars.Configuration.GaugeColor.Draw( $"Color{suffix}{id}", subTimer.Name, subTimer.Color, out var newColor ) ) {
                    subTimer.Color = newColor;
                    newVisual = true;
                }

                if( JobBars.Configuration.GaugeTimerOffset.Draw( $"Time offset{suffix}{id}", subTimer.Name, subTimer.Offset, out var newOffset ) ) {
                    subTimer.Offset = newOffset;
                }

                if( JobBars.Configuration.GaugeInvert.Draw( $"Invert{suffix}{id}", subTimer.Name, subTimer.Invert, out var newInvert ) ) {
                    subTimer.Invert = newInvert;
                }

                if( JobBars.Configuration.GaugeLowTimerWarning_2.Draw( $"Low warning time{suffix}{id}", subTimer.Name, subTimer.LowWarningTime, out var newLowWarning ) ) {
                    subTimer.LowWarningTime = newLowWarning;
                }
            }
        }
    }
}
