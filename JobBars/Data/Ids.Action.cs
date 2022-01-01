namespace JobBars.Data {
    public enum ActionIds : uint {

        // Tank Role Actions
        ArmsLength = 7548,  // and Melee
        Rampart = 7531,
        LowBlow = 7540,
        Provoke = 7533,
        Interject = 7538,
        Reprisal = 7535,
        Shirk = 7537,

        // Healer Role Actions
        Swiftcast = 7561,       // and Caster
        LucidDreaming = 7562,    // and Caster
        Surecast = 7559,         // and Caster
        Rescue = 7571,

        // Melee Role Actions
        SecondWind = 7541,      // and Renger
        LegSweep = 7863,
        Bloodbath = 7542,
        Feint = 7549,
        TrueNorth = 7546,

        // Ranger Role Actions
        LegGraze = 7554,
        FootGraze = 7553,
        // Peloton = 7557,
        HeadGraze = 7551,

        // Caster Role Actions
        Addle = 7560,

        // DRK =========
        ShadowWall = 3636,
        DarkMind = 3634,
        // Oblation = 25754,
        DarkMissionary = 16471,
        LivingDead = 3638,
        Delirium = 7390,
        LivingShadow = 0x4058,
        TheBlackestNight = 7393,
        BloodWeapon = 3625,

        // WAR ==========
        ThrillOfBattle = 40,
        Vengeance = 44,
        InnerRelease = 7389,
        ShakeItOff = 7388,
        Holmgang = 43,
        NascentFlash = 16464,
        RawIntuition = 3551,
        StormsEye = 45,
        Bloodwhetting = 25751,

        // PLD ===========
        Sentinel = 17,
        Requiescat = 7383,
        FastBlade = 0x09,
        RiotBlade = 0x0F,
        GoringBlade = 3538,
        RageOfHalone = 21,
        ShieldLob = 24,
        RoyalAuthority = 0xDD3,
        Atonement = 0x404C,
        TotalEclipse = 0x1CD5,
        Prominence = 0x4049,
        HallowedGround = 30,
        DivineVeil = 3540,
        PassageOfArms = 7385,
        FightOrFlight = 20,

        // GNB ===========
        Camouflage = 16140,
        Nebula = 16148,
        NoMercy = 16138,
        HeartOfLight = 16160,
        Superbolide = 16152,
        HeartofStone = 16161,
        HeartofCorundum = 25758,

        // SCH ===========
        Biolysis = 16540,
        SchBio = 17864,
        SchBio2 = 17865,
        SacredSoil = 188,
        Indomitability = 3583,
        Dissipation = 3587,
        Excogitation = 7434,
        ChainStratagem = 7436,
        SummonSeraph = 16545,
        Protraction = 25867,
        Expedient = 25868,

        // WHM ===========
        Dia = 16532,
        Aero = 121,
        Aero2 = 132,
        PresenceOfMind = 136,
        Benediction = 140,
        Asylum = 3569,
        Assize = 3571,
        Temperance = 16536,
        Aquaveil = 25861,
        LilyBell = 25862,

        // AST ===========
        Combust1 = 3599,
        Combust2 = 3608,
        Combust3 = 16554,
        TheBalance = 4401,
        TheArrow = 4402,
        TheSpear = 4403,
        TheBole = 4404,
        TheEwer = 4405,
        TheSpire = 4406,
        Synastry = 3612,
        Divination = 16552,
        Astrodyne = 25870,
        CollectiveUnconscious = 3613,
        CelestialOpposition = 16553,
        EarthlyStar = 7439,
        NeutralSect = 16559,
        Lightspeed = 3606,
        StellarDetonation = 8324,
        Exaltation = 25873,
        Macrocosmos = 25874,

        // SGE ============
        Dosis = 24283,
        Dosis2 = 24306,
        Dosis3 = 24312,
        EukrasianDosis = 24293,
        EukrasianDosis2 = 24308,
        EukrasianDosis3 = 24314,
        Physis = 24288,
        Physis2 = 24302,
        Soteria = 24294,
        Kerachole = 24298,
        Ixochole = 24299,
        Taurochole = 24303,
        Haima = 24305,
        Holos = 24310,
        Panhaima = 24311,
        Pneuma = 24318,

        // MNK ===========
        Brotherhood = 7396,
        PerfectBalance = 69,
        RiddleOfFire = 7395,
        TwinSnakes = 61,
        Demolish = 66,
        Mantra = 65,

        // DRG ===========
        DragonSight = 7398,
        BattleLitany = 3557,
        LanceCharge = 85,
        Disembowel = 87,
        ChaosThrust = 88,
        ChaoticSpring = 25772,

        // NIN ===========
        TrickAttack = 2258,
        Bunshin = 16493,

        // SAM ===========
        Higanbana = 7489,
        Jinpu = 7478,
        Shifu = 7479,
        OgiNamikiri = 25781,

        // RPR ===========
        ArcaneCircle = 24405,
        ArcaneCrest = 24404,
        ShadowOfDeath = 24378,

        // BRD ===========
        CausticBite = 0x1CEE,
        Stormbite = 0x1CEF,
        VenomousBite = 0x64,
        Windbite = 0x71,
        BattleVoice = 118,
        RagingStrikes = 101,
        IronJaws = 3560,
        Barrage = 107,
        BloodLetter = 110,
        Troubadour = 7405,
        NaturesMinne = 7408,
        RadiantFinale = 25785,

        // MCH ===========
        Wildfire = 2878,
        Hypercharge = 0x4339,
        HeatBlast = 0x1CF2,
        AutoCrossbow = 0x4071,
        GaussRound = 2874,
        Ricochet = 2890,
        Tactician = 16889,

        // DNC ===========
        QuadTechFinish = 16196,
        Devilment = 16011,
        ShieldSamba = 16012,
        Improvisation = 16014,
        CuringWaltz = 16015,

        // BLM ===========
        Thunder = 144,
        Thunder2 = 7447,
        Thunder3 = 153,
        Thunder4 = 7420,
        LeyLines = 3573,
        Sharpcast = 3574,

        // SMN ===========
        SummonBahamut = 7427,
        SummonPhoenix = 25831,
        SearingLight = 25801,

        // RDM ===========
        Embolden = 7520,
        Manafication = 7521,
        MagickBarrier = 25857,

        // BLU ===========
        SongOfTorment = 11386,
        PeculiarLight = 11421,
        OffGuard = 11411,
        Nightbloom = 23290,
        BadBreath = 11388,
        CondensedLibra = 18321,
        AngelWhisper = 18317
    }
}
