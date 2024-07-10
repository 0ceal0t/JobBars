using JobBars.Data;
using JobBars.GameStructs;
using System.Linq;

namespace JobBars.Icons.Manager {
    public unsafe partial class IconManager : PerJobManager<IconReplacer[]> {
        public JobIds CurrentJob = JobIds.OTHER;
        private IconReplacer[] CurrentIcons => JobToValue.TryGetValue( CurrentJob, out var gauges ) ? gauges : JobToValue[JobIds.OTHER];

        public IconManager() : base( "##JobBars_Icons" ) { }

        public void SetJob( JobIds job ) {
            CurrentJob = job;
        }

        public void Reset() {
            foreach( var item in CurrentIcons ) item.Reset();
        }

        public void ResetJob( JobIds job ) {
            if( job == CurrentJob ) Reset();
        }

        public void PerformAction( Item action ) {
            if( !JobBars.Configuration.IconsEnabled ) return;
            foreach( var icon in CurrentIcons.Where( i => i.Enabled ) ) icon.ProcessAction( action );
        }

        public void Tick() {
            if( !JobBars.Configuration.IconsEnabled ) return;
            foreach( var icon in CurrentIcons.Where( i => i.Enabled ) ) icon.Tick();
        }

        public void UpdateIcon( HotbarSlotStruct* data ) {
            if( !JobBars.Configuration.IconsEnabled ) return;
            CurrentIcons.FirstOrDefault( i => i.AppliesTo( data->ActionId ) )?.UpdateIcon( data );
        }
    }
}
