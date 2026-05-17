using JobBars.Nodes.Gauge;
using System.Numerics;

namespace JobBars.Gauges {
    public abstract class GaugeTracker {
        public bool Enabled => GetConfig().Enabled;

        public int Order => GetConfig().Order;

        public abstract GaugeConfig GetConfig();

        public abstract bool GetActive();

        public abstract void ProcessAction( Item action );

        public abstract void TickTracker();

        public static R[] SplitArray<R>( R left, int size, bool reverse = false ) => SplitArray( left, left, size, size, reverse );
        public static R[] SplitArray<R>( R left, R right, int value, int size, bool reverse = false ) {
            var ret = new R[size];
            for( var i = 0; i < size; i++ ) ret[reverse ? ( size - i - 1 ) : i] = i < value ? left : right;
            return ret;
        }
    }
}
