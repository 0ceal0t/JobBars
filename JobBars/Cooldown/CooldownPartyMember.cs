using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using System;
using System.Collections.Generic;
using static FFXIVClientStructs.FFXIV.Client.UI.AddonPartyList;

namespace JobBars.Cooldown {
    public unsafe class CooldownPartyMember {
        public readonly int ObjectId;

        private List<CooldownItem> Items = new();
        private JobIds Job = JobIds.OTHER;
        private AtkComponentBase* Component = null;

        public CooldownPartyMember(int objectId) {
            ObjectId = objectId;
        }

        public void Tick(PartyListMemberStruct addonItem, uint iconId) {
            var job = JobFromIcon(iconId);
            if (job != Job) { // TODO: find a better way for jobs?
                // change job
                // delete old elements, if any
                // create new ones
            }
            if (!new IntPtr(Component).Equals(new IntPtr(addonItem.PartyMemberComponent))) {
                // change item
                // detach current elements, if any
                // attach to new position
            }

            Job = job;
            Component = addonItem.PartyMemberComponent;
        }

        public void Dispose() {
            Component = null;
        }

        private static JobIds JobFromIcon(uint icon) {
            return icon switch {
                62133 => JobIds.AST,
                _ => JobIds.OTHER
            };
        }
    }
}