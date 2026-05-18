using JobBars.Data;
using JobBars.Gauges.Types.Arrow;
using JobBars.Gauges.Types.Bar;
using JobBars.Gauges.Types.BarDiamondCombo;
using JobBars.Gauges.Types.Diamond;
using JobBars.Helper;
using JobBars.Nodes.Gauge;
using KamiToolKit.Overlay.UiOverlay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace JobBars.Gauges.Manager {
    public unsafe partial class GaugeManager : PerJobManagerNested<GaugeConfig> {
        public JobIds CurrentJob = JobIds.OTHER;
        private GaugeConfig[] CurrentConfigs => JobToValue.TryGetValue( CurrentJob, out var configs ) ? configs : JobToValue[JobIds.OTHER];
        private readonly List<GaugeTracker> CurrentGauges = [];

        private static readonly List<BuffIds> GaugeBuffsOnPartyMembers = new( [BuffIds.Excog] ); // which buffs on party members do we care about?

        private OverlayController? Controller;
        private GaugeRoot? Root;

        public GaugeManager() : base( "##JobBars_Gauges" ) {
            Controller = new();
            Controller.CreateNode( () => {
                Root = new( this );
                return Root;
            } );
        }

        public void Hide() {
            Root?.IsVisible = false;
        }

        public void Dispose() {
            Controller?.Dispose();
            Controller = null;
            Root = null;
        }

        public void SetJob( JobIds job ) {
            Root?.HideAll();
            CurrentGauges.Clear();
            CurrentJob = job;
            CurrentGauges.AddRange( CurrentConfigs.Select( x => x.GetTracker() ) );
        }

        public void PerformAction( Item action ) {
            if( !JobBars.Configuration.GaugesEnabled ) return;

            foreach( var gauge in CurrentGauges.Where( g => g.Enabled ) ) gauge.ProcessAction( action );
        }

        public void Tick() {
            if( Root == null ) return;

            // Visibility

            if( UiHelper.CalcDoHide( JobBars.Configuration.GaugesEnabled, JobBars.Configuration.GaugesHideOutOfCombat, JobBars.Configuration.GaugesHideWeaponSheathed ) ) {
                Root!.IsVisible = false;
                return;
            }
            else {
                Root!.IsVisible = true;
            }
            Root.HideAll();

            // Global position + scale

            Root.Position = JobBars.Configuration.GaugePositionType == GaugePositionType.PerJob ? GetPerJobPosition() : JobBars.Configuration.GaugePositionGlobal;
            Root.Scale = new( JobBars.Configuration.GaugeScale, JobBars.Configuration.GaugeScale );

            // Evaluate gauges

            if( CurrentJob == JobIds.SCH && !UiHelper.OutOfCombat ) { // only need this to catch excog for now
                JobBars.SearchForPartyMemberStatus( ( int )Dalamud.Objects.LocalPlayer.GameObjectId, UiHelper.PlayerStatus, GaugeBuffsOnPartyMembers );
            }

            // Pulses

            var time = DateTime.Now;
            var millis = time.Second * 1000 + time.Millisecond;
            var percent = ( float )( millis % 1000 ) / 1000;

            foreach( var gauge in CurrentGauges.Where( g => g.Enabled ) ) gauge.TickTracker();

            var arrowIdx = 0;
            var barIdx = 0;
            var diamondIdx = 0;
            var position = 0;

            foreach( var gauge in CurrentGauges.Where( x => x.Enabled ).OrderBy( x => x.Order ) ) {
                IGaugeNode? node = null;
                var requestedType = gauge.GetConfig().Type;
                var width = 0;
                var height = 0;
                var yOffset = 0;

                // Kind of jank but oh well
                if( requestedType == GaugeVisualType.Bar && gauge is IGaugeBarInterface barTracker && barIdx <= GaugeRoot.MAX_GAUGES ) {
                    var bar = Root.Bars[barIdx];
                    node = bar;
                    bar.Tick( barTracker );

                    width = bar.GetWidth( barTracker );
                    height = bar.GetHeight( barTracker );
                    yOffset = 0;

                    barIdx++;
                }
                else if( requestedType == GaugeVisualType.Diamond && gauge is IGaugeDiamondInterface diamondTracker && diamondIdx <= GaugeRoot.MAX_GAUGES ) {
                    var diamond = Root.Diamonds[diamondIdx];
                    node = diamond;
                    diamond.Tick( diamondTracker, percent );

                    width = diamond.GetWidth( diamondTracker );
                    height = diamond.GetHeight( diamondTracker );
                    yOffset = -3;

                    diamondIdx++;
                }
                else if( requestedType == GaugeVisualType.Arrow && gauge is IGaugeArrowInterface arrowTracker && arrowIdx <= GaugeRoot.MAX_GAUGES ) {
                    var arrow = Root.Arrows[arrowIdx];
                    node = arrow;
                    arrow.Tick( arrowTracker, percent );

                    width = arrow.GetWidth( arrowTracker );
                    height = arrow.GetHeight( arrowTracker );
                    yOffset = -3;

                    arrowIdx++;
                }
                else if( requestedType == GaugeVisualType.BarDiamondCombo && gauge is IGaugeBarDiamondComboInterface comboTracker && barIdx <= GaugeRoot.MAX_GAUGES ) {
                    var combo = Root.BarDiamondCombos[barIdx];
                    node = combo;
                    combo.Tick( comboTracker, percent );

                    width = combo.GetWidth( comboTracker );
                    height = combo.GetHeight( comboTracker );
                    yOffset = 0;

                    barIdx++;
                    diamondIdx++;
                }

                if( JobBars.Configuration.GaugePositionType == GaugePositionType.Split ) {
                    node?.SetSplitPosition( Root, gauge.GetConfig().Position );
                }
                else {
                    var x = JobBars.Configuration.GaugeHorizontal ? position :
                        ( JobBars.Configuration.GaugeAlignRight ? 160 - width : 0 );

                    var y = JobBars.Configuration.GaugeHorizontal ? yOffset :
                        ( JobBars.Configuration.GaugeBottomToTop ? position - height : position );

                    var posChange = JobBars.Configuration.GaugeHorizontal ? width :
                        ( JobBars.Configuration.GaugeBottomToTop ? -1 * height : height );

                    node?.SetPosition( new Vector2( x, y ) );
                    position += posChange;
                }
            }
        }

        private Vector2 GetPerJobPosition() => JobBars.Configuration.GaugePerJobPosition.Get( $"{CurrentJob}" );

        public void Reset() => SetJob( CurrentJob );
    }
}
