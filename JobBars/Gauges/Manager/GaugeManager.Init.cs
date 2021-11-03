using Dalamud;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;

using JobBars.Gauges.GCD;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Gauges.Charges;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Custom;

namespace JobBars.Gauges.Manager {
    public unsafe partial class GaugeManager {
        private void Init() {
            string procText = JobBars.ClientState.ClientLanguage switch {
                ClientLanguage.Japanese => "Procs",
                ClientLanguage.English => "Procs",
                ClientLanguage.German => "Procs",
                ClientLanguage.French => "Procs",
                _ => "触发"
            };

            JobToValue.Add(JobIds.OTHER, Array.Empty<GaugeConfig>());
            // ============ GNB ==================
            JobToValue.Add(JobIds.GNB, new GaugeConfig[] {
                new GaugeGCDConfig(UIHelper.Localize(BuffIds.NoMercy), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                    MaxCounter = 9,
                    MaxDuration = 20,
                    Color = UIColor.Orange,
                    Triggers = new []{
                        new Item(BuffIds.NoMercy)
                    }
                })
            });
            // ============ PLD ==================
            JobToValue.Add(JobIds.PLD, new GaugeConfig[] {
                new GaugeStacksConfig(UIHelper.Localize(BuffIds.SwordOath), GaugeVisualType.Diamond, new GaugeStacksProps {
                    MaxStacks = 3,
                    Triggers = new []{
                        new Item(BuffIds.SwordOath)
                    },
                    Color = UIColor.BlueGreen
                }),
                new GaugeGCDConfig(UIHelper.Localize(BuffIds.Requiescat), GaugeVisualType.Arrow, new GaugeSubGCDProps {
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
                new GaugeGCDConfig(UIHelper.Localize(BuffIds.FightOrFlight), GaugeVisualType.Bar, new GaugeSubGCDProps {
                    MaxCounter = 11,
                    MaxDuration = 25,
                    Color = UIColor.Red,
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
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.GoringBlade), GaugeVisualType.Bar, new GaugeSubTimerProps {
                    MaxDuration = 21,
                    Color = UIColor.Orange,
                    Triggers = new []{
                        new Item(BuffIds.GoringBlade)
                    }
                })
            });
            // ============ WAR ==================
            JobToValue.Add(JobIds.WAR, new GaugeConfig[] {
                new GaugeGCDConfig(UIHelper.Localize(BuffIds.InnerRelease), GaugeVisualType.Arrow, new GaugeSubGCDProps {
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
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.StormsEye), GaugeVisualType.Bar, new GaugeSubTimerProps {
                    MaxDuration = 60,
                    DefaultDuration = 30,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.StormsEye)
                    }
                })
            });
            // ============ DRK ==================
            JobToValue.Add(JobIds.DRK, new GaugeConfig[] {
                new GaugeDrkMpConfig($"MP ({UIHelper.Localize(JobIds.DRK)})", GaugeVisualType.BarDiamondCombo, new GaugeDrkMpProps {
                    Color = UIColor.Purple,
                    DarkArtsColor = UIColor.LightBlue,
                    Segments = new[] { 0.3f, 0.6f, 0.9f, 1f }
                }),
                new GaugeGCDConfig(UIHelper.Localize(BuffIds.Delirium), GaugeVisualType.Arrow, new GaugeSubGCDProps {
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
                new GaugeGCDConfig(UIHelper.Localize(BuffIds.BloodWeapon), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                    MaxCounter = 5,
                    MaxDuration = 10,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{
                        new Item(BuffIds.BloodWeapon)
                    }
                })
            });
            // ============ AST ==================
            JobToValue.Add(JobIds.AST, new GaugeConfig[] {
                new GaugeProcsConfig($"{UIHelper.Localize(JobIds.AST)} {procText}", GaugeVisualType.Diamond, new GaugeProcProps {
                    Procs = new []{
                        new ProcConfig(UIHelper.Localize(BuffIds.GiantDominance), BuffIds.GiantDominance, UIColor.LightBlue)
                    }
                }),
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.Combust3), GaugeVisualType.Bar, new GaugeTimerProps {
                    SubTimers = new [] {
                        new GaugeSubTimerProps {
                            MaxDuration = 30,
                            Color = UIColor.LightBlue,
                            SubName = UIHelper.Localize(BuffIds.Combust3),
                            Triggers = new []{
                                new Item(BuffIds.Combust2),
                                new Item(BuffIds.Combust3)
                            }
                        },
                        new GaugeSubTimerProps {
                            MaxDuration = 18,
                            Color = UIColor.LightBlue,
                            SubName = UIHelper.Localize(BuffIds.Combust),
                            Triggers = new []{
                                new Item(BuffIds.Combust)
                            }
                        }
                    }
                })
            });
            // ============ SCH ==================
            JobToValue.Add(JobIds.SCH, new GaugeConfig[] {
                new GaugeProcsConfig($"{UIHelper.Localize(JobIds.SCH)} {procText}", GaugeVisualType.Diamond, new GaugeProcProps{
                    Procs = new []{
                        new ProcConfig(UIHelper.Localize(BuffIds.Excog), BuffIds.Excog, UIColor.BrightGreen)
                    },
                    NoSoundOnProc = true
                }),
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.Biolysis), GaugeVisualType.Bar, new GaugeSubTimerProps {
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
            JobToValue.Add(JobIds.WHM, new GaugeConfig[] {
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.Dia), GaugeVisualType.Bar, new GaugeTimerProps {
                    SubTimers = new [] {
                        new GaugeSubTimerProps {
                            MaxDuration = 30,
                            Color = UIColor.LightBlue,
                            SubName = UIHelper.Localize(BuffIds.Dia),
                            Triggers = new []{
                                new Item(BuffIds.Dia)
                            }
                        },
                        new GaugeSubTimerProps {
                            MaxDuration = 18,
                            Color = UIColor.LightBlue,
                            SubName = UIHelper.Localize(BuffIds.Aero2),
                            Triggers = new [] {
                                new Item(BuffIds.Aero),
                                new Item(BuffIds.Aero2)
                            }
                        }
                    }
                })
            });
            // ============ BRD ==================
            JobToValue.Add(JobIds.BRD, new GaugeConfig[] {
                new GaugeProcsConfig($"{UIHelper.Localize(JobIds.BRD)} {procText}", GaugeVisualType.Diamond, new GaugeProcProps {
                    Procs = new []{
                        new ProcConfig(UIHelper.Localize(BuffIds.StraightShotReady), BuffIds.StraightShotReady, UIColor.Yellow),
                        new ProcConfig(UIHelper.Localize(ActionIds.BloodLetter), ActionIds.BloodLetter, UIColor.Red)
                    }
                }),
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.VenomousBite), GaugeVisualType.Bar, new GaugeSubTimerProps {
                    MaxDuration = 30,
                    Color = UIColor.Purple,
                    Triggers = new []{
                        new Item(BuffIds.CausticBite),
                        new Item(BuffIds.VenomousBite)
                    }
                }),
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.Stormbite), GaugeVisualType.Bar, new GaugeSubTimerProps {
                    MaxDuration = 30,
                    Color = UIColor.LightBlue,
                    Triggers = new []{
                        new Item(BuffIds.Windbite),
                        new Item(BuffIds.Stormbite),
                    }
                }),
                new GaugeGCDConfig(UIHelper.Localize(BuffIds.RagingStrikes), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                    MaxCounter = 9,
                    MaxDuration = 20,
                    Color = UIColor.Orange,
                    Triggers = new []{
                        new Item(BuffIds.RagingStrikes)
                    },
                })
            });
            // ============ DRG ==================
            JobToValue.Add(JobIds.DRG, new GaugeConfig[] {
                new GaugeGCDConfig(UIHelper.Localize(BuffIds.LanceCharge), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                    MaxCounter = 8,
                    MaxDuration = 20,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.LanceCharge)
                    },
                }),
                new GaugeGCDConfig(UIHelper.Localize(BuffIds.RightEye), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                    MaxCounter = 8,
                    MaxDuration = 20,
                    Color = UIColor.Orange,
                    Triggers = new []{
                        new Item(BuffIds.RightEye),
                        new Item(BuffIds.RightEye2)
                    }
                }),
                new GaugeChargesConfig($"{UIHelper.Localize(ActionIds.TrueNorth)} ({UIHelper.Localize(JobIds.DRG)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                    BarColor = UIColor.NoColor,
                    SameColor = true,
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
                    },
                    CompletionSound = GaugeCompleteSoundType.Never
                })
            });
            // ============ SMN ==================
            JobToValue.Add(JobIds.SMN, new GaugeConfig[] {
                new GaugeStacksConfig(UIHelper.Localize(BuffIds.Ruin4), GaugeVisualType.Diamond, new GaugeStacksProps {
                    MaxStacks = 4,
                    Triggers = new []{
                        new Item(BuffIds.Ruin4)
                    },
                    Color = UIColor.DarkBlue
                }),
                new GaugeGCDConfig(UIHelper.Localize(ActionIds.SummonBahamut), GaugeVisualType.Arrow, new GaugeGCDProps {
                    SubGCDs = new [] {
                        new GaugeSubGCDProps {
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
                        new GaugeSubGCDProps {
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
                    }
                }),
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.Bio3), GaugeVisualType.Bar, new GaugeSubTimerProps {
                    MaxDuration = 30,
                    Color = UIColor.HealthGreen,
                    Triggers = new []{
                        new Item(BuffIds.ArcBio),
                        new Item(BuffIds.ArcBio2),
                        new Item(BuffIds.Bio3)
                    }
                }),
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.Miasma3), GaugeVisualType.Bar, new GaugeSubTimerProps {
                    MaxDuration = 30,
                    Color = UIColor.BlueGreen,
                    Triggers = new []{
                        new Item(BuffIds.Miasma),
                        new Item(BuffIds.Miasma3)
                    }
                })
            });
            // ============ SAM ==================
            JobToValue.Add(JobIds.SAM, new GaugeConfig[] {
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.Jinpu), GaugeVisualType.Bar, new GaugeSubTimerProps {
                    MaxDuration = 40,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{
                        new Item(BuffIds.Jinpu)
                    }
                }),
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.Shifu), GaugeVisualType.Bar, new GaugeSubTimerProps {
                    MaxDuration = 40,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.Shifu)
                    }
                }),
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.Higanbana), GaugeVisualType.Bar, new GaugeSubTimerProps {
                    MaxDuration = 60,
                    Color = UIColor.Orange,
                    Triggers = new []{
                        new Item(BuffIds.Higanbana)
                    }
                }),
                new GaugeChargesConfig($"{UIHelper.Localize(ActionIds.TrueNorth)} ({UIHelper.Localize(JobIds.SAM)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                    BarColor = UIColor.NoColor,
                    SameColor = true,
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
                    },
                    CompletionSound = GaugeCompleteSoundType.Never
                })
            });
            // ============ BLM ==================
            JobToValue.Add(JobIds.BLM, new GaugeConfig[] {
                new GaugeProcsConfig($"{UIHelper.Localize(JobIds.BLM)} {procText}", GaugeVisualType.Diamond, new GaugeProcProps{
                    ShowText = true,
                    Procs = new []{
                        new ProcConfig(UIHelper.Localize(BuffIds.Firestarter), BuffIds.Firestarter, UIColor.Orange),
                        new ProcConfig(UIHelper.Localize(BuffIds.Thundercloud), BuffIds.Thundercloud, UIColor.LightBlue)
                    }
                }),
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.Thunder3), GaugeVisualType.Bar, new GaugeTimerProps {
                    SubTimers = new[] {
                        new GaugeSubTimerProps {
                            MaxDuration = 24,
                            Color = UIColor.DarkBlue,
                            SubName = UIHelper.Localize(BuffIds.Thunder3),
                            Triggers = new []{
                                new Item(BuffIds.Thunder3),
                                new Item(BuffIds.Thunder)
                            }
                        },
                        new GaugeSubTimerProps {
                            MaxDuration = 18,
                            Color = UIColor.Purple,
                            SubName = UIHelper.Localize(BuffIds.Thunder4),
                            Triggers = new []{
                                new Item(BuffIds.Thunder4),
                                new Item(BuffIds.Thunder2)
                            }
                        }
                    }
                })
            });
            // ============ RDM ==================
            JobToValue.Add(JobIds.RDM, new GaugeConfig[] {
                new GaugeProcsConfig($"{UIHelper.Localize(JobIds.RDM)} {procText}", GaugeVisualType.Diamond, new GaugeProcProps{
                    Procs = new []{
                        new ProcConfig(UIHelper.Localize(BuffIds.VerstoneReady), BuffIds.VerstoneReady, UIColor.NoColor),
                        new ProcConfig(UIHelper.Localize(BuffIds.VerfireReady), BuffIds.VerfireReady, UIColor.Red)
                    }
                }),
                new GaugeGCDConfig(UIHelper.Localize(BuffIds.Manafication), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                    MaxCounter = 5,
                    MaxDuration = 10,
                    Color = UIColor.DarkBlue,
                    Triggers = new[] {
                        new Item(BuffIds.Manafication)
                    }
                }),
                new GaugeStacksConfig(UIHelper.Localize(BuffIds.Acceleration), GaugeVisualType.Diamond, new GaugeStacksProps {
                    MaxStacks = 3,
                    Triggers = new []{
                        new Item(BuffIds.Acceleration)
                    },
                    Color = UIColor.PurplePink
                }),
            });
            // ============ MCH ==================
            JobToValue.Add(JobIds.MCH, new GaugeConfig[] {
                new GaugeGCDConfig(UIHelper.Localize(ActionIds.Hypercharge), GaugeVisualType.Arrow, new GaugeSubGCDProps {
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
                new GaugeGCDConfig(UIHelper.Localize(BuffIds.Wildfire), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                    MaxCounter = 6,
                    MaxDuration = 10,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.Wildfire)
                    }
                }),
                new GaugeChargesConfig(UIHelper.Localize(ActionIds.GaussRound), GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                    BarColor = UIColor.Red,
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
                new GaugeChargesConfig(UIHelper.Localize(ActionIds.Ricochet), GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                    BarColor = UIColor.LightBlue,
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
            JobToValue.Add(JobIds.DNC, new GaugeConfig[] {
                new GaugeProcsConfig($"{UIHelper.Localize(JobIds.DNC)} {procText}", GaugeVisualType.Diamond, new GaugeProcProps{
                    Procs = new []{
                        new ProcConfig(UIHelper.Localize(BuffIds.FlourishingCascade), BuffIds.FlourishingCascade, UIColor.BrightGreen),
                        new ProcConfig(UIHelper.Localize(BuffIds.FlourishingFountain), BuffIds.FlourishingFountain, UIColor.Yellow),
                        new ProcConfig(UIHelper.Localize(BuffIds.FlourishingWindmill), BuffIds.FlourishingWindmill, UIColor.DarkBlue),
                        new ProcConfig(UIHelper.Localize(BuffIds.FlourishingShower), BuffIds.FlourishingShower, UIColor.Red),
                        new ProcConfig(UIHelper.Localize(BuffIds.FlourishingFanDance), BuffIds.FlourishingFanDance, UIColor.HealthGreen)
                    }
                })
            });
            // ============ NIN ==================
            JobToValue.Add(JobIds.NIN, new GaugeConfig[] {
                new GaugeGCDConfig(UIHelper.Localize(BuffIds.Bunshin), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                    MaxCounter = 5,
                    MaxDuration = 15,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.Bunshin)
                    }
                }),
                new GaugeChargesConfig($"{UIHelper.Localize(ActionIds.TrueNorth)} ({UIHelper.Localize(JobIds.NIN)})", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                    BarColor = UIColor.NoColor,
                    SameColor = true,
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
                    },
                    CompletionSound = GaugeCompleteSoundType.Never
                })
            });
            // ============ MNK ==================
            JobToValue.Add(JobIds.MNK, new GaugeConfig[] {
                new GaugeGCDConfig(UIHelper.Localize(BuffIds.PerfectBalance), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                    MaxCounter = 6,
                    MaxDuration = 15,
                    Color = UIColor.Orange,
                    Triggers = new []{
                        new Item(BuffIds.PerfectBalance)
                    }
                }),
                new GaugeGCDConfig(UIHelper.Localize(BuffIds.RiddleOfFire), GaugeVisualType.Bar, new GaugeSubGCDProps {
                    MaxCounter = 11,
                    MaxDuration = 20,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.RiddleOfFire)
                    }
                }),
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.TwinSnakes), GaugeVisualType.Bar, new GaugeSubTimerProps
                {
                    MaxDuration = 15,
                    Color = UIColor.PurplePink,
                    Triggers = new []{
                        new Item(BuffIds.TwinSnakes)
                    }
                }),
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.Demolish), GaugeVisualType.Bar, new GaugeSubTimerProps
                {
                    MaxDuration = 18,
                    Color = UIColor.Yellow,
                    Triggers = new [] {
                        new Item(BuffIds.Demolish)
                    }
                }),
                new GaugeChargesConfig($"{UIHelper.Localize(ActionIds.TrueNorth)}/{UIHelper.Localize(ActionIds.RiddleOfEarth)}", GaugeVisualType.BarDiamondCombo, new GaugeChargesProps {
                    BarColor = UIColor.LightBlue,
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
                    },
                    CompletionSound = GaugeCompleteSoundType.Never
                }),
            });
            // ============ BLU ==================
            JobToValue.Add(JobIds.BLU, new GaugeConfig[] {
                new GaugeProcsConfig($"{UIHelper.Localize(JobIds.BLU)} {procText}", GaugeVisualType.Diamond, new GaugeProcProps{
                    Procs = new []{
                        new ProcConfig(UIHelper.Localize(BuffIds.AstralAttenuation), BuffIds.AstralAttenuation, UIColor.NoColor),
                        new ProcConfig(UIHelper.Localize(BuffIds.UmbralAttenuation), BuffIds.UmbralAttenuation, UIColor.DarkBlue),
                        new ProcConfig(UIHelper.Localize(BuffIds.PhysicalAttenuation), BuffIds.PhysicalAttenuation, UIColor.Orange)
                    }
                }),
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.BluBleed), GaugeVisualType.Bar, new GaugeSubTimerProps
                {
                    MaxDuration = 60,
                    Color = UIColor.Red,
                    Triggers = new []{
                        new Item(BuffIds.BluBleed)
                    }
                }),
                new GaugeTimerConfig(UIHelper.Localize(BuffIds.Poison), GaugeVisualType.Bar, new GaugeSubTimerProps
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
