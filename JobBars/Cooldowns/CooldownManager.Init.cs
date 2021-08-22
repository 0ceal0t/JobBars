using JobBars.Data;
using System;

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
                    Name = "Superbolide",
                    Trigger = ActionIds.Superbolide,
                    Duration = 8,
                    CD = 360
                },
                new CooldownProps {
                    Name = "Reprisal (GNB)",
                    Trigger = ActionIds.Reprisal,
                    Duration = 10,
                    CD = 60
                },
                new CooldownProps {
                    Name = "Heart of Light",
                    Trigger = ActionIds.HeartOfLight,
                    Duration = 15,
                    CD = 90
                },
            });
            // ============ PLD ==================
            JobToValue.Add(JobIds.PLD, new [] {
                new CooldownProps {
                    Name = "Hallowed Ground",
                    Trigger = ActionIds.HallowedGround,
                    Duration = 10,
                    CD = 420
                },
                new CooldownProps {
                    Name = "Reprisal (PLD)",
                    Trigger = ActionIds.Reprisal,
                    Duration = 10,
                    CD = 60
                },
                new CooldownProps {
                    Name = "Divine Veil",
                    Trigger = ActionIds.DivineVeil,
                    Duration = 5,
                    CD = 90
                },
                new CooldownProps {
                    Name = "Passage of Arms",
                    Trigger = ActionIds.PassageOfArms,
                    Duration = 5,
                    CD = 120
                }
            });
            // ============ WAR ==================
            JobToValue.Add(JobIds.WAR, new [] {
                new CooldownProps {
                    Name = "Holmgang",
                    Trigger = ActionIds.Holmgang,
                    Duration = 8,
                    CD = 240
                },
                new CooldownProps {
                    Name = "Reprisal (WAR)",
                    Trigger = ActionIds.Reprisal,
                    Duration = 10,
                    CD = 60
                },
                new CooldownProps {
                    Name = "Shake it Off",
                    Trigger = ActionIds.ShakeItOff,
                    Duration = 15,
                    CD = 90
                },
                new CooldownProps {
                    Name = "Nascent Flash",
                    Trigger = ActionIds.NascentFlash,
                    AdditionalTriggers = new[] {ActionIds.RawIntuition },
                    Duration = 6,
                    CD = 25
                }
            });
            // ============ DRK ==================
            JobToValue.Add(JobIds.DRK, new []{
                new CooldownProps {
                    Name = "Living Dead",
                    Trigger = ActionIds.LivingDead,
                    Duration = 10,
                    CD = 300
                },
                new CooldownProps {
                    Name = "Reprisal (DRK)",
                    Trigger = ActionIds.Reprisal,
                    Duration = 10,
                    CD = 60
                },
                new CooldownProps {
                    Name = "Dark Missionary",
                    Trigger = ActionIds.DarkMissionary,
                    Duration = 15,
                    CD = 90
                },
                new CooldownProps {
                    Name = "The Blackest Night",
                    Trigger = ActionIds.TheBlackestNight,
                    Duration = 7,
                    CD = 15
                }
            });
            // ============ AST ==================
            JobToValue.Add(JobIds.AST, new [] {
                new CooldownProps {
                    Name = "Netural Sect",
                    Trigger = ActionIds.NeutralSect,
                    Duration = 20,
                    CD = 120
                },
                new CooldownProps {
                    Name = "Celestial Opposition",
                    Trigger = ActionIds.CelestialOpposition,
                    CD = 60
                },
                new CooldownProps {
                    Name = "Collective Unconscious",
                    Trigger = ActionIds.CollectiveUnconscious,
                    Duration = 15,
                    CD = 60
                },
                new CooldownProps {
                    Name = "Earthly Star",
                    Trigger = ActionIds.EarthlyStar,
                    Duration = 10,
                    CD = 60
                }
            });
            // ============ SCH ==================
            JobToValue.Add(JobIds.SCH, new [] {
                new CooldownProps {
                    Name = "Seraph",
                    Trigger = ActionIds.SummonSeraph,
                    Duration = 22,
                    CD = 120
                },
                new CooldownProps {
                    Name = "Deployment Tactics",
                    Trigger = ActionIds.DeploymentTactics,
                    CD = 120
                },
                new CooldownProps {
                    Name = "Recitation",
                    Trigger = ActionIds.Recitation,
                    CD = 90
                }
            });
            // ============ WHM ==================
            JobToValue.Add(JobIds.WHM, new [] {
                new CooldownProps {
                    Name = "Temperance",
                    Trigger = ActionIds.Temperance,
                    Duration = 20,
                    CD = 120
                },
                new CooldownProps {
                    Name = "Benediction",
                    Trigger = ActionIds.Benediction,
                    CD = 180
                },
                new CooldownProps {
                    Name = "Asylum",
                    Trigger = ActionIds.Asylum,
                    Duration = 24,
                    CD = 90
                }
            });
            // ============ BRD ==================
            JobToValue.Add(JobIds.BRD, new [] {
                new CooldownProps {
                    Name = "Troubadour",
                    Trigger = ActionIds.Troubadour,
                    Duration = 15,
                    CD = 120
                },
                new CooldownProps {
                    Name = "Nature's Minne",
                    Trigger = ActionIds.NaturesMinne,
                    Duration = 15,
                    CD = 90
                }
            });
            // ============ DRG ==================
            JobToValue.Add(JobIds.DRG, new [] {
                new CooldownProps {
                    Name = "Feint (DRG)",
                    Trigger = ActionIds.Feint,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ SMN ==================
            JobToValue.Add(JobIds.SMN, new [] {
                new CooldownProps {
                    Name = "Addle (SMN)",
                    Trigger = ActionIds.Addle,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ SAM ==================
            JobToValue.Add(JobIds.SAM, new [] {
                new CooldownProps {
                    Name = "Feint (SAM)",
                    Trigger = ActionIds.Feint,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ BLM ==================
            JobToValue.Add(JobIds.BLM, new [] {
                new CooldownProps {
                    Name = "Addle (BLM)",
                    Trigger = ActionIds.Addle,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ RDM ==================
            JobToValue.Add(JobIds.RDM, new [] {
                new CooldownProps {
                    Name = "Addle (RDM)",
                    Trigger = ActionIds.Addle,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ MCH ==================
            JobToValue.Add(JobIds.MCH, new [] {
                new CooldownProps {
                    Name = "Tactician",
                    Trigger = ActionIds.Tactician,
                    Duration = 15,
                    CD = 120
                }
            });
            // ============ DNC ==================
            JobToValue.Add(JobIds.DNC, new [] {
                new CooldownProps {
                    Name = "Shield Samba",
                    Trigger = ActionIds.ShieldSamba,
                    Duration = 15,
                    CD = 120
                },
                new CooldownProps {
                    Name = "Improvisation",
                    Trigger = ActionIds.Improvisation,
                    Duration = 15,
                    CD = 120
                }
            });
            // ============ NIN ==================
            JobToValue.Add(JobIds.NIN, new [] {
                new CooldownProps {
                    Name = "Feint (NIN)",
                    Trigger = ActionIds.Feint,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ MNK ==================
            JobToValue.Add(JobIds.MNK, new [] {
                new CooldownProps {
                    Name = "Feint (MNK)",
                    Trigger = ActionIds.Feint,
                    Duration = 10,
                    CD = 90
                },
                new CooldownProps {
                    Name = "Mantra",
                    Trigger = ActionIds.Mantra,
                    Duration = 15,
                    CD = 90
                }
            });
            // ============ BLU ==================
            JobToValue.Add(JobIds.BLU, new CooldownProps[] {

            });
        }
    }
}