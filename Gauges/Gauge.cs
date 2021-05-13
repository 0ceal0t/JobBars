using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars.Gauges {
    // ======= STATE ==========
    public enum GaugeState {
        INACTIVE,
        ACTIVE,
        FINISHED,
    }

    public abstract class Gauge {
        public string Name;
        public Item[] Triggers;
        public UIElement UI = null;
        public GaugeVisual DefaultVisual;
        public GaugeVisual Visual;

        public bool Enabled = true;
        public GaugeState State = GaugeState.INACTIVE;
        public bool AllowRefresh = true;
        public Item LastActiveTrigger;
        public DateTime ActiveTime;

        public Gauge HideGauge = null;
        public bool StartHidden = false;

        public Gauge(string name) {
            Name = name;
            Triggers = new Item[0];
        }

        public void SetActive(Item trigger) {
            PerformHide();
            State = GaugeState.ACTIVE;
            LastActiveTrigger = trigger;
            ActiveTime = DateTime.Now;
        }
        public unsafe void PerformHide() {
            if(HideGauge != null && UiHelper.GetNodeVisible(UI.RootRes) == false) {
                UiHelper.SetPosition(UI.RootRes, HideGauge.UI.RootRes->X, HideGauge.UI.RootRes->Y);
                HideGauge.UI.Hide();
                UI.Show();
            }
        }

        public abstract void Setup();
        public abstract void SetColor();
        public abstract void ProcessAction(Item action);
        public abstract void ProcessDuration(Item buff, float duration, bool isRefresh);
        public abstract void Tick(DateTime time, float delta);
    }

    // ======= VISUAL =========
    public enum GaugeVisualType {
        Bar,
        Arrow
    }
    public struct GaugeVisual {
        public GaugeVisualType Type;
        public ElementColor Color;

        public static GaugeVisual Bar(ElementColor color) {
            return new GaugeVisual
            {
                Type = GaugeVisualType.Bar,
                Color = color
            };
        }

        public static GaugeVisual Arrow(ElementColor color) {
            return new GaugeVisual
            {
                Type = GaugeVisualType.Arrow,
                Color = color
            };
        }
    }

    // ===== BUFF OR ACTION ======
    public enum ItemType {
        BUFF,
        ACTION, // either GCD or OGCD
        GCD,
        OGCD
    }
    public struct Item {
        public uint Id;
        public ItemType Type;

        // GENERATORS
        public Item(ActionIds action) {
            Id = (uint)action;
            Type = ItemType.ACTION;
        }
        public Item(BuffIds buff) {
            Id = (uint)buff;
            Type = ItemType.BUFF;
        }

        // EQUALITY
        public override bool Equals(object obj) {
            return obj is Item overrides && Equals(overrides);
        }

        public bool Equals(Item other) {
            return (Id == other.Id) && ((Type == ItemType.BUFF) == (other.Type == ItemType.BUFF));
        }

        public override int GetHashCode() {
            int hash = 13;
            hash = (hash * 7) + Id.GetHashCode();
            hash = (hash * 7) + (Type == ItemType.BUFF).GetHashCode();
            return hash;
        }

        public static bool operator ==(Item left, Item right) {
            return left.Equals(right);
        }

        public static bool operator !=(Item left, Item right) {
            return !(left == right);
        }
    }
}
