using JobBars.Data;
using JobBars.UI;
using System.Collections.Generic;

namespace JobBars.Buffs {
    public unsafe partial class BuffManager {
        private void Init() {
            JobToBuffs = new Dictionary<JobIds, Buff[]>();
            JobToBuffs.Add(JobIds.OTHER, new Buff[] { });
            // ======= GNB ==========
            JobToBuffs.Add(JobIds.GNB, new Buff[] {
            });
            // ======= PLD ==========
            JobToBuffs.Add(JobIds.PLD, new Buff[]  {
            });
            // ======= WAR ==========
            JobToBuffs.Add(JobIds.WAR, new Buff[] {
                new Buff("Inner Release", new BuffProps {
                    CD = 90,
                    Duration = 10,
                    Icon = IconIds.InnerRelease,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.InnerRelease) }
                })
            });
            JobToBuffs.Add(JobIds.DRK, new Buff[] {
                new Buff("Delirium", new BuffProps {
                    CD = 90,
                    Duration = 10,
                    Icon = IconIds.Delirium,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(ActionIds.Delirium) }
                }),
                new Buff("Living Shadow", new BuffProps {
                    CD = 120,
                    Duration = 24,
                    Icon = IconIds.LivingShadow,
                    Color = UIColor.Purple,
                    Triggers = new []{ new Item(ActionIds.LivingShadow) }
                })
            });
            // ======= AST ==========
            JobToBuffs.Add(JobIds.AST, new Buff[] {
                new Buff("The Balance", new BuffProps {
                    Duration = 15,
                    Icon = IconIds.TheBalance,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(BuffIds.TheBalance) }
                }),
                new Buff("The Bole", new BuffProps {
                    Duration = 15,
                    Icon = IconIds.TheBole,
                    Color = UIColor.BrightGreen,
                    Triggers = new []{ new Item(BuffIds.TheBole) }
                }),
                new Buff("The Spear", new BuffProps {
                    Duration = 15,
                    Icon = IconIds.TheSpear,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{ new Item(BuffIds.TheSpear) }
                }),
                new Buff("The Spire", new BuffProps {
                    Duration = 15,
                    Icon = IconIds.TheSpire,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(BuffIds.TheSpire) }
                }),
                new Buff("The Arrow", new BuffProps {
                    Duration = 15,
                    Icon = IconIds.TheArrow,
                    Color = UIColor.LightBlue,
                    Triggers = new []{ new Item(BuffIds.TheArrow) }
                }),
                new Buff("The Ewer", new BuffProps {
                    Duration = 15,
                    Icon = IconIds.TheEwer,
                    Color = UIColor.LightBlue,
                    Triggers = new []{ new Item(BuffIds.TheEwer) }
                }),
                new Buff("Lady of Crowns", new BuffProps {
                    Duration = 15,
                    Icon = IconIds.LadyOfCrowns,
                    Color = UIColor.Purple,
                    Triggers = new []{ new Item(BuffIds.LadyOfCrowns) }
                }),
                new Buff("Lady of Crowns", new BuffProps {
                    Duration = 15,
                    Icon = IconIds.LordOfCrowns,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(BuffIds.LordOfCrowns) }
                }),
                new Buff("Divination", new BuffProps {
                    CD = 120,
                    Duration = 15,
                    Icon = IconIds.Divination,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(ActionIds.Divination) }
                })
            });
            // ======= SCH ==========
            JobToBuffs.Add(JobIds.SCH, new Buff[] {
                new Buff("Chain Stratagem", new BuffProps {
                    CD = 120,
                    Duration = 15,
                    Icon = IconIds.ChainStratagem,
                    Color = UIColor.White,
                    Triggers = new []{ new Item(ActionIds.ChainStratagem) }
                })
            });
            // ======= WHM ==========
            JobToBuffs.Add(JobIds.WHM, new Buff[] {
            });
            // ======= BRD ==========
            JobToBuffs.Add(JobIds.BRD, new Buff[] {
                new Buff("Battle Voice", new BuffProps {
                    CD = 180,
                    Duration = 20,
                    Icon = IconIds.BattleVoice,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.BattleVoice) }
                }),
                new Buff("Barrage", new BuffProps
                {
                    CD = 80,
                    Duration = 5,
                    Icon = IconIds.Barrage,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(ActionIds.Barrage) }
                }),
                new Buff("Raging Strike", new BuffProps {
                    CD = 80,
                    Duration = 20,
                    Icon = IconIds.RagingStrikes,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(ActionIds.RagingStrikes) }
                })
            });
            // ======= DRG ==========
            JobToBuffs.Add(JobIds.DRG, new Buff[] {
                new Buff("Dragon Sight", new BuffProps {
                    CD = 120,
                    Duration = 20,
                    Icon = IconIds.DragonSight,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.DragonSight) }
                }),
                new Buff("Battle Litany", new BuffProps {
                    CD = 180,
                    Duration = 20,
                    Icon = IconIds.BattleLitany,
                    Color = UIColor.LightBlue,
                    Triggers = new []{ new Item(ActionIds.BattleLitany) }
                }),
                new Buff("Lance Charge", new BuffProps {
                    CD = 90,
                    Duration = 20,
                    Icon = IconIds.LanceCharge,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(ActionIds.LanceCharge) }
                })
            });
            // ======= SMN ==========
            JobToBuffs.Add(JobIds.SMN, new Buff[] {
                new Buff("Devotion", new BuffProps {
                    CD = 180,
                    Duration = 15,
                    Icon = IconIds.Devotion,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(ActionIds.Devotion) }
                }),
                new Buff("Summon Bahamut", new BuffProps {
                    CD = 120,
                    Duration = 20,
                    Icon = IconIds.SummonBahamut,
                    Color = UIColor.LightBlue,
                    Triggers = new []{ new Item(ActionIds.SummonBahamut) }
                }),
                new Buff("Firebird Trance", new BuffProps {
                    CD = 120,
                    Duration = 20,
                    Icon = IconIds.FirebirdTrance,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.FirebirdTrance) }
                })
            });
            // ======= SAM ==========
            JobToBuffs.Add(JobIds.SAM, new Buff[] {
                new Buff("Double Midare", new BuffProps {
                    CD = 60,
                    Duration = 5,
                    Icon = IconIds.DoubleMidare,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{ new Item(ActionIds.DoubleMidare) }
                })
            });
            // ======= BLM ==========
            JobToBuffs.Add(JobIds.BLM, new Buff[] {
            });
            // ======= RDM ==========
            JobToBuffs.Add(JobIds.RDM, new Buff[] {
                new Buff("Manafication", new BuffProps {
                    CD = 110,
                    Duration = 10,
                    Icon = IconIds.Manafication,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{ new Item(ActionIds.Manafication) }
                }),
                new Buff("Embolden", new BuffProps {
                    CD = 120,
                    Duration = 20,
                    Icon = IconIds.Embolden,
                    Color = UIColor.White,
                    Triggers = new []{ new Item(ActionIds.Embolden) }
                })
            });
            // ======= MCH ==========
            JobToBuffs.Add(JobIds.MCH, new Buff[] {
                new Buff("Wildfire", new BuffProps {
                    CD = 120,
                    Duration = 10,
                    Icon = IconIds.Wildfire,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.Wildfire) }
                }),
                new Buff("Reassemble", new BuffProps
                {
                    CD = 55,
                    Duration = 5,
                    Icon = IconIds.Reassemble,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{ new Item(ActionIds.Reassemble) }
                }),
            });
            // ======= DNC ==========
            JobToBuffs.Add(JobIds.DNC, new Buff[] {
                new Buff("Technical Finish", new BuffProps {
                    CD = 120,
                    Duration = 20,
                    Icon = IconIds.TechnicalFinish,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.QuadTechFinish) }
                }),
                new Buff("Devilment", new BuffProps {
                    CD = 120,
                    Duration = 20,
                    Icon = IconIds.Devilment,
                    Color = UIColor.BrightGreen,
                    Triggers = new []{ new Item(ActionIds.Devilment) }
                })
            });
            // ======= NIN ==========
            JobToBuffs.Add(JobIds.NIN, new Buff[] {
                new Buff("Trick Attack", new BuffProps {
                    CD = 60,
                    Duration = 15,
                    Icon = IconIds.TrickAttack,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(ActionIds.TrickAttack) }
                }),
                new Buff("Bunshin", new BuffProps {
                    CD = 90,
                    Duration = 15,
                    Icon = IconIds.Bunshin,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.Bunshin) }
                })
            });
            // ======= MNK ==========
            JobToBuffs.Add(JobIds.MNK, new Buff[] {
                new Buff("Brotherhood", new BuffProps {
                    CD = 90,
                    Duration = 15,
                    Icon = IconIds.Brotherhood,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.Brotherhood) }
                }),
                new Buff("Riddle of Fire", new BuffProps {
                    CD = 90,
                    Duration = 20,
                    Icon = IconIds.RiddleOfFire,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(ActionIds.RiddleOfFire) }
                }),
                new Buff("Perfect Balance", new BuffProps {
                    CD = 90,
                    Duration = 15,
                    Icon = IconIds.PerfectBalance,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.PerfectBalance) }
                })
            });
            // ======= BLU ==========
            JobToBuffs.Add(JobIds.BLU, new Buff[] {
                new Buff("Off-Guard", new BuffProps {
                    CD = 60,
                    Duration = 15,
                    Icon = IconIds.OffGuard,
                    Color = UIColor.BrightGreen,
                    Triggers = new []{ new Item(ActionIds.OffGuard) }
                }),
                new Buff("Peculiar Light", new BuffProps {
                    CD = 60,
                    Duration = 15,
                    Icon = IconIds.PeculiarLight,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(ActionIds.PeculiarLight) }
                })
            });
        }
    }
}
