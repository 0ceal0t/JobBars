﻿using System;
using System.Runtime.InteropServices;

namespace JobBars.GameStructs {
    [StructLayout(LayoutKind.Sequential, Size = 519 * 4)]
    public unsafe struct AddonPartyListIntArray {
        public int Unknown000;
        public int Unknown001;
        public int PartyLeaderIndex;
        public int Unknown003;
        public int Unknown004;
        public int PartyMemberCount;
        public int Unknown006;
        public int Unknown007;
        public int Unknown008;

        public AddonPartyListMembersIntArray PartyMember;

        public int Unknown321;
        public OneHundredIntegers Unknowns322;
        public OneHundredIntegers Unknowns422;
        public TenIntegers Unknowns432;
        public TenIntegers Unknowns442;
        public TenIntegers Unknowns452;
        public TenIntegers Unknowns462;
        public TenIntegers Unknowns472;
        public TenIntegers Unknowns482;
        public TenIntegers Unknowns492;
        public TenIntegers Unknowns502;
        public TenIntegers Unknowns512;
        public int Unknown513;
        public int Unknown514;
        public int Unknown515;
        public int Unknown516;
        public int Unknown517;
        public int Unknown518;
        public int Unknown519;
    }

    [StructLayout(LayoutKind.Sequential, Size = 10 * 4)]
    public struct TenIntegers {
        public int i0;
        public int i1;
        public int i2;
        public int i3;
        public int i4;
        public int i5;
        public int i6;
        public int i7;
        public int i8;
        public int i9;
    }

    [StructLayout(LayoutKind.Sequential, Size = 100 * 4)]
    public struct OneHundredIntegers {
        public TenIntegers i0;
        public TenIntegers i1;
        public TenIntegers i2;
        public TenIntegers i3;
        public TenIntegers i4;
        public TenIntegers i5;
        public TenIntegers i6;
        public TenIntegers i7;
        public TenIntegers i8;
        public TenIntegers i9;
    }


    [StructLayout(LayoutKind.Sequential, Size = 39 * 4 * 8)]
    public unsafe struct AddonPartyListMembersIntArray {
        public AddonPartyListMemberIntArray Member0;
        public AddonPartyListMemberIntArray Member1;
        public AddonPartyListMemberIntArray Member2;
        public AddonPartyListMemberIntArray Member3;
        public AddonPartyListMemberIntArray Member4;
        public AddonPartyListMemberIntArray Member5;
        public AddonPartyListMemberIntArray Member6;
        public AddonPartyListMemberIntArray Member7;

        public AddonPartyListMemberIntArray this[int i] => i switch {
            0 => Member0,
            1 => Member1,
            2 => Member2,
            3 => Member3,
            4 => Member4,
            5 => Member5,
            6 => Member6,
            7 => Member7,
            _ => throw new IndexOutOfRangeException("Index should be between 0 and 7")
        };
    }

    [StructLayout(LayoutKind.Sequential, Size = 39 * 4)]
    public unsafe struct AddonPartyListMemberIntArray {
        public int Level;
        public int ClassJobIcon;

        public int Unknown2;

        public int HP;
        public int HPMax;
        public int ShieldPercentage;
        public int MP;
        public int MPMax;

        public int Unknown8;
        public int Unknown9;

        public int PartySlot;

        public int Unknown11;
        public int Unknown12;

        public int StatusEffectCount;
        public fixed int StatusEffects[10];

        public int Unknown24;
        public int Unknown25;
        public int Unknown26;
        public int Unknown27;
        public int Unknown28;
        public int Unknown29;
        public int Unknown30;
        public int Unknown31;
        public int Unknown32;
        public int Unknown33;

        public int CastingPercent;
        public int ObjectID;
    }
}