using System;
using JobBars.Data;
using JobBars.UI;

namespace JobBars.Buffs {
    public unsafe partial class BuffManager {
        private void Init() {
            JobToValue.Add(JobIds.OTHER, Array.Empty<Buff>());
            // ======= GNB ==========
            JobToValue.Add(JobIds.GNB, Array.Empty<Buff>());
            // ======= PLD ==========
            JobToValue.Add(JobIds.PLD, Array.Empty<Buff>());
            // ======= WAR ==========
            JobToValue.Add(JobIds.WAR, new Buff[] {
                new Buff("Inner Release", new BuffProps {
                    CD = 90,
                    Duration = 10,
                    Icon = ActionIds.InnerRelease,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.InnerRelease) }
                })
            });
            // ======= DRK =========
            JobToValue.Add(JobIds.DRK, new Buff[] {
                new Buff("Delirium", new BuffProps {
                    CD = 90,
                    Duration = 10,
                    Icon = ActionIds.Delirium,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(ActionIds.Delirium) }
                }),
                new Buff("Living Shadow", new BuffProps {
                    CD = 120,
                    Duration = 24,
                    Icon = ActionIds.LivingShadow,
                    Color = UIColor.Purple,
                    Triggers = new []{ new Item(ActionIds.LivingShadow) }
                })
            });
            // ======= AST ==========
            JobToValue.Add(JobIds.AST, new Buff[] {
                new Buff("The Balance", new BuffProps {
                    Duration = 15,
                    Icon = ActionIds.TheBalance,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(BuffIds.TheBalance) }
                }),
                new Buff("The Bole", new BuffProps {
                    Duration = 15,
                    Icon = ActionIds.TheBole,
                    Color = UIColor.BrightGreen,
                    Triggers = new []{ new Item(BuffIds.TheBole) }
                }),
                new Buff("The Spear", new BuffProps {
                    Duration = 15,
                    Icon = ActionIds.TheSpear,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{ new Item(BuffIds.TheSpear) }
                }),
                new Buff("The Spire", new BuffProps {
                    Duration = 15,
                    Icon = ActionIds.TheSpire,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(BuffIds.TheSpire) }
                }),
                new Buff("The Arrow", new BuffProps {
                    Duration = 15,
                    Icon = ActionIds.TheArrow,
                    Color = UIColor.LightBlue,
                    Triggers = new []{ new Item(BuffIds.TheArrow) }
                }),
                new Buff("The Ewer", new BuffProps {
                    Duration = 15,
                    Icon = ActionIds.TheEwer,
                    Color = UIColor.LightBlue,
                    Triggers = new []{ new Item(BuffIds.TheEwer) }
                }),
                new Buff("Lady of Crowns", new BuffProps {
                    Duration = 15,
                    Icon = ActionIds.LadyOfCrowns,
                    Color = UIColor.Purple,
                    Triggers = new []{ new Item(BuffIds.LadyOfCrowns) }
                }),
                new Buff("Lord of Crowns", new BuffProps {
                    Duration = 15,
                    Icon = ActionIds.LordOfCrowns,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(BuffIds.LordOfCrowns) }
                }),
                new Buff("Divination", new BuffProps {
                    CD = 120,
                    Duration = 15,
                    Icon = ActionIds.Divination,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(ActionIds.Divination) }
                })
            });
            // ======= SCH ==========
            JobToValue.Add(JobIds.SCH, new Buff[] {
                new Buff("Chain Stratagem", new BuffProps {
                    CD = 120,
                    Duration = 15,
                    Icon = ActionIds.ChainStratagem,
                    Color = UIColor.White,
                    Triggers = new []{ new Item(ActionIds.ChainStratagem) }
                })
            });
            // ======= WHM ==========
            JobToValue.Add(JobIds.WHM, Array.Empty<Buff>());
            // ======= BRD ==========
            JobToValue.Add(JobIds.BRD, new Buff[] {
                new Buff("Battle Voice", new BuffProps {
                    CD = 180,
                    Duration = 20,
                    Icon = ActionIds.BattleVoice,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.BattleVoice) }
                }),
                new Buff("Barrage", new BuffProps
                {
                    CD = 80,
                    Duration = 5,
                    Icon = ActionIds.Barrage,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(ActionIds.Barrage) }
                }),
                new Buff("Raging Strike", new BuffProps {
                    CD = 80,
                    Duration = 20,
                    Icon = ActionIds.RagingStrikes,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(ActionIds.RagingStrikes) }
                })
            });
            // ======= DRG ==========
            JobToValue.Add(JobIds.DRG, new Buff[] {
                new Buff("Dragon Sight", new BuffProps {
                    CD = 120,
                    Duration = 20,
                    Icon = ActionIds.DragonSight,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.DragonSight) }
                }),
                new Buff("Battle Litany", new BuffProps {
                    CD = 180,
                    Duration = 20,
                    Icon = ActionIds.BattleLitany,
                    Color = UIColor.LightBlue,
                    Triggers = new []{ new Item(ActionIds.BattleLitany) }
                }),
                new Buff("Lance Charge", new BuffProps {
                    CD = 90,
                    Duration = 20,
                    Icon = ActionIds.LanceCharge,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(ActionIds.LanceCharge) }
                })
            });
            // ======= SMN ==========
            JobToValue.Add(JobIds.SMN, new Buff[] {
                new Buff("Devotion", new BuffProps {
                    CD = 180,
                    Duration = 15,
                    Icon = ActionIds.Devotion,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(ActionIds.Devotion) }
                }),
                new Buff("Summon Bahamut", new BuffProps {
                    CD = 120,
                    Duration = 20,
                    Icon = ActionIds.SummonBahamut,
                    Color = UIColor.LightBlue,
                    Triggers = new []{ new Item(ActionIds.SummonBahamut) }
                }),
                new Buff("Firebird Trance", new BuffProps {
                    CD = 120,
                    Duration = 20,
                    Icon = ActionIds.FirebirdTrance,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.FirebirdTrance) }
                })
            });
            // ======= SAM ==========
            JobToValue.Add(JobIds.SAM, new Buff[] {
                new Buff("Double Midare", new BuffProps {
                    CD = 60,
                    Duration = 5,
                    Icon = ActionIds.DoubleMidare,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{ new Item(ActionIds.DoubleMidare) }
                })
            });
            // ======= BLM ==========
            JobToValue.Add(JobIds.BLM, Array.Empty<Buff>());
            // ======= RDM ==========
            JobToValue.Add(JobIds.RDM, new Buff[] {
                new Buff("Manafication", new BuffProps {
                    CD = 110,
                    Duration = 10,
                    Icon = ActionIds.Manafication,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{ new Item(ActionIds.Manafication) }
                }),
                new Buff("Embolden", new BuffProps {
                    CD = 120,
                    Duration = 20,
                    Icon = ActionIds.Embolden,
                    Color = UIColor.White,
                    Triggers = new []{ new Item(ActionIds.Embolden) }
                })
            });
            // ======= MCH ==========
            JobToValue.Add(JobIds.MCH, new Buff[] {
                new Buff("Wildfire", new BuffProps {
                    CD = 120,
                    Duration = 10,
                    Icon = ActionIds.Wildfire,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.Wildfire) }
                }),
                new Buff("Reassemble", new BuffProps
                {
                    CD = 55,
                    Duration = 5,
                    Icon = ActionIds.Reassemble,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{ new Item(ActionIds.Reassemble) }
                }),
            });
            // ======= DNC ==========
            JobToValue.Add(JobIds.DNC, new Buff[] {
                new Buff("Technical Finish", new BuffProps {
                    CD = 115, // -5 seconds for the dance to actually be cast
                    Duration = 20,
                    Icon = ActionIds.QuadTechFinish,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.QuadTechFinish) }
                }),
                new Buff("Devilment", new BuffProps {
                    CD = 120,
                    Duration = 20,
                    Icon = ActionIds.Devilment,
                    Color = UIColor.BrightGreen,
                    Triggers = new []{ new Item(ActionIds.Devilment) }
                })
            });
            // ======= NIN ==========
            JobToValue.Add(JobIds.NIN, new Buff[] {
                new Buff("Trick Attack", new BuffProps {
                    CD = 60,
                    Duration = 15,
                    Icon = ActionIds.TrickAttack,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(ActionIds.TrickAttack) }
                }),
                new Buff("Bunshin", new BuffProps {
                    CD = 90,
                    Duration = 30,
                    Icon = ActionIds.Bunshin,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.Bunshin) }
                })
            });
            // ======= MNK ==========
            JobToValue.Add(JobIds.MNK, new Buff[] {
                new Buff("Brotherhood", new BuffProps {
                    CD = 90,
                    Duration = 15,
                    Icon = ActionIds.Brotherhood,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.Brotherhood) }
                }),
                new Buff("Riddle of Fire", new BuffProps {
                    CD = 90,
                    Duration = 20,
                    Icon = ActionIds.RiddleOfFire,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(ActionIds.RiddleOfFire) }
                }),
                new Buff("Perfect Balance", new BuffProps {
                    CD = 90,
                    Duration = 15,
                    Icon = ActionIds.PerfectBalance,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.PerfectBalance) }
                })
            });
            // ======= BLU ==========
            JobToValue.Add(JobIds.BLU, new Buff[] {
                new Buff("Off-Guard", new BuffProps {
                    CD = 60,
                    Duration = 15,
                    Icon = ActionIds.OffGuard,
                    Color = UIColor.BrightGreen,
                    Triggers = new []{ new Item(ActionIds.OffGuard) }
                }),
                new Buff("Peculiar Light", new BuffProps {
                    CD = 60,
                    Duration = 15,
                    Icon = ActionIds.PeculiarLight,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(ActionIds.PeculiarLight) }
                })
            });
        }
    }
}
