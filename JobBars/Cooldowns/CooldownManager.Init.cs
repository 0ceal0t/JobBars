using Dalamud.Game.ClientState.JobGauge;
using JobBars.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Cooldowns {
    public struct CooldownProps {
        public string Name;
        public ActionIds Trigger;
        public float Duration;
        public float CD;
    }

    public partial class CooldownManager {

        public Dictionary<JobIds, CooldownProps[]> JobToCooldowns;

        private void Init() {
            JobToCooldowns = new();
            JobToCooldowns.Add(JobIds.OTHER, Array.Empty<CooldownProps>());
            // ============ GNB ==================
            JobToCooldowns.Add(JobIds.GNB, new [] {
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
            JobToCooldowns.Add(JobIds.PLD, new [] {
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
            JobToCooldowns.Add(JobIds.WAR, new CooldownProps[] {
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
                }
            });
            // ============ DRK ==================
            JobToCooldowns.Add(JobIds.DRK, new []{
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
                }
            });
            // ============ AST ==================
            JobToCooldowns.Add(JobIds.AST, new CooldownProps[] {
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
            JobToCooldowns.Add(JobIds.SCH, new CooldownProps[] {
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
            JobToCooldowns.Add(JobIds.WHM, new CooldownProps[] {
                new CooldownProps {
                    Name = "Asylum",
                    Trigger = ActionIds.Asylum,
                    Duration = 24,
                    CD = 90
                },
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
                }
            });
            // ============ BRD ==================
            JobToCooldowns.Add(JobIds.BRD, new CooldownProps[] {

            });
            // ============ DRG ==================
            JobToCooldowns.Add(JobIds.DRG, new CooldownProps[] {

            });
            // ============ SMN ==================
            JobToCooldowns.Add(JobIds.SMN, new CooldownProps[] {

            });
            // ============ SAM ==================
            JobToCooldowns.Add(JobIds.SAM, new CooldownProps[] {

            });
            // ============ BLM ==================
            JobToCooldowns.Add(JobIds.BLM, new CooldownProps[] {

            });
            // ============ RDM ==================
            JobToCooldowns.Add(JobIds.RDM, new CooldownProps[] {

            });
            // ============ MCH ==================
            JobToCooldowns.Add(JobIds.MCH, new CooldownProps[] {

            });
            // ============ DNC ==================
            JobToCooldowns.Add(JobIds.DNC, new CooldownProps[] {

            });
            // ============ NIN ==================
            JobToCooldowns.Add(JobIds.NIN, new CooldownProps[] {

            });
            // ============ MNK ==================
            JobToCooldowns.Add(JobIds.MNK, new CooldownProps[] {

            });
            // ============ BLU ==================
            JobToCooldowns.Add(JobIds.BLU, new CooldownProps[] {

            });
        }
    }
}