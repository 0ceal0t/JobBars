using Dalamud.Plugin;
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

        public bool Enabled = true;
        public bool AllowRefresh = true;
        public GaugeState State = GaugeState.INACTIVE;

        public Item LastActiveTrigger;
        public DateTime ActiveTime;

        public Gauge HideGauge = null;
        public bool StartHidden = false;

        public int Order => Configuration.Config.GaugeOrderOverride.TryGetValue(Name, out var newOrder) ? newOrder : -1;

        public Gauge(string name) {
            Name = name;
            Triggers = new Item[0];
        }

        public void SetActive(Item trigger) {
            PerformHide();
            LastActiveTrigger = trigger;
            ActiveTime = DateTime.Now;
            State = GaugeState.ACTIVE;
        }
        public unsafe void PerformHide() {
            if(HideGauge != null && UiHelper.GetNodeVisible(UI.RootRes) == false) {
                UiHelper.SetPosition(UI.RootRes, HideGauge.UI.RootRes->X, HideGauge.UI.RootRes->Y);
                HideGauge.UI.Hide();
                UI.Show();
            }
        }
        public float TimeLeft(float defaultDuration, DateTime time, Dictionary<Item, float> buffDict) {
            if (LastActiveTrigger.Type == ItemType.BUFF) {
                if (buffDict.TryGetValue(LastActiveTrigger, out var duration)) { // duration exists, use that
                    return duration;
                }
                else { // time isn't there, are we just waiting on it?
                    var timeSinceActive = (time - ActiveTime).TotalSeconds;
                    if (timeSinceActive <= 2) { // hasn't been enough time for it to show up in the buff list
                        return (float)(defaultDuration - timeSinceActive);
                    }
                    return -1; // yeah lmao it's gone
                }
            }
            else {
                return (float)(defaultDuration - (time - ActiveTime).TotalSeconds); // triggered by an action, just calculate the time
            }
        }

        public void GetVisualConfig() {
            if (Configuration.Config.GetColorOverride(Name, out var color)) {
                Visual.Color = color;
            }
            if (Configuration.Config.GaugeTypeOverride.TryGetValue(Name, out var type)) {
                Visual.Type = type;
            }
        }

        public abstract void Setup();
        public abstract void SetColor();
        public abstract void ProcessAction(Item action);
        public abstract void Tick(DateTime time, Dictionary<Item, float> buffDict);
        public abstract int GetHeight();
        public abstract int GetWidth();
    }

    // ======= STATE ==========
    public enum GaugeState {
        INACTIVE,
        ACTIVE,
        FINISHED,
    }

    // ======= VISUAL =========
    public enum GaugeVisualType {
        Bar,
        Arrow,
        Diamond
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
