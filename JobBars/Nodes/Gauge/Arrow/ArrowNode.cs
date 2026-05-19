using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
using JobBars.Gauges.Types.Arrow;
using KamiToolKit.Timelines;
using System.Collections.Generic;

namespace JobBars.Nodes.Gauge.Arrow {
    public class ArrowNode : GaugeNode {
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

        public void SetValue( int idx, bool value ) => Ticks[idx].SetValue( value );

        public void Clear() {
            foreach( var tick in Ticks ) {
                tick.Selected.IsVisible = false;
            }
        }

        // ====================

        public void Tick( IGaugeArrowInterface tracker ) {
            SetVisible( !tracker.GetConfig().HideWhenInactive || tracker.GetActive() );
            SetScale( tracker.GetConfig().Scale );

            SetMaxValue( tracker.GetTotalMaxTicks() );
            Clear();

            for( var i = 0; i < tracker.GetCurrentMaxTicks(); i++ ) {
                var trackerIndex = tracker.GetReverseFill() ? ( tracker.GetCurrentMaxTicks() - i - 1 ) : i;
                SetValue( i, tracker.GetTickValue( trackerIndex ) );
                SetColor( i, tracker.GetTickColor( trackerIndex ) );
            }
        }

        public int GetHeight( IGaugeArrowInterface tracker ) => ( int )( tracker.GetConfig().Scale * 32 );

        public int GetWidth( IGaugeArrowInterface tracker ) => ( int )( tracker.GetConfig().Scale * ( 32 + 18 * ( tracker.GetCurrentMaxTicks() - 1 ) ) );
    }
}
