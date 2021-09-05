using ImGuiNET;
using JobBars.Data;
using JobBars.UI;
using System;
using System.Numerics;
using Dalamud;
using JobBars.Helper;

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
        Finished,
    }

    public abstract class Gauge {
        public readonly string Name;
        public UIGaugeElement UI;
        public bool Enabled;
        private string LocalName;

        public int Order => JobBars.Config.GaugeOrder.Get(Name);
        public Vector2 Position => JobBars.Config.GaugeSplitPosition.Get(Name);
        public float Scale => JobBars.Config.GaugeIndividualScale.Get(Name);

        protected bool ShowText => JobBars.Config.GaugeShowText.Get(Name);

        public Gauge(string name) {
            Name = name;
            Enabled = JobBars.Config.GaugeEnabled.Get(Name);
        }

        public void LoadUI(UIGaugeElement ui) {
            UI = ui;
            LoadUI_();
            ApplyUIConfig();
        }
        protected abstract void LoadUI_();

        public void ApplyUIConfig() {
            if (UI == null) return;
            UI.SetVisible(Enabled);
            UI.SetScale(Scale);
            if(JobBars.Config.GaugeSplit) UI.SetSplitPosition(Position);

            ApplyUIConfig_();
        }
        protected abstract void ApplyUIConfig_();

        public void UnloadUI() {
            UI.Cleanup();
            UI = null;
        }

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

            if (LocalName == null)
            {
                var item = new Item();
                switch (this)
                {
                    case GaugeGCD:
                    {
                        var gau = (GaugeGCD)this;
                        item = gau.SubGauges[0].Props.Triggers[0];
                        break;
                    }
                    case GaugeTimer:
                    {
                        var gau = (GaugeTimer)this;
                        item = gau.SubGauges[0].Props.Triggers[0];
                        break;
                    }
                    case GaugeCharges:
                    {
                        var gau = (GaugeCharges)this;
                        item = gau.Props.Parts[0].Triggers[0];
                        break;
                    }
                    case GaugeStacks:
                    {
                        var gau = (GaugeStacks)this;
                        item = gau.Props.Triggers[0];
                        break;
                    }
                    case GaugeProc:
                    {
                        var Prog = JobBars.ClientState.ClientLanguage > ClientLanguage.French ? " 触发" : " Proc";
                        LocalName = UIHelper.JobToString(job)+Prog;
                        item.Id = 0;
                        break;
                    }
                    case GaugeResources:
                    {
                        LocalName = "MP ("+UIHelper.JobToString(job)+")";
                        item.Id = 0;
                        break;
                    }
                }

                LocalName = item.Id != 0 ? UIHelper.ItemToString(item) : LocalName;
            }
            

            ImGui.TextColored(Enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1), $"{LocalName} [{type}]");

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

            DrawGaugeOptions(_ID);

            DrawGauge(_ID, job);
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
        }

        private void DrawGaugeOptions(string _ID) {
            var type = GetVisualType();

            //  text above
            //  text spacing?

            if(type == GaugeVisualType.Bar || type == GaugeVisualType.BarDiamondCombo) {
                if(JobBars.Config.GaugeShowText.Draw($"Show Text{_ID}", Name)) {
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

        public void SetSplitPosition(Vector2 pos) {
            ApplyUIConfig();
            JobBars.SetWindowPosition(Name + "##GaugePosition", pos);
        }
    }
}
