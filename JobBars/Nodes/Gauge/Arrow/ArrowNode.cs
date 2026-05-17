using JobBars.Atk;
using JobBars.Data;
using JobBars.Gauges.Types.Arrow;
using System.Collections.Generic;

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

            if( value && !prevVisible ) Animation.AddAnim( f => Ticks[idx].Selected.Scale = new( f, f ), 0.2f, 2.5f, 1.0f );
        }

        public void Clear() {
            for( var idx = 0; idx < MAX_ITEMS; idx++ ) SetValue( idx, false );
        }

        // ====================

        public void Tick( IGaugeArrowInterface tracker,  float percent ) {
            SetVisible( !tracker.GetConfig().HideWhenInactive || tracker.GetActive() );
            SetScale( tracker.GetConfig().Scale );

            SetMaxValue( tracker.GetTotalMaxTicks() );
            Clear();

            for( var i = 0; i < tracker.GetCurrentMaxTicks(); i++ ) {
                var trackerIndex = tracker.GetReverseFill() ? ( tracker.GetCurrentMaxTicks() - i - 1 ) : i;
                SetValue( i, tracker.GetTickValue( trackerIndex ) );
                SetColor( i, tracker.GetTickColor( trackerIndex ) );
            }

            Ticks.ForEach( t => t.Tick( percent ) ); // pulse
        }

        public int GetHeight( IGaugeArrowInterface tracker ) => ( int )( tracker.GetConfig().Scale * 32 );

        public int GetWidth( IGaugeArrowInterface tracker ) => ( int )( tracker.GetConfig().Scale * ( 32 + 18 * ( tracker.GetCurrentMaxTicks() - 1 ) ) );
    }
}
