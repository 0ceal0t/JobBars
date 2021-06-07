using Dalamud.Game.ClientState.Actors.Resolvers;
using Dalamud.Game.ClientState.Structs;
using Dalamud.Plugin;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Gauges {
    public unsafe class GaugeManager {
        public DalamudPluginInterface PluginInterface;
        public UIBuilder UI;

        public Dictionary<JobIds, Gauge[]> JobToGauges;
        public JobIds CurrentJob = JobIds.OTHER;
        public Gauge[] CurrentGauges => JobToGauges[CurrentJob];

        public GaugeManager(DalamudPluginInterface pi, UIBuilder ui) {
            UI = ui;
            PluginInterface = pi;

            JobToGauges = new Dictionary<JobIds, Gauge[]>();
            JobToGauges.Add(JobIds.OTHER, new Gauge[] { });
            // ============ GNB ==================
            JobToGauges.Add(JobIds.GNB, new Gauge[] {
                new GaugeGCD("No Mercy", 20, 9)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.NoMercy) // buffs are more reliable for tracking gcds within a buff window than an action
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Orange))
            });
            // ============ PLD ==================
            JobToGauges.Add(JobIds.PLD, new Gauge[] {
                new GaugeGCD("Requiescat", 12, 5)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Requiescat)
                    })
                    .WithSpecificIncrement(new []
                    {
                        new Item(ActionIds.HolySpirit),
                        new Item(ActionIds.HolyCircle),
                        new Item(ActionIds.Confiteor)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.LightBlue)),
                new GaugeGCD("Fight or Flight", 25, 11)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.FightOrFlight)
                    })
                    .WithSpecificIncrement(new [] // has to be physical
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
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.GoringBlade)
                    })
                    .WithReplaceIcon(new []
                    {
                        ActionIds.GoringBlade
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.Orange))
            });
            // ============ WAR ==================
            JobToGauges.Add(JobIds.WAR, new Gauge[] {
                new GaugeGCD("Inner Release", 10, 5)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.InnerRelease)
                    })
                    .WithSpecificIncrement(new []
                    {
                        new Item(ActionIds.FellCleave),
                        new Item(ActionIds.Decimate)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Orange)),
                new GaugeTimer("Storm's Eye", 60)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.StormsEye)
                    })
                    .WithDefaultDuration(30)
                    .WithVisual(GaugeVisual.Bar(UIColor.Red))
            });
            // ============ DRK ==================
            JobToGauges.Add(JobIds.DRK, new Gauge[] {
                new GaugeGCD("Delirium", 10, 5)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Delirium)
                    })
                    .WithSpecificIncrement(new []
                    { 
                        new Item(ActionIds.BloodSpiller),
                        new Item(ActionIds.Quietus)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Red)),
                new GaugeGCD("Blood Weapon", 10, 5)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.BloodWeapon)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.DarkBlue))
            });
            // ============ AST ==================
            JobToGauges.Add(JobIds.AST, new Gauge[] {
                new GaugeProc("Earthly Star Primed")
                    .WithProcs(new []
                    {
                        new Proc(BuffIds.GiantDominance, UIColor.LightBlue)
                    }),
                new GaugeTimer("Combust", 30)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Combust),
                        new Item(BuffIds.Combust2),
                        new Item(BuffIds.Combust3),
                    })
                    .WithReplaceIcon(new []
                    { 
                        ActionIds.Combust1,
                        ActionIds.Combust2,
                        ActionIds.Combust3
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.LightBlue)),
                new GaugeTimer("Lightspeed", 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Lightspeed)
                    })
                    .WithVisual(GaugeVisual.Bar(UIColor.Yellow))
                    .WithNoLowWarning()
            });
            // ============ SCH ==================
            JobToGauges.Add(JobIds.SCH, new Gauge[] {
                new GaugeTimer("Biolysis", 30)
                    .WithTriggers(new[]
                    {
                        new Item(BuffIds.ArcBio),
                        new Item(BuffIds.ArcBio2),
                        new Item(BuffIds.Biolysis)
                    })
                    .WithReplaceIcon(new []
                    {
                        ActionIds.ArcBio,
                        ActionIds.ArcBio2,
                        ActionIds.Biolysis
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.BlueGreen))
            });
            // ============ WHM ==================
            JobToGauges.Add(JobIds.WHM, new Gauge[] {
                new GaugeTimer("Dia", 30)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Aero),
                        new Item(BuffIds.Aero2),
                        new Item(BuffIds.Dia)
                    })
                    .WithReplaceIcon(new []
                    {
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
                        new Item(BuffIds.CausticBite),
                        new Item(BuffIds.VenomousBite),
                    })
                    .WithReplaceIcon(new []
                    {
                        ActionIds.CausticBite,
                        ActionIds.VenomousBite
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.Purple)),
                new GaugeTimer("Stormbite", 30)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Windbite),
                        new Item(BuffIds.Stormbite),
                    })
                    .WithReplaceIcon(new []
                    {
                        ActionIds.Windbite,
                        ActionIds.Stormbite
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.LightBlue)),
                new GaugeGCD("Raging Strikes", 20, 9)
                    .WithTriggers(new []
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
                    .WithSpecificIncrement(new []
                    {
                        new Item(ActionIds.Wyrmwave)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.LightBlue)),
                new GaugeGCD("Firebird Trance", 21, 8)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.FirebirdTrance)
                    })
                    .WithSpecificIncrement(new []
                    {
                        new Item(ActionIds.ScarletFlame)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Orange))
                    .WithStartHidden(),
                new GaugeTimer("Bio", 30)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.ArcBio),
                        new Item(BuffIds.ArcBio2),
                        new Item(BuffIds.Bio3),
                    })
                    .WithReplaceIcon(new []
                    {
                        ActionIds.ArcBio,
                        ActionIds.ArcBio2,
                        ActionIds.Bio3
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.HealthGreen)),
                new GaugeTimer("Miasma", 30)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Miasma),
                        new Item(BuffIds.Miasma3),
                    })
                    .WithReplaceIcon(new []
                    {
                        ActionIds.Miasma,
                        ActionIds.Miasma3
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.BlueGreen))
            });
            // ============ SAM ==================
            JobToGauges.Add(JobIds.SAM, new Gauge[] {
                new GaugeTimer("Jinpu", 40)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Jinpu)
                    })
                    .WithReplaceIcon(new []
                    {
                        ActionIds.Jinpu
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.DarkBlue)),
                new GaugeTimer("Shifu", 40)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Shifu)
                    })
                    .WithReplaceIcon(new []
                    {
                        ActionIds.Shifu
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.Red)),
                new GaugeTimer("Higanbana", 60)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Higanbana)
                    })
                    .WithVisual(GaugeVisual.Bar(UIColor.Orange)),
            });
            // ============ BLM ==================
            JobToGauges.Add(JobIds.BLM, new Gauge[] {
                new GaugeProc("Firestarter/Thundercloud")
                    .WithProcs(new []
                    {
                        new Proc(BuffIds.Thundercloud, UIColor.DarkBlue),
                        new Proc(BuffIds.Firestarter, UIColor.Orange)
                    }),
                new GaugeTimer("Thunder 3", 24)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Thunder3),
                        new Item(BuffIds.Thunder)
                    })
                    .WithReplaceIcon(new []
                    {
                        ActionIds.Thunder3,
                        ActionIds.Thunder
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.DarkBlue)),
                new GaugeTimer("Thunder 4", 18)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Thunder4),
                        new Item(BuffIds.Thunder2)
                    })
                    .WithReplaceIcon(new []
                    {
                        ActionIds.Thunder4,
                        ActionIds.Thunder2
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.Purple))
                    .WithStartHidden()
            });
            // ============ RDM ==================
            JobToGauges.Add(JobIds.RDM, new Gauge[] {
                new GaugeGCD("Manafication", 10, 5)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Manafication)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.DarkBlue)),
                new GaugeProc("Verfire/Verstone")
                    .WithProcs(new []
                    {
                        new Proc(BuffIds.VerstoneReady, UIColor.White),
                        new Proc(BuffIds.VerfireReady, UIColor.Red)
                    })
            });
            // ============ MCH ==================
            JobToGauges.Add(JobIds.MCH, new Gauge[] {
                new GaugeGCD("Hypercharge", 9, 5)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Hypercharge)
                    })
                    .WithSpecificIncrement(new []
                    {
                        new Item(ActionIds.AutoCrossbow),
                        new Item(ActionIds.HeatBlast)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Orange)),
                new GaugeGCD("Wildfire", 10, 6)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Wildfire)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Red)),
                new GaugeCharges("Gauss Round Charges", 30, 3)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.GaussRound)
                    })
                    .WithVisual(GaugeVisual.BarDiamondCombo(UIColor.Red)),
                new GaugeCharges("Ricochet Charges", 30, 3)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Ricochet)
                    })
                    .WithVisual(GaugeVisual.BarDiamondCombo(UIColor.LightBlue))
            });
            // ============ DNC ==================
            JobToGauges.Add(JobIds.DNC, new Gauge[] {
                new GaugeProc("Dancer Procs")
                    .WithProcs(new []
                    {
                        new Proc(BuffIds.FlourishingCascade, UIColor.BrightGreen),
                        new Proc(BuffIds.FlourishingFountain, UIColor.Yellow),
                        new Proc(BuffIds.FlourishingWindmill, UIColor.DarkBlue),
                        new Proc(BuffIds.FlourishingShower, UIColor.Red),
                        new Proc(BuffIds.FlourishingFanDance, UIColor.HealthGreen)
                    })
            });
            // ============ NIN ==================
            JobToGauges.Add(JobIds.NIN, new Gauge[] {
                new GaugeGCD("Bunshin", 15, 5)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Bunshin)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Red))
            });
            // ============ MNK ==================
            JobToGauges.Add(JobIds.MNK, new Gauge[] {
                new GaugeGCD("Perfect Balance", 15, 6)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.PerfectBalance)
                    })
                    .WithNoRefresh()
                    .WithVisual(GaugeVisual.Arrow(UIColor.Orange)),
                new GaugeGCD("Riddle of Fire", 20, 9)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.RiddleOfFire)
                    })
                    .WithVisual(GaugeVisual.Arrow(UIColor.Red))
            });
            // ============ BLU ==================
            JobToGauges.Add(JobIds.BLU, new Gauge[] {
                new GaugeTimer("Song of Torment", 30)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.SongOfTorment)
                    })
                    .WithReplaceIcon(new []
                    {
                        ActionIds.SongOfTorment
                    }, UI.Icon)
                    .WithVisual(GaugeVisual.Bar(UIColor.Red))
            });

            // ======== HIDING ===========
            JobToGauges[JobIds.SMN][0].HideGauge = JobToGauges[JobIds.SMN][1]; // bahamut + pheonix
            JobToGauges[JobIds.SMN][1].HideGauge = JobToGauges[JobIds.SMN][0];

            JobToGauges[JobIds.BLM][1].HideGauge = JobToGauges[JobIds.BLM][2]; // thunder 3 + thunder 4
            JobToGauges[JobIds.BLM][2].HideGauge = JobToGauges[JobIds.BLM][1];
        }

        public void SetJob(JobIds job) {
            // RESET
            foreach (var gauge in CurrentGauges) {
                gauge.State = GaugeState.Inactive;
                gauge.UI = null;
            }
            UI.HideAllGauges();
            UI.Icon.Reset();

            CurrentJob = job;
            int totalPosition = 0;
            int enabledIdx = 0;
            foreach (var gauge in CurrentGauges.OrderBy(g => g.Order)) {
                if (!(gauge.Enabled = !Configuration.Config.GaugeDisabled.Contains(gauge.Name))) { continue; }

                gauge.UI = GetUI(enabledIdx, gauge.Visual.Type);
                if(!gauge.StartHidden) {
                    gauge.UI.Show();
                    if(Configuration.Config.GaugeSplit) { // SPLIT
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
                    enabledIdx++;
                }
                else {
                    gauge.UI.Hide();
                }
                gauge.SetupVisual();
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
            foreach(var gauge in CurrentGauges.Where(x => x.Enabled)) {
                gauge.ProcessAction(action);
            }
        }

        public void Tick() {
            var currentTime = DateTime.Now;

            Dictionary<Item, float> BuffDict = new Dictionary<Item, float>();
            /*foreach(var status in PluginInterface.ClientState.LocalPlayer.StatusEffects) {
                BuffDict[new Item
                {
                    Id = (uint)status.EffectId,
                    Type = ItemType.Buff
                }] = status.Duration > 0 ? status.Duration : status.Duration * -1;
            }*/
            var selfBuffAddr = PluginInterface.ClientState.LocalPlayer.Address + ActorOffsets.UIStatusEffects;
            for (int i = 0; i < 30; i++) {
                var addr = selfBuffAddr + i * 0xC;
                var status = (StatusEffect)Marshal.PtrToStructure(addr, typeof(StatusEffect));
                BuffDict[new Item
                {
                    Id = (uint)status.EffectId,
                    Type = ItemType.Buff
                }] = status.Duration > 0 ? status.Duration : status.Duration * -1;
            }

            if (PluginInterface.ClientState.Targets.CurrentTarget != null) {
                /*foreach (var status in PluginInterface.ClientState.Targets.CurrentTarget.StatusEffects) {
                    if (status.OwnerId.Equals(PluginInterface.ClientState.LocalPlayer?.ActorId)) {
                        BuffDict[new Item
                        {
                            Id = (uint)status.EffectId,
                            Type = ItemType.Buff
                        }] = status.Duration > 0 ? status.Duration : status.Duration * -1;
                    }
                }*/

                var buffAddr = PluginInterface.ClientState.Targets.CurrentTarget.Address + ActorOffsets.UIStatusEffects;
                for(int i = 0; i < 30; i++) {
                    var addr = buffAddr + i * 0xC;
                    var status = (StatusEffect) Marshal.PtrToStructure(addr, typeof(StatusEffect));
                    if (status.OwnerId.Equals(PluginInterface.ClientState.LocalPlayer?.ActorId)) {
                        BuffDict[new Item
                        {
                            Id = (uint)status.EffectId,
                            Type = ItemType.Buff
                        }] = status.Duration > 0 ? status.Duration : status.Duration * -1;
                    }
                }
            }

            foreach(var gauge in CurrentGauges) {
                if (!gauge.Enabled) { continue; }
                gauge.Tick(currentTime, BuffDict);
            }
            UI.Icon.Update();
        }
    }
}
