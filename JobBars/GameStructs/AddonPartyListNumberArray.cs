using System;
using System.Runtime.InteropServices;

// https://github.com/Caraxi/PartyListLayout/blob/ef40b191cd9e08349e1a0e9899aa252d580ff2ac/GameStructs/NumberArray/AddonPartyListNumberArray.cs
namespace JobBars.GameStructs {
    [StructLayout( LayoutKind.Sequential, Size = 727 * 4 )]
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
    }


    [StructLayout( LayoutKind.Sequential, Size = 42 * 4 * 8 )]
    public unsafe struct AddonPartyListMembersIntArray {
        public AddonPartyListMemberIntArray Member0;
        public AddonPartyListMemberIntArray Member1;
        public AddonPartyListMemberIntArray Member2;
        public AddonPartyListMemberIntArray Member3;
        public AddonPartyListMemberIntArray Member4;
        public AddonPartyListMemberIntArray Member5;
        public AddonPartyListMemberIntArray Member6;
        public AddonPartyListMemberIntArray Member7;

        public readonly AddonPartyListMemberIntArray this[int i] => i switch {
            0 => Member0,
            1 => Member1,
            2 => Member2,
            3 => Member3,
            4 => Member4,
            5 => Member5,
            6 => Member6,
            7 => Member7,
            _ => throw new IndexOutOfRangeException( "Index should be between 0 and 7" )
        };
    }

    [StructLayout( LayoutKind.Sequential, Size = 42 * 4 )]
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
        public int CastingTargetIcon;
        public int Unknown36;
        public int Unknown37;
        public int ObjectID;
    }
}