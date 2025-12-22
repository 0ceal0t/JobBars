using JobBars.Atk;
using JobBars.Helper;
using JobBars.Nodes.Gauge.Bar;
using JobBars.Nodes.Gauge.Diamond;
using System.Numerics;

namespace JobBars.Nodes.Gauge.BarDiamondCombo {
    public class BarDiamondComboNode : IGaugeNode {
        private readonly BarNode Bar;
        private readonly DiamondNode Diamond;

        public BarDiamondComboNode( BarNode bar, DiamondNode diamond ) {
            Bar = bar;
            Diamond = diamond;
        }

        public void SetPosition( Vector2 pos ) {
            Bar.Position = pos;
            Diamond.Position = new( pos.X, pos.Y + 10 );
        }

        public void SetScale( float scale ) {
            Bar.Scale = new( scale, scale );
            Diamond.Scale = new( scale, scale );
        }

        public void SetVisible( bool visible ) {
            Bar.IsVisible = visible;
            Diamond.IsVisible = visible;
        }

        public void SetSegments( float[] segments ) => Bar.SetSegments( segments );

        public void ClearSegments() => Bar.ClearSegments();

        public void SetText( string text ) => Bar.SetText( text );

        public void SetTextColor( ElementColor color ) => Bar.SetTextColor( color );

        public void SetBarTextVisible( bool visible ) => Bar.SetTextVisible( visible );

        public void SetBarColor( ElementColor color ) => Bar.SetColor( color );

        public void SetDiamondVlaue( int idx, bool value ) => Diamond.SetValue( idx, value );

        public void SetDiamondColor( int idx, ElementColor color ) => Diamond.SetColor( idx, color );

        public void SetDiamondValue( int idx, bool value ) => Diamond.SetValue( idx, value );

        public void SetMaxValue( int value ) {
            Diamond.SetMaxValue( value );
            Diamond.SetTextVisible( false );
        }

        public void SetPercent( float value ) => Bar.SetPercent( value );

        public void Clear() => Diamond.Clear();

        public unsafe void SetSplitPosition( Vector2 pos ) {
            var p = UiHelper.GetGlobalPosition( JobBars.NodeBuilder.GaugeRoot.Node );
            var pScale = UiHelper.GetGlobalScale( JobBars.NodeBuilder.GaugeRoot.Node );

            var x = ( pos.X - p.X ) / pScale.X;
            var y = ( pos.Y - p.Y ) / pScale.Y;

            Bar.Position = new( x, y );
            Diamond.Position = new( x, y + 10 );
        }
    }
}
