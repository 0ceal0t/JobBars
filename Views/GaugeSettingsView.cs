using ImGuiNET;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars.Views {
    public class GaugeSettingsView {
        private Gauge Gauge;
        private GaugeManager Manager;
        private JobIds Job;

        public GaugeSettingsView(JobIds job, Gauge gauge, GaugeManager manager) {
            Job = job;
            Gauge = gauge;
            Manager = manager;
        }

        public void Draw(string _ID) {
            var enabled = !Configuration.Config.GaugeDisabled.Contains(Gauge.Name);
            string type = Gauge switch
            {
                GaugeGCD _ => "GCDS",
                GaugeTimer _ => "TIMER",
                GaugeProc _ => "PROCS",
                GaugeCharges _ => "CHARGES",
                _ => ""
            };

            ImGui.TextColored(enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1), $"{Gauge.Name} [{type}]");
            if (ImGui.Checkbox("Enabled" + _ID + Gauge.Name, ref enabled)) {
                if (enabled) {
                    Configuration.Config.GaugeDisabled.Remove(Gauge.Name);
                }
                else {
                    Configuration.Config.GaugeDisabled.Add(Gauge.Name);
                }
                Configuration.Config.Save();
                Manager.ResetJob(Job);
            }
            // ===== ICON (TIMER ONLY + IF THERE IS AN ICON TO REPLACE) =====
            if(Gauge is GaugeTimer Timer && Timer.ReplaceIcon) {
                var iconEnabled = !Configuration.Config.GaugeIconDisabled.Contains(Gauge.Name);
                if (ImGui.Checkbox("Icon Replacement Enabled" + _ID + Gauge.Name, ref iconEnabled)) {
                    if (iconEnabled) {
                        Configuration.Config.GaugeIconDisabled.Remove(Gauge.Name);
                    }
                    else {
                        Configuration.Config.GaugeIconDisabled.Add(Gauge.Name);
                    }
                    Configuration.Config.Save();
                    Manager.ResetJob(Job);
                }
            }
            // ===== ORDER =======
            int order = Gauge.Order;
            if (ImGui.InputInt("Order" + _ID + Gauge.Name, ref order)) {
                if (order <= -1) {
                    order = -1;
                    Configuration.Config.GaugeOrderOverride.Remove(Gauge.Name);
                }
                else {
                    Configuration.Config.GaugeOrderOverride[Gauge.Name] = order;
                }
                Configuration.Config.Save();
                Manager.ResetJob(Job);
            }
            // ===== COLOR =======
            if (!(Gauge is GaugeProc)) {
                var isOverrideColor = Configuration.Config.GetColorOverride(Gauge.Name, out var colorOverride);

                var colorText = $"DEFAULT ({Gauge.Visual.Color.Name})";
                if(isOverrideColor) {
                    colorText = Gauge.Visual.Color.Name;
                }

                if (ImGui.BeginCombo("Color" + _ID + Gauge.Name, colorText)) {
                    if (ImGui.Selectable($"DEFAULT ({Gauge.DefaultVisual.Color.Name}){_ID}{Gauge.Name}", !isOverrideColor)) { // DEFAULT
                        Configuration.Config.GaugeColorOverride.Remove(Gauge.Name);
                        Configuration.Config.Save();
                        SetGaugeColor(Gauge, Gauge.DefaultVisual.Color);
                    }
                    foreach (var entry in UIColor.AllColors) {
                        if (ImGui.Selectable($"{entry.Key}{_ID}{Gauge.Name}", (Gauge.Visual.Color.Name == entry.Key) && isOverrideColor)) { // OTHER
                            Configuration.Config.GaugeColorOverride[Gauge.Name] = entry.Key;
                            Configuration.Config.Save();
                            SetGaugeColor(Gauge, entry.Value);
                        }
                    }

                    ImGui.EndCombo();
                }
            }
            // ====== TYPE ======
            if (Gauge is GaugeGCD) {
                DrawTypeOptions(Gauge, GaugeGCD.ValidGaugeVisualType, _ID);
            }
            else if(Gauge is GaugeCharges) {
                DrawTypeOptions(Gauge, GaugeCharges.ValidGaugeVisualType, _ID);
            }

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
        }

        private void DrawTypeOptions(Gauge gauge, GaugeVisualType[] typeOptions, string _ID) {
            var isOverrideType = Configuration.Config.GaugeTypeOverride.TryGetValue(gauge.Name, out var typeOverride);
            if (ImGui.BeginCombo("Type" + _ID + gauge.Name, isOverrideType ? $"{gauge.Visual.Type}" : $"DEFAULT ({gauge.Visual.Type})")) {
                if (ImGui.Selectable($"DEFAULT ({gauge.DefaultVisual.Type}){_ID}{gauge.Name}", !isOverrideType)) { // DEFAULT
                    Configuration.Config.GaugeTypeOverride.Remove(gauge.Name);
                    Configuration.Config.Save();
                    gauge.Visual.Type = gauge.DefaultVisual.Type;
                    Manager.ResetJob(Job);
                }
                foreach (GaugeVisualType gType in typeOptions) {
                    if (ImGui.Selectable($"{gType}{_ID}{gauge.Name}", (gauge.Visual.Type == gType) && isOverrideType)) { // OTHER
                        Configuration.Config.GaugeTypeOverride[gauge.Name] = gType;
                        Configuration.Config.Save();
                        gauge.Visual.Type = gType;
                        Manager.ResetJob(Job);
                    }
                }
                ImGui.EndCombo();
            }
        }

        private void SetGaugeColor(Gauge gauge, ElementColor color) {
            gauge.Visual.Color = color;
            gauge.SetupVisual(resetValue: false);
        }
    }
}
