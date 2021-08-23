using Dalamud.Game.ClientState.Objects.Types;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace JobBars.Gauges {
    public unsafe partial class GaugeManager : JobConfigurationManager<Gauge[]> {
        public JobIds CurrentJob = JobIds.OTHER;
        private Gauge[] CurrentGauges => JobToValue.TryGetValue(CurrentJob, out var gauges) ? gauges : JobToValue[JobIds.OTHER];

        public GaugeManager() : base("##JobBars_Gauges") {
            Init();
        }

        public void SetJob(JobIds job) {
            //===== CLEANUP OLD =======
            foreach (var gauge in CurrentGauges) gauge.UnloadUI();
            JobBars.Builder.HideAllGauges();
            JobBars.Icon.Reset();

            //====== SET UP NEW =======
            CurrentJob = job;
            int idx = 0;
            foreach (var gauge in CurrentGauges) {
                gauge.LoadUI(GetUI(idx, gauge.GetVisualType()));
                idx++;
            }
            UpdatePositionScale();
        }

        public void UpdatePositionScale(JobIds job) {
            if (job == CurrentJob) UpdatePositionScale();
        }

        public void UpdatePositionScale() {
            JobBars.Builder.SetGaugePosition(JobBars.Config.GaugePosition);
            JobBars.Builder.SetGaugeScale(JobBars.Config.GaugeScale);

            int totalPosition = 0;
            foreach (var gauge in CurrentGauges.OrderBy(g => g.Order).Where(g => g.Enabled)) {
                if (JobBars.Config.GaugeSplit) { // SPLIT
                    gauge.UI.SetSplitPosition(gauge.Position);
                }
                else {
                    if (JobBars.Config.GaugeHorizontal) { // HORIZONTAL
                        gauge.UI.SetPosition(new Vector2(totalPosition, gauge.UI.GetHorizontalYOffset()));
                        totalPosition += gauge.Width;
                    }
                    else { // VERTICAL
                        int xPosition = JobBars.Config.GaugeAlignRight ? 160 - gauge.Width : 0;
                        if(JobBars.Config.GaugeBottomToTop) { // BOTTOM TO TOP
                            gauge.UI.SetPosition(new Vector2(xPosition, totalPosition - gauge.Height));
                            totalPosition -= gauge.Height;
                        }
                        else {
                            gauge.UI.SetPosition(new Vector2(xPosition, totalPosition));
                            totalPosition += gauge.Height;
                        }
                    }
                }
            }
        }

        public void Reset() => SetJob(CurrentJob);

        public void ResetJob(JobIds job) {
            if (job == CurrentJob) Reset();
        }

        public void PerformAction(Item action) {
            if (!JobBars.Config.GaugesEnabled) return;

            foreach (var gauge in CurrentGauges) {
                if (!gauge.DoProcessInput()) continue;
                gauge.ProcessAction(action);
            }
        }

        public void Tick(bool inCombat) {
            if (!JobBars.Config.GaugesEnabled) return;

            if (JobBars.Config.GaugesHideOutOfCombat) {
                if (inCombat) JobBars.Builder.ShowGauges();
                else JobBars.Builder.HideGauges();
            }

            Dictionary<Item, BuffElem> BuffDict = new();
            var currentTime = DateTime.Now;
            int ownerId = (int)JobBars.ClientState.LocalPlayer.ObjectId;

            AddBuffs(JobBars.ClientState.LocalPlayer, ownerId, BuffDict);

            var prevEnemy = UIHelper.PreviousEnemyTarget;
            if (prevEnemy != null) AddBuffs(prevEnemy, ownerId, BuffDict);

            if (CurrentJob == JobIds.SCH && inCombat) { // only need this to catch excog for now
                UIHelper.GetPartyStatus(ownerId, BuffDict);
            }

            foreach (var gauge in CurrentGauges) {
                if (!gauge.DoProcessInput()) continue;
                gauge.Tick(currentTime, BuffDict);
            }

            JobBars.Icon.Tick();
        }

        private static UIGaugeElement GetUI(int idx, GaugeVisualType type) {
            return type switch {
                GaugeVisualType.Arrow => JobBars.Builder.Arrows[idx],
                GaugeVisualType.Bar => JobBars.Builder.Bars[idx],
                GaugeVisualType.Diamond => JobBars.Builder.Diamonds[idx],
                GaugeVisualType.BarDiamondCombo => new UIBarDiamondCombo(
                    JobBars.Builder.Bars[idx],
                    JobBars.Builder.Diamonds[idx]
                ), // kind of scuffed, but oh well
                _ => null,
            };
        }

        private static void AddBuffs(GameObject actor, int ownerId, Dictionary<Item, BuffElem> buffDict) {
            if (actor == null) return;
            if (actor is BattleChara charaActor) {
                foreach (var status in charaActor.StatusList) {
                    if (status.SourceID == ownerId) {
                        buffDict[new Item {
                            Id = status.StatusID,
                            Type = ItemType.Buff
                        }] = new BuffElem {
                            Duration = status.RemainingTime > 0 ? status.RemainingTime : status.RemainingTime * -1,
                            StackCount = status.StackCount
                        };
                    }
                }
            }
        }
    }

    public struct BuffElem {
        public float Duration;
        public byte StackCount;
    }
}
