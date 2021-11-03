using JobBars.UI;
using System.Numerics;

namespace JobBars.Gauges {
    public enum GaugeVisualType {
        Bar,
        Arrow,
        Diamond,
        BarDiamondCombo
    }

    public enum GaugeState {
        Inactive,
        Active,
        Finished
    }

    public enum GaugeCompleteSoundType {
        When_Full,
        When_Empty,
        When_Empty_or_Full,
        Never
    }

    public abstract class Gauge {
        public abstract void UpdateVisual();
        public abstract void Tick();
        public abstract void Cleanup();
        public abstract int GetHeight();
        public abstract int GetWidth();
        public abstract int GetYOffset();
        public abstract void SetPosition(Vector2 position);
        public abstract void SetSplitPosition(Vector2 position);
    }

    public abstract class Gauge<T, S> : Gauge where T : UIGauge where S : GaugeTracker {
        protected T UI;
        protected S Tracker;

        public override void Cleanup() {
            Tracker = null;
            UI = null;
        }

        public override void Tick() {
            TickGauge();

            UI.SetVisible(!Tracker.GetConfig().HideWhenInactive || Tracker.GetActive());
        }

        public override void SetPosition(Vector2 position) => UI.SetPosition(position);

        public override void SetSplitPosition(Vector2 position) => UI.SetSplitPosition(position);

        public override void UpdateVisual() {
            UI.SetVisible(Tracker.GetConfig().Enabled);
            UI.SetScale(Tracker.GetConfig().Scale);

            UpdateVisualGauge();
        }

        public override int GetHeight() => (int)(Tracker.GetConfig().Scale * GetHeightGauge());

        public override int GetWidth() => (int)(Tracker.GetConfig().Scale * GetWidthGauge());

        protected abstract void TickGauge();

        protected abstract void UpdateVisualGauge();

        protected abstract int GetHeightGauge();

        protected abstract int GetWidthGauge();
    }
}
