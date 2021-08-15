﻿using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using JobBars.Data;
using JobBars.PartyList;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Cooldown {
    public unsafe partial class CooldownManager {
        private HashSet<CooldownPartyMember> Members = new(); // unordered
        private Dictionary<JobIds, CooldownStruct[]> JobToCooldowns;

        private readonly DalamudPluginInterface PluginInterface;
        private readonly PList Party;

        public CooldownManager(DalamudPluginInterface pluginInterface, PList party) {
            PluginInterface = pluginInterface;
            Party = party;
        }
        
        public void Tick() {
            var addon = (AddonPartyList*) PluginInterface?.Framework.Gui.GetUiObjectByName("_PartyList", 1);
            if (addon == null || addon->AtkUnitBase.RootNode == null) return;

            var activeMembers = new HashSet<CooldownPartyMember>();

            for(int idx = 0; idx < addon->MemberCount; idx++) {
                var addonItem = addon->PartyMember[idx];

                var iconId = addon->PartyClassJobIconId[idx];
                var partyMemberId = Party.Count == 0 ? (int) PluginInterface.ClientState.LocalPlayer.ObjectId : Party[idx].ObjectId; // if no party members, just you

                var found = false;
                foreach(var cdMember in Members) {
                    if (cdMember.ObjectId == partyMemberId) {
                        cdMember.Tick(addonItem, iconId);
                        activeMembers.Add(cdMember);

                        found = true;
                        break;
                    }
                }
                if (found) continue;

                var newMember = new CooldownPartyMember(partyMemberId);
                newMember.Tick(addonItem, iconId);
                Members.Add(newMember);
                activeMembers.Add(newMember);
            }

            foreach (var cdMember in Members) {
                if (activeMembers.Contains(cdMember)) continue;

                cdMember.Dispose();
                Members.Remove(cdMember);
            }
        }

        public void RefreshPartyList() {
            /*
             * skip if party list doesn't exist or is hidden, etc.
             * 
             * go through party list addon
             * go through party (struct), skip companions (should be done automatically)
             * 
             *  look for the right CooldownPartyMember
             *      found? update (might need to switch component)
             *      else? make a new one
             *      
             *  keep track of unclaimed cooldownparty members, dispose
             * 
             */
        }

        public void DisposeInstance() {
            foreach (var member in Members) member.Dispose();
            Members.Clear();
        }
    }
}