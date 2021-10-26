using System.Numerics;

namespace JobBars.Gauges {
    public abstract class GaugeTracker {
        public bool Disposed { get; private set; } = false;

        protected Gauge UI { get; private set; }

        public bool Enabled => GetConfig().Enabled;

        public int Order => GetConfig().Order;

        public int Height => UI.GetHeight();

        public int Width => UI.GetWidth();

        public int YOffset => UI.GetYOffset();

        public abstract GaugeConfig GetConfig();

        public abstract bool GetActive();

        public abstract void ProcessAction(Item action);

        public void SetPosition(Vector2 position) => UI.SetPosition(position);

        public void UpdateSplitPosition() => UI.SetSplitPosition(GetConfig().Position);

        public void UpdateVisual() => UI.UpdateVisual();

        public void Tick() {
            TickTracker();
            UI.Tick();
        }

        public void Cleanup() {
            Disposed = true;
            UI.Cleanup();
            UI = null;
        }

        protected void LoadUI(Gauge ui) {
            UI = ui;
            UI.UpdateVisual();
        }

        protected abstract void TickTracker();

        public static R[] SplitArray<R>(R left, int size) => SplitArray<R>(left, left, size, size);
        public static R[] SplitArray<R>(R left, R right, int value, int size) {
            var ret = new R[size];
            for (int i = 0; i < size; i++) ret[i] = i < value ? left : right;
            return ret;
        }
    }
}
