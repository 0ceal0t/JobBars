using System;
using Dalamud.Game;
using Dalamud.Plugin;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace JobBars.PartyList {
    public class PList : IReadOnlyCollection<PartyMember> {
        private readonly DalamudPluginInterface PluginInterface;

        private readonly GetPartyMemberCountDelegate getCrossPartyMemberCount;
        private readonly GetCompanionMemberCountDelegate getCompanionMemberCount;
        private readonly GetCrossMemberByGrpIndexDelegate getCrossMemberByGrpIndex;

        public IntPtr GetCrossRealmMemberCountPtr { get; private set; }
        public IntPtr GetCompanionMemberCountPtr { get; private set; }
        public IntPtr GetCrossMemberByGrpIndexPtr { get; private set; }

        public IntPtr GroupManager { get; private set; }
        public IntPtr CrossRealmGroupManagerPtr { get; private set; }
        public IntPtr CompanionManagerPtr { get; private set; }


        public PList(DalamudPluginInterface pi, SigScanner sig) {
            PluginInterface = pi;

            GroupManager = sig.GetStaticAddressFromSig("48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 80 B8 ?? ?? ?? ?? ?? 76 50");
            CrossRealmGroupManagerPtr = sig.GetStaticAddressFromSig("77 71 48 8B 05", 2);
            CompanionManagerPtr = sig.GetStaticAddressFromSig("4C 8B 15 ?? ?? ?? ?? 4C 8B C9");
            GetCrossRealmMemberCountPtr = sig.ScanText("E8 ?? ?? ?? ?? 3C 01 77 4B");
            GetCrossMemberByGrpIndexPtr = sig.ScanText("E8 ?? ?? ?? ?? 44 89 7C 24 ?? 4C 8B C8");
            GetCompanionMemberCountPtr = sig.ScanText("E8 ?? ?? ?? ?? 8B D3 85 C0");

            this.getCrossPartyMemberCount = Marshal.GetDelegateForFunctionPointer<GetPartyMemberCountDelegate>(GetCrossRealmMemberCountPtr);
            this.getCrossMemberByGrpIndex = Marshal.GetDelegateForFunctionPointer<GetCrossMemberByGrpIndexDelegate>(GetCrossMemberByGrpIndexPtr);
            this.getCompanionMemberCount = Marshal.GetDelegateForFunctionPointer<GetCompanionMemberCountDelegate>(GetCompanionMemberCountPtr);
        }

        private delegate byte GetPartyMemberCountDelegate();
        private delegate IntPtr GetCrossMemberByGrpIndexDelegate(int index, int group);
        private delegate byte GetCompanionMemberCountDelegate(IntPtr manager);

        public int Count {
            get {
                var count = this.getCrossPartyMemberCount();
                if (count > 0)
                    return count;
                count = this.GetRegularMemberCount();
                if (count > 1)
                    return count;
                count = this.GetCompanionMemberCount();
                return count > 0 ? count + 1 : 0;
            }
        }

        public PartyMember this[int index] {
            get {
                if (index < 0 || index >= this.Count)
                    return null;

                if (this.getCrossPartyMemberCount() > 0) {
                    var member = this.getCrossMemberByGrpIndex(index, -1);
                    if (member == IntPtr.Zero)
                        return null;
                    return PartyMember.CrossRealmMember(member);
                }

                if (this.GetRegularMemberCount() > 1) {
                    var member = GroupManager + (0x230 * index);
                    return PartyMember.RegularMember(member);
                }

                if (this.GetCompanionMemberCount() > 0) {
                    if (index >= 3) // return a dummy player member if it's not one of the npcs
                        return PartyMember.LocalPlayerMember(PluginInterface);
                    var member = Marshal.ReadIntPtr(CompanionManagerPtr) + (0x198 * index);
                    return PartyMember.CompanionMember(member);
                }

                return null;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public IEnumerator<PartyMember> GetEnumerator() {
            for (var i = 0; i < this.Count; i++) {
                var member = this[i];
                if (member != null)
                    yield return member;
            }
        }

        private byte GetRegularMemberCount() {
            return Marshal.ReadByte(GroupManager, 0x3D5C);
        }

        private byte GetCompanionMemberCount() {
            var manager = Marshal.ReadIntPtr(CompanionManagerPtr);
            if (manager == IntPtr.Zero)
                return 0;
            return this.getCompanionMemberCount(CompanionManagerPtr);
        }
    }
}