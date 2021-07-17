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

        [StructLayout(LayoutKind.Explicit)] //idk about size
        private unsafe struct RegularMemberStruct {
            [FieldOffset(0x1A8)] public int ActorId;
        }

        [StructLayout(LayoutKind.Explicit)] //idk about size
        private unsafe struct CrossRealmMemberStruct {
            [FieldOffset(0x10)] public int ActorId;
        }

        [StructLayout(LayoutKind.Explicit)] //idk about size
        private unsafe struct CompanionMemberStruct {
            [FieldOffset(0x0)] public int ActorId;
        }

        private PartyMember() {
        }

        public int ActorId { get; private set; }

        public IntPtr Address { get; private set; }

        internal unsafe static PartyMember RegularMember(IntPtr memberAddress) {
            return new PartyMember
            {
                ActorId = ((RegularMemberStruct*)memberAddress)->ActorId,
                Address = memberAddress,
            };
        }

        internal unsafe static PartyMember CrossRealmMember(IntPtr crossMemberAddress) {
            return new PartyMember
            {
                ActorId = ((CrossRealmMemberStruct*)crossMemberAddress)->ActorId,
                Address = crossMemberAddress,
            };
        }

        internal unsafe static PartyMember CompanionMember(IntPtr companionMemberAddress) {
            return new PartyMember
            {
                ActorId = ((CompanionMemberStruct*)companionMemberAddress)->ActorId,
                Address = companionMemberAddress,
            };
        }

        internal static PartyMember LocalPlayerMember(DalamudPluginInterface pi) {
            var player = pi.ClientState.LocalPlayer;
            return new PartyMember
            {
                ActorId = player.ActorId,
                Address = player?.Address ?? IntPtr.Zero,
            };
        }
    }
}