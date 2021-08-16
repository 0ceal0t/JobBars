using Dalamud.Game.ClientState.JobGauge;
using JobBars.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Cooldowns {
    public struct CooldownProps {
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
            JobToCooldowns.Add(JobIds.GNB, new CooldownProps[] {

            });
            // ============ PLD ==================
            JobToCooldowns.Add(JobIds.PLD, new CooldownProps[] {

            });
            // ============ WAR ==================
            JobToCooldowns.Add(JobIds.WAR, new CooldownProps[] {

            });
            // ============ DRK ==================
            JobToCooldowns.Add(JobIds.DRK, new []{
                new CooldownProps {
                    Trigger = ActionIds.Rampart,
                    Duration = 20,
                    CD = 90
                },
                new CooldownProps {
                    Trigger = ActionIds.Reprisal,
                    Duration = 10,
                    CD = 60
                },
                new CooldownProps {
                    Trigger = ActionIds.DarkMissionary,
                    Duration = 15,
                    CD = 90
                },
                new CooldownProps {
                    Trigger = ActionIds.ShadowWall,
                    Duration = 15,
                    CD = 120
                },
                new CooldownProps {
                    Trigger = ActionIds.DarkMind,
                    Duration = 10,
                    CD = 60
                }
            });
            // ============ AST ==================
            JobToCooldowns.Add(JobIds.AST, new CooldownProps[] {

            });
            // ============ SCH ==================
            JobToCooldowns.Add(JobIds.SCH, new CooldownProps[] {

            });
            // ============ WHM ==================
            JobToCooldowns.Add(JobIds.WHM, new CooldownProps[] {

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