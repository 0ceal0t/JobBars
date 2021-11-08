using System.Numerics;

namespace JobBars.Gauges {
    public abstract class GaugeTracker {
        public bool Disposed { get; private set; } = false;

        protected Gauge UI { get; private set; }

        public bool Enabled => GetConfig().Enabled;

        public int Order => GetConfig().Order;

        public int Height => UI?.GetHeight() ?? 0;

        public int Width => UI?.GetWidth() ?? 0;

        public int YOffset => UI?.GetYOffset() ?? 0;

        public abstract GaugeConfig GetConfig();

        public abstract bool GetActive();

        public abstract void ProcessAction(Item action);

        public void SetPosition(Vector2 position) => UI?.SetPosition(position);

        public void UpdateSplitPosition() => UI?.SetSplitPosition(GetConfig().Position);

        public void UpdateVisual() => UI?.UpdateVisual();

        public void Tick() {
            if (UI == null) return;
            TickTracker();
            UI.Tick();
        }

        public void Cleanup() {
            Disposed = true;
            if (UI == null) return;
            UI.Cleanup();
            UI = null;
        }

        protected void LoadUI(Gauge ui) {
            UI = ui;
            if (UI == null) return;
            UI.UpdateVisual();
        }

        protected abstract void TickTracker();

        public static R[] SplitArray<R>(R left, int size, bool reverse = false) => SplitArray(left, left, size, size, reverse);
        public static R[] SplitArray<R>(R left, R right, int value, int size, bool reverse = false) {
            var ret = new R[size];
            for (int i = 0; i < size; i++) ret[reverse ? (size - i - 1) : i] = i < value ? left : right;
            return ret;
        }
    }
}
