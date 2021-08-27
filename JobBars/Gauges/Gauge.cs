using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Numerics;

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
        public bool Enabled;

        public int Order => JobBars.Config.GaugeOrder.Get(Name);
        public Vector2 Position => JobBars.Config.GaugeSplitPosition.Get(Name);
        public float Scale => JobBars.Config.GaugeIndividualScale.Get(Name);

        public Gauge(string name) {
            Name = name;
            Enabled = JobBars.Config.GaugeEnabled.Get(Name);
        }

        public void LoadUI(UIGaugeElement ui) {
            UI = ui;
            UI.SetVisible(Enabled);
            UI.SetScale(Scale);
            LoadUI_Impl();
        }
        protected abstract void LoadUI_Impl();

        public void RefreshUI() {
            if (UI == null) return;

            UI.SetVisible(Enabled);
            UI.SetScale(Scale);
            if(JobBars.Config.GaugeSplit) UI.SetSplitPosition(Position);

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
        public abstract void Tick();

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
                GaugeResources _ => "RESOURCES",
                _ => ""
            };

            ImGui.TextColored(Enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1), $"{Name} [{type}]");

            if(JobBars.Config.GaugeEnabled.Draw($"Enabled{_ID}", Name, out var newEnabled)) {
                Enabled = newEnabled;
                RefreshUI();
                JobBars.GaugeManager.UpdatePositionScale(job);
            }

            if (JobBars.Config.GaugeIndividualScale.Draw($"Scale{_ID}", Name, out var scale)) {
                JobBars.Config.GaugeIndividualScale.Set(Name, Math.Max(scale, 0.1f));
                RefreshUI();
                JobBars.GaugeManager.UpdatePositionScale(job);
            }

            if(JobBars.Config.GaugeSplit) {
                if (JobBars.Config.GaugeSplitPosition.Draw($"Split Position{_ID}", Name, out var pos)) {
                    SetSplitPosition(pos);
                }
            }
            else {
                if (JobBars.Config.GaugeOrder.Draw($"Order{_ID}", Name)) {
                    JobBars.GaugeManager.UpdatePositionScale(job);
                }
            }

            DrawGauge(_ID, job);
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
        }

        public void DrawPositionBox() {
            if (JobBars.DrawPositionView(Name + "##GaugePosition", Position, out var pos)) {
                JobBars.Config.GaugeSplitPosition.Set(Name, pos);
                SetSplitPosition(pos);
            }
        }

        public void SetSplitPosition(Vector2 pos) {
            RefreshUI();
            JobBars.SetWindowPosition(Name + "##GaugePosition", pos);
        }
    }
}
