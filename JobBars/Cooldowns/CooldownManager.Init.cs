using JobBars.Data;
using System;
using JobBars.Helper;

namespace JobBars.Cooldowns {
    public struct CooldownProps {
        public string Name;
        public ActionIds Trigger;
        public ActionIds[] AdditionalTriggers;
        public float Duration;
        public float CD;

        public bool Enabled => JobBars.Config.CooldownEnabled.Get(Name);
        public int Order => JobBars.Config.CooldownOrder.Get(Name);
    }

    public partial class CooldownManager {
        private void Init() {
            JobToValue.Add(JobIds.OTHER, Array.Empty<CooldownProps>());
            // ============ GNB ==================
            JobToValue.Add(JobIds.GNB, new [] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Superbolide),
                    Trigger = ActionIds.Superbolide,
                    Duration = 8,
                    CD = 360
                },
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Reprisal)} ({UIHelper.Localize(JobIds.GNB)})",
                    Trigger = ActionIds.Reprisal,
                    Duration = 10,
                    CD = 60
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.HeartOfLight),
                    Trigger = ActionIds.HeartOfLight,
                    Duration = 15,
                    CD = 90
                },
            });
            // ============ PLD ==================
            JobToValue.Add(JobIds.PLD, new [] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.HallowedGround),
                    Trigger = ActionIds.HallowedGround,
                    Duration = 10,
                    CD = 420
                },
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Reprisal)} ({UIHelper.Localize(JobIds.PLD)})",
                    Trigger = ActionIds.Reprisal,
                    Duration = 10,
                    CD = 60
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.DivineVeil),
                    Trigger = ActionIds.DivineVeil,
                    Duration = 5,
                    CD = 90
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.PassageOfArms),
                    Trigger = ActionIds.PassageOfArms,
                    Duration = 5,
                    CD = 120
                }
            });
            // ============ WAR ==================
            JobToValue.Add(JobIds.WAR, new [] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Holmgang),
                    Trigger = ActionIds.Holmgang,
                    Duration = 8,
                    CD = 240
                },
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Reprisal)} ({UIHelper.Localize(JobIds.WAR)})",
                    Trigger = ActionIds.Reprisal,
                    Duration = 10,
                    CD = 60
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.ShakeItOff),
                    Trigger = ActionIds.ShakeItOff,
                    Duration = 15,
                    CD = 90
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.NascentFlash),
                    Trigger = ActionIds.NascentFlash,
                    AdditionalTriggers = new[] {ActionIds.RawIntuition },
                    Duration = 6,
                    CD = 25
                }
            });
            // ============ DRK ==================
            JobToValue.Add(JobIds.DRK, new []{
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.LivingDead),
                    Trigger = ActionIds.LivingDead,
                    Duration = 10,
                    CD = 300
                },
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Reprisal)} ({UIHelper.Localize(JobIds.DRK)})",
                    Trigger = ActionIds.Reprisal,
                    Duration = 10,
                    CD = 60
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.DarkMissionary),
                    Trigger = ActionIds.DarkMissionary,
                    Duration = 15,
                    CD = 90
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.TheBlackestNight),
                    Trigger = ActionIds.TheBlackestNight,
                    Duration = 7,
                    CD = 15
                }
            });
            // ============ AST ==================
            JobToValue.Add(JobIds.AST, new [] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.NeutralSect),
                    Trigger = ActionIds.NeutralSect,
                    Duration = 20,
                    CD = 120
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.CelestialOpposition),
                    Trigger = ActionIds.CelestialOpposition,
                    CD = 60
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.CollectiveUnconscious),
                    Trigger = ActionIds.CollectiveUnconscious,
                    Duration = 15,
                    CD = 60
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.EarthlyStar),
                    Trigger = ActionIds.EarthlyStar,
                    Duration = 20,
                    CD = 60
                }
            });
            // ============ SCH ==================
            JobToValue.Add(JobIds.SCH, new [] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.SummonSeraph),
                    Trigger = ActionIds.SummonSeraph,
                    Duration = 22,
                    CD = 120
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.DeploymentTactics),
                    Trigger = ActionIds.DeploymentTactics,
                    CD = 120
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Recitation),
                    Trigger = ActionIds.Recitation,
                    CD = 90
                }
            });
            // ============ WHM ==================
            JobToValue.Add(JobIds.WHM, new [] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Temperance),
                    Trigger = ActionIds.Temperance,
                    Duration = 20,
                    CD = 120
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Benediction),
                    Trigger = ActionIds.Benediction,
                    CD = 180
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Asylum),
                    Trigger = ActionIds.Asylum,
                    Duration = 24,
                    CD = 90
                }
            });
            // ============ BRD ==================
            JobToValue.Add(JobIds.BRD, new [] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Troubadour),
                    Trigger = ActionIds.Troubadour,
                    Duration = 15,
                    CD = 120
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.NaturesMinne),
                    Trigger = ActionIds.NaturesMinne,
                    Duration = 15,
                    CD = 90
                }
            });
            // ============ DRG ==================
            JobToValue.Add(JobIds.DRG, new [] {
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.DRG)})",
                    Trigger = ActionIds.Feint,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ SMN ==================
            JobToValue.Add(JobIds.SMN, new [] {
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Addle)} ({UIHelper.Localize(JobIds.SMN)})",
                    Trigger = ActionIds.Addle,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ SAM ==================
            JobToValue.Add(JobIds.SAM, new [] {
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.SAM)})",
                    Trigger = ActionIds.Feint,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ BLM ==================
            JobToValue.Add(JobIds.BLM, new [] {
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Addle)} ({UIHelper.Localize(JobIds.BLM)})",
                    Trigger = ActionIds.Addle,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ RDM ==================
            JobToValue.Add(JobIds.RDM, new [] {
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Addle)} ({UIHelper.Localize(JobIds.RDM)})",
                    Trigger = ActionIds.Addle,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ MCH ==================
            JobToValue.Add(JobIds.MCH, new [] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Tactician),
                    Trigger = ActionIds.Tactician,
                    Duration = 15,
                    CD = 120
                }
            });
            // ============ DNC ==================
            JobToValue.Add(JobIds.DNC, new [] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.ShieldSamba),
                    Trigger = ActionIds.ShieldSamba,
                    Duration = 15,
                    CD = 120
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Improvisation),
                    Trigger = ActionIds.Improvisation,
                    Duration = 15,
                    CD = 120
                }
            });
            // ============ NIN ==================
            JobToValue.Add(JobIds.NIN, new [] {
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.NIN)})",
                    Trigger = ActionIds.Feint,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ MNK ==================
            JobToValue.Add(JobIds.MNK, new [] {
                new CooldownProps {
                    Name = $"{UIHelper.Localize(ActionIds.Feint)} ({UIHelper.Localize(JobIds.MNK)})",
                    Trigger = ActionIds.Feint,
                    Duration = 10,
                    CD = 90
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Mantra),
                    Trigger = ActionIds.Mantra,
                    Duration = 15,
                    CD = 90
                }
            });
            // ============ BLU ==================
            JobToValue.Add(JobIds.BLU, new CooldownProps[] {
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.Addle)+" "+UIHelper.Localize(JobIds.BLU),
                    Trigger = ActionIds.Addle,
                    Duration = 10,
                    CD = 90
                },
                new CooldownProps {
                    Name = UIHelper.Localize(ActionIds.AngelWhisper),
                    Trigger = ActionIds.AngelWhisper,
                    CD = 300
                },
            });
        }
    }
}