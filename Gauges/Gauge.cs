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
    public enum GaugeState {
        Inactive,
        Active,
        Finished,
    }

    public abstract class Gauge {
        public string Name;
        public Item[] Triggers;
        public UIElement UI;
        public bool Enabled = true;
        public int Order => Configuration.Config.GaugeOrderOverride.TryGetValue(Name, out var newOrder) ? newOrder : -1;

        public GaugeVisual DefaultVisual;
        public GaugeVisual Visual;

        public bool AllowRefresh = true;
        public GaugeState State = GaugeState.Inactive;

        public Item LastActiveTrigger;
        public DateTime ActiveTime;

        public Gauge HideGauge;
        public bool StartHidden = false;

        public Gauge(string name) {
            Name = name;
            Triggers = new Item[0];
        }

        public unsafe void SetActive(Item trigger) {
            if (HideGauge != null && UiHelper.GetNodeVisible(UI.RootRes) == false) {
                UiHelper.SetPosition(UI.RootRes, HideGauge.UI.RootRes->X, HideGauge.UI.RootRes->Y);
                HideGauge.UI.Hide();
                UI.Show();
            }

            LastActiveTrigger = trigger;
            ActiveTime = DateTime.Now;
            State = GaugeState.Active;
        }

        public float TimeLeft(float defaultDuration, DateTime time, Dictionary<Item, BuffElem> buffDict) {
            if (LastActiveTrigger.Type == ItemType.Buff) {
                if (buffDict.TryGetValue(LastActiveTrigger, out var elem)) { // duration exists, use that
                    return elem.Duration;
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

        public virtual bool DoProcessInput() {
            return Enabled;
        }

        public abstract void SetupVisual(bool resetValue = true);
        public abstract void ProcessAction(Item action);
        public abstract void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict);
        public abstract int GetHeight();
        public abstract int GetWidth();
    }

    public enum GaugeVisualType {
        Bar,
        Arrow,
        Diamond,
        BarDiamondCombo
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

        public static GaugeVisual Diamond(ElementColor color) {
            return new GaugeVisual
            {
                Type = GaugeVisualType.Diamond,
                Color = color
            };
        }

        public static GaugeVisual BarDiamondCombo(ElementColor color) {
            return new GaugeVisual
            {
                Type = GaugeVisualType.BarDiamondCombo,
                Color = color
            };
        }
    }
}
