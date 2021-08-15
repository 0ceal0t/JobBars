using Dalamud.Game.ClientState.Objects.Types;
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
        public static GaugeManager Manager { get; private set; }

        private readonly DalamudPluginInterface PluginInterface;
        private readonly PList Party;

        public Dictionary<JobIds, Gauge[]> JobToGauges;
        public JobIds CurrentJob = JobIds.OTHER;
        public Gauge[] CurrentGauges => JobToGauges.TryGetValue(CurrentJob, out var gauges) ? gauges : JobToGauges[JobIds.OTHER];

        public IntPtr TargetAddress;

        public GaugeManager(DalamudPluginInterface pi, PList party) {
            Manager = this;
            PluginInterface = pi;
            Party = party;
            TargetAddress = PluginInterface.TargetModuleScanner.GetStaticAddressFromSig("48 8B 05 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? FF 50 ?? 48 85 DB", 3);
            Init();
        }

        public void SetJob(JobIds job) {
            foreach (var gauge in CurrentGauges) gauge.Dispose();
            UIBuilder.Builder.ResetGauges();
            UIIconManager.Manager.Reset();

            CurrentJob = job;
            foreach (var gauge in CurrentGauges) gauge.Setup();
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
                    gauge.UI?.SetSplitPosition(gauge.Position);
                }
                else {
                    if (Configuration.Config.GaugeHorizontal) { // HORIZONTAL
                        gauge.UI?.SetPosition(new Vector2(totalPosition, gauge.UI.GetHorizontalYOffset()));
                        totalPosition += gauge.Width;
                    }
                    else { // VERTICAL
                        int xPosition = Configuration.Config.GaugeAlignRight ? 160 - gauge.Width : 0;
                        gauge.UI?.SetPosition(new Vector2(xPosition, totalPosition));
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
            foreach (var gauge in CurrentGauges.Where(x => x.DoProcessInput())) {
                gauge.ProcessAction(action);
            }
        }

        public void Tick(bool inCombat) {
            if (!Configuration.Config.GaugesEnabled) return;

            if (Configuration.Config.GaugesHideOutOfCombat) {
                UIBuilder.Builder.SetGaugeVisible(inCombat);
            }

            Dictionary<Item, BuffElem> BuffDict = new();
            var currentTime = DateTime.Now;

            int ownerId = (int)PluginInterface.ClientState.LocalPlayer.ObjectId;

            AddBuffs(PluginInterface.ClientState.LocalPlayer, ownerId, BuffDict);

            var prevEnemy = GetPreviousEnemyTarget();
            if (prevEnemy != null) {
                AddBuffs(prevEnemy, ownerId, BuffDict);
            }

            if (CurrentJob == JobIds.SCH && inCombat) { // only need this to catch excog for now
                foreach (var pMember in Party) {
                    if (pMember == null) continue;

                    foreach (var actor in PluginInterface.ClientState.Objects) {
                        if (actor == null) continue;
                        if (actor.ObjectId == pMember.ObjectId) {
                            AddBuffs(actor, ownerId, BuffDict);
                        }
                    }
                }
            }

            foreach (var gauge in CurrentGauges) {
                if (!gauge.DoProcessInput()) continue;
                gauge.Tick(currentTime, BuffDict);
            }

            UIIconManager.Manager.Tick();
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
