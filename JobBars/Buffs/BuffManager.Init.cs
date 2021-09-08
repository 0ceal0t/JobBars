using System;
using JobBars.Data;
using JobBars.Helper;
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
                    Name = UIHelper.Localize(BuffIds.InnerRelease),
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
                    Name = UIHelper.Localize(BuffIds.Delirium),
                    CD = 90,
                    Duration = 10,
                    Icon = ActionIds.Delirium,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(BuffIds.Delirium) }
                },
                new BuffProps {
                    Name = UIHelper.Localize(ActionIds.LivingShadow),
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
                    Name = UIHelper.Localize(BuffIds.TheBalance),
                    Duration = 15,
                    Icon = ActionIds.TheBalance,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(BuffIds.TheBalance) },
                    IsPlayerOnly = true
                },
                new BuffProps {
                    Name = UIHelper.Localize(BuffIds.TheBole),
                    Duration = 15,
                    Icon = ActionIds.TheBole,
                    Color = UIColor.BrightGreen,
                    Triggers = new []{ new Item(BuffIds.TheBole) },
                    IsPlayerOnly = true
                },
                new BuffProps {
                    Name = UIHelper.Localize(BuffIds.TheSpear),
                    Duration = 15,
                    Icon = ActionIds.TheSpear,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{ new Item(BuffIds.TheSpear) },
                    IsPlayerOnly = true
                },
                new BuffProps {
                    Name = UIHelper.Localize(BuffIds.TheSpire),
                    Duration = 15,
                    Icon = ActionIds.TheSpire,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(BuffIds.TheSpire) },
                    IsPlayerOnly = true
                },
                new BuffProps {
                    Name = UIHelper.Localize(BuffIds.TheArrow),
                    Duration = 15,
                    Icon = ActionIds.TheArrow,
                    Color = UIColor.LightBlue,
                    Triggers = new []{ new Item(BuffIds.TheArrow) },
                    IsPlayerOnly = true
                },
                new BuffProps {
                    Name = UIHelper.Localize(BuffIds.TheEwer),
                    Duration = 15,
                    Icon = ActionIds.TheEwer,
                    Color = UIColor.LightBlue,
                    Triggers = new []{ new Item(BuffIds.TheEwer) },
                    IsPlayerOnly = true
                },
                new BuffProps {
                    Name = UIHelper.Localize(BuffIds.LadyOfCrowns),
                    Duration = 15,
                    Icon = ActionIds.LadyOfCrowns,
                    Color = UIColor.Purple,
                    Triggers = new []{ new Item(BuffIds.LadyOfCrowns) },
                    IsPlayerOnly = true
                },
                new BuffProps {
                    Name = UIHelper.Localize(BuffIds.LordOfCrowns),
                    Duration = 15,
                    Icon = ActionIds.LordOfCrowns,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(BuffIds.LordOfCrowns) },
                    IsPlayerOnly = true
                },
                new BuffProps {
                    Name = UIHelper.Localize(ActionIds.Divination),
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
                    Name = UIHelper.Localize(ActionIds.ChainStratagem),
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
                    Name = UIHelper.Localize(ActionIds.BattleVoice),
                    CD = 180,
                    Duration = 20,
                    Icon = ActionIds.BattleVoice,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.BattleVoice) }
                },
                new BuffProps {
                    Name = UIHelper.Localize(BuffIds.Barrage),
                    CD = 80,
                    Duration = 10,
                    Icon = ActionIds.Barrage,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(BuffIds.Barrage) }
                },
                new BuffProps {
                    Name = UIHelper.Localize(ActionIds.RagingStrikes),
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
                    Name = UIHelper.Localize(ActionIds.DragonSight),
                    CD = 120,
                    Duration = 20,
                    Icon = ActionIds.DragonSight,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.DragonSight) }
                },
                new BuffProps {
                    Name = UIHelper.Localize(ActionIds.BattleLitany),
                    CD = 180,
                    Duration = 20,
                    Icon = ActionIds.BattleLitany,
                    Color = UIColor.LightBlue,
                    Triggers = new []{ new Item(ActionIds.BattleLitany) }
                },
                new BuffProps {
                    Name = UIHelper.Localize(ActionIds.LanceCharge),
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
                    Name = UIHelper.Localize(ActionIds.Devotion),
                    CD = 180,
                    Duration = 15,
                    Icon = ActionIds.Devotion,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(ActionIds.Devotion) }
                },
                new BuffProps {
                    Name = UIHelper.Localize(ActionIds.SummonBahamut),
                    CD = 120,
                    Duration = 20,
                    Icon = ActionIds.SummonBahamut,
                    Color = UIColor.LightBlue,
                    Triggers = new []{ new Item(ActionIds.SummonBahamut) }
                },
                new BuffProps {
                    Name = UIHelper.Localize(ActionIds.FirebirdTrance),
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
                    Name = UIHelper.Localize(ActionIds.DoubleMidare),
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
                    Name = UIHelper.Localize(ActionIds.Manafication),
                    CD = 110,
                    Duration = 10,
                    Icon = ActionIds.Manafication,
                    Color = UIColor.DarkBlue,
                    Triggers = new []{ new Item(ActionIds.Manafication) }
                },
                new BuffProps {
                    Name = UIHelper.Localize(ActionIds.Embolden),
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
                    Name = UIHelper.Localize(ActionIds.Wildfire),
                    CD = 120,
                    Duration = 10,
                    Icon = ActionIds.Wildfire,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.Wildfire) }
                },
                new BuffProps {
                    Name = UIHelper.Localize(BuffIds.Reassemble),
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
                    Name = UIHelper.Localize(ActionIds.QuadTechFinish),
                    CD = 115, // -5 seconds for the dance to actually be cast
                    Duration = 20,
                    Icon = ActionIds.QuadTechFinish,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.QuadTechFinish) }
                },
                new BuffProps {
                    Name = UIHelper.Localize(ActionIds.Devilment),
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
                    Name = UIHelper.Localize(ActionIds.TrickAttack),
                    CD = 60,
                    Duration = 15,
                    Icon = ActionIds.TrickAttack,
                    Color = UIColor.Yellow,
                    Triggers = new []{ new Item(ActionIds.TrickAttack) }
                },
                new BuffProps {
                    Name = UIHelper.Localize(BuffIds.Bunshin),
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
                    Name = UIHelper.Localize(ActionIds.Brotherhood),
                    CD = 90,
                    Duration = 15,
                    Icon = ActionIds.Brotherhood,
                    Color = UIColor.Orange,
                    Triggers = new []{ new Item(ActionIds.Brotherhood) }
                },
                new BuffProps {
                    Name = UIHelper.Localize(ActionIds.RiddleOfFire),
                    CD = 90,
                    Duration = 20,
                    Icon = ActionIds.RiddleOfFire,
                    Color = UIColor.Red,
                    Triggers = new []{ new Item(ActionIds.RiddleOfFire) }
                },
                new BuffProps {
                    Name = UIHelper.Localize(BuffIds.PerfectBalance),
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
                    Name = UIHelper.Localize(ActionIds.OffGuard),
                    CD = 60,
                    Duration = 15,
                    Icon = ActionIds.OffGuard,
                    Color = UIColor.BrightGreen,
                    Triggers = new []{ new Item(ActionIds.OffGuard) }
                },
                new BuffProps {
                    Name = UIHelper.Localize(ActionIds.PeculiarLight),
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