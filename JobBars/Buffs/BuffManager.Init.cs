using System;
using JobBars.Data;
using JobBars.UI;

namespace JobBars.Buffs {
    public struct BuffProps {
        public string Name;
        public float Duration;
        public float CD;

        public ActionIds Icon;
        public ElementColor Color;
        public Item[] Triggers;
        public bool IsPlayerOnly;

        public bool Enabled => JobBars.Config.BuffEnabled.Get(Name);
    }

    public unsafe partial class BuffManager {
        private void Init() {
            JobToValue.Add(JobIds.OTHER, Array.Empty<BuffProps>());
            // ======= GNB ==========
            JobToValue.Add(JobIds.GNB, Array.Empty<BuffProps>());
            // ======= PLD ==========
            JobToValue.Add(JobIds.PLD, Array.Empty<BuffProps>());
            // ======= WAR ==========
            JobToValue.Add(JobIds.WAR, new [] {
                new BuffProps {
                    Name = "Inner Release",
                    Duration = 10,
                    CD = 90,
                    Icon = ActionIds.InnerRelease,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(BuffIds.InnerRelease) }
                }
            });
            // ======= DRK =========
            JobToValue.Add(JobIds.DRK, new [] {
                new BuffProps {
                    Name = "Delirium",
                    CD = 90,
                    Duration = 10,
                    Icon = ActionIds.Delirium,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(BuffIds.Delirium) }
                },
                new BuffProps {
                    Name = "Living Shadow",
                    CD = 120,
                    Duration = 24,
                    Icon = ActionIds.LivingShadow,
                    Color = UIColor.Purple,
                    Triggers = new []{ new Item(ActionIds.LivingShadow) }
                }
            });
            // ======= AST ==========
            JobToValue.Add(JobIds.AST, new [] {
                new BuffProps {
                    Name = "The Balance",
                    Duration = 15,
                    Icon = ActionIds.TheBalance,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(BuffIds.TheBalance) },
                    IsPlayerOnly = true
                },
                new BuffProps {
                    Name = "The Bole",
                    Duration = 15,
                    Icon = ActionIds.TheBole,
                    Color = UIColor.BrightGreen,
                    Triggers = new []{ new Item(BuffIds.TheBole) },
                    IsPlayerOnly = true
                },
                new BuffProps {
                    Name = "The Spear",
                    Duration = 15,
                    Icon = ActionIds.TheSpear,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{ new Item(BuffIds.TheSpear) },
                    IsPlayerOnly = true
                },
                new BuffProps {
                    Name = "The Spire",
                    Duration = 15,
                    Icon = ActionIds.TheSpire,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(BuffIds.TheSpire) },
                    IsPlayerOnly = true
                },
                new BuffProps {
                    Name = "The Arrow",
                    Duration = 15,
                    Icon = ActionIds.TheArrow,
                    Color = UIColor.LightBlue,
                    Triggers = new []{ new Item(BuffIds.TheArrow) },
                    IsPlayerOnly = true
                },
                new BuffProps {
                    Name = "The Ewer",
                    Duration = 15,
                    Icon = ActionIds.TheEwer,
                    Color = UIColor.LightBlue,
                    Triggers = new []{ new Item(BuffIds.TheEwer) },
                    IsPlayerOnly = true
                },
                new BuffProps {
                    Name = "Lady of Crowns",
                    Duration = 15,
                    Icon = ActionIds.LadyOfCrowns,
                    Color = UIColor.Purple,
                    Triggers = new []{ new Item(BuffIds.LadyOfCrowns) },
                    IsPlayerOnly = true
                },
                new BuffProps {
                    Name = "Lord of Crowns",
                    Duration = 15,
                    Icon = ActionIds.LordOfCrowns,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(BuffIds.LordOfCrowns) },
                    IsPlayerOnly = true
                },
                new BuffProps {
                    Name = "Divination",
                    CD = 120,
                    Duration = 15,
                    Icon = ActionIds.Divination,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(ActionIds.Divination) }
                }
            });
            // ======= SCH ==========
            JobToValue.Add(JobIds.SCH, new [] {
                new BuffProps {
                    Name = "Chain Stratagem",
                    CD = 120,
                    Duration = 15,
                    Icon = ActionIds.ChainStratagem,
                    Color = UIColor.White,
                    Triggers = new []{ new Item(ActionIds.ChainStratagem) }
                }
            });
            // ======= WHM ==========
            JobToValue.Add(JobIds.WHM, Array.Empty<BuffProps>());
            // ======= BRD ==========
            JobToValue.Add(JobIds.BRD, new [] {
                new BuffProps {
                    Name = "Battle Voice",
                    CD = 180,
                    Duration = 20,
                    Icon = ActionIds.BattleVoice,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.BattleVoice) }
                },
                new BuffProps {
                    Name = "Barrage",
                    CD = 80,
                    Duration = 10,
                    Icon = ActionIds.Barrage,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(BuffIds.Barrage) }
                },
                new BuffProps {
                    Name = "Raging Strikes",
                    CD = 80,
                    Duration = 20,
                    Icon = ActionIds.RagingStrikes,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(ActionIds.RagingStrikes) }
                }
            });
            // ======= DRG ==========
            JobToValue.Add(JobIds.DRG, new [] {
                new BuffProps {
                    Name = "Dragon Sight",
                    CD = 120,
                    Duration = 20,
                    Icon = ActionIds.DragonSight,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.DragonSight) }
                },
                new BuffProps {
                    Name = "Battle Litany",
                    CD = 180,
                    Duration = 20,
                    Icon = ActionIds.BattleLitany,
                    Color = UIColor.LightBlue,
                    Triggers = new []{ new Item(ActionIds.BattleLitany) }
                },
                new BuffProps {
                    Name = "Lance Charge",
                    CD = 90,
                    Duration = 20,
                    Icon = ActionIds.LanceCharge,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(ActionIds.LanceCharge) }
                }
            });
            // ======= SMN ==========
            JobToValue.Add(JobIds.SMN, new [] {
                new BuffProps {
                    Name = "Devotion",
                    CD = 180,
                    Duration = 15,
                    Icon = ActionIds.Devotion,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(ActionIds.Devotion) }
                },
                new BuffProps {
                    Name = "Summon Bahamut",
                    CD = 120,
                    Duration = 20,
                    Icon = ActionIds.SummonBahamut,
                    Color = UIColor.LightBlue,
                    Triggers = new []{ new Item(ActionIds.SummonBahamut) }
                },
                new BuffProps {
                    Name = "Firebird Trance",
                    CD = 120,
                    Duration = 20,
                    Icon = ActionIds.FirebirdTrance,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.FirebirdTrance) }
                }
            });
            // ======= SAM ==========
            JobToValue.Add(JobIds.SAM, new [] {
                new BuffProps {
                    Name = "Double Midare",
                    CD = 60,
                    Duration = 5,
                    Icon = ActionIds.DoubleMidare,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{ new Item(ActionIds.DoubleMidare) }
                }
            });
            // ======= BLM ==========
            JobToValue.Add(JobIds.BLM, Array.Empty<BuffProps>());
            // ======= RDM ==========
            JobToValue.Add(JobIds.RDM, new [] {
                new BuffProps {
                    Name = "Manafication",
                    CD = 110,
                    Duration = 10,
                    Icon = ActionIds.Manafication,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{ new Item(ActionIds.Manafication) }
                },
                new BuffProps {
                    Name = "Embolden",
                    CD = 120,
                    Duration = 20,
                    Icon = ActionIds.Embolden,
                    Color = UIColor.White,
                    Triggers = new []{ new Item(ActionIds.Embolden) }
                }
            });
            // ======= MCH ==========
            JobToValue.Add(JobIds.MCH, new [] {
                new BuffProps {
                    Name = "Wildfire",
                    CD = 120,
                    Duration = 10,
                    Icon = ActionIds.Wildfire,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.Wildfire) }
                },
                new BuffProps {
                    Name = "Reassemble",
                    CD = 55,
                    Duration = 5,
                    Icon = ActionIds.Reassemble,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{ new Item(BuffIds.Reassemble) }
                }
            });
            // ======= DNC ==========
            JobToValue.Add(JobIds.DNC, new [] {
                new BuffProps {
                    Name = "Technical Finish",
                    CD = 115, // -5 seconds for the dance to actually be cast
                    Duration = 20,
                    Icon = ActionIds.QuadTechFinish,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.QuadTechFinish) }
                },
                new BuffProps {
                    Name = "Devilment",
                    CD = 120,
                    Duration = 20,
                    Icon = ActionIds.Devilment,
                    Color = UIColor.BrightGreen,
                    Triggers = new []{ new Item(ActionIds.Devilment) }
                }
            });
            // ======= NIN ==========
            JobToValue.Add(JobIds.NIN, new [] {
                new BuffProps {
                    Name = "Trick Attack",
                    CD = 60,
                    Duration = 15,
                    Icon = ActionIds.TrickAttack,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(ActionIds.TrickAttack) }
                },
                new BuffProps {
                    Name = "Bunshin",
                    CD = 90,
                    Duration = 30,
                    Icon = ActionIds.Bunshin,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(BuffIds.Bunshin) }
                }
            });
            // ======= MNK ==========
            JobToValue.Add(JobIds.MNK, new [] {
                new BuffProps {
                    Name = "Brotherhood",
                    CD = 90,
                    Duration = 15,
                    Icon = ActionIds.Brotherhood,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.Brotherhood) }
                },
                new BuffProps {
                    Name = "Riddle of Fire",
                    CD = 90,
                    Duration = 20,
                    Icon = ActionIds.RiddleOfFire,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(ActionIds.RiddleOfFire) }
                },
                new BuffProps {
                    Name = "Perfect Balance",
                    CD = 90,
                    Duration = 15,
                    Icon = ActionIds.PerfectBalance,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(BuffIds.PerfectBalance) }
                }
            });
            // ======= BLU ==========
            JobToValue.Add(JobIds.BLU, new [] {
                new BuffProps {
                    Name = "Off-Guard",
                    CD = 60,
                    Duration = 15,
                    Icon = ActionIds.OffGuard,
                    Color = UIColor.BrightGreen,
                    Triggers = new []{ new Item(ActionIds.OffGuard) }
                },
                new BuffProps {
                    Name = "Peculiar Light",
                    CD = 60,
                    Duration = 15,
                    Icon = ActionIds.PeculiarLight,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(ActionIds.PeculiarLight) }
                }
            });
        }
    }
}
