using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using System;
using System.Collections.Generic;

namespace JobBars.Cooldown {
    public unsafe class CooldownPartyMember {
        public readonly int ObjectId;

        private List<CooldownItem> Items = new();
        private JobIds Job = JobIds.OTHER;

        public CooldownPartyMember(int objectId) {
            ObjectId = objectId;
        }

        public void Tick(uint iconId) {
            var job = JobFromIcon(iconId);

            Job = job;
        }

        public void Dispose() {

        }

        private static JobIds JobFromIcon(uint icon) {
            return icon switch {
                62133 => JobIds.AST,
                _ => JobIds.OTHER
            };
        }
    }
}