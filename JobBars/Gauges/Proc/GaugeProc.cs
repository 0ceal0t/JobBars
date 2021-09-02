using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Linq;
using System.Collections.Generic;

namespace JobBars.Gauges {
    public struct GaugeProcProps {
        public bool ShowText;
        public Proc[] Procs;
        public bool NoSoundOnFull;
    }

    public class Proc {
        public string Name;
        public Item Trigger;
        public ElementColor Color;

        public int Idx = 0;
        public int Order;

        public Proc(string name, BuffIds buff, ElementColor color) : this(name, new Item(buff), color) { }

        public Proc(string name, ActionIds action, ElementColor color) : this(name, new Item(action), color) { }

        public Proc(string name, Item trigger, ElementColor color) {
            Name = name;
            Trigger = trigger;
            Color = color;
            Order = JobBars.Config.GaugeProcOrder.Get(Name);
        }
    }

    public class GaugeProc : Gauge {
        private GaugeProcProps Props;
        private readonly int Size;
        private readonly List<bool> ProcsActive = new();

        public GaugeProc(string name, GaugeProcProps props) : base(name) {
            Props = props;
            Props.NoSoundOnFull = JobBars.Config.GaugeNoSoundOnFull.Get(Name, Props.NoSoundOnFull);

            Size = Props.Procs.Length;
            RefreshIdx();
        }

        private void RefreshIdx() {
            var idx = 0;
            foreach(var proc in Props.Procs.OrderBy(x => x.Order)) {
                proc.Idx = idx;
                idx++;
            }
        }

        protected override void LoadUI_() {
            if (UI is UIDiamond diamond) {
                diamond.SetMaxValue(Size, showText: Props.ShowText);
            }
            for(int idx = 0; idx < Size; idx++) {
                SetValue(idx, false);
            }
            ResetProcActive();
        }

        protected override void ApplyUIConfig_() {
            if (UI is UIDiamond diamond) {
                foreach (var proc in Props.Procs) diamond.SetColor(proc.Color, proc.Idx);
            }
        }

        private void ResetProcActive() {
            ProcsActive.Clear();
            for (var i = 0; i < Size; i++) ProcsActive.Add(true);
        }

        public unsafe override void Tick() {
            foreach(var proc in Props.Procs) {
                bool procActive;

                if (proc.Trigger.Type == ItemType.Buff) {
                    SetValue(proc.Idx, procActive = UIHelper.PlayerStatus.TryGetValue(proc.Trigger, out var buff), buff.RemainingTime);
                }
                else {
                    var recastActive = UIHelper.GetRecastActive(proc.Trigger.Id, out _);
                    SetValue(proc.Idx, procActive = !recastActive);
                }

                if (procActive && !ProcsActive[proc.Idx] && !Props.NoSoundOnFull) UIHelper.PlaySeComplete(); // play when any proc becomes active
                ProcsActive[proc.Idx] = procActive;
            }
        }

        private void SetValue(int idx, bool value, float duration = 0) {
            if (UI is UIDiamond diamond) {
                if (value) {
                    diamond.SelectPart(idx);
                    if (Props.ShowText) diamond.SetText(idx, ((int)Math.Round(duration)).ToString());
                }
                else diamond.UnselectPart(idx);
            }
        }

        public override void ProcessAction(Item action) { }

        protected override int GetHeight() => UI.GetHeight(0);
        protected override int GetWidth() => UI.GetWidth(Size);
        public override GaugeVisualType GetVisualType() => GaugeVisualType.Diamond;

        protected override void DrawGauge(string _ID, JobIds job) {
            if (JobBars.Config.GaugeNoSoundOnFull.Draw($"Don't Play Sound On Proc{_ID}", Name, Props.NoSoundOnFull, out var newSound)) {
                Props.NoSoundOnFull = newSound;
            }

            foreach(var proc in Props.Procs) {
                if (JobBars.Config.GaugeProcOrder.Draw($"Order ({proc.Name})", proc.Name, proc.Order, out var newOrder)) {
                    proc.Order = newOrder;
                    RefreshIdx();
                    JobBars.GaugeManager.ResetJob(job);
                }
            }
        }
    }
}
