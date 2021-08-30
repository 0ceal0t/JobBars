using JobBars.Data;
using JobBars.UI;
using System;

namespace JobBars.Gauges {
    public unsafe partial class GaugeManager {
        private void Init() {
            JobToValue.Add(JobIds.OTHER, Array.Empty<Gauge>());
            // ============ GNB ==================
            JobToValue.Add(JobIds.GNB, new Gauge[] {
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
            JobToValue.Add(JobIds.PLD, new Gauge[] {
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
                    NoSoundOnFull = true,
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
                    }
                })
            });
            // ============ WAR ==================
            JobToValue.Add(JobIds.WAR, new Gauge[] {
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
            JobToValue.Add(JobIds.DRK, new Gauge[] {
                new GaugeResources("MP (DRK)", GaugeResources.DrkMp, new GaugeResourcesProps {
                    Color = UIColor.Purple,
                    Segments = new[] { 0.3f, 0.6f, 0.9f, 1f}
                }),
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
            JobToValue.Add(JobIds.AST, new Gauge[] {
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
                        }
                    },
                    new SubGaugeTimerProps {
                        MaxDuration = 18,
                        Color = UIColor.LightBlue,
                        SubName = "Combust 1",
                        Triggers = new []{
                            new Item(BuffIds.Combust)
                        }
                    },
                })
            });
            // ============ SCH ==================
            JobToValue.Add(JobIds.SCH, new Gauge[] {
                new GaugeProc("Excog", new GaugeProcProps{
                    NoSoundOnFull = true,
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
                    }
                })
            });
            // ============ WHM ==================
            JobToValue.Add(JobIds.WHM, new Gauge[] {
                new GaugeTimer("Dia", new []{
                    new SubGaugeTimerProps {
                        MaxDuration = 30,
                        Color = UIColor.LightBlue,
                        SubName = "Dia",
                        Triggers = new []{
                            new Item(BuffIds.Dia)
                        }
                    },
                    new SubGaugeTimerProps {
                        MaxDuration = 18,
                        Color = UIColor.LightBlue,
                        SubName = "Aero 1+2",
                        Triggers = new [] {
                            new Item(BuffIds.Aero),
                            new Item(BuffIds.Aero2)
                        }
                    }
                })
            });
            // ============ BRD ==================
            JobToValue.Add(JobIds.BRD, new Gauge[] {
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
                    }
                }),
                new GaugeTimer("Stormbite", new SubGaugeTimerProps {
                    MaxDuration = 30,
                    Color = UIColor.LightBlue,
                    Triggers = new []{
                        new Item(BuffIds.Windbite),
                        new Item(BuffIds.Stormbite),
                    }
                }),
                new GaugeGCD("Raging Strikes", GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 9,
                    MaxDuration = 20,
                    Color = UIColor.Orange,
                    Triggers = new []{
                        new Item(BuffIds.RagingStrikes)
                    },
                })
            });
            // ============ DRG ==================
            JobToValue.Add(JobIds.DRG, new Gauge[] {
                new GaugeGCD("Lance Charge", GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 8,
                    MaxDuration = 20,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.LanceCharge)
                    },
                }),
                new GaugeGCD("Dragon Sight", GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 8,
                    MaxDuration = 20,
                    Color = UIColor.Orange,
                    Triggers = new []{
                        new Item(BuffIds.RightEye),
                        new Item(BuffIds.RightEye2)
                    }
                }),
                new GaugeCharges("True North (DRG)", new GaugeChargesProps {
                    BarColor = UIColor.NoColor,
                    SameColor = true,
                    Type = GaugeVisualType.BarDiamondCombo,
                    NoSoundOnFull = true,
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
            JobToValue.Add(JobIds.SMN, new Gauge[] {
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
                    }
                }),
                new GaugeTimer("Miasma", new SubGaugeTimerProps {
                    MaxDuration = 30,
                    Color = UIColor.BlueGreen,
                    Triggers = new []{
                        new Item(BuffIds.Miasma),
                        new Item(BuffIds.Miasma3)
                    }
                })
            });
            // ============ SAM ==================
            JobToValue.Add(JobIds.SAM, new Gauge[] {
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
                    BarColor = UIColor.NoColor,
                    SameColor = true,
                    Type = GaugeVisualType.BarDiamondCombo,
                    NoSoundOnFull = true,
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
            JobToValue.Add(JobIds.BLM, new Gauge[] {
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
                        }
                    }
                })
            });
            // ============ RDM ==================
            JobToValue.Add(JobIds.RDM, new Gauge[] {
                new GaugeProc("Verfire/Verstone", new GaugeProcProps{
                    Procs = new []{
                        new Proc(BuffIds.VerstoneReady, UIColor.NoColor),
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
                    NoSoundOnFull = true,
                    Triggers = new []{
                        new Item(BuffIds.Acceleration)
                    },
                    Type = GaugeVisualType.Diamond,
                    Color = UIColor.PurplePink
                }),
            });
            // ============ MCH ==================
            JobToValue.Add(JobIds.MCH, new Gauge[] {
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
            JobToValue.Add(JobIds.DNC, new Gauge[] {
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
            JobToValue.Add(JobIds.NIN, new Gauge[] {
                new GaugeGCD("Bunshin", GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 5,
                    MaxDuration = 15,
                    Color = UIColor.Red,
                    NoSoundOnFull = true,
                    Triggers = new []{
                        new Item(BuffIds.Bunshin)
                    }
                }),
                new GaugeCharges("True North (NIN)", new GaugeChargesProps {
                    BarColor = UIColor.NoColor,
                    SameColor = true,
                    Type = GaugeVisualType.BarDiamondCombo,
                    NoSoundOnFull = true,
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
            JobToValue.Add(JobIds.MNK, new Gauge[] {
                new GaugeGCD("Perfect Balance", GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 6,
                    MaxDuration = 15,
                    Color = UIColor.Orange,
                    NoSoundOnFull = true,
                    Triggers = new []{
                        new Item(BuffIds.PerfectBalance)
                    }
                }),
                new GaugeGCD("Riddle of Fire", GaugeVisualType.Bar, new SubGaugeGCDProps {
                    MaxCounter = 11,
                    MaxDuration = 20,
                    Color = UIColor.Red,
                    NoSoundOnFull = true,
                    Triggers = new []{
                        new Item(BuffIds.RiddleOfFire)
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
                    Color = UIColor.Yellow,
                    Triggers = new [] {
                        new Item(BuffIds.Demolish)
                    }
                }),
                new GaugeCharges("True North / Riddle of Earth", new GaugeChargesProps {
                    BarColor = UIColor.LightBlue,
                    Type = GaugeVisualType.BarDiamondCombo,
                    NoSoundOnFull = true,
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
                            Color = UIColor.NoColor,
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
            JobToValue.Add(JobIds.BLU, new Gauge[] {
                new GaugeProc("Condensed Libra", new GaugeProcProps{
                    Procs = new []{
                        new Proc(BuffIds.AstralAttenuation, UIColor.NoColor),
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
                    }
                }),
                new GaugeTimer("Bad Breath", new SubGaugeTimerProps
                {
                    MaxDuration = 15,
                    Color = UIColor.HealthGreen,
                    Triggers = new []{
                        new Item(BuffIds.Poison)
                    }
                })
            });
        }
    }
}
