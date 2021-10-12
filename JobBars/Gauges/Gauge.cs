using ImGuiNET;
using JobBars.Data;
using JobBars.UI;
using System;
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

    public abstract class Gauge {
        public readonly string Name;
        public UIGauge UI;
        public bool Enabled;

        public int Order => JobBars.Config.GaugeOrder.Get(Name);
        public Vector2 Position => JobBars.Config.GaugeSplitPosition.Get(Name);
        public float Scale => JobBars.Config.GaugeIndividualScale.Get(Name);

        protected bool ShowText => JobBars.Config.GaugeShowText.Get(Name);
        protected bool SwapText => JobBars.Config.GaugeSwapText.Get(Name);

        public Gauge(string name) {
            Name = name;
            Enabled = JobBars.Config.GaugeEnabled.Get(Name);
        }

        public void LoadUI(UIGauge ui) {
            UI = ui;
            LoadUIImpl();
            ApplyUIConfig();
        }
        protected abstract void LoadUIImpl();

        public void ApplyUIConfig() {
            if (UI == null) return;
            UI.SetVisible(Enabled);
            UI.SetScale(Scale);
            if(JobBars.Config.GaugePositionType == GaugePositionType.Split) UI.SetSplitPosition(Position);

            ApplyUIConfigImpl();
        }
        protected abstract void ApplyUIConfigImpl();

        public void UnloadUI() {
            UI.Cleanup();
            UI = null;
        }

        public abstract void ProcessAction(Item action);

        public abstract void Tick();

        public void TickActive() {
            UI?.SetVisible(!JobBars.Config.GaugesHideWhenInactive || GetActive());
        }

        protected abstract bool GetActive();

        public int Height => UI == null ? 0 : (int)(Scale * GetHeight());
        public int Width => UI == null ? 0 : (int)(Scale * GetWidth());
        protected abstract int GetHeight();
        protected abstract int GetWidth();
        public abstract GaugeVisualType GetVisualType();

        // ========= DRAW =============

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
                ApplyUIConfig();
                JobBars.GaugeManager.UpdatePositionScale(job);
            }

            if (JobBars.Config.GaugeIndividualScale.Draw($"Scale{_ID}", Name, out var scale)) {
                JobBars.Config.GaugeIndividualScale.Set(Name, Math.Max(scale, 0.1f));
                ApplyUIConfig();
                JobBars.GaugeManager.UpdatePositionScale(job);
            }

            if(JobBars.Config.GaugePositionType == GaugePositionType.Split) {
                if (JobBars.Config.GaugeSplitPosition.Draw($"Split Position{_ID}", Name, out var pos)) {
                    SetSplitPosition(pos);
                }
            }
            else {
                if (JobBars.Config.GaugeOrder.Draw($"Order{_ID}", Name)) {
                    JobBars.GaugeManager.UpdatePositionScale(job);
                }
            }

            DrawGaugeOptions(_ID);
            DrawGauge(_ID, job);
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
        }

        protected abstract void DrawGauge(string _ID, JobIds job);

        private void DrawGaugeOptions(string _ID) {
            var type = GetVisualType();

            if(type == GaugeVisualType.Bar || type == GaugeVisualType.BarDiamondCombo) {
                if(JobBars.Config.GaugeShowText.Draw($"Show Text{_ID}", Name)) {
                    ApplyUIConfig();
                }
            }

            if(type == GaugeVisualType.Bar) {
                if (JobBars.Config.GaugeSwapText.Draw($"Swap Text Position{_ID}", Name)) {
                    ApplyUIConfig();
                }
            }
        }

        public void DrawPositionBox() {
            if (JobBars.DrawPositionView(Name + "##GaugePosition", Position, out var pos)) {
                JobBars.Config.GaugeSplitPosition.Set(Name, pos);
                SetSplitPosition(pos);
            }
        }

        private void SetSplitPosition(Vector2 pos) {
            ApplyUIConfig();
            JobBars.SetWindowPosition(Name + "##GaugePosition", pos);
        }
    }
}
