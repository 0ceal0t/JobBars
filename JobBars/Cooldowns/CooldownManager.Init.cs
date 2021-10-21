using JobBars.Data;
using System;
using JobBars.Helper;

namespace JobBars.Cooldowns {
    public struct CooldownProps {
        public string Name;
        public ActionIds Icon;
        public Item[] Triggers;
        public float Duration;
        public float CD;

        public bool Enabled => JobBars.Config.CooldownEnabled.Get(Name);
        public int Order => JobBars.Config.CooldownOrder.Get(Name);
    }

    public partial class CooldownManager {
        private void Init() {
            JobToValue.Add(JobIds.OTHER, Array.Empty<CooldownProps>());
            // ============ GNB ==================
            JobToValue.Add(JobIds.GNB, new[] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Superbolide),
                    Icon = ActionIds.Superbolide,
                    Duration = 8,
                    CD = 360,
                    Triggers = new []{ new Item(ActionIds.Superbolide) }
                },
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Reprisal)} ({UIHelper.Localize(JobIds.GNB)})",
                    Icon = ActionIds.Reprisal,
                    Duration = 10,
                    CD = 60,
                    Triggers = new []{ new Item(ActionIds.Reprisal) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.HeartOfLight),
                    Icon = ActionIds.HeartOfLight,
                    Duration = 15,
                    CD = 90,
                    Triggers = new []{ new Item(ActionIds.HeartOfLight) }
                },
            });
            // ============ PLD ==================
            JobToValue.Add(JobIds.PLD, new[] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.HallowedGround),
                    Icon = ActionIds.HallowedGround,
                    Duration = 10,
                    CD = 420,
                    Triggers = new []{ new Item(ActionIds.HallowedGround) }
                },
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Reprisal)} ({UIHelper.Localize(JobIds.PLD)})",
                    Icon = ActionIds.Reprisal,
                    Duration = 10,
                    CD = 60,
                    Triggers = new []{ new Item(ActionIds.Reprisal) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.DivineVeil),
                    Icon = ActionIds.DivineVeil,
                    Duration = 30,
                    CD = 90,
                    Triggers = new []{ new Item(BuffIds.DivineVeil) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.PassageOfArms),
                    Icon = ActionIds.PassageOfArms,
                    Duration = 18,
                    CD = 120,
                    Triggers = new []{ new Item(BuffIds.PassageOfArms) }
                }
            });
            // ============ WAR ==================
            JobToValue.Add(JobIds.WAR, new[] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Holmgang),
                    Icon = ActionIds.Holmgang,
                    Duration = 8,
                    CD = 240,
                    Triggers = new []{ new Item(ActionIds.Holmgang) }
                },
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Reprisal)} ({UIHelper.Localize(JobIds.WAR)})",
                    Icon = ActionIds.Reprisal,
                    Duration = 10,
                    CD = 60,
                    Triggers = new []{ new Item(ActionIds.Reprisal) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.ShakeItOff),
                    Icon = ActionIds.ShakeItOff,
                    Duration = 15,
                    CD = 90,
                    Triggers = new []{ new Item(ActionIds.ShakeItOff) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.NascentFlash),
                    Icon = ActionIds.NascentFlash,
                    Duration = 6,
                    CD = 25,
                    Triggers = new []{
                        new Item(ActionIds.NascentFlash),
                        new Item(ActionIds.RawIntuition)
                    }
                }
            });
            // ============ DRK ==================
            JobToValue.Add(JobIds.DRK, new[]{
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.LivingDead),
                    Icon = ActionIds.LivingDead,
                    Duration = 10,
                    CD = 300,
                    Triggers = new []{ new Item(BuffIds.LivingDead) }
                },
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Reprisal)} ({UIHelper.Localize(JobIds.DRK)})",
                    Icon = ActionIds.Reprisal,
                    Duration = 10,
                    CD = 60,
                    Triggers = new []{ new Item(ActionIds.Reprisal) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.DarkMissionary),
                    Icon = ActionIds.DarkMissionary,
                    Duration = 15,
                    CD = 90,
                    Triggers = new []{ new Item(ActionIds.DarkMissionary) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.TheBlackestNight),
                    Icon = ActionIds.TheBlackestNight,
                    Duration = 7,
                    CD = 15,
                    Triggers = new []{ new Item(ActionIds.TheBlackestNight) }
                }
            });
            // ============ AST ==================
            JobToValue.Add(JobIds.AST, new[] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.NeutralSect),
                    Icon = ActionIds.NeutralSect,
                    Duration = 20,
                    CD = 120,
                    Triggers = new []{ new Item(ActionIds.NeutralSect) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.CelestialOpposition),
                    Icon = ActionIds.CelestialOpposition,
                    CD = 60,
                    Triggers = new []{ new Item(ActionIds.CelestialOpposition) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.EarthlyStar),
                    Icon = ActionIds.EarthlyStar,
                    Duration = 20,
                    CD = 60,
                    Triggers = new []{ new Item(ActionIds.EarthlyStar) }
                },
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Swiftcast)} ({UIHelper.Localize(JobIds.AST)})",
                    Icon = ActionIds.Swiftcast,
                    CD = 60,
                    Triggers = new []{ new Item(ActionIds.Swiftcast) }
                }
            });
            // ============ SCH ==================
            JobToValue.Add(JobIds.SCH, new[] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.SummonSeraph),
                    Icon = ActionIds.SummonSeraph,
                    Duration = 22,
                    CD = 120,
                    Triggers = new []{ new Item(ActionIds.SummonSeraph) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.DeploymentTactics),
                    Icon = ActionIds.DeploymentTactics,
                    CD = 120,
                    Triggers = new []{ new Item(ActionIds.DeploymentTactics) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Recitation),
                    Icon = ActionIds.Recitation,
                    CD = 90,
                    Triggers = new []{ new Item(ActionIds.Recitation) }
                },
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Swiftcast)} ({UIHelper.Localize(JobIds.SCH)})",
                    Icon = ActionIds.Swiftcast,
                    CD = 60,
                    Triggers = new []{ new Item(ActionIds.Swiftcast) }
                }
            });
            // ============ WHM ==================
            JobToValue.Add(JobIds.WHM, new[] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Temperance),
                    Icon = ActionIds.Temperance,
                    Duration = 20,
                    CD = 120,
                    Triggers = new []{ new Item(ActionIds.Temperance) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Benediction),
                    Icon = ActionIds.Benediction,
                    CD = 180,
                    Triggers = new []{ new Item(ActionIds.Benediction) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Asylum),
                    Icon = ActionIds.Asylum,
                    Duration = 24,
                    CD = 90,
                    Triggers = new []{ new Item(ActionIds.Asylum) }
                },
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Swiftcast)} ({UIHelper.Localize(JobIds.WHM)})",
                    Icon = ActionIds.Swiftcast,
                    CD = 60,
                    Triggers = new []{ new Item(ActionIds.Swiftcast) }
                }
            });
            // ============ BRD ==================
            JobToValue.Add(JobIds.BRD, new[] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Troubadour),
                    Icon = ActionIds.Troubadour,
                    Duration = 15,
                    CD = 120,
                    Triggers = new []{ new Item(ActionIds.Troubadour) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.NaturesMinne),
                    Icon = ActionIds.NaturesMinne,
                    Duration = 15,
                    CD = 90,
                    Triggers = new []{ new Item(ActionIds.NaturesMinne) }
                }
            });
            // ============ DRG ==================
            JobToValue.Add(JobIds.DRG, new[] {
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.DRG)})",
                    Icon = ActionIds.Feint,
                    Duration = 10,
                    CD = 90,
                    Triggers = new []{ new Item(ActionIds.Feint) }
                }
            });
            // ============ SMN ==================
            JobToValue.Add(JobIds.SMN, new[] {
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Addle)} ({UIHelper.Localize(JobIds.SMN)})",
                    Icon = ActionIds.Addle,
                    Duration = 10,
                    CD = 90,
                    Triggers = new []{ new Item(ActionIds.Addle) }
                },
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Swiftcast)} ({UIHelper.Localize(JobIds.SMN)})",
                    Icon = ActionIds.Swiftcast,
                    CD = 60,
                    Triggers = new []{ new Item(ActionIds.Swiftcast) }
                }
            });
            // ============ SAM ==================
            JobToValue.Add(JobIds.SAM, new[] {
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.SAM)})",
                    Icon = ActionIds.Feint,
                    Duration = 10,
                    CD = 90,
                    Triggers = new []{ new Item(ActionIds.Feint) }
                }
            });
            // ============ BLM ==================
            JobToValue.Add(JobIds.BLM, new[] {
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Addle)} ({UIHelper.Localize(JobIds.BLM)})",
                    Icon = ActionIds.Addle,
                    Duration = 10,
                    CD = 90,
                    Triggers = new []{ new Item(ActionIds.Addle) }
                }
            });
            // ============ RDM ==================
            JobToValue.Add(JobIds.RDM, new[] {
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Addle)} ({UIHelper.Localize(JobIds.RDM)})",
                    Icon = ActionIds.Addle,
                    Duration = 10,
                    CD = 90,
                    Triggers = new []{ new Item(ActionIds.Addle) }
                }
            });
            // ============ MCH ==================
            JobToValue.Add(JobIds.MCH, new[] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Tactician),
                    Icon = ActionIds.Tactician,
                    Duration = 15,
                    CD = 120,
                    Triggers = new []{ new Item(ActionIds.Tactician) }
                }
            });
            // ============ DNC ==================
            JobToValue.Add(JobIds.DNC, new[] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.ShieldSamba),
                    Icon = ActionIds.ShieldSamba,
                    Duration = 15,
                    CD = 120,
                    Triggers = new []{ new Item(ActionIds.ShieldSamba) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Improvisation),
                    Icon = ActionIds.Improvisation,
                    Duration = 15,
                    CD = 120,
                    Triggers = new []{ new Item(BuffIds.Improvisation) }
                }
            });
            // ============ NIN ==================
            JobToValue.Add(JobIds.NIN, new[] {
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.NIN)})",
                    Icon = ActionIds.Feint,
                    Duration = 10,
                    CD = 90,
                    Triggers = new []{ new Item(ActionIds.Feint) }
                }
            });
            // ============ MNK ==================
            JobToValue.Add(JobIds.MNK, new[] {
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.MNK)})",
                    Icon = ActionIds.Feint,
                    Duration = 10,
                    CD = 90,
                    Triggers = new []{ new Item(ActionIds.Feint) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Mantra),
                    Icon = ActionIds.Mantra,
                    Duration = 15,
                    CD = 90,
                    Triggers = new []{ new Item(ActionIds.Mantra) }
                }
            });
            // ============ BLU ==================
            JobToValue.Add(JobIds.BLU, new CooldownProps[] {
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Addle)} ({UIHelper.Localize(JobIds.BLU)})",
                    Icon = ActionIds.Addle,
                    Duration = 10,
                    CD = 90,
                    Triggers = new []{ new Item(ActionIds.Addle) }
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.AngelWhisper),
                    Icon = ActionIds.AngelWhisper,
                    CD = 300,
                    Triggers = new []{ new Item(ActionIds.AngelWhisper) }
                },
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Swiftcast)} ({UIHelper.Localize(JobIds.BLU)})",
                    Icon = ActionIds.Swiftcast,
                    CD = 60,
                    Triggers = new []{ new Item(ActionIds.Swiftcast) }
                }
            });
        }
    }
}