using JobBars.Atk;
using JobBars.Data;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Nodes.Gauge.Arrow {
    public unsafe class ArrowNode : GaugeNode {
        public readonly List<ArrowTick> Ticks = [];

        public static readonly int MAX_ITEMS = 12;

        public ArrowNode() : base() {
            Size = new( 160, 46 );

            for( var idx = 0; idx < MAX_ITEMS; idx++ ) {
                var tick = new ArrowTick {
                    Position = new( 18 * idx, 0 )
                };
                Ticks.Add( tick );
            }

            Ticks.ForEach( x => x.AttachNode(this) );
        }

        public void SetMaxValue( int value ) {
            for( var idx = 0; idx < MAX_ITEMS; idx++ ) Ticks[idx].IsVisible = idx < value;
        }

        public void SetColor( int idx, ElementColor color ) => Ticks[idx].SetColor( color );

        public void SetValue( int idx, bool value ) {
            var prevVisible = Ticks[idx].Selected.IsVisible;
            Ticks[idx].Selected.IsVisible = value;

            if( value && !prevVisible ) Animation.AddAnim( ( float f ) => Ticks[idx].Selected.Scale = new( f, f ), 0.2f, 2.5f, 1.0f );
        }

        public void Clear() {
            for( var idx = 0; idx < MAX_ITEMS; idx++ ) SetValue( idx, false );
        }

        public void Tick( float percent ) => Ticks.ForEach( t => t.Tick( percent ) );
    }
}
