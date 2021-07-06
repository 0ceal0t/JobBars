using Dalamud.Game.ClientState.Actors;
using Dalamud.Game.ClientState.Actors.Types;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.PartyList {
    public class PartyMember {
        private PartyMember() {
        }

        public int ActorId { get; private set; }

        public IntPtr Address { get; private set; }

        internal static PartyMember RegularMember(IntPtr memberAddress) {
            var member = new PartyMember
            {
                ActorId = Marshal.ReadInt32(memberAddress, 0x1A8),
                Address = memberAddress,
            };
            return member;
        }

        internal static PartyMember CrossRealmMember(IntPtr crossMemberAddress) {
            var member = new PartyMember
            {
                ActorId = Marshal.ReadInt32(crossMemberAddress, 0x10),
                Address = crossMemberAddress,
            };
            return member;
        }

        internal static PartyMember CompanionMember(IntPtr companionMemberAddress) {
            var member = new PartyMember
            {
                ActorId = Marshal.ReadInt32(companionMemberAddress, 0),
                Address = companionMemberAddress,
            };
            return member;
        }

        internal static PartyMember LocalPlayerMember(DalamudPluginInterface pi) {
            var player = pi.ClientState.LocalPlayer;
            return new PartyMember()
            {
                ActorId = player.ActorId,
                Address = player?.Address ?? IntPtr.Zero,
            };
        }
    }
}