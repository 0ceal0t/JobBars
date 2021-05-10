using Dalamud.Game.ClientState.Actors.Resolvers;
using Dalamud.Plugin;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Gauges {
    public unsafe class ActionGaugeManager {
        public UIBuilder _UI;

        public Dictionary<JobIds, ActionGauge[]> JobToGauges;
        public JobIds CurrentJob = JobIds.OTHER;
        public ActionGauge[] CurrentGauges => JobToGauges[CurrentJob];
        private DateTime LastTick = DateTime.Now;

        public ActionGaugeManager(UIBuilder ui) {
            _UI = ui;

            JobToGauges = new Dictionary<JobIds, ActionGauge[]>();
            JobToGauges.Add(JobIds.OTHER, new ActionGauge[] { });
            JobToGauges.Add(JobIds.GNB, new ActionGauge[] {
                new ActionGaugeGCD("No Mercy", 20, 9)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.NoMercy)
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
            });
            JobToGauges.Add(JobIds.AST, new ActionGauge[] {
                new ActionGaugeTimer("Combust", 30)
                    .WithTriggers(new[]{
                        new Item(BuffIds.Combust1),
                        new Item(BuffIds.Combust2),
                        new Item(BuffIds.Combust3)
                    })
                    .WithReplaceIcon(new []{ ActionIds.Combust1, ActionIds.Combust2, ActionIds.Combust3 }, _UI.Icon)
            });
            JobToGauges.Add(JobIds.PLD, new ActionGauge[] {
                new ActionGaugeGCD("Requiescat", 12, 5)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.Requiescat)
                    })
                    .WithIncrement(new[]
                    {
                        new Item(ActionIds.HolySpirit),
                        new Item(ActionIds.HolyCircle),
                        new Item(ActionIds.Confiteor)
                    }),
                new ActionGaugeGCD("Fight or Flight", 25, 11)
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
                    }),
                new ActionGaugeTimer("Goring Blade", 21)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.GoringBlade)
                    })
                    .WithReplaceIcon(new[]
                    {
                        ActionIds.GoringBlade
                    }, _UI.Icon)
            });
            JobToGauges.Add(JobIds.WAR, new ActionGauge[] {
                new ActionGaugeGCD("Inner Release", 10, 5)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.InnerRelease)
                    })
                    .WithIncrement(new[]
                    {
                        new Item(ActionIds.FellCleave),
                        new Item(ActionIds.Decimate)
                    }),
                new ActionGaugeTimer("Storm's Eye", 60)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.StormsEye)
                    })
            });
            JobToGauges.Add(JobIds.DRK, new ActionGauge[] {
                new ActionGaugeGCD("Delirium", 10, 5)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.Delirium)
                    })
                    .WithIncrement(new[]{ 
                        new Item(ActionIds.BloodSpiller),
                        new Item(ActionIds.Quietus)
                    }),
                new ActionGaugeGCD("Blood Weapon", 10, 5)
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
            }); ;
            JobToGauges.Add(JobIds.SCH, new ActionGauge[] { });
            JobToGauges.Add(JobIds.WHM, new ActionGauge[] { });
            JobToGauges.Add(JobIds.BRD, new ActionGauge[] { });
            JobToGauges.Add(JobIds.DRG, new ActionGauge[] { });
            JobToGauges.Add(JobIds.SMN, new ActionGauge[] { });
            JobToGauges.Add(JobIds.SAM, new ActionGauge[] { });
            JobToGauges.Add(JobIds.BLM, new ActionGauge[] { });
            JobToGauges.Add(JobIds.RDM, new ActionGauge[] { });
            JobToGauges.Add(JobIds.MCH, new ActionGauge[] { });
            JobToGauges.Add(JobIds.DNC, new ActionGauge[] { });
            JobToGauges.Add(JobIds.NIN, new ActionGauge[] { });
            JobToGauges.Add(JobIds.MNK, new ActionGauge[] { });
            JobToGauges.Add(JobIds.BLU, new ActionGauge[] { });
        }

        public void SetJob(ClassJob job) {
            JobIds _job = job.Id < 19 ? JobIds.OTHER : (JobIds)job.Id;
            if(_job != CurrentJob) {
                // ==== REMOVE OLD =======
                foreach(var g_ in CurrentGauges) {
                    g_._UI = null;
                }
                _UI.HideAllGauges();
                _UI.Icon.Reset();

                CurrentJob = _job;
                PluginLog.Log($"SWITCHED JOB TO {CurrentJob}");

                // ==== SETUP NEW =======
                int idx = 0;
                int y = 0;
                foreach(var g_ in CurrentGauges) {
                    g_._UI = g_.Type == ActionGaugeType.GCDs ? _UI.Arrows[idx] : _UI.Gauges[idx];
                    _UI.Show(g_._UI);
                    UiHelper.SetPosition(g_._UI.RootRes, 0, y);
                    y += g_._UI.Height;
                    idx++;

                    g_.Setup();
                }
            }
        }

        public void PerformAction(Item action) {
            foreach(var g_ in CurrentGauges) {
                g_.ProcessAction(action);
            }
        }

        public void GetBuffDuration(Item buff, float duration) {
            if(duration < 0) { duration *= -1; }
            foreach (var g_ in CurrentGauges) {
                g_.ProcessDuration(buff, duration);
            }
        }

        public void Tick() {
            var currentTime = DateTime.Now;
            float deltaSecond = (float)(currentTime - LastTick).TotalSeconds;
            foreach(var g_ in CurrentGauges) {
                g_.Tick(currentTime, deltaSecond);
            }
            _UI.Icon.Update();

            LastTick = currentTime;
        }
    }
}
