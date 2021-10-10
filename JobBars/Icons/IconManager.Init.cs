using JobBars.Data;
using System;
using JobBars.Helper;

namespace JobBars.Icons {
    public partial class IconManager {
        private void Init() {
            JobToValue.Add(JobIds.OTHER, Array.Empty<IconReplacer>());
            // ============ GNB ==================
            JobToValue.Add(JobIds.GNB, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.NoMercy), new IconProps {
                    Icons = new [] { ActionIds.NoMercy },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.NoMercy), Duration = 20 }
                    }
                })
            });
            // ============ PLD ==================
            JobToValue.Add(JobIds.PLD, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.Requiescat), new IconProps {
                    Icons = new [] { ActionIds.Requiescat },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Requiescat), Duration = 12 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.FightOrFlight), new IconProps {
                    Icons = new [] { ActionIds.FightOrFlight },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.FightOrFlight), Duration = 25 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.GoringBlade), new IconProps {
                    IsTimer = true,
                    Icons = new [] { ActionIds.GoringBlade },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.GoringBlade), Duration = 21 }
                    }
                })
            });
            // ============ WAR ==================
            JobToValue.Add(JobIds.WAR, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.InnerRelease), new IconProps {
                    Icons = new [] { ActionIds.InnerRelease },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.InnerRelease), Duration = 10 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.StormsEye), new IconProps {
                    IsTimer = true,
                    Icons = new [] { ActionIds.StormsEye },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.StormsEye), Duration = 60 }
                    }
                })
            });
            // ============ DRK ==================
            JobToValue.Add(JobIds.DRK, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.Delirium), new IconProps {
                    Icons = new [] { ActionIds.Delirium },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Delirium), Duration = 10 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.BloodWeapon), new IconProps {
                    Icons = new [] { ActionIds.BloodWeapon },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.BloodWeapon), Duration = 10 }
                    }
                })
            });
            // ============ AST ==================
            JobToValue.Add(JobIds.AST, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.Combust3), new IconProps {
                    IsTimer = true,
                    Icons = new [] {
                        ActionIds.Combust1,
                        ActionIds.Combust2,
                        ActionIds.Combust3
                    },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Combust), Duration = 18 },
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Combust2), Duration = 30 },
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Combust3), Duration = 30 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.Lightspeed), new IconProps {
                    Icons = new [] { ActionIds.Lightspeed },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Lightspeed), Duration = 15 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(ActionIds.EarthlyStar), new IconProps {
                    Icons = new [] {
                        ActionIds.EarthlyStar,
                        ActionIds.StellarDetonation
                    },
                    Triggers = new [] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.GiantDominance), Duration = 10 }
                    }
                })
            });
            // ============ SCH ==================
            JobToValue.Add(JobIds.SCH, new[] {
                new IconReplacer(UIHelper.Localize(ActionIds.ChainStratagem), new IconProps {
                    Icons = new [] { ActionIds.ChainStratagem },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.ChainStratagem), Duration = 15 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.Biolysis), new IconProps {
                    IsTimer = true,
                    Icons = new [] {
                        ActionIds.SchBio,
                        ActionIds.SchBio2,
                        ActionIds.Biolysis
                    },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.ArcBio), Duration = 30 },
                        new IconTriggerStruct { Trigger = new Item(BuffIds.ArcBio2), Duration = 30 },
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Biolysis), Duration = 30 }
                    }
                })
            });
            // ============ WHM ==================
            JobToValue.Add(JobIds.WHM, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.Dia), new IconProps {
                    IsTimer = true,
                    Icons = new [] {
                        ActionIds.Aero,
                        ActionIds.Aero2,
                        ActionIds.Dia
                    },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Aero), Duration = 18 },
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Aero2), Duration = 18 },
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Dia), Duration = 30 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.PresenceOfMind), new IconProps {
                    Icons = new [] { ActionIds.PresenceOfMind },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.PresenceOfMind), Duration = 15 }
                    }
                })
            });
            // ============ BRD ==================
            JobToValue.Add(JobIds.BRD, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.RagingStrikes), new IconProps {
                    Icons = new [] { ActionIds.RagingStrikes },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.RagingStrikes), Duration = 20 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.VenomousBite), new IconProps {
                    IsTimer = true,
                    Icons = new [] {
                        ActionIds.CausticBite,
                        ActionIds.VenomousBite,
                    },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.CausticBite), Duration = 30 },
                        new IconTriggerStruct { Trigger = new Item(BuffIds.VenomousBite), Duration = 30 },
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.Stormbite), new IconProps {
                    IsTimer = true,
                    Icons = new [] {
                        ActionIds.Windbite,
                        ActionIds.Stormbite,
                    },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Windbite), Duration = 30 },
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Stormbite), Duration = 30 },
                    }
                })
            });
            // ============ DRG ==================
            JobToValue.Add(JobIds.DRG, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.LanceCharge), new IconProps {
                    Icons = new [] { ActionIds.LanceCharge },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.LanceCharge), Duration = 20 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.RightEye), new IconProps {
                    Icons = new [] { ActionIds.DragonSight },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.RightEye), Duration = 20 },
                        new IconTriggerStruct { Trigger = new Item(BuffIds.RightEye2), Duration = 20 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.Disembowel), new IconProps {
                    IsTimer = true,
                    Icons = new [] { ActionIds.Disembowel },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Disembowel), Duration = 24 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.ChaosThrust), new IconProps {
                    IsTimer = true,
                    Icons = new [] { ActionIds.ChaosThrust },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.ChaosThrust), Duration = 30 }
                    }
                })
            });
            // ============ SMN ==================
            JobToValue.Add(JobIds.SMN, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.Bio3), new IconProps {
                    IsTimer = true,
                    Icons = new [] {
                        ActionIds.ArcBio,
                        ActionIds.ArcBio2,
                        ActionIds.Bio3
                    },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.ArcBio), Duration = 30 },
                        new IconTriggerStruct { Trigger = new Item(BuffIds.ArcBio2), Duration = 30 },
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Bio3), Duration = 30 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.Miasma), new IconProps {
                    IsTimer = true,
                    Icons = new [] {
                        ActionIds.Miasma,
                        ActionIds.Miasma3
                    },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Miasma), Duration = 30 },
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Miasma3), Duration = 30 }
                    }
                })
            });
            // ============ SAM ==================
            JobToValue.Add(JobIds.SAM, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.Jinpu), new IconProps {
                    IsTimer = true,
                    Icons = new [] { ActionIds.Jinpu },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Jinpu), Duration = 40 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.Shifu), new IconProps {
                    IsTimer = true,
                    Icons = new [] { ActionIds.Shifu },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Shifu), Duration = 40 }
                    }
                })
            });
            // ============ BLM ==================
            JobToValue.Add(JobIds.BLM, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.Thunder3), new IconProps {
                    IsTimer = true,
                    Icons = new [] {
                        ActionIds.Thunder,
                        ActionIds.Thunder3
                    },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Thunder), Duration = 24 },
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Thunder3), Duration = 24 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.Thunder4), new IconProps {
                    IsTimer = true,
                    Icons = new [] {
                        ActionIds.Thunder2,
                        ActionIds.Thunder4
                    },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Thunder2), Duration = 18 },
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Thunder4), Duration = 18 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.LeyLines), new IconProps {
                    Icons = new [] { ActionIds.LeyLines },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.LeyLines), Duration = 30 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.Sharpcast), new IconProps {
                    Icons = new [] { ActionIds.Sharpcast },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Sharpcast), Duration = 15 }
                    }
                })
            });
            // ============ RDM ==================
            JobToValue.Add(JobIds.RDM, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.Manafication), new IconProps {
                    Icons = new [] { ActionIds.Manafication },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Manafication), Duration = 10 }
                    }
                })
            });
            // ============ MCH ==================
            JobToValue.Add(JobIds.MCH, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.Wildfire), new IconProps {
                    Icons = new [] { ActionIds.Wildfire },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Wildfire), Duration = 10 }
                    }
                })
            });
            // ============ DNC ==================
            JobToValue.Add(JobIds.DNC, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.Devilment), new IconProps {
                    Icons = new [] { ActionIds.Devilment },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Devilment), Duration = 20 }
                    }
                })
            });
            // ============ NIN ==================
            JobToValue.Add(JobIds.NIN, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.TrickAttack), new IconProps {
                    Icons = new [] { ActionIds.TrickAttack },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.TrickAttack), Duration = 15 }
                    }
                })
            });
            // ============ MNK ==================
            JobToValue.Add(JobIds.MNK, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.RiddleOfFire), new IconProps {
                    Icons = new [] { ActionIds.RiddleOfFire },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.RiddleOfFire), Duration = 20 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.TwinSnakes), new IconProps {
                    IsTimer = true,
                    Icons = new [] { ActionIds.TwinSnakes },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.TwinSnakes), Duration = 15 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.Demolish), new IconProps {
                    IsTimer = true,
                    Icons = new [] { ActionIds.Demolish },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Demolish), Duration = 18 }
                    }
                })
            });
            // ============ BLU ==================
            JobToValue.Add(JobIds.BLU, new[] {
                new IconReplacer(UIHelper.Localize(BuffIds.BluBleed), new IconProps {
                    IsTimer = true,
                    Icons = new [] { ActionIds.SongOfTorment },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.BluBleed), Duration = 60 }
                    }
                }),
                new IconReplacer(UIHelper.Localize(BuffIds.Poison), new IconProps {
                    IsTimer = true,
                    Icons = new [] { ActionIds.BadBreath },
                    Triggers = new[] {
                        new IconTriggerStruct { Trigger = new Item(BuffIds.Poison), Duration = 15 }
                    }
                })
            });
        }
    }
}
