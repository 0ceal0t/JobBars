using Dalamud.Logging;
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
        private List<CooldownPartyMember> Members = new(); // unordered
        private Dictionary<JobIds, CooldownStruct[]> JobToCooldowns;

        private readonly DalamudPluginInterface PluginInterface;
        private readonly PList Party;

        public CooldownManager(DalamudPluginInterface pluginInterface, PList party) {
            PluginInterface = pluginInterface;
            Party = party;
        }

        public void Tick() {
            //            var addon = (AddonPartyList*)PluginInterface?.Framework.Gui.GetUiObjectByName("_PartyList", 1);
        }

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

        public void DisposeInstance() {
        }
    }
}