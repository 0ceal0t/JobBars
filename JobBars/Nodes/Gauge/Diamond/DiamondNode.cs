using JobBars.Atk;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Nodes.Gauge.Diamond {
    public unsafe class DiamondNode : GaugeNode {
        public readonly List<DiamondTick> Ticks = [];

        public static readonly int MAX_ITEMS = 12;

        public DiamondNode() : base() {
            Size = new( 160, 46 );

            for( var idx = 0; idx < MAX_ITEMS; idx++ ) {
                var tick = new DiamondTick {
                    Position = new( 20 * idx, 0 )
                };
                Ticks.Add( tick );
            }

            Ticks.ForEach( x => x.AttachNode( this ) );
        }

        public void SetMaxValue( int value ) {
            for( var idx = 0; idx < MAX_ITEMS; idx++ ) Ticks[idx].IsVisible = idx < value;
        }

        public void SetTextVisible( bool showText ) {
            SetSpacing( showText ? 5 : 0 );
            foreach( var tick in Ticks ) tick.Text.IsVisible = showText;
        }

        private void SetSpacing( int spacing ) {
            for( var idx = 0; idx < MAX_ITEMS; idx++ ) Ticks[idx].X = ( 20 + spacing ) * idx;
        }

        public void SetColor( int idx, ElementColor color ) => Ticks[idx].SetColor( color );

        public void SetValue( int idx, bool value ) {
            Ticks[idx].SelectedContainer.IsVisible = value;
        }

        public void SetText( int idx, string text ) {
            Ticks[idx].Text.String = text;
            Ticks[idx].Text.IsVisible = true;
        }

        public void ShowText( int idx ) {
            Ticks[idx].Text.IsVisible = true;
        }

        public void HideText( int idx ) {
            Ticks[idx].Text.IsVisible = false;
        }

        public void Clear() {
            for( var idx = 0; idx < MAX_ITEMS; idx++ ) SetValue( idx, false );
        }

        public void Tick( float percent ) => Ticks.ForEach( t => t.Tick( percent ) );
    }
}
