using JobBars.Gauges.Manager;
using JobBars.Nodes.Gauge.Arrow;
using JobBars.Nodes.Gauge.Bar;
using JobBars.Nodes.Gauge.BarDiamondCombo;
using JobBars.Nodes.Gauge.Diamond;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Overlay.UiOverlay;
using System.Collections.Generic;

namespace JobBars.Nodes.Gauge {
    public class GaugeRoot : OverlayNode {
        public override OverlayLayer OverlayLayer => OverlayLayer.AboveUserInterface;

        public static readonly int MAX_GAUGES = 7;

        public readonly List<BarNode> Bars = [];
        public readonly List<ArrowNode> Arrows = [];
        public readonly List<DiamondNode> Diamonds = [];
        public readonly List<BarDiamondComboNode> BarDiamondCombos = [];

        private readonly GaugeManager Manager;

        public GaugeRoot( GaugeManager manager ) {
            Manager = manager;
            Size = new( 256, 100 );

            for( var i = 0; i < MAX_GAUGES; i++ ) {
                Bars.Add( new BarNode() );
                Arrows.Add( new ArrowNode() );
                Diamonds.Add( new DiamondNode() );
                BarDiamondCombos.Add( new( Bars[^1], Diamonds[^1] ) );
            }

            var allGauges = new List<NodeBase>();
            allGauges.AddRange( Bars );
            allGauges.AddRange( Arrows );
            allGauges.AddRange( Diamonds );

            allGauges.ForEach( x => x.AttachNode( this ) );
        }

        public void HideAll() {
            foreach( var item in Bars ) item.IsVisible = false;
            foreach( var item in Arrows ) item.IsVisible = false;
            foreach( var item in Diamonds ) item.IsVisible = false;
        }

        protected override void OnUpdate() => Manager.Tick();
    }
}
