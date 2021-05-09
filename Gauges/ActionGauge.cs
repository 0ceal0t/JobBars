using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Gauges {
    public enum ActionGaugeType {
        GCDs,
        Timer
    }

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

    public abstract class ActionGauge {
        public bool Active = false;
        public Item LastActiveTrigger;
        public DateTime ActiveTime;

        public bool HideGauge = false;
        public string HideGaugeName;
        public bool AllowRefresh = true;

        public string Name;
        public ActionGaugeType Type;
        public Item[] Triggers;

        public UIElement _UI = null;

        public ActionGauge(string name, ActionGaugeType type) {
            Name = name;
            Type = type;
            Triggers = new Item[0];
        }

        public void SetActive(Item trigger) {
            Active = true;
            LastActiveTrigger = trigger;
            ActiveTime = DateTime.Now;
        }

        public abstract void Setup();
        public abstract void ProcessAction(Item action);
        public abstract void ProcessDuration(Item buff, float duration);
        public abstract void Tick(DateTime time, float delta);
    }
}
