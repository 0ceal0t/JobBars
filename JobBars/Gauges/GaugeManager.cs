using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin;
using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace JobBars.Gauges {
    public unsafe partial class GaugeManager {
        public static GaugeManager Manager { get; private set; }
        public static void Dispose() { Manager = null; }

        public JobIds CurrentJob = JobIds.OTHER;

        private readonly DalamudPluginInterface PluginInterface;

        private Dictionary<JobIds, Gauge[]> JobToGauges;
        private Gauge[] CurrentGauges => JobToGauges.TryGetValue(CurrentJob, out var gauges) ? gauges : JobToGauges[JobIds.OTHER];

        private readonly IntPtr TargetAddress;

        public GaugeManager(DalamudPluginInterface pluginInterface) {
            Manager = this;
            PluginInterface = pluginInterface;
            TargetAddress = PluginInterface.TargetModuleScanner.GetStaticAddressFromSig("48 8B 05 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? FF 50 ?? 48 85 DB", 3);

            Init();
        }

        public void SetJob(JobIds job) {
            //===== CLEANUP OLD =======
            foreach (var gauge in CurrentGauges) gauge.UnloadUI();
            UIBuilder.Builder.HideAllGauges();
            UIIconManager.Manager.Reset();

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
            UIBuilder.Builder.SetGaugePosition(Configuration.Config.GaugePosition);
            UIBuilder.Builder.SetGaugeScale(Configuration.Config.GaugeScale);

            int totalPosition = 0;
            foreach (var gauge in CurrentGauges.OrderBy(g => g.Order).Where(g => g.Enabled)) {
                if (Configuration.Config.GaugeSplit) { // SPLIT
                    gauge.UI.SetSplitPosition(gauge.Position);
                }
                else {
                    if (Configuration.Config.GaugeHorizontal) { // HORIZONTAL
                        gauge.UI.SetPosition(new Vector2(totalPosition, gauge.UI.GetHorizontalYOffset()));
                        totalPosition += gauge.Width;
                    }
                    else { // VERTICAL
                        int xPosition = Configuration.Config.GaugeAlignRight ? 160 - gauge.Width : 0;
                        gauge.UI.SetPosition(new Vector2(xPosition, totalPosition));
                        totalPosition += gauge.Height;
                    }
                }
            }
        }

        public void Reset() => SetJob(CurrentJob);

        public void ResetJob(JobIds job) {
            if (job == CurrentJob) Reset();
        }

        public void PerformAction(Item action) {
            if (!Configuration.Config.GaugesEnabled) return;

            foreach (var gauge in CurrentGauges) {
                if (!gauge.DoProcessInput()) continue;
                gauge.ProcessAction(action);
            }
        }

        public void Tick(bool inCombat) {
            if (!Configuration.Config.GaugesEnabled) return;

            if (Configuration.Config.GaugesHideOutOfCombat) {
                if (inCombat) UIBuilder.Builder.ShowGauges();
                else UIBuilder.Builder.HideGauges();
            }

            Dictionary<Item, BuffElem> BuffDict = new();
            var currentTime = DateTime.Now;
            int ownerId = (int)PluginInterface.ClientState.LocalPlayer.ObjectId;

            AddBuffs(PluginInterface.ClientState.LocalPlayer, ownerId, BuffDict);

            var prevEnemy = GetPreviousEnemyTarget();
            if (prevEnemy != null) AddBuffs(prevEnemy, ownerId, BuffDict);

            if (CurrentJob == JobIds.SCH && inCombat) { // only need this to catch excog for now
                DataManager.GetPartyStatus(ownerId, BuffDict);
            }

            foreach (var gauge in CurrentGauges) {
                if (!gauge.DoProcessInput()) continue;
                gauge.Tick(currentTime, BuffDict);
            }

            UIIconManager.Manager.Tick();
        }

        private static UIGaugeElement GetUI(int idx, GaugeVisualType type) {
            return type switch {
                GaugeVisualType.Arrow => UIBuilder.Builder.Arrows[idx],
                GaugeVisualType.Bar => UIBuilder.Builder.Bars[idx],
                GaugeVisualType.Diamond => UIBuilder.Builder.Diamonds[idx],
                GaugeVisualType.BarDiamondCombo => new UIBarDiamondCombo(
                    UIBuilder.Builder.Bars[idx],
                    UIBuilder.Builder.Diamonds[idx]
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

        private GameObject GetPreviousEnemyTarget() {
            var actorAddress = Marshal.ReadIntPtr(TargetAddress + 0xF0);
            if (actorAddress == IntPtr.Zero) return null;

            return PluginInterface.ClientState.Objects.CreateObjectReference(actorAddress);
        }
    }

    public struct BuffElem {
        public float Duration;
        public byte StackCount;
    }
}
