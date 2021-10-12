using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using Dalamud;

namespace JobBars.Gauges {
    public unsafe partial class GaugeManager {
        private void Init() {
            string procText = JobBars.ClientState.ClientLanguage switch {
                ClientLanguage.Japanese => "Procs",
                ClientLanguage.English => "Procs",
                ClientLanguage.German => "Procs",
                ClientLanguage.French => "Procs",
                _ => "触发"
            };

            JobToValue.Add(JobIds.OTHER, Array.Empty<Gauge>());
            // ============ GNB ==================
            JobToValue.Add(JobIds.GNB, new Gauge[] {
                new GaugeGCD(UIHelper.Localize(BuffIds.NoMercy), GaugeVisualType.Arrow, new SubGaugeGCDProps {
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
                new GaugeStacks(UIHelper.Localize(BuffIds.SwordOath), new GaugeStacksProps {
                    MaxStacks = 3,
                    NoSoundOnFull = true,
                    Triggers = new []{
                        new Item(BuffIds.SwordOath)
                    },
                    Type = GaugeVisualType.Diamond,
                    Color = UIColor.BlueGreen
                }),
                new GaugeGCD(UIHelper.Localize(BuffIds.Requiescat), GaugeVisualType.Arrow, new SubGaugeGCDProps {
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
                new GaugeGCD(UIHelper.Localize(BuffIds.FightOrFlight), GaugeVisualType.Bar, new SubGaugeGCDProps {
                    MaxCounter = 11,
                    MaxDuration = 25,
                    Color = UIColor.Red,
                    NoSoundOnFull = true,
                    Increment = new []{
                        new Item(ActionIds.ShieldLob),
                        new Item(ActionIds.RageOfHalone),
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
                new GaugeTimer(UIHelper.Localize(BuffIds.GoringBlade), new SubGaugeTimerProps {
                    MaxDuration = 21,
                    Color = UIColor.Orange,
                    Triggers = new []{
                        new Item(BuffIds.GoringBlade)
                    }
                })
            });
            // ============ WAR ==================
            JobToValue.Add(JobIds.WAR, new Gauge[] {
                new GaugeGCD(UIHelper.Localize(BuffIds.InnerRelease), GaugeVisualType.Arrow, new SubGaugeGCDProps {
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
                new GaugeTimer(UIHelper.Localize(BuffIds.StormsEye), new SubGaugeTimerProps {
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
                new GaugeResources($"MP ({UIHelper.Localize(JobIds.DRK)})", GaugeResources.DrkMp, new GaugeResourcesProps {
                    Color = UIColor.Purple,
                    Segments = new[] { 0.3f, 0.6f, 0.9f, 1f }
                }),
                new GaugeGCD(UIHelper.Localize(BuffIds.Delirium), GaugeVisualType.Arrow, new SubGaugeGCDProps {
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
                new GaugeGCD(UIHelper.Localize(BuffIds.BloodWeapon), GaugeVisualType.Arrow, new SubGaugeGCDProps {
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
                new GaugeProc($"{UIHelper.Localize(JobIds.AST)} {procText}", new GaugeProcProps{
                    Procs = new []{
                        new Proc(UIHelper.Localize(BuffIds.GiantDominance), BuffIds.GiantDominance, UIColor.LightBlue)
                    }
                }),
                new GaugeTimer(UIHelper.Localize(BuffIds.Combust3), new [] {
                    new SubGaugeTimerProps {
                        MaxDuration = 30,
                        Color = UIColor.LightBlue,
                        SubName = UIHelper.Localize(BuffIds.Combust3),
                        Triggers = new []{
                            new Item(BuffIds.Combust2),
                            new Item(BuffIds.Combust3)
                        }
                    },
                    new SubGaugeTimerProps {
                        MaxDuration = 18,
                        Color = UIColor.LightBlue,
                        SubName = UIHelper.Localize(BuffIds.Combust),
                        Triggers = new []{
                            new Item(BuffIds.Combust)
                        }
                    },
                })
            });
            // ============ SCH ==================
            JobToValue.Add(JobIds.SCH, new Gauge[] {
                new GaugeProc($"{UIHelper.Localize(JobIds.SCH)} {procText}", new GaugeProcProps{
                    NoSoundOnFull = true,
                    Procs = new []{
                        new Proc(UIHelper.Localize(BuffIds.Excog), BuffIds.Excog, UIColor.BrightGreen)
                    }
                }),
                new GaugeTimer(UIHelper.Localize(BuffIds.Biolysis), new SubGaugeTimerProps {
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
                new GaugeTimer(UIHelper.Localize(BuffIds.Dia), new []{
                    new SubGaugeTimerProps {
                        MaxDuration = 30,
                        Color = UIColor.LightBlue,
                        SubName = UIHelper.Localize(BuffIds.Dia),
                        Triggers = new []{
                            new Item(BuffIds.Dia)
                        }
                    },
                    new SubGaugeTimerProps {
                        MaxDuration = 18,
                        Color = UIColor.LightBlue,
                        SubName = UIHelper.Localize(BuffIds.Aero2),
                        Triggers = new [] {
                            new Item(BuffIds.Aero),
                            new Item(BuffIds.Aero2)
                        }
                    }
                })
            });
            // ============ BRD ==================
            JobToValue.Add(JobIds.BRD, new Gauge[] {
                new GaugeProc($"{UIHelper.Localize(JobIds.BRD)} {procText}", new GaugeProcProps {
                    Procs = new []{
                        new Proc(UIHelper.Localize(BuffIds.StraightShotReady), BuffIds.StraightShotReady, UIColor.Yellow),
                        new Proc(UIHelper.Localize(ActionIds.BloodLetter), ActionIds.BloodLetter, UIColor.Red)
                    }
                }),
                new GaugeTimer(UIHelper.Localize(BuffIds.VenomousBite), new SubGaugeTimerProps {
                    MaxDuration = 30,
                    Color = UIColor.Purple,
                    Triggers = new []{
                        new Item(BuffIds.CausticBite),
                        new Item(BuffIds.VenomousBite)
                    }
                }),
                new GaugeTimer(UIHelper.Localize(BuffIds.Stormbite), new SubGaugeTimerProps {
                    MaxDuration = 30,
                    Color = UIColor.LightBlue,
                    Triggers = new []{
                        new Item(BuffIds.Windbite),
                        new Item(BuffIds.Stormbite),
                    }
                }),
                new GaugeGCD(UIHelper.Localize(BuffIds.RagingStrikes), GaugeVisualType.Arrow, new SubGaugeGCDProps {
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
                new GaugeGCD(UIHelper.Localize(BuffIds.LanceCharge), GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 8,
                    MaxDuration = 20,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.LanceCharge)
                    },
                }),
                new GaugeGCD(UIHelper.Localize(BuffIds.RightEye), GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 8,
                    MaxDuration = 20,
                    Color = UIColor.Orange,
                    Triggers = new []{
                        new Item(BuffIds.RightEye),
                        new Item(BuffIds.RightEye2)
                    }
                }),
                new GaugeCharges($"{UIHelper.Localize(ActionIds.TrueNorth)} ({UIHelper.Localize(JobIds.DRG)})", new GaugeChargesProps {
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
                new GaugeStacks(UIHelper.Localize(BuffIds.Ruin4), new GaugeStacksProps {
                    MaxStacks = 4,
                    Triggers = new []{
                        new Item(BuffIds.Ruin4)
                    },
                    Type = GaugeVisualType.Diamond,
                    Color = UIColor.DarkBlue
                }),
                new GaugeGCD(UIHelper.Localize(ActionIds.SummonBahamut), GaugeVisualType.Arrow, new []{
                    new SubGaugeGCDProps {
                        MaxCounter = 8,
                        MaxDuration = 21,
                        Color = UIColor.LightBlue,
                        SubName = UIHelper.Localize(ActionIds.SummonBahamut),
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
                        SubName = UIHelper.Localize(ActionIds.FirebirdTrance),
                        Increment = new []{
                            new Item(ActionIds.ScarletFlame)
                        },
                        Triggers = new []{
                            new Item(ActionIds.FirebirdTrance),
                            new Item(ActionIds.ScarletFlame) // in case this registers first for some reason
                        }
                    }
                }),
                new GaugeTimer(UIHelper.Localize(BuffIds.Bio3), new SubGaugeTimerProps {
                    MaxDuration = 30,
                    Color = UIColor.HealthGreen,
                    Triggers = new []{
                        new Item(BuffIds.ArcBio),
                        new Item(BuffIds.ArcBio2),
                        new Item(BuffIds.Bio3)
                    }
                }),
                new GaugeTimer(UIHelper.Localize(BuffIds.Miasma3), new SubGaugeTimerProps {
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
                new GaugeTimer(UIHelper.Localize(BuffIds.Jinpu), new SubGaugeTimerProps {
                    MaxDuration = 40,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{
                        new Item(BuffIds.Jinpu)
                    }
                }),
                new GaugeTimer(UIHelper.Localize(BuffIds.Shifu), new SubGaugeTimerProps {
                    MaxDuration = 40,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.Shifu)
                    }
                }),
                new GaugeTimer(UIHelper.Localize(BuffIds.Higanbana), new SubGaugeTimerProps {
                    MaxDuration = 60,
                    Color = UIColor.Orange,
                    Triggers = new []{
                        new Item(BuffIds.Higanbana)
                    }
                }),
                new GaugeCharges($"{UIHelper.Localize(ActionIds.TrueNorth)} ({UIHelper.Localize(JobIds.SAM)})", new GaugeChargesProps {
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
                new GaugeProc($"{UIHelper.Localize(JobIds.BLM)} {procText}", new GaugeProcProps{
                    ShowText = true,
                    Procs = new []{
                        new Proc(UIHelper.Localize(BuffIds.Firestarter), BuffIds.Firestarter, UIColor.Orange),
                        new Proc(UIHelper.Localize(BuffIds.Thundercloud), BuffIds.Thundercloud, UIColor.LightBlue)
                    }
                }),
                new GaugeTimer(UIHelper.Localize(BuffIds.Thunder3), new []{
                    new SubGaugeTimerProps
                    {
                        MaxDuration = 24,
                        Color = UIColor.DarkBlue,
                        SubName = UIHelper.Localize(BuffIds.Thunder3),
                        Triggers = new []{
                            new Item(BuffIds.Thunder3),
                            new Item(BuffIds.Thunder)
                        }
                    },
                    new SubGaugeTimerProps
                    {
                        MaxDuration = 18,
                        Color = UIColor.Purple,
                        SubName = UIHelper.Localize(BuffIds.Thunder4),
                        Triggers = new []{
                            new Item(BuffIds.Thunder4),
                            new Item(BuffIds.Thunder2)
                        }
                    }
                })
            });
            // ============ RDM ==================
            JobToValue.Add(JobIds.RDM, new Gauge[] {
                new GaugeProc($"{UIHelper.Localize(JobIds.RDM)} {procText}", new GaugeProcProps{
                    Procs = new []{
                        new Proc(UIHelper.Localize(BuffIds.VerstoneReady), BuffIds.VerstoneReady, UIColor.NoColor),
                        new Proc(UIHelper.Localize(BuffIds.VerfireReady), BuffIds.VerfireReady, UIColor.Red)
                    }
                }),
                new GaugeGCD(UIHelper.Localize(BuffIds.Manafication), GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 5,
                    MaxDuration = 10,
                    Color = UIColor.DarkBlue,
                    Triggers = new[] {
                        new Item(BuffIds.Manafication)
                    }
                }),
                new GaugeStacks(UIHelper.Localize(BuffIds.Acceleration), new GaugeStacksProps {
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
                new GaugeGCD(UIHelper.Localize(ActionIds.Hypercharge), GaugeVisualType.Arrow, new SubGaugeGCDProps {
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
                new GaugeGCD(UIHelper.Localize(BuffIds.Wildfire), GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 6,
                    MaxDuration = 10,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.Wildfire)
                    }
                }),
                new GaugeCharges(UIHelper.Localize(ActionIds.GaussRound), new GaugeChargesProps {
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
                new GaugeCharges(UIHelper.Localize(ActionIds.Ricochet), new GaugeChargesProps {
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
                new GaugeProc($"{UIHelper.Localize(JobIds.DNC)} {procText}", new GaugeProcProps{
                    Procs = new []{
                        new Proc(UIHelper.Localize(BuffIds.FlourishingCascade), BuffIds.FlourishingCascade, UIColor.BrightGreen),
                        new Proc(UIHelper.Localize(BuffIds.FlourishingFountain), BuffIds.FlourishingFountain, UIColor.Yellow),
                        new Proc(UIHelper.Localize(BuffIds.FlourishingWindmill), BuffIds.FlourishingWindmill, UIColor.DarkBlue),
                        new Proc(UIHelper.Localize(BuffIds.FlourishingShower), BuffIds.FlourishingShower, UIColor.Red),
                        new Proc(UIHelper.Localize(BuffIds.FlourishingFanDance), BuffIds.FlourishingFanDance, UIColor.HealthGreen)
                    }
                })
            });
            // ============ NIN ==================
            JobToValue.Add(JobIds.NIN, new Gauge[] {
                new GaugeGCD(UIHelper.Localize(BuffIds.Bunshin), GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 5,
                    MaxDuration = 15,
                    Color = UIColor.Red,
                    NoSoundOnFull = true,
                    Triggers = new []{
                        new Item(BuffIds.Bunshin)
                    }
                }),
                new GaugeCharges($"{UIHelper.Localize(ActionIds.TrueNorth)} ({UIHelper.Localize(JobIds.NIN)})", new GaugeChargesProps {
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
                new GaugeGCD(UIHelper.Localize(BuffIds.PerfectBalance), GaugeVisualType.Arrow, new SubGaugeGCDProps {
                    MaxCounter = 6,
                    MaxDuration = 15,
                    Color = UIColor.Orange,
                    NoSoundOnFull = true,
                    Triggers = new []{
                        new Item(BuffIds.PerfectBalance)
                    }
                }),
                new GaugeGCD(UIHelper.Localize(BuffIds.RiddleOfFire), GaugeVisualType.Bar, new SubGaugeGCDProps {
                    MaxCounter = 11,
                    MaxDuration = 20,
                    Color = UIColor.Red,
                    NoSoundOnFull = true,
                    Triggers = new []{
                        new Item(BuffIds.RiddleOfFire)
                    }
                }),
                new GaugeTimer(UIHelper.Localize(BuffIds.TwinSnakes), new SubGaugeTimerProps
                {
                    MaxDuration = 15,
                    Color = UIColor.PurplePink,
                    Triggers = new []{
                        new Item(BuffIds.TwinSnakes)
                    }
                }),
                new GaugeTimer(UIHelper.Localize(BuffIds.Demolish), new SubGaugeTimerProps
                {
                    MaxDuration = 18,
                    Color = UIColor.Yellow,
                    Triggers = new [] {
                        new Item(BuffIds.Demolish)
                    }
                }),
                new GaugeCharges($"{UIHelper.Localize(ActionIds.TrueNorth)}/{UIHelper.Localize(ActionIds.RiddleOfEarth)}", new GaugeChargesProps {
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
                new GaugeProc($"{UIHelper.Localize(JobIds.BLU)} {procText}", new GaugeProcProps{
                    Procs = new []{
                        new Proc(UIHelper.Localize(BuffIds.AstralAttenuation), BuffIds.AstralAttenuation, UIColor.NoColor),
                        new Proc(UIHelper.Localize(BuffIds.UmbralAttenuation), BuffIds.UmbralAttenuation, UIColor.DarkBlue),
                        new Proc(UIHelper.Localize(BuffIds.PhysicalAttenuation), BuffIds.PhysicalAttenuation, UIColor.Orange)
                    }
                }),
                new GaugeTimer(UIHelper.Localize(BuffIds.BluBleed), new SubGaugeTimerProps
                {
                    MaxDuration = 60,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.BluBleed)
                    }
                }),
                new GaugeTimer(UIHelper.Localize(BuffIds.Poison), new SubGaugeTimerProps
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
