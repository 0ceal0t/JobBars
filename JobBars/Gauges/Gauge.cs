using ImGuiNET;
using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Numerics;
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
        public readonly string Name;
        public UIGaugeElement UI;

        protected GaugeConfig Config;
        public bool Enabled => Config.Enabled;
        public int Order => Config.Order;
        public Vector2 Position => Config.SplitPosition;
        public float Scale => Config.Scale;

        public Gauge(string name) {
            Name = name;
            Config = Configuration.Config.GetGaugeConfig(name);
        }

        public void LoadUI(UIGaugeElement ui) {
            UI = ui;
            UI.SetVisible(Config.Enabled);
            UI.SetScale(Config.Scale);
            LoadUI_Impl();
        }
        protected abstract void LoadUI_Impl();

        public void RefreshUI() {
            if (UI == null) return;

            UI.SetVisible(Enabled);
            UI.SetScale(Config.Scale);

            if(Configuration.Config.GaugeSplit) UI.SetSplitPosition(Config.SplitPosition);

            RefreshUI_Impl();
        }
        protected abstract void RefreshUI_Impl();

        public void UnloadUI() {
            UI.Cleanup();
            UI = null;
        }

        public virtual bool DoProcessInput() => Enabled;
        public abstract void ProcessAction(Item action);
        public abstract GaugeVisualType GetVisualType();
        public abstract void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict);

        public int Height => UI == null ? 0 : (int)(Scale * GetHeight());
        public int Width => UI == null ? 0 : (int)(Scale * GetWidth());
        protected abstract int GetHeight();
        protected abstract int GetWidth();

        protected abstract void DrawGauge(string _ID, JobIds job);

        public void Draw(string id, JobIds job) {
            var _ID = id + Name;
            string type = this switch {
                GaugeGCD _ => "GCDS",
                GaugeTimer _ => "TIMER",
                GaugeProc _ => "PROCS",
                GaugeCharges _ => "CHARGES",
                GaugeStacks _ => "STACKS",
                _ => ""
            };

            ImGui.TextColored(Enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1), $"{Name} [{type}]");
            if (ImGui.Checkbox("Enabled" + _ID, ref Config.Enabled)) {
                Configuration.Config.Save();

                RefreshUI();
                GaugeManager.Manager.UpdatePositionScale(job);
            }

            if (ImGui.InputInt("Order" + _ID, ref Config.Order)) {
                Config.Order = Math.Max(Config.Order, -1);
                Configuration.Config.Save();

                GaugeManager.Manager.UpdatePositionScale(job);
            }

            if (ImGui.InputFloat("Scale" + _ID, ref Config.Scale)) {
                Config.Scale = Math.Max(Config.Scale, 0.1f);
                Configuration.Config.Save();

                RefreshUI();
                GaugeManager.Manager.UpdatePositionScale(job);
            }

            var pos = Config.SplitPosition;
            if (ImGui.InputFloat2("Split Position" + _ID, ref pos)) {
                SetSplitPosition(pos);
            }

            DrawGauge(_ID, job);
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
        }

        public void DrawPositionBox() {
            if (JobBars.DrawPositionView(Name + "##GaugePosition", Position, out var pos)) {
                SetSplitPosition(pos);
            }
        }

        public void SetSplitPosition(Vector2 pos) {
            Config.SplitPosition = pos;
            Configuration.Config.Save();

            JobBars.SetWindowPosition(Name + "##GaugePosition", pos);
            RefreshUI();
        }

        // =============================

        public static bool DrawTypeOptions(string _ID, GaugeVisualType[] typeOptions, GaugeVisualType currentType, out GaugeVisualType newType) {
            newType = GaugeVisualType.Bar;
            if (ImGui.BeginCombo("Type" + _ID, $"{currentType}")) {
                foreach (GaugeVisualType gType in typeOptions) {
                    if (ImGui.Selectable($"{gType}{_ID}", gType == currentType)) {
                        newType = gType;
                        ImGui.EndCombo();
                        return true;
                    }
                }
                ImGui.EndCombo();
            }
            return false;
        }

        public static bool DrawColorOptions(string _ID, ElementColor currentColor, out string newColorString, out ElementColor newColor, string title = "Color") {
            newColor = NoColor;
            newColorString = string.Empty;
            if (ImGui.BeginCombo(title + _ID, currentColor.Name)) {
                foreach (var entry in AllColors) {
                    if (ImGui.Selectable($"{entry.Key}{_ID}", currentColor.Name == entry.Key)) {
                        newColorString = entry.Key;
                        newColor = entry.Value;
                        ImGui.EndCombo();
                        return true;
                    }
                }
                ImGui.EndCombo();
            }
            return false;
        }

        public static float TimeLeft(float defaultDuration, DateTime time, Dictionary<Item, BuffElem> buffDict, Item lastActiveTrigger, DateTime lastActiveTime) {
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

#nullable enable
        public static T GetConfigValue<T>(T? configValue, T defaultValue) {
            return configValue == null ? defaultValue : configValue;
        }
#nullable disable
    }
}
