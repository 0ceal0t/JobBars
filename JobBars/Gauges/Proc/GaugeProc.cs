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
        public readonly string Name;
        public readonly Item Trigger;

        public int Idx = 0;
        public int Order;
        public ElementColor Color;

        public Proc(string name, BuffIds buff, ElementColor color) : this(name, new Item(buff), color) { }

        public Proc(string name, ActionIds action, ElementColor color) : this(name, new Item(action), color) { }

        public Proc(string name, Item trigger, ElementColor color) {
            Name = name;
            Trigger = trigger;
            Color = JobBars.Config.GaugeProcColor.Get(Name, color);
            Order = JobBars.Config.GaugeProcOrder.Get(Name);
        }
    }

    public class GaugeProc : Gauge {
        private readonly Proc[] Procs;
        private bool ProcsShowText;
        private bool NoSoundOnFull;

        private readonly int Size;
        private readonly List<bool> ProcsActive = new();
        private GaugeState State = GaugeState.Inactive;

        public GaugeProc(string name, GaugeProcProps props) : base(name) {
            Procs = props.Procs;
            NoSoundOnFull = JobBars.Config.GaugeNoSoundOnFull.Get(Name, props.NoSoundOnFull);
            ProcsShowText = JobBars.Config.GaugeShowText.Get(Name, props.ShowText);
            Size = Procs.Length;
            RefreshIdx();
        }

        private void RefreshIdx() {
            var idx = 0;
            foreach (var proc in Procs.OrderBy(x => x.Order)) {
                proc.Idx = idx++;
            }
        }

        protected override void LoadUIImpl() {
            if (UI is UIDiamond diamond) {
                diamond.SetMaxValue(Size);
            }
            for (int idx = 0; idx < Size; idx++) {
                SetValue(idx, false);
            }

            State = GaugeState.Inactive;
            ResetProcActive();
        }

        protected override void ApplyUIConfigImpl() {
            if (UI is UIDiamond diamond) {
                foreach (var proc in Procs) diamond.SetColor(proc.Color, proc.Idx);
                diamond.SetTextVisible(ProcsShowText);
            }
        }

        private void ResetProcActive() {
            ProcsActive.Clear();
            for (var i = 0; i < Size; i++) ProcsActive.Add(true);
        }

        public unsafe override void Tick() {
            var playSound = false;
            var procActiveCount = 0;
            foreach (var proc in Procs) {
                bool procActive;

                if (proc.Trigger.Type == ItemType.Buff) {
                    SetValue(proc.Idx, procActive = UIHelper.PlayerStatus.TryGetValue(proc.Trigger, out var buff), buff.RemainingTime);
                }
                else {
                    var recastActive = UIHelper.GetRecastActive(proc.Trigger.Id, out _);
                    SetValue(proc.Idx, procActive = !recastActive);
                }

                if (procActive && !ProcsActive[proc.Idx] && !NoSoundOnFull) playSound = true;
                if (procActive) procActiveCount++;
                ProcsActive[proc.Idx] = procActive;
            }

            if (playSound) UIHelper.PlaySeComplete();
            State = procActiveCount == 0 ? GaugeState.Inactive : GaugeState.Active;
        }

        protected override bool GetActive() => State != GaugeState.Inactive;

        private void SetValue(int idx, bool value, float duration = -1) {
            if (UI is UIDiamond diamond) {
                if (value) {
                    diamond.SelectPart(idx);
                    if (ProcsShowText) {
                        diamond.SetText(idx, duration >= 0 ? ((int)Math.Round(duration)).ToString() : "");
                    }
                }
                else diamond.UnselectPart(idx);
            }
        }

        public override void ProcessAction(Item action) { }

        protected override int GetHeight() => UI.GetHeight(0);
        protected override int GetWidth() => UI.GetWidth(Size);
        public override GaugeVisualType GetVisualType() => GaugeVisualType.Diamond;

        protected override void DrawGauge(string _ID, JobIds job) {
            if (JobBars.Config.GaugeShowText.Draw($"Show Text{_ID}", Name, ProcsShowText, out var newProcsShowText)) {
                ProcsShowText = newProcsShowText;
                ApplyUIConfig();
                JobBars.GaugeManager.UpdatePositionScale(job); // procs with text are taller than without, so update positions
            }

            if (JobBars.Config.GaugeNoSoundOnFull.Draw($"Don't Play Sound On Proc{_ID}", Name, NoSoundOnFull, out var newSound)) {
                NoSoundOnFull = newSound;
            }

            foreach (var proc in Procs) {
                if (JobBars.Config.GaugeProcOrder.Draw($"Order ({proc.Name})", proc.Name, proc.Order, out var newOrder)) {
                    proc.Order = newOrder;
                    RefreshIdx();
                    JobBars.GaugeManager.ResetJob(job);
                }

                if (JobBars.Config.GaugeProcColor.Draw($"Color ({proc.Name})", proc.Name, proc.Color, out var newColor)) {
                    proc.Color = newColor;
                    JobBars.GaugeManager.ResetJob(job);
                }
            }
        }
    }
}
