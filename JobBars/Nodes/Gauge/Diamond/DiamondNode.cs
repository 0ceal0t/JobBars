using JobBars.Atk;
using JobBars.Gauges.Types.Arrow;
using JobBars.Gauges.Types.Diamond;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using System.Collections.Generic;
using System.Diagnostics;
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
            if( text == null ) return;
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

        // ====================

        public void Tick( IGaugeDiamondInterface tracker, float percent ) {
            SetVisible( !tracker.GetConfig().HideWhenInactive || tracker.GetActive() );
            SetScale( tracker.GetConfig().Scale );

            SetMaxValue( tracker.GetTotalMaxTicks() );
            SetTextVisible( tracker.GetDiamondTextVisible() );
            Clear();

            for( var i = 0; i < tracker.GetCurrentMaxTicks(); i++ ) {
                var trackerIndex = tracker.GetReverseFill() ? ( tracker.GetCurrentMaxTicks() - i - 1 ) : i;
                SetValue( i, tracker.GetTickValue( trackerIndex ) );
                SetColor( i, tracker.GetTickColor( trackerIndex ) );
                if( tracker.GetDiamondTextVisible()) {
                    SetText( i, tracker.GetDiamondText( trackerIndex ) );
                }
            }

            Ticks.ForEach( t => t.Tick( percent ) ); // pulse
        }

        public int GetHeight( IGaugeDiamondInterface tracker ) => ( int )( tracker.GetConfig().Scale * ( tracker.GetDiamondTextVisible() ? 40 : 32 ) );

        public int GetWidth( IGaugeDiamondInterface tracker ) => ( int )( tracker.GetConfig().Scale * ( 32 + 20 * ( tracker.GetCurrentMaxTicks() - 1 ) ) );
    }
}
