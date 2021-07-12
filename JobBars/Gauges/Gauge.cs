using Dalamud.Plugin;
using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars.Gauges {
    public enum GaugeState {
        Inactive,
        Active,
        Finished,
    }

    public enum GaugeVisualType {
        Bar,
        Arrow,
        Diamond,
        BarDiamondCombo
    }

    public abstract class Gauge {
        public string Name;
        public UIElement UI;

        public bool Enabled;
        public int Order;
        public float Scale;

        public Gauge(string name) {
            Name = name;
            Enabled = !Configuration.Config.GaugeDisabled.Contains(Name);
            Order = Configuration.Config.GaugeOrderOverride.TryGetValue(Name, out var newOrder) ? newOrder : -1;
            Scale = Configuration.Config.GaugeIndividualScale.TryGetValue(name, out var newScale) ? newScale : 1;
        }

        public float TimeLeft(float defaultDuration, DateTime time, Dictionary<Item, BuffElem> buffDict, Item lastActiveTrigger, DateTime lastActiveTime) {
            if (lastActiveTrigger.Type == ItemType.Buff) {
                if (buffDict.TryGetValue(lastActiveTrigger, out var elem)) { // duration exists, use that
                    return elem.Duration;
                }
                else { // time isn't there, are we just waiting on it?
                    var timeSinceActive = (time - lastActiveTime).TotalSeconds;
                    if (timeSinceActive <= 2) { // hasn't been enough time for it to show up in the buff list
                        return defaultDuration;
                    }
                    return -1; // yeah lmao it's gone
                }
            }
            else {
                return (float)(defaultDuration - (time - lastActiveTime).TotalSeconds); // triggered by an action, just calculate the time
            }
        }

        public void SetupUI() {
            UI.SetVisible(Enabled);
            UI.SetScale(Scale);
            Setup();
        }
        protected abstract void Setup();

        public virtual bool DoProcessInput() {
            return Enabled;
        }
        public abstract void ProcessAction(Item action);

        public abstract GaugeVisualType GetVisualType();

        public abstract void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict);

        public int Height => UI == null ? 0 : (int)(Scale * GetWidth());
        public int Width => UI == null ? 0 : (int)(Scale * GetHeight());
        protected abstract int GetHeight();
        protected abstract int GetWidth();

        protected abstract void DrawGauge(string _ID, JobIds job);
        public void Draw(string id, JobIds job) {
            var _ID = id + Name;
            string type = this switch
            {
                GaugeGCD _ => "GCDS",
                GaugeTimer _ => "TIMER",
                GaugeProc _ => "PROCS",
                GaugeCharges _ => "CHARGES",
                GaugeStacks _ => "STACKS",
                _ => ""
            };

            // ======== ENABLED/DISABLED =========
            ImGui.TextColored(Enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1), $"{Name} [{type}]");
            if (ImGui.Checkbox("Enabled" + _ID, ref Enabled)) {
                if (Enabled) {
                    Configuration.Config.GaugeDisabled.Remove(Name);
                    if(job == GaugeManager.Manager.CurrentJob) {
                        UI?.Show();
                    }
                }
                else {
                    Configuration.Config.GaugeDisabled.Add(Name);
                    if (job == GaugeManager.Manager.CurrentJob) {
                        UI?.Hide();
                    }
                }
                Configuration.Config.Save();
                if (job == GaugeManager.Manager.CurrentJob) {
                    GaugeManager.Manager.SetPositionScale();
                }
            }

            // ===== ORDER =======
            if (ImGui.InputInt("Order" + _ID, ref Order)) {
                if (Order <= -1) {
                    Order = -1;
                    Configuration.Config.GaugeOrderOverride.Remove(Name);
                }
                else {
                    Configuration.Config.GaugeOrderOverride[Name] = Order;
                }
                Configuration.Config.Save();
                if (job == GaugeManager.Manager.CurrentJob) {
                    GaugeManager.Manager.SetPositionScale();
                }
            }

            // ====== SCALE ========
            if (ImGui.InputFloat("Scale" + _ID, ref Scale)) {
                if(Scale <= 0.1f) { Scale = 0.1f; }
                if(Scale == 1) {
                    Configuration.Config.GaugeIndividualScale.Remove(Name);
                }
                else {
                    Configuration.Config.GaugeIndividualScale[Name] = Scale;
                }
                Configuration.Config.Save();
                UI?.SetScale(Scale);
                if (job == GaugeManager.Manager.CurrentJob) {
                    GaugeManager.Manager.SetPositionScale();
                }
            }

            DrawGauge(_ID, job);
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
        }

        public void SetSplitPosition(Vector2 pos) {
            Configuration.Config.GaugeSplitPosition[Name] = pos;
            Configuration.Config.Save();
            UI?.SetSplitPosition(pos);
        }

        public bool DrawTypeOptions(string _ID, GaugeVisualType[] typeOptions, GaugeVisualType currentType, out GaugeVisualType newType) {
            newType = GaugeVisualType.Bar;
            if (ImGui.BeginCombo("Type" + _ID, $"{currentType}")) {
                foreach (GaugeVisualType gType in typeOptions) {
                    if (ImGui.Selectable($"{gType}{_ID}", gType == currentType)) {
                        Configuration.Config.GaugeTypeOverride[Name] = gType;
                        Configuration.Config.Save();
                        newType = gType;
                        return true;
                    }
                }
                ImGui.EndCombo();
            }
            return false;
        }

        public bool DrawColorOptions(string _ID, string lookupId, ElementColor currentColor, out ElementColor newColor, string title = "Color") {
            newColor = NoColor;
            if (ImGui.BeginCombo(title + _ID, currentColor.Name)) {
                foreach (var entry in AllColors) {
                    if (ImGui.Selectable($"{entry.Key}{_ID}", currentColor.Name == entry.Key)) {
                        Configuration.Config.GaugeColorOverride[lookupId] = entry.Key;
                        Configuration.Config.Save();
                        newColor = entry.Value;
                        return true;
                    }
                }
                ImGui.EndCombo();
            }
            return false;
        }
    }
}
