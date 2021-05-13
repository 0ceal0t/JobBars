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
            // ============ GNB ==================
            JobToGauges.Add(JobIds.GNB, new Gauge[] {
                new GaugeGCD("No Mercy", 20, 9)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.NoMercy) // buffs are more reliable for tracking gcds within a buff window than an action
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Orange))
            });
            // ============ PLD ==================
            JobToGauges.Add(JobIds.PLD, new Gauge[] {
                new GaugeGCD("Requiescat", 12, 5)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.Requiescat)
                    })
                    .WithSpecificIncrement(new[]
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
                    .WithSpecificIncrement(new[] // has to be physical
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
            // ============ WAR ==================
            JobToGauges.Add(JobIds.WAR, new Gauge[] {
                new GaugeGCD("Inner Release", 10, 5)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.InnerRelease)
                    })
                    .WithSpecificIncrement(new[]
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
            // ============ DRK ==================
            JobToGauges.Add(JobIds.DRK, new Gauge[] {
                new GaugeGCD("Delirium", 10, 5)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.Delirium)
                    })
                    .WithSpecificIncrement(new[]{ 
                        new Item(ActionIds.BloodSpiller),
                        new Item(ActionIds.Quietus)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Red)),
                new GaugeGCD("Blood Weapon", 10, 5)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.BloodWeapon)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.DarkBlue))
            });
            // ============ AST ==================
            JobToGauges.Add(JobIds.AST, new Gauge[] {
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
            // ============ SCH ==================
            JobToGauges.Add(JobIds.SCH, new Gauge[] {
                new GaugeTimer("Biolysis", 30)
                    .WithTriggers(new[]{
                        new Item(ActionIds.ArcBio),
                        new Item(ActionIds.ArcBio2),
                        new Item(ActionIds.Biolysis)
                    })
                    .WithReplaceIcon(new []{
                        ActionIds.ArcBio,
                        ActionIds.ArcBio2,
                        ActionIds.Biolysis
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.BlueGreen))
            });
            // ============ WHM ==================
            JobToGauges.Add(JobIds.WHM, new Gauge[] {
                new GaugeTimer("Dia", 30)
                    .WithTriggers(new[]{
                        new Item(ActionIds.Aero),
                        new Item(ActionIds.Aero2),
                        new Item(ActionIds.Dia)
                    })
                    .WithReplaceIcon(new []{
                        ActionIds.Aero,
                        ActionIds.Aero2,
                        ActionIds.Dia
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.LightBlue))
            });
            // ============ BRD ==================
            JobToGauges.Add(JobIds.BRD, new Gauge[] {
                new GaugeTimer("Caustic Bite", 30)
                    .WithTriggers(new[]{
                        new Item(ActionIds.CausticBite),
                        new Item(ActionIds.VenomousBite)
                    })
                    .WithReplaceIcon(new []{
                        ActionIds.CausticBite,
                        ActionIds.VenomousBite
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.Purple)),
                new GaugeTimer("Stormbite", 30)
                    .WithTriggers(new[]{
                        new Item(ActionIds.Windbite),
                        new Item(ActionIds.Stormbite)
                    })
                    .WithReplaceIcon(new []{
                        ActionIds.Windbite,
                        ActionIds.Stormbite
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.LightBlue)),
                new GaugeGCD("Raging Strikes", 20, 9)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.RagingStrikes)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Orange))
            });
            // ============ DRG ==================
            JobToGauges.Add(JobIds.DRG, new Gauge[] {
                new GaugeGCD("Lance Charge", 20, 9)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.LanceCharge)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Red)),
                new GaugeGCD("Dragon Sight", 20, 9)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.RightEye),
                        new Item(BuffIds.RightEye2)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Orange))
            });
            // ============ SMN ==================
            JobToGauges.Add(JobIds.SMN, new Gauge[] {
                new GaugeGCD("Summon Bahamut", 21, 8)
                    .WithTriggers(new[]
                    {
                        new Item(ActionIds.SummonBahamut)
                    })
                    .WithSpecificIncrement(new[]{
                        new Item(ActionIds.Wyrmwave)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.LightBlue)),
                new GaugeGCD("Firebird Trance", 21, 8)
                    .WithTriggers(new[]
                    {
                        new Item(ActionIds.FirebirdTrance)
                    })
                    .WithSpecificIncrement(new[]{
                        new Item(ActionIds.ScarletFlame)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Orange))
                    .WithStartHidden(),
                new GaugeTimer("Bio", 30)
                    .WithTriggers(new[]{
                        new Item(ActionIds.ArcBio),
                        new Item(ActionIds.ArcBio2),
                        new Item(ActionIds.Bio3)
                    })
                    .WithReplaceIcon(new []{
                        ActionIds.ArcBio,
                        ActionIds.ArcBio2,
                        ActionIds.Bio3
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.HealthGreen)),
                new GaugeTimer("Miasma", 30)
                    .WithTriggers(new[]{
                        new Item(ActionIds.Miasma),
                        new Item(ActionIds.Miasma3)
                    })
                    .WithReplaceIcon(new []{
                        ActionIds.Miasma,
                        ActionIds.Miasma3
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.BlueGreen))
            });
            // ============ SAM ==================
            JobToGauges.Add(JobIds.SAM, new Gauge[] {
                new GaugeTimer("Jinpu", 40)
                    .WithTriggers(new[]{
                        new Item(BuffIds.Jinpu)
                    })
                    .WithReplaceIcon(new []{
                        ActionIds.Jinpu
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.DarkBlue)),
                new GaugeTimer("Shifu", 40)
                    .WithTriggers(new[]{
                        new Item(BuffIds.Shifu)
                    })
                    .WithReplaceIcon(new []{
                        ActionIds.Shifu
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.Red)),
                new GaugeTimer("Higanbana", 60)
                    .WithTriggers(new[]{
                        new Item(ActionIds.Higanbana)
                    })
                    .WithVisual(GaugeVisual.Bar(UIColor.Orange)),
            });
            // ============ BLM ==================
            JobToGauges.Add(JobIds.BLM, new Gauge[] {
                new GaugeTimer("Thunder 3", 24)
                    .WithTriggers(new[]{
                        new Item(ActionIds.Thunder3),
                        new Item(ActionIds.Thunder)
                    })
                    .WithReplaceIcon(new []{
                        ActionIds.Thunder3,
                        ActionIds.Thunder
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.DarkBlue)),
                new GaugeTimer("Thunder 4", 18)
                    .WithTriggers(new[]{
                        new Item(ActionIds.Thunder4),
                        new Item(ActionIds.Thunder2)
                    })
                    .WithReplaceIcon(new []{
                        ActionIds.Thunder4,
                        ActionIds.Thunder2
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.Purple))
                    .WithStartHidden()
            });
            // ============ RDM ==================
            JobToGauges.Add(JobIds.RDM, new Gauge[] {
                new GaugeGCD("Manafication", 10, 5)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.Manafication)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.DarkBlue)),
                new GaugeGCD("Embolden", 20, 9)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.Embolden)
                    })
                    .WithNoRefresh()
                    .WithVisual(GaugeVisual.Arrow(UIColor.Orange))
            });
            // ============ MCH ==================
            JobToGauges.Add(JobIds.MCH, new Gauge[] {
                new GaugeGCD("Hypercharge", 9, 5)
                    .WithTriggers(new[]
                    {
                        new Item(ActionIds.Hypercharge)
                    })
                    .WithSpecificIncrement(new[]
                    {
                        new Item(ActionIds.AutoCrossbow),
                        new Item(ActionIds.HeatBlast)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Orange)),
                new GaugeGCD("Wildfire", 10, 6)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.Wildfire)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Red))
            });
            // ============ DNC ==================
            JobToGauges.Add(JobIds.DNC, new Gauge[] {
                new GaugeGCD("Devilment", 20, 9)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.Devilment)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Red))
            });
            // ============ NIN ==================
            JobToGauges.Add(JobIds.NIN, new Gauge[] {
                new GaugeGCD("Bunshin", 15, 5)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.Bunshin)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Red))
            });
            // ============ MNK ==================
            JobToGauges.Add(JobIds.MNK, new Gauge[] {
                new GaugeGCD("Riddle of Fire", 20, 9)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.RiddleOfFire)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Red)),
                new GaugeGCD("Perfect Balance", 15, 6)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.PerfectBalance)
                    })
                    .WithNoRefresh()
                    .WithVisual(GaugeVisual.Arrow(UIColor.Orange))
            });
            // ============ BLU ==================
            JobToGauges.Add(JobIds.BLU, new Gauge[] {
                new GaugeTimer("Song of Torment", 30)
                    .WithTriggers(new[]{
                        new Item(ActionIds.SongOfTorment)
                    })
                    .WithReplaceIcon(new []{
                        ActionIds.SongOfTorment
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.Red))
            });

            // ======== HIDING ===========
            JobToGauges[JobIds.SMN][0].HideGauge = JobToGauges[JobIds.SMN][1]; // bahamut + pheonix
            JobToGauges[JobIds.SMN][1].HideGauge = JobToGauges[JobIds.SMN][0];

            JobToGauges[JobIds.BLM][0].HideGauge = JobToGauges[JobIds.BLM][1]; // thunder 3 + thunder 4
            JobToGauges[JobIds.BLM][1].HideGauge = JobToGauges[JobIds.BLM][0];
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
                gauge.State = GaugeState.INACTIVE;
                gauge.UI = null;
            }
            UI.HideAllGauges();
            UI.Icon.Reset();
        }

        public void ResetJob(JobIds job) {
            if(job == CurrentJob) {
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
