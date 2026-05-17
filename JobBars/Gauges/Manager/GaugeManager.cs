using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;
using JobBars.Nodes.Buff;
using JobBars.Nodes.Builder;
using JobBars.Nodes.Gauge;
using KamiToolKit.Controllers;
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

        private MultiAddonController? Controller;
        private GaugeRoot? Root;

        public GaugeManager() : base( "##JobBars_Gauges" ) {
            Controller = new MultiAddonController {
                AddonNames = ["ChatLog", "_ParameterWidget", "_PartyList"],
                OnSetup = SetupAddon,
                OnFinalize = ResetAddon,
                OnUpdate = UpdateAddon
            };
            Controller.Enable();
        }

        public void Hide() {
            Root?.IsVisible = false;
        }

        private void SetupAddon( AtkUnitBase* addon ) {
            if( addon->NameString == UiHelper.BuffGaugeAttachAddonName ) {
                Root = new();
                Root.AttachNode( addon );
            }
        }

        private void ResetAddon( AtkUnitBase* addon ) {
            if( addon->NameString == UiHelper.BuffGaugeAttachAddonName ) {
                Root?.Dispose();
                Root = null;
            }
        }

        private void UpdateAddon( AtkUnitBase* addon ) {
            if( addon->NameString == UiHelper.BuffGaugeAttachAddonName ) {
                Tick();
            }
        }

        public void Dispose() {
            Controller?.Dispose();
            Controller = null;
        }

        public void SetJob( JobIds job ) {
            CurrentGauges.Clear();
            Root?.HideAll();

            CurrentJob = job;
            for( var idx = 0; idx < CurrentConfigs.Length; idx++ ) {
                CurrentGauges.Add( CurrentConfigs[idx].GetTracker( idx ) );
            }
        }

        public void PerformAction( Item action ) {
            if( !JobBars.Configuration.GaugesEnabled ) return;

            foreach( var gauge in CurrentGauges.Where( g => g.Enabled ) ) gauge.ProcessAction( action );
        }

        private void Tick() {
            if( Root == null ) return;

            // Visibility

            if( UiHelper.CalcDoHide( JobBars.Configuration.GaugesEnabled, JobBars.Configuration.GaugesHideOutOfCombat, JobBars.Configuration.GaugesHideWeaponSheathed ) ) {
                Root!.IsVisible = false;
                return;
            }
            else {
                Root!.IsVisible = true;
            }

            // Global position + scale

            NodeBuilder.SetPositionGlobal( Root,
                JobBars.Configuration.GaugePositionType == GaugePositionType.PerJob ? GetPerJobPosition() : JobBars.Configuration.GaugePositionGlobal );
            NodeBuilder.SetScaleGlobal( Root, JobBars.Configuration.GaugeScale );

            // Evaluate gauges

            if( CurrentJob == JobIds.SCH && !UiHelper.OutOfCombat ) { // only need this to catch excog for now
                JobBars.SearchForPartyMemberStatus( ( int )Dalamud.Objects.LocalPlayer.GameObjectId, UiHelper.PlayerStatus, GaugeBuffsOnPartyMembers );
            }

            foreach( var gauge in CurrentGauges.Where( g => g.Enabled ) ) gauge.Tick();

            // Pulses

            var time = DateTime.Now;
            var millis = time.Second * 1000 + time.Millisecond;
            var percent = ( float )( millis % 1000 ) / 1000;
            Root?.Arrows.ForEach( x => x.Tick( percent ) );
            Root?.Diamonds.ForEach( x => x.Tick( percent ) );
        }

        private Vector2 GetPerJobPosition() => JobBars.Configuration.GaugePerJobPosition.Get( $"{CurrentJob}" );

        /*public void UpdatePositionScale() {
            if( Root == null ) return;

            // Global postion + scale

            NodeBuilder.SetPositionGlobal( Root,
                JobBars.Configuration.GaugePositionType == GaugePositionType.PerJob ? GetPerJobPosition() : JobBars.Configuration.GaugePositionGlobal );
            NodeBuilder.SetScaleGlobal( Root, JobBars.Configuration.GaugeScale );

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
        }*/

        public void Reset() => SetJob( CurrentJob );
    }
}
