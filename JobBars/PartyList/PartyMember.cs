using Dalamud.Plugin;
using System;
using System.Runtime.InteropServices;

namespace JobBars.PartyList {
    public class PartyMember {

        [StructLayout(LayoutKind.Explicit)]
        private unsafe struct RegularMemberStruct {
            [FieldOffset(0x1A8)] public int ObjectId;
        }

        [StructLayout(LayoutKind.Explicit)]
        private unsafe struct CrossRealmMemberStruct {
            [FieldOffset(0x10)] public int ObjectId;
        }

        [StructLayout(LayoutKind.Explicit)]
        private unsafe struct CompanionMemberStruct {
            [FieldOffset(0x0)] public int ObjectId;
        }

        private PartyMember() { }

        public int ObjectId { get; private set; }

        public IntPtr Address { get; private set; }

        internal unsafe static PartyMember RegularMember(IntPtr memberAddress) {
            return new PartyMember {
                ObjectId = ((RegularMemberStruct*)memberAddress)->ObjectId,
                Address = memberAddress,
            };
        }

        internal unsafe static PartyMember CrossRealmMember(IntPtr crossMemberAddress) {
            return new PartyMember {
                ObjectId = ((CrossRealmMemberStruct*)crossMemberAddress)->ObjectId,
                Address = crossMemberAddress,
            };
        }

        internal unsafe static PartyMember CompanionMember(IntPtr companionMemberAddress) {
            return new PartyMember {
                ObjectId = ((CompanionMemberStruct*)companionMemberAddress)->ObjectId,
                Address = companionMemberAddress,
            };
        }

        internal static PartyMember LocalPlayerMember(DalamudPluginInterface pi) {
            var player = pi.ClientState.LocalPlayer;
            return new PartyMember {
                ObjectId = (int)player.ObjectId,
                Address = player?.Address ?? IntPtr.Zero,
            };
        }
    }
}