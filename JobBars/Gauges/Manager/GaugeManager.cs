using JobBars.Data;
using JobBars.Helper;
using JobBars.Nodes.Builder;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace JobBars.Gauges.Manager {
    public partial class GaugeManager : PerJobManagerNested<GaugeConfig> {
        public JobIds CurrentJob = JobIds.OTHER;
        private GaugeConfig[] CurrentConfigs => JobToValue.TryGetValue( CurrentJob, out var configs ) ? configs : JobToValue[JobIds.OTHER];
        private readonly List<GaugeTracker> CurrentGauges = [];

        private static readonly List<BuffIds> GaugeBuffsOnPartyMembers = new( [BuffIds.Excog] ); // which buffs on party members do we care about?

        public GaugeManager() : base( "##JobBars_Gauges" ) { }

        public void SetJob( JobIds job ) {
            foreach( var gauge in CurrentGauges ) gauge.Cleanup();
            CurrentGauges.Clear();
            JobBars.NodeBuilder.GaugeRoot.HideAll();

            CurrentJob = job;
            for( var idx = 0; idx < CurrentConfigs.Length; idx++ ) {
                CurrentGauges.Add( CurrentConfigs[idx].GetTracker( idx ) );
            }
            UpdatePositionScale();
        }

        public void PerformAction( Item action ) {
            if( !JobBars.Configuration.GaugesEnabled ) return;

            foreach( var gauge in CurrentGauges.Where( g => g.Enabled && !g.Disposed ) ) gauge.ProcessAction( action );
        }

        public void Tick() {
            if( UiHelper.CalcDoHide( JobBars.Configuration.GaugesEnabled, JobBars.Configuration.GaugesHideOutOfCombat, JobBars.Configuration.GaugesHideWeaponSheathed ) ) {
                JobBars.NodeBuilder.GaugeRoot.IsVisible = false;
                return;
            }
            else {
                JobBars.NodeBuilder.GaugeRoot.IsVisible = true;
            }

            // ============================

            if( CurrentJob == JobIds.SCH && !UiHelper.OutOfCombat ) { // only need this to catch excog for now
                JobBars.SearchForPartyMemberStatus( ( int )Service.ClientState.LocalPlayer.GameObjectId, UiHelper.PlayerStatus, GaugeBuffsOnPartyMembers );
            }

            foreach( var gauge in CurrentGauges.Where( g => g.Enabled && !g.Disposed ) ) gauge.Tick();
        }

        private Vector2 GetPerJobPosition() => JobBars.Configuration.GaugePerJobPosition.Get( $"{CurrentJob}" );

        public void UpdatePositionScale() {
            NodeBuilder.SetPositionGlobal( JobBars.NodeBuilder.GaugeRoot,
                JobBars.Configuration.GaugePositionType == GaugePositionType.PerJob ? GetPerJobPosition() : JobBars.Configuration.GaugePositionGlobal );
            NodeBuilder.SetScaleGlobal( JobBars.NodeBuilder.GaugeRoot, JobBars.Configuration.GaugeScale );

            var position = 0;
            foreach( var gauge in CurrentGauges.OrderBy( g => g.Order ).Where( g => g.Enabled ) ) {
                if( JobBars.Configuration.GaugePositionType == GaugePositionType.Split ) {
                    gauge.UpdateSplitPosition();
                }
                else {
                    var x = JobBars.Configuration.GaugeHorizontal ? position :
                        ( JobBars.Configuration.GaugeAlignRight ? 160 - gauge.Width : 0 );

                    var y = JobBars.Configuration.GaugeHorizontal ? gauge.YOffset :
                        ( JobBars.Configuration.GaugeBottomToTop ? position - gauge.Height : position );

                    var posChange = JobBars.Configuration.GaugeHorizontal ? gauge.Width :
                        ( JobBars.Configuration.GaugeBottomToTop ? -1 * gauge.Height : gauge.Height );

                    gauge.SetPosition( new Vector2( x, y ) );
                    position += posChange;
                }
            }
        }

        public void UpdateVisuals() {
            foreach( var gauge in CurrentGauges ) gauge.UpdateVisual();
        }

        public void Reset() => SetJob( CurrentJob );
    }
}
