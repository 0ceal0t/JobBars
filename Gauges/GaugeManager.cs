using Dalamud.Game.ClientState.Actors.Resolvers;
using Dalamud.Plugin;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Gauges {
    public unsafe class GaugeManager {
        public UIBuilder UI;

        public Dictionary<JobIds, Gauge[]> JobToGauges;
        public JobIds CurrentJob = JobIds.OTHER;
        public Gauge[] CurrentGauges => JobToGauges[CurrentJob];
        private DateTime LastTick = DateTime.Now;

        public GaugeManager(UIBuilder ui) {
            UI = ui;

            JobToGauges = new Dictionary<JobIds, Gauge[]>();
            JobToGauges.Add(JobIds.OTHER, new Gauge[] { });
            JobToGauges.Add(JobIds.GNB, new Gauge[] { // <===== GNB
                new GaugeGCD("No Mercy", 20, 9)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.NoMercy) // buffs are more reliable for tracking gcds within a buff window than an action
                    })
                    .WithIncrement(new[]
                    {
                        new Item(ActionIds.DemonSlice),
                        new Item(ActionIds.DemonSlaughter),
                        new Item(ActionIds.KeenEdge),
                        new Item(ActionIds.BrutalShell),
                        new Item(ActionIds.SolidBarrel),
                        new Item(ActionIds.BurstStrike),
                        new Item(ActionIds.SonicBreak),
                        new Item(ActionIds.GnashingFang),
                        new Item(ActionIds.SavageClaw),
                        new Item(ActionIds.WickedTalon),
                        new Item(ActionIds.FatedCicle)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Orange))
            });
            JobToGauges.Add(JobIds.PLD, new Gauge[] {  // <===== PLD
                new GaugeGCD("Requiescat", 12, 5)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.Requiescat)
                    })
                    .WithIncrement(new[]
                    {
                        new Item(ActionIds.HolySpirit),
                        new Item(ActionIds.HolyCircle),
                        new Item(ActionIds.Confiteor)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.LightBlue)),
                new GaugeGCD("Fight or Flight", 25, 11)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.FightOrFlight)
                    })
                    .WithIncrement(new[]
                    {
                        new Item(ActionIds.FastBlade),
                        new Item(ActionIds.RiotBlade),
                        new Item(ActionIds.RoyalAuthority),
                        new Item(ActionIds.Atonement),
                        new Item(ActionIds.GoringBlade),
                        new Item(ActionIds.TotalEclipse),
                        new Item(ActionIds.Prominence)
                    })
                    .WithVisual(GaugeVisual.Bar(UIColor.Red)),
                new GaugeTimer("Goring Blade", 21)
                    .WithTriggers(new[]
                    {
                        new Item(ActionIds.GoringBlade)
                    })
                    .WithReplaceIcon(new[]
                    {
                        ActionIds.GoringBlade
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.Orange))
            });
            JobToGauges.Add(JobIds.WAR, new Gauge[] { // <===== WAR
                new GaugeGCD("Inner Release", 10, 5)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.InnerRelease)
                    })
                    .WithIncrement(new[]
                    {
                        new Item(ActionIds.FellCleave),
                        new Item(ActionIds.Decimate)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Orange)),
                new GaugeTimer("Storm's Eye", 60)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.StormsEye)
                    })
                    .WithDefaultDuration(30)
                    .WithVisual(GaugeVisual.Bar(UIColor.Red))
            });
            JobToGauges.Add(JobIds.DRK, new Gauge[] { // <===== DRK
                new GaugeGCD("Delirium", 10, 5)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.Delirium)
                    })
                    .WithIncrement(new[]{ 
                        new Item(ActionIds.BloodSpiller),
                        new Item(ActionIds.Quietus)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Red)),
                new GaugeGCD("Blood Weapon", 10, 5)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.BloodWeapon)
                    })
                    .WithIncrement(new[]
                    {
                        new Item(ActionIds.HardSlash),
                        new Item(ActionIds.SiphonStrike),
                        new Item(ActionIds.Souleater),
                        new Item(ActionIds.Unmend),
                        new Item(ActionIds.Unleash),
                        new Item(ActionIds.StalwartSoul),
                        new Item(ActionIds.BloodSpiller),
                        new Item(ActionIds.Quietus)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.DarkBlue))
            });
            JobToGauges.Add(JobIds.AST, new Gauge[] { // <===== AST
                new GaugeTimer("Combust", 30)
                    .WithTriggers(new[]{
                        new Item(ActionIds.Combust1),
                        new Item(ActionIds.Combust2),
                        new Item(ActionIds.Combust3)
                    })
                    .WithReplaceIcon(new []{ 
                        ActionIds.Combust1,
                        ActionIds.Combust2,
                        ActionIds.Combust3
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.LightBlue))
            });
            JobToGauges.Add(JobIds.SCH, new Gauge[] { });
            JobToGauges.Add(JobIds.WHM, new Gauge[] { });
            JobToGauges.Add(JobIds.BRD, new Gauge[] { });
            JobToGauges.Add(JobIds.DRG, new Gauge[] { });
            JobToGauges.Add(JobIds.SMN, new Gauge[] { // <===== SMN
                new GaugeGCD("Summon Bahamut", 21, 8)
                    .WithTriggers(new[]
                    {
                        new Item(ActionIds.SummonBahamut)
                    })
                    .WithIncrement(new[]{
                        new Item(ActionIds.Wyrmwave)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.LightBlue)),
                new GaugeGCD("Firebird Trance", 21, 8)
                    .WithTriggers(new[]
                    {
                        new Item(ActionIds.FirebirdTrance)
                    })
                    .WithIncrement(new[]{
                        new Item(ActionIds.ScarletFlame)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Orange))
                    .WithStartHidden()
                    // TODO: DOTS! <---------------------------------------------

            });
            JobToGauges.Add(JobIds.SAM, new Gauge[] { });
            JobToGauges.Add(JobIds.BLM, new Gauge[] { });
            JobToGauges.Add(JobIds.RDM, new Gauge[] { });
            JobToGauges.Add(JobIds.MCH, new Gauge[] { });
            JobToGauges.Add(JobIds.DNC, new Gauge[] { });
            JobToGauges.Add(JobIds.NIN, new Gauge[] { });
            JobToGauges.Add(JobIds.MNK, new Gauge[] { });
            JobToGauges.Add(JobIds.BLU, new Gauge[] { });

            // ======== HIDING ===========
            JobToGauges[JobIds.SMN][0].HideGauge = JobToGauges[JobIds.SMN][1]; // bahamut + pheonix
            JobToGauges[JobIds.SMN][1].HideGauge = JobToGauges[JobIds.SMN][0];
        }

        public void SetJob(JobIds job) {
            Reset();
            CurrentJob = job;

            int idx = 0;
            int yPosition = 0;
            foreach(var gauge in CurrentGauges) {
                if (!(gauge.Enabled == !Configuration.Config.GaugeDisabled.Contains(gauge.Name))) { continue; }

                gauge.UI = (gauge.Visual.Type == GaugeVisualType.Arrow ? UI.Arrows[idx] : UI.Gauges[idx]);
                if(!gauge.StartHidden) {
                    gauge.UI.Show();
                    UiHelper.SetPosition(gauge.UI.RootRes, 0, yPosition);
                    yPosition += gauge.UI.Height;
                    idx++;
                }
                else {
                    gauge.UI.Hide();
                }
                gauge.Setup();
            }
        }

        public void Reset() {
            foreach (var gauge in CurrentGauges) {
                gauge.Active = false;
                gauge.UI = null;
            }
            UI.HideAllGauges();
            UI.Icon.Reset();
        }

        public void RefreshJob(JobIds job) {
            if(job == CurrentJob) {
                Reset();
                SetJob(job);
            }
        }

        public void PerformAction(Item action) {
            foreach(var gauge in CurrentGauges) {
                if(!gauge.Enabled) { continue; }
                gauge.ProcessAction(action);
            }
        }

        public void SetBuffDuration(Item buff, float duration, bool isRefresh) {
            if(duration < 0) { duration *= -1; }
            foreach (var gauge in CurrentGauges) {
                if (!gauge.Enabled) { continue; }
                gauge.ProcessDuration(buff, duration, isRefresh);
            }
        }

        public void Tick() {
            var currentTime = DateTime.Now;
            float deltaSecond = (float)(currentTime - LastTick).TotalSeconds;
            foreach(var gauge in CurrentGauges) {
                if (!gauge.Enabled) { continue; }
                gauge.Tick(currentTime, deltaSecond);
            }
            UI.Icon.Update();
            LastTick = currentTime;
        }
    }
}
