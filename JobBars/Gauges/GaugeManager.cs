using Dalamud.Game.ClientState.Structs;
using Dalamud.Plugin;
using JobBars.Data;
using JobBars.PartyList;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace JobBars.Gauges {
    public unsafe partial class GaugeManager {
        public static GaugeManager Manager;
        public DalamudPluginInterface PluginInterface;
        public UIBuilder UI;

        public Dictionary<JobIds, Gauge[]> JobToGauges;
        public JobIds CurrentJob = JobIds.OTHER;
        public Gauge[] CurrentGauges => JobToGauges.TryGetValue(CurrentJob, out var gauges) ? gauges : JobToGauges[JobIds.OTHER];

        public GaugeManager(DalamudPluginInterface pi, UIBuilder ui) {
            Manager = this;
            UI = ui;
            PluginInterface = pi;
            if(!Configuration.Config.GaugesEnabled) {
                UI.HideGauges();
            }
            //===== SETUP =======
            Init();
        }

        public void SetJob(JobIds job) {
            //===== CLEANUP OLD =======
            foreach (var gauge in CurrentGauges) {
                gauge.UI?.Cleanup();
                gauge.UI = null;
            }
            UI.HideAllGauges();
            UI.Icon.Reset();
            //====== SET UP NEW =======
            CurrentJob = job;
            int idx = 0;
            foreach(var gauge in CurrentGauges) {
                gauge.UI = GetUI(idx, gauge.GetVisualType());
                gauge.SetupUI();
                idx++;
            }
            SetPositionScale();
        }

        public void SetPositionScale() {
            UI.SetGaugePosition(Configuration.Config.GaugePosition);
            UI.SetGaugeScale(Configuration.Config.GaugeScale);

            int totalPosition = 0;
            foreach (var gauge in CurrentGauges.OrderBy(g => g.Order).Where(g => g.Enabled)) {
                if (Configuration.Config.GaugeSplit) { // SPLIT
                    gauge.UI.SetSplitPosition(Configuration.Config.GetGaugeSplitPosition(gauge.Name));
                }
                else {
                    if (Configuration.Config.GaugeHorizontal) { // HORIZONTAL
                        gauge.UI.SetPosition(new Vector2(totalPosition, gauge.UI.GetHorizontalYOffset()));
                        totalPosition += gauge.GetWidth();
                    }
                    else { // VERTICAL
                        int xPosition = Configuration.Config.GaugeAlignRight ? 160 - gauge.GetWidth() : 0;
                        gauge.UI.SetPosition(new Vector2(xPosition, totalPosition));
                        totalPosition += gauge.GetHeight();
                    }
                }
            }
        }

        public UIElement GetUI(int idx, GaugeVisualType type) {
            switch(type) {
                case GaugeVisualType.Arrow:
                    return UI.Arrows[idx];
                case GaugeVisualType.Bar:
                    return UI.Gauges[idx];
                case GaugeVisualType.Diamond:
                    return UI.Diamonds[idx];
                case GaugeVisualType.BarDiamondCombo:
                    return new UIGaugeDiamondCombo(UI, UI.Gauges[idx], UI.Diamonds[idx]); // kind of scuffed, but oh well
                default:
                    return null;
            }
        }

        public void Reset() {
            SetJob(CurrentJob);
        }

        public void ResetJob(JobIds job) {
            if(job == CurrentJob) {
                SetJob(job);
            }
        }

        public void PerformAction(Item action) {
            if (!Configuration.Config.GaugesEnabled) return;
            foreach (var gauge in CurrentGauges.Where(x => x.DoProcessInput())) {
                gauge.ProcessAction(action);
            }
        }

        public void Tick(PList party, bool inCombat) {
            if (!Configuration.Config.GaugesEnabled) return;

            if (Configuration.Config.GaugesHideOutOfCombat) {
                if (inCombat) UI.ShowGauges();
                else UI.HideGauges();
            }

            Dictionary<Item, BuffElem> BuffDict = new Dictionary<Item, BuffElem>();
            var currentTime = DateTime.Now;

            int ownerId = PluginInterface.ClientState.LocalPlayer.ActorId;

            AddBuffs(PluginInterface.ClientState.LocalPlayer, ownerId, BuffDict);

            if (PluginInterface.ClientState.Targets.CurrentTarget != null) {
                AddBuffs(PluginInterface.ClientState.Targets.CurrentTarget, ownerId, BuffDict);
            }

            if (CurrentJob == JobIds.SCH && inCombat) { // only need this to catch excog for now
                var actorTable = PluginInterface.ClientState.Actors;
                foreach(var pMember in party) {
                    if (pMember == null) continue;

                    for (int i = 0; i < actorTable.Length; i++) {
                        if (actorTable[i] == null) continue;

                        if (actorTable[i].ActorId == pMember.ActorId) {
                            AddBuffs(actorTable[i], ownerId, BuffDict);
                        }
                    }
                }
            }

            foreach(var gauge in CurrentGauges) {
                if (!gauge.DoProcessInput()) { continue; }
                gauge.Tick(currentTime, BuffDict);
            }
            UI.Icon.Update();
        }

        private void AddBuffs(Dalamud.Game.ClientState.Actors.Types.Actor actor, int ownerId, Dictionary<Item, BuffElem> buffDict) {
            if (actor == null) return;
            /*foreach (var status in PluginInterface.ClientState.Targets.CurrentTarget.StatusEffects) {
                if (status.OwnerId.Equals(ownerId)) {
                    buffDict[new Item
                    {
                        Id = (uint)status.EffectId,
                        Type = ItemType.Buff
                    }] = new BuffElem
                    {
                        Duration = status.Duration > 0 ? status.Duration : status.Duration * -1,
                        StackCount = status.StackCount
                    };
                }
            }*/
            var buffAddr = actor.Address + ActorOffsets.UIStatusEffects;
            for (int i = 0; i < 30; i++) {
                var addr = buffAddr + i * 0xC;
                var status = (StatusEffect)Marshal.PtrToStructure(addr, typeof(StatusEffect));
                if (status.OwnerId.Equals(ownerId)) {
                    buffDict[new Item
                    {
                        Id = (uint)status.EffectId,
                        Type = ItemType.Buff
                    }] = new BuffElem
                    {
                        Duration = status.Duration > 0 ? status.Duration : status.Duration * -1,
                        StackCount = status.StackCount
                    };
                }
            }
        }
    }

    public struct BuffElem {
        public float Duration;
        public byte StackCount;
    }
}
