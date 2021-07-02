using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Gauges {
    public unsafe partial class GaugeManager {
        private void Init() {
            JobToGauges = new Dictionary<JobIds, Gauge[]>();
            JobToGauges.Add(JobIds.OTHER, new Gauge[] { });
            // ============ GNB ==================
            JobToGauges.Add(JobIds.GNB, new Gauge[] {
                new GaugeGCD("No Mercy", GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 9,
                    MaxDuration = 20,
                    Color = UIColor.Orange,
                    Triggers = new []{
                        new Item(BuffIds.NoMercy)
                    }
                })
            });
            // ============ PLD ==================
            JobToGauges.Add(JobIds.PLD, new Gauge[] {
                new GaugeGCD("Requiescat", GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 5,
                    MaxDuration = 12,
                    Color = UIColor.LightBlue,
                    Increment = new []{
                        new Item(ActionIds.HolySpirit),
                        new Item(ActionIds.HolyCircle),
                        new Item(ActionIds.Confiteor)
                    },
                    Triggers = new []{
                        new Item(BuffIds.Requiescat)
                    }
                }),
                new GaugeGCD("Fight or Flight", GaugeVisualType.Bar, new SubGaugeGCDProps {
                    MaxCounter = 11,
                    MaxDuration = 25,
                    Color = UIColor.Red,
                    Increment = new []{
                        new Item(ActionIds.FastBlade),
                        new Item(ActionIds.RiotBlade),
                        new Item(ActionIds.RoyalAuthority),
                        new Item(ActionIds.Atonement),
                        new Item(ActionIds.GoringBlade),
                        new Item(ActionIds.TotalEclipse),
                        new Item(ActionIds.Prominence)
                    },
                    Triggers = new[] {
                        new Item(BuffIds.FightOrFlight)
                    }
                }),
                new GaugeTimer("Goring Blade", new SubGaugeTimerProps {
                    MaxDuration = 21,
                    Color = UIColor.Orange,
                    Triggers = new []{
                        new Item(BuffIds.GoringBlade)
                    },
                    Icons = new []{
                        ActionIds.GoringBlade
                    }
                })
            });
            // ============ WAR ==================
            JobToGauges.Add(JobIds.WAR, new Gauge[] {
                new GaugeGCD("Inner Release", GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 5,
                    MaxDuration = 10,
                    Color = UIColor.Orange,
                    Increment = new []{
                        new Item(ActionIds.FellCleave),
                        new Item(ActionIds.Decimate)
                    },
                    Triggers = new []{
                        new Item(BuffIds.InnerRelease)
                    }
                }),
                new GaugeTimer("Storm's Eye", new SubGaugeTimerProps {
                    MaxDuration = 60,
                    DefaultDuration = 30,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.StormsEye)
                    }
                })
            });
            // ============ DRK ==================
            JobToGauges.Add(JobIds.DRK, new Gauge[] {
                new GaugeGCD("Delirium", GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 5,
                    MaxDuration = 10,
                    Color = UIColor.Red,
                    Increment = new []{
                        new Item(ActionIds.BloodSpiller),
                        new Item(ActionIds.Quietus)
                    },
                    Triggers = new []{
                        new Item(BuffIds.Delirium)
                    }
                }),
                new GaugeGCD("Blood Weapon", GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 5,
                    MaxDuration = 10,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{
                        new Item(BuffIds.BloodWeapon)
                    }
                })
            });
            // ============ AST ==================
            JobToGauges.Add(JobIds.AST, new Gauge[] {
                new GaugeProc("Earthly Star Primed", new GaugeProcProps{
                    Procs = new []{
                        new Proc(BuffIds.GiantDominance, UIColor.LightBlue)
                    }
                }),
                new GaugeTimer("Combust", new [] {
                    new SubGaugeTimerProps {
                        MaxDuration = 30,
                        Color = UIColor.LightBlue,
                        SubName = "Combust 2+3",
                        Triggers = new []{
                            new Item(BuffIds.Combust2),
                            new Item(BuffIds.Combust3)
                        },
                        Icons = new []{
                            ActionIds.Combust1,
                            ActionIds.Combust2,
                            ActionIds.Combust3
                        }
                    },
                    new SubGaugeTimerProps {
                        MaxDuration = 18,
                        Color = UIColor.LightBlue,
                        SubName = "Combust 1",
                        Triggers = new []{
                            new Item(BuffIds.Combust)
                        },
                        Icons = new []{
                            ActionIds.Combust1,
                            ActionIds.Combust2,
                            ActionIds.Combust3
                        }
                    },
                }),
                new GaugeTimer("Lightspeed", new SubGaugeTimerProps {
                    MaxDuration = 15,
                    Color = UIColor.Yellow,
                    HideLowWarning = true,
                    Triggers = new []{
                        new Item(BuffIds.Lightspeed)
                    }
                })
            });
            // ============ SCH ==================
            JobToGauges.Add(JobIds.SCH, new Gauge[] {
                new GaugeProc("Excog", new GaugeProcProps{
                    Procs = new []{
                        new Proc(BuffIds.Excog, UIColor.BrightGreen)
                    }
                }),
                new GaugeTimer("Biolysis", new SubGaugeTimerProps {
                    MaxDuration = 30,
                    Color = UIColor.BlueGreen,
                    Triggers = new []{
                        new Item(BuffIds.ArcBio),
                        new Item(BuffIds.ArcBio2),
                        new Item(BuffIds.Biolysis)
                    },
                    Icons = new []{
                        ActionIds.SchBio,
                        ActionIds.SchBio2,
                        ActionIds.Biolysis
                    }
                })
            });
            // ============ WHM ==================
            JobToGauges.Add(JobIds.WHM, new Gauge[] {
                new GaugeTimer("Dia", new []{
                    new SubGaugeTimerProps {
                        MaxDuration = 30,
                        Color = UIColor.LightBlue,
                        SubName = "Dia",
                        Triggers = new []{
                            new Item(BuffIds.Dia)
                        },
                        Icons = new []{
                            ActionIds.Dia,
                            ActionIds.Aero,
                            ActionIds.Aero2
                        }
                    },
                    new SubGaugeTimerProps {
                        MaxDuration = 18,
                        Color = UIColor.LightBlue,
                        SubName = "Aero 1+2",
                        Triggers = new [] {
                            new Item(BuffIds.Aero),
                            new Item(BuffIds.Aero2)
                        },
                        Icons = new []{
                            ActionIds.Dia,
                            ActionIds.Aero,
                            ActionIds.Aero2
                        }
                    }
                })
            });
            // ============ BRD ==================
            JobToGauges.Add(JobIds.BRD, new Gauge[] {
                new GaugeProc("Straight Shot Ready/BloodLetter", new GaugeProcProps {
                    Procs = new []{
                        new Proc(BuffIds.StraightShotReady, UIColor.Yellow),
                        new Proc(ActionIds.BloodLetter, UIColor.Red)
                    }
                }),
                new GaugeTimer("Caustic Bite", new SubGaugeTimerProps {
                    MaxDuration = 30,
                    Color = UIColor.Purple,
                    Triggers = new []{
                        new Item(BuffIds.CausticBite),
                        new Item(BuffIds.VenomousBite)
                    },
                    Icons = new []{
                        ActionIds.CausticBite,
                        ActionIds.VenomousBite
                    }
                }),
                new GaugeTimer("Stormbite", new SubGaugeTimerProps {
                    MaxDuration = 30,
                    Color = UIColor.LightBlue,
                    Triggers = new []{
                        new Item(BuffIds.Windbite),
                        new Item(BuffIds.Stormbite),
                    },
                    Icons = new []{
                        ActionIds.Windbite,
                        ActionIds.Stormbite
                    }
                }),
                new GaugeGCD("Raging Strikes", GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 9,
                    MaxDuration = 20,
                    Color = UIColor.Orange,
                    Triggers = new []{
                        new Item(BuffIds.RagingStrikes)
                    }
                })
            });
            // ============ DRG ==================
            JobToGauges.Add(JobIds.DRG, new Gauge[] {
                new GaugeTimer("Disembowel", new SubGaugeTimerProps {
                    MaxDuration = 30,
                    Color = UIColor.Yellow,
                    Triggers = new []{
                        new Item(BuffIds.Disembowel)
                    }
                }),
                new GaugeTimer("Chaos Thrust", new SubGaugeTimerProps {
                    MaxDuration = 24,
                    Color = UIColor.Purple,
                    Triggers = new []{
                        new Item(BuffIds.ChaosThrust)
                    }
                }),
                new GaugeCharges("True North (DRG)", new GaugeChargesProps {
                    BarColor = UIColor.White,
                    SameColor = true,
                    Type = GaugeVisualType.BarDiamondCombo,
                    Parts = new []{
                        new GaugesChargesPartProps {
                            Diamond = true,
                            MaxCharges = 2,
                            CD = 45,
                            Triggers = new []{
                                new Item(ActionIds.TrueNorth)
                            }
                        },
                        new GaugesChargesPartProps {
                            Bar = true,
                            Duration = 10,
                            Triggers = new []{
                                new Item(BuffIds.TrueNorth)
                            }
                        }
                    }
                })
            });
            // ============ SMN ==================
            JobToGauges.Add(JobIds.SMN, new Gauge[] {
                new GaugeStacks("Ruin 4", new GaugeStacksProps {
                    MaxStacks = 4,
                    Triggers = new []{
                        new Item(BuffIds.Ruin4)
                    },
                    Type = GaugeVisualType.Diamond,
                    Color = UIColor.DarkBlue
                }),
                new GaugeGCD("Summon Bahamut", GaugeVisualType.Arrow, new []{
                    new SubGaugeGCDProps {
                        MaxCounter = 8,
                        MaxDuration = 21,
                        Color = UIColor.LightBlue,
                        SubName = "Bahamut",
                        Increment = new []{
                            new Item(ActionIds.Wyrmwave)
                        },
                        Triggers = new []{
                            new Item(ActionIds.SummonBahamut),
                            new Item(ActionIds.Wyrmwave) // in case this registers first for some reason
                        }
                    },
                    new SubGaugeGCDProps {
                        MaxCounter = 8,
                        MaxDuration = 21,
                        Color = UIColor.Orange,
                        SubName = "Phoenix",
                        Increment = new []{
                            new Item(ActionIds.ScarletFlame)
                        },
                        Triggers = new []{
                            new Item(ActionIds.FirebirdTrance),
                            new Item(ActionIds.ScarletFlame) // in case this registers first for some reason
                        }
                    }
                }),
                new GaugeTimer("Bio", new SubGaugeTimerProps {
                    MaxDuration = 30,
                    Color = UIColor.HealthGreen,
                    Triggers = new []{
                        new Item(BuffIds.ArcBio),
                        new Item(BuffIds.ArcBio2),
                        new Item(BuffIds.Bio3)
                    },
                    Icons = new []{
                        ActionIds.ArcBio,
                        ActionIds.ArcBio2,
                        ActionIds.Bio3
                    }
                }),
                new GaugeTimer("Miasma", new SubGaugeTimerProps {
                    MaxDuration = 30,
                    Color = UIColor.BlueGreen,
                    Triggers = new []{
                        new Item(BuffIds.Miasma),
                        new Item(BuffIds.Miasma3)
                    },
                    Icons = new []{
                        ActionIds.Miasma,
                        ActionIds.Miasma3
                    }
                })
            });
            // ============ SAM ==================
            JobToGauges.Add(JobIds.SAM, new Gauge[] {
                new GaugeTimer("Jinpu", new SubGaugeTimerProps {
                    MaxDuration = 40,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{
                        new Item(BuffIds.Jinpu)
                    }
                }),
                new GaugeTimer("Shifu", new SubGaugeTimerProps {
                    MaxDuration = 40,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.Shifu)
                    }
                }),
                new GaugeTimer("Higanbana", new SubGaugeTimerProps {
                    MaxDuration = 60,
                    Color = UIColor.Orange,
                    Triggers = new []{
                        new Item(BuffIds.Higanbana)
                    }
                }),
                new GaugeCharges("True North (SAM)", new GaugeChargesProps {
                    BarColor = UIColor.White,
                    SameColor = true,
                    Type = GaugeVisualType.BarDiamondCombo,
                    Parts = new []{
                        new GaugesChargesPartProps {
                            Diamond = true,
                            MaxCharges = 2,
                            CD = 45,
                            Triggers = new []{
                                new Item(ActionIds.TrueNorth)
                            }
                        },
                        new GaugesChargesPartProps {
                            Bar = true,
                            Duration = 10,
                            Triggers = new []{
                                new Item(BuffIds.TrueNorth)
                            }
                        }
                    }
                })
            });
            // ============ BLM ==================
            JobToGauges.Add(JobIds.BLM, new Gauge[] {
                new GaugeProc("Firestarter/Thundercloud", new GaugeProcProps{
                    ShowText = true,
                    Procs = new []{
                        new Proc(BuffIds.Firestarter, UIColor.Orange),
                        new Proc(BuffIds.Thundercloud, UIColor.LightBlue)
                    }
                }),
                new GaugeTimer("Thunder", new []{
                    new SubGaugeTimerProps
                    {
                        MaxDuration = 24,
                        Color = UIColor.DarkBlue,
                        SubName = "Thunder 3",
                        Triggers = new []{
                            new Item(BuffIds.Thunder3),
                            new Item(BuffIds.Thunder)
                        },
                        Icons = new []{
                            ActionIds.Thunder3,
                            ActionIds.Thunder
                        }
                    },
                    new SubGaugeTimerProps
                    {
                        MaxDuration = 18,
                        Color = UIColor.Purple,
                        SubName = "Thunder 4",
                        Triggers = new []{
                            new Item(BuffIds.Thunder4),
                            new Item(BuffIds.Thunder2)
                        },
                        Icons = new []{
                            ActionIds.Thunder4,
                            ActionIds.Thunder2
                        }
                    }
                })
            });
            // ============ RDM ==================
            JobToGauges.Add(JobIds.RDM, new Gauge[] {
                new GaugeProc("Verfire/Verstone", new GaugeProcProps{
                    Procs = new []{
                        new Proc(BuffIds.VerstoneReady, UIColor.White),
                        new Proc(BuffIds.VerfireReady, UIColor.Red)
                    }
                }),
                new GaugeGCD("Manafication", GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 5,
                    MaxDuration = 10,
                    Color = UIColor.DarkBlue,
                    Triggers = new[] {
                        new Item(BuffIds.Manafication)
                    }
                }),
                new GaugeStacks("Acceleration", new GaugeStacksProps {
                    MaxStacks = 3,
                    Triggers = new []{
                        new Item(BuffIds.Acceleration)
                    },
                    Type = GaugeVisualType.Diamond,
                    Color = UIColor.PurplePink
                }),
            });
            // ============ MCH ==================
            JobToGauges.Add(JobIds.MCH, new Gauge[] {
                new GaugeGCD("Hypercharge", GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 5,
                    MaxDuration = 9,
                    Color = UIColor.Orange,
                    Increment = new []{
                        new Item(ActionIds.AutoCrossbow),
                        new Item(ActionIds.HeatBlast)
                    },
                    Triggers = new []{
                        new Item(ActionIds.Hypercharge)
                    }
                }),
                new GaugeGCD("Wildfire", GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 6,
                    MaxDuration = 10,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.Wildfire)
                    }
                }),
                new GaugeCharges("Gauss Round Charges", new GaugeChargesProps {
                    BarColor = UIColor.Red,
                    Type = GaugeVisualType.BarDiamondCombo,
                    SameColor = true,
                    Parts = new []{
                        new GaugesChargesPartProps {
                            Bar = true,
                            Diamond = true,
                            CD = 30,
                            MaxCharges = 3,
                            Triggers = new[] { new Item(ActionIds.GaussRound) }
                        }
                    }
                }),
                new GaugeCharges("Ricochet Charges", new GaugeChargesProps {
                    BarColor = UIColor.LightBlue,
                    Type = GaugeVisualType.BarDiamondCombo,
                    SameColor = true,
                    Parts = new []{
                        new GaugesChargesPartProps {
                            Bar = true,
                            Diamond = true,
                            CD = 30,
                            MaxCharges = 3,
                            Triggers = new[] { new Item(ActionIds.Ricochet) }
                        }
                    }
                })
            });
            // ============ DNC ==================
            JobToGauges.Add(JobIds.DNC, new Gauge[] {
                new GaugeProc("Dancer Procs", new GaugeProcProps{
                    Procs = new []{
                        new Proc(BuffIds.FlourishingCascade, UIColor.BrightGreen),
                        new Proc(BuffIds.FlourishingFountain, UIColor.Yellow),
                        new Proc(BuffIds.FlourishingWindmill, UIColor.DarkBlue),
                        new Proc(BuffIds.FlourishingShower, UIColor.Red),
                        new Proc(BuffIds.FlourishingFanDance, UIColor.HealthGreen)
                    }
                })
            });
            // ============ NIN ==================
            JobToGauges.Add(JobIds.NIN, new Gauge[] {
                new GaugeGCD("Bunshin", GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 5,
                    MaxDuration = 15,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.Bunshin)
                    }
                }),
                new GaugeCharges("True North (NIN)", new GaugeChargesProps {
                    BarColor = UIColor.White,
                    SameColor = true,
                    Type = GaugeVisualType.BarDiamondCombo,
                    Parts = new []{
                        new GaugesChargesPartProps {
                            Diamond = true,
                            MaxCharges = 2,
                            CD = 45,
                            Triggers = new []{
                                new Item(ActionIds.TrueNorth)
                            }
                        },
                        new GaugesChargesPartProps {
                            Bar = true,
                            Duration = 10,
                            Triggers = new []{
                                new Item(BuffIds.TrueNorth)
                            }
                        }
                    }
                })
            });
            // ============ MNK ==================
            JobToGauges.Add(JobIds.MNK, new Gauge[] {
                new GaugeGCD("Perfect Balance", GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 6,
                    MaxDuration = 15,
                    Color = UIColor.Orange,
                    Triggers = new []{
                        new Item(BuffIds.PerfectBalance)
                    }
                }),
                new GaugeTimer("Twin Snakes", new SubGaugeTimerProps
                {
                    MaxDuration = 15,
                    Color = UIColor.PurplePink,
                    Triggers = new []{
                        new Item(BuffIds.TwinSnakes)
                    }
                }),
                new GaugeTimer("Demolish", new SubGaugeTimerProps
                {
                    MaxDuration = 18,
                    Color = UIColor.Orange,
                    Triggers = new [] {
                        new Item(BuffIds.Demolish)
                    }
                }),
                new GaugeCharges("True North / Riddle of Earth", new GaugeChargesProps {
                    BarColor = UIColor.LightBlue,
                    Type = GaugeVisualType.BarDiamondCombo,
                    Parts = new []{
                        new GaugesChargesPartProps {
                            Diamond = true,
                            MaxCharges = 3,
                            Color = UIColor.Yellow,
                            CD = 30,
                            Triggers = new []{
                                new Item(ActionIds.RiddleOfEarth)
                            }
                        },
                        new GaugesChargesPartProps {
                            Diamond = true,
                            MaxCharges = 2,
                            Color = UIColor.White,
                            CD = 45,
                            Triggers = new []{
                                new Item(ActionIds.TrueNorth)
                            }
                        },
                        new GaugesChargesPartProps {
                            Bar = true,
                            Duration = 10,
                            Triggers = new []{
                                new Item(BuffIds.RiddleOfEarth)
                            }
                        },
                        new GaugesChargesPartProps {
                            Bar = true,
                            Duration = 10,
                            Triggers = new []{
                                new Item(BuffIds.TrueNorth)
                            }
                        }
                    }
                }),
            });
            // ============ BLU ==================
            JobToGauges.Add(JobIds.BLU, new Gauge[] {
                new GaugeProc("Condensed Libra", new GaugeProcProps{
                    Procs = new []{
                        new Proc(BuffIds.AstralAttenuation, UIColor.White),
                        new Proc(BuffIds.UmbralAttenuation, UIColor.DarkBlue),
                        new Proc(BuffIds.PhysicalAttenuation, UIColor.Orange)
                    }
                }),
                new GaugeTimer("Song of Torment/Nightbloom", new SubGaugeTimerProps
                {
                    MaxDuration = 60,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.BluBleed)
                    },
                    Icons = new []{
                        ActionIds.SongOfTorment
                    }
                }),
                new GaugeTimer("Bad Breath", new SubGaugeTimerProps
                {
                    MaxDuration = 15,
                    Color = UIColor.HealthGreen,
                    Triggers = new []{
                        new Item(BuffIds.Poison)
                    },
                    Icons = new []{
                        ActionIds.BadBreath
                    }
                })
            });
        }
    }
}
