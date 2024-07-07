using System.Numerics;

namespace JobBars.Gauges {
    public abstract class GaugeTracker {
        public bool Disposed { get; private set; } = false;

        protected Gauge GaugeUi { get; private set; }

        public bool Enabled => GetConfig().Enabled;

        public int Order => GetConfig().Order;

        public int Height => GaugeUi?.GetHeight() ?? 0;

        public int Width => GaugeUi?.GetWidth() ?? 0;

        public int YOffset => GaugeUi?.GetYOffset() ?? 0;

        public abstract GaugeConfig GetConfig();

        public abstract bool GetActive();

        public abstract void ProcessAction( Item action );

        public void SetPosition( Vector2 position ) => GaugeUi?.SetPosition( position );

        public void UpdateSplitPosition() => GaugeUi?.SetSplitPosition( GetConfig().Position );

        public void UpdateVisual() => GaugeUi?.UpdateVisual();

        public void Tick() {
            if( GaugeUi == null ) return;
            TickTracker();
            GaugeUi.Tick();
        }

        public void Cleanup() {
            Disposed = true;
            if( GaugeUi == null ) return;
            GaugeUi = null;
        }

        protected void LoadUI( Gauge ui ) {
            GaugeUi = ui;
            if( GaugeUi == null ) return;
            GaugeUi.UpdateVisual();
        }

        protected abstract void TickTracker();

        public static R[] SplitArray<R>( R left, int size, bool reverse = false ) => SplitArray( left, left, size, size, reverse );
        public static R[] SplitArray<R>( R left, R right, int value, int size, bool reverse = false ) {
            var ret = new R[size];
            for( var i = 0; i < size; i++ ) ret[reverse ? ( size - i - 1 ) : i] = i < value ? left : right;
            return ret;
        }
    }
}
