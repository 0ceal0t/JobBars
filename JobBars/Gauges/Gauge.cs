using JobBars.Nodes.Gauge;
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
        public abstract int GetHeight();
        public abstract int GetWidth();
        public abstract int GetYOffset();
        public abstract void SetPosition( Vector2 position );
        public abstract void SetSplitPosition( Vector2 position );
    }

    public abstract class Gauge<T, S> : Gauge where T : class, IGaugeNode where S : GaugeTracker {
        protected T Node;
        protected S Tracker;

        public override void Tick() {
            if( Node == null ) return;
            TickGauge();
            Node.SetVisible( !Tracker.GetConfig().HideWhenInactive || Tracker.GetActive() );
        }

        public override void SetPosition( Vector2 position ) => Node.SetPosition( position );

        public override void SetSplitPosition( Vector2 position ) => Node.SetSplitPosition( position );

        public override void UpdateVisual() {
            if( Node == null ) return;
            Node.SetVisible( Tracker.GetConfig().Enabled );
            Node.SetScale( Tracker.GetConfig().Scale );

            UpdateVisualGauge();
        }

        public override int GetHeight() => ( int )( Tracker.GetConfig().Scale * GetHeightGauge() );

        public override int GetWidth() => ( int )( Tracker.GetConfig().Scale * GetWidthGauge() );

        protected abstract void TickGauge();

        protected abstract void UpdateVisualGauge();

        protected abstract int GetHeightGauge();

        protected abstract int GetWidthGauge();
    }
}
