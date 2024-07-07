using JobBars.Atk;
using System;

namespace JobBars.Gauges.Rolling {
    public enum GaugeGCDRollingType {
        GCD,
        CastTime
    }

    public class GaugeRollingConfig : GaugeConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = [GaugeVisualType.Bar];
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public static readonly GaugeGCDRollingType[] ValidRollingType = ( GaugeGCDRollingType[] )Enum.GetValues( typeof( GaugeGCDRollingType ) );

        public ElementColor Color { get; private set; }
        public GaugeGCDRollingType RollingType { get; private set; }

        public GaugeRollingConfig( string name, GaugeVisualType type ) : base( name, type ) {
            Enabled = JobBars.Configuration.GaugeEnabled.Get( Name, false ); // default disabled
            Color = JobBars.Configuration.GaugeColor.Get( Name, ColorConstants.Yellow );
            RollingType = JobBars.Configuration.GaugeGCDRolling.Get( Name, GaugeGCDRollingType.GCD );
        }

        public override GaugeTracker GetTracker( int idx ) => new GaugeRollingTracker( this, idx );

        protected override void DrawConfig( string id, ref bool newVisual, ref bool reset ) {
            if( JobBars.Configuration.GaugeGCDRolling.Draw( $"Data type{id}", Name, ValidRollingType, RollingType, out var newRollingType ) ) {
                RollingType = newRollingType;
                newVisual = true;
            }

            if( JobBars.Configuration.GaugeColor.Draw( $"Color{id}", Name, Color, out var newColor ) ) {
                Color = newColor;
                newVisual = true;
            }
        }
    }
}
