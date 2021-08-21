using JobBars.Data;
using JobBars.UI;
using System;
using System.Collections.Generic;

namespace JobBars.Utilities {
    public struct UtilityProps {
        public bool Enabled;
        public CursorRing Inner;
        public CursorRing Outer;

        /*public bool Enabled {
            get {
                var disabled = Configuration.Config.CooldownDisabled.Contains(Name);
                return DisabledByDefault ? disabled : !disabled;
            }
            set {
                if (value != DisabledByDefault) Configuration.Config.CooldownDisabled.Remove(Name);
                else Configuration.Config.CooldownDisabled.Add(Name);

                Configuration.Config.Save();
            }
        }*/
    }

    public struct CursorRing {
        public CursorType Type;
        public ElementColor Color;
    }

    public enum CursorType {
        None,
        GCD,
        CastTime
    }

    public partial class UtilityManager {
        public Dictionary<JobIds, UtilityProps> JobToUtilities;
        /*private void Init() {
            JobToUtilities = new();
            JobToUtilities.Add(JobIds.OTHER, new UtilityProps());
            // ============ GNB ==================
            JobToUtilities.Add(JobIds.GNB, new [] {
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
            JobToUtilities.Add(JobIds.PLD, new [] {
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
            JobToUtilities.Add(JobIds.WAR, new [] {
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
                    CD = 25,
                    DisabledByDefault = true
                }
            });
            // ============ DRK ==================
            JobToUtilities.Add(JobIds.DRK, new []{
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
                    CD = 15,
                    DisabledByDefault = true
                }
            });
            // ============ AST ==================
            JobToUtilities.Add(JobIds.AST, new [] {
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
            JobToUtilities.Add(JobIds.SCH, new [] {
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
            JobToUtilities.Add(JobIds.WHM, new [] {
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
            JobToUtilities.Add(JobIds.BRD, new [] {
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
            JobToUtilities.Add(JobIds.DRG, new [] {
                new CooldownProps {
                    Name = "Feint (DRG)",
                    Trigger = ActionIds.Feint,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ SMN ==================
            JobToUtilities.Add(JobIds.SMN, new [] {
                new CooldownProps {
                    Name = "Addle (SMN)",
                    Trigger = ActionIds.Addle,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ SAM ==================
            JobToUtilities.Add(JobIds.SAM, new [] {
                new CooldownProps {
                    Name = "Feint (SAM)",
                    Trigger = ActionIds.Feint,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ BLM ==================
            JobToUtilities.Add(JobIds.BLM, new [] {
                new CooldownProps {
                    Name = "Addle (BLM)",
                    Trigger = ActionIds.Addle,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ RDM ==================
            JobToUtilities.Add(JobIds.RDM, new [] {
                new CooldownProps {
                    Name = "Addle (RDM)",
                    Trigger = ActionIds.Addle,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ MCH ==================
            JobToUtilities.Add(JobIds.MCH, new [] {
                new CooldownProps {
                    Name = "Tactician",
                    Trigger = ActionIds.Tactician,
                    Duration = 15,
                    CD = 120
                }
            });
            // ============ DNC ==================
            JobToUtilities.Add(JobIds.DNC, new [] {
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
            JobToUtilities.Add(JobIds.NIN, new [] {
                new CooldownProps {
                    Name = "Feint (NIN)",
                    Trigger = ActionIds.Feint,
                    Duration = 10,
                    CD = 90
                }
            });
            // ============ MNK ==================
            JobToUtilities.Add(JobIds.MNK, new [] {
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
            JobToUtilities.Add(JobIds.BLU, new CooldownProps[] {

            });
        }*/
    }
}