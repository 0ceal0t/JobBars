using Dalamud.Game.ClientState.JobGauge;
using JobBars.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Cooldown {
    public partial class CooldownManager {
        public struct CooldownStruct {
            public ActionIds[] Trigger;
            public IconIds Icon;
        }

        private void Init() {
            JobToCooldowns = new Dictionary<JobIds, CooldownStruct[]>();
            JobToCooldowns.Add(JobIds.OTHER, Array.Empty<CooldownStruct>());
            // ============ GNB ==================
            JobToCooldowns.Add(JobIds.GNB, new CooldownStruct[] {

            });
            // ============ PLD ==================
            JobToCooldowns.Add(JobIds.PLD, new CooldownStruct[] {

            });
            // ============ WAR ==================
            JobToCooldowns.Add(JobIds.WAR, new CooldownStruct[] {

            });
            // ============ DRK ==================
            JobToCooldowns.Add(JobIds.DRK, new CooldownStruct[] {

            });
            // ============ AST ==================
            JobToCooldowns.Add(JobIds.AST, new CooldownStruct[] {

            });
            // ============ SCH ==================
            JobToCooldowns.Add(JobIds.SCH, new CooldownStruct[] {

            });
            // ============ WHM ==================
            JobToCooldowns.Add(JobIds.WHM, new CooldownStruct[] {

            });
            // ============ BRD ==================
            JobToCooldowns.Add(JobIds.BRD, new CooldownStruct[] {

            });
            // ============ DRG ==================
            JobToCooldowns.Add(JobIds.DRG, new CooldownStruct[] {

            });
            // ============ SMN ==================
            JobToCooldowns.Add(JobIds.SMN, new CooldownStruct[] {

            });
            // ============ SAM ==================
            JobToCooldowns.Add(JobIds.SAM, new CooldownStruct[] {

            });
            // ============ BLM ==================
            JobToCooldowns.Add(JobIds.BLM, new CooldownStruct[] {

            });
            // ============ RDM ==================
            JobToCooldowns.Add(JobIds.RDM, new CooldownStruct[] {

            });
            // ============ MCH ==================
            JobToCooldowns.Add(JobIds.MCH, new CooldownStruct[] {

            });
            // ============ DNC ==================
            JobToCooldowns.Add(JobIds.DNC, new CooldownStruct[] {

            });
            // ============ NIN ==================
            JobToCooldowns.Add(JobIds.NIN, new CooldownStruct[] {

            });
            // ============ MNK ==================
            JobToCooldowns.Add(JobIds.MNK, new CooldownStruct[] {

            });
            // ============ BLU ==================
            JobToCooldowns.Add(JobIds.BLU, new CooldownStruct[] {

            });
        }
    }
}