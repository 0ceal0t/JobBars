using JobBars.Helper;
using System.Numerics;

namespace JobBars.Atk {
    public unsafe class AtkBarDiamondCombo : AtkGauge {
        private readonly AtkBar Gauge;
        private readonly AtkDiamond Diamond;

        public AtkBarDiamondCombo( AtkBar gauge, AtkDiamond diamond ) {
            Gauge = gauge;
            Diamond = diamond;
            RootRes = gauge.RootRes;

            Gauge.SetLayout( false, false );
            Diamond.SetTextVisible( false );
        }

        public override void Dispose() { }

        public override void Hide() {
            AtkHelper.Hide( RootRes );
            AtkHelper.Hide( Diamond.RootRes );
        }

        public override void Show() {
            AtkHelper.Show( RootRes );
            AtkHelper.Show( Diamond.RootRes );
        }

        public void SetText( string text ) => Gauge.SetText( text );
        public void SetTextColor( ElementColor color ) => Gauge.SetTextColor( color );
        public void SetBarTextVisible( bool visible ) => Gauge.SetTextVisible( visible );

        public void SetSegments( float[] segments ) => Gauge.SetSegments( segments );
        public void ClearSegments() => Gauge.ClearSegments();

        public void SetPercent( float value ) => Gauge.SetPercent( value );
        public void SetMaxValue( int value ) {
            Diamond.SetMaxValue( value );
            Diamond.SetTextVisible( false );
        }

        public void Clear() => Diamond.Clear();
        public void SetDiamondValue( int idx, bool value ) => Diamond.SetValue( idx, value );

        public void SetGaugeColor( ElementColor color ) => Gauge.SetColor( color );

        public void SetDiamondColor( int idx, ElementColor color ) => Diamond.SetColor( idx, color );

        public override void SetSplitPosition( Vector2 pos ) {
            var p = AtkHelper.GetNodePosition( JobBars.Builder.GaugeRoot );
            var pScale = AtkHelper.GetNodeScale( JobBars.Builder.GaugeRoot );
            var x = ( pos.X - p.X ) / pScale.X;
            var y = ( pos.Y - p.Y ) / pScale.Y;

            AtkHelper.SetPosition( Gauge.RootRes, x, y );
            AtkHelper.SetPosition( Diamond.RootRes, x, y + 10 );
        }

        public override void SetScale( float scale ) {
            AtkHelper.SetScale( Gauge.RootRes, scale, scale );
            AtkHelper.SetScale( Diamond.RootRes, scale, scale );
        }

        public override void SetPosition( Vector2 pos ) {
            AtkHelper.SetPosition( Gauge.RootRes, pos.X, pos.Y );
            AtkHelper.SetPosition( Diamond.RootRes, pos.X, pos.Y + 10 );
        }
    }
}
