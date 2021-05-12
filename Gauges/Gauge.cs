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
    public abstract class Gauge {
        public string Name;
        public Item[] Triggers;
        public UIElement UI = null;
        public GaugeVisual DefaultVisual;
        public GaugeVisual Visual;

        public bool Active = false;
        public bool Enabled = true;
        public Item LastActiveTrigger;
        public DateTime ActiveTime;

        public Gauge HideGauge = null;
        public bool StartHidden = false;
        public bool AllowRefresh = true;

        public Gauge(string name) {
            Name = name;
            Triggers = new Item[0];
        }

        public void SetActive(Item trigger) {
            PerformHide();
            Active = true;
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
    public struct Item {
        public uint Id;
        public bool IsBuff;

        // GENERATORS
        public Item(ActionIds action) {
            Id = (uint)action;
            IsBuff = false;
        }
        public Item(BuffIds buff) {
            Id = (uint)buff;
            IsBuff = true;
        }

        // EQUALITY
        public override bool Equals(object obj) {
            return obj is Item overrides && Equals(overrides);
        }

        public bool Equals(Item other) {
            return (Id == other.Id) && (IsBuff == other.IsBuff);
        }

        public override int GetHashCode() {
            int hash = 13;
            hash = (hash * 7) + Id.GetHashCode();
            hash = (hash * 7) + IsBuff.GetHashCode();
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
