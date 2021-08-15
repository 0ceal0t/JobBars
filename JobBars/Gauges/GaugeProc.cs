using Dalamud.Plugin;
using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using static JobBars.UI.UIColor;

namespace JobBars.Gauges {
    public struct GaugeProcProps {
        public bool ShowText;
        public Proc[] Procs;
        public bool NoSoundOnFull;
    }

    public struct Proc {
        public Item Trigger;
        public ElementColor Color;
        public int Idx;

        public Proc(BuffIds buff, ElementColor color) {
            Trigger = new Item(buff);
            Color = color;
            Idx = 0;
        }
        public Proc(ActionIds action, ElementColor color) {
            Trigger = new Item(action);
            Color = color;
            Idx = 0;
        }
    }

    public class GaugeProc : Gauge {
        private GaugeProcProps Props;
        private readonly int Size;

        private readonly List<bool> ProcsActive = new();

        public GaugeProc(string name, GaugeProcProps props) : base(name) {
            Props = props;
            Props.NoSoundOnFull = GetConfigValue(Config.NoSoundOnFull, Props.NoSoundOnFull).Value;

            for (int idx = 0; idx < Props.Procs.Length; idx++) Props.Procs[idx].Idx = idx;
            Size = Props.Procs.Length;

            ResetProcActive();
        }

        protected override void SetupUI() {
            if (UI is UIDiamond diamond) {
                diamond.SetMaxValue(Size, showText: Props.ShowText);
                foreach (var proc in Props.Procs) {
                    diamond.SetColor(proc.Color, proc.Idx);
                }
            }
            foreach (var proc in Props.Procs) {
                SetValue(proc.Idx, false);
            }

            ResetProcActive();
        }

        private void ResetProcActive() {
            ProcsActive.Clear();
            for (var i = 0; i < Size; i++) ProcsActive.Add(true);
        }

        public unsafe override void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict) {
            for(int idx = 0; idx < Size; idx++) {
                var proc = Props.Procs[idx];
                bool procActive;

                if (proc.Trigger.Type == ItemType.Buff) {
                    SetValue(proc.Idx, procActive = buffDict.TryGetValue(proc.Trigger, out var buff), buff.Duration);
                }
                else {
                    var recastActive = UIHelper.GetRecastActive(proc.Trigger.Id, out _);
                    SetValue(proc.Idx, procActive = !recastActive);
                }

                if (procActive && !ProcsActive[idx] && !Props.NoSoundOnFull) UIHelper.PlaySeComplete(); // play when any proc becomes active
                ProcsActive[idx] = procActive;
            }
        }

        private void SetValue(int idx, bool value, float duration = 0) {
            if (UI is UIDiamond diamond) {
                if (value) {
                    diamond.SelectPart(idx);
                    if (Props.ShowText) {
                        diamond.SetText(idx, ((int)Math.Round(duration)).ToString());
                    }
                }
                else {
                    diamond.UnselectPart(idx);
                }
            }
        }

        public override void ProcessAction(Item action) { }

        protected override int GetHeight() => UI == null ? 0 : UI.GetHeight(0);

        protected override int GetWidth() => UI == null ? 0 : UI.GetWidth(Size);

        public override GaugeVisualType GetVisualType() => GaugeVisualType.Diamond;

        protected override void DrawGauge(string _ID, JobIds job) {
            if (ImGui.Checkbox($"Don't Play Sound On Proc {_ID}", ref Props.NoSoundOnFull)) {
                Config.Invert = Props.NoSoundOnFull;
                Configuration.Config.Save();
            }
        }
    }
}
