using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.UI {
    public class UIColor {
        public static readonly ByteColor BYTE_White = new() {
            R = 255,
            G = 255,
            B = 255,
            A = 255
        };

        public static readonly ByteColor BYTE_Transparent = new() {
            R = 0,
            G = 0,
            B = 0,
            A = 0
        };

        public struct ElementColor {
            public string Name;

            public ushort AddRed;
            public ushort AddGreen;
            public ushort AddBlue;

            public byte MultiplyRed;
            public byte MultiplyGreen;
            public byte MultiplyBlue;

            public ElementColor(string name, ushort addRed, ushort addGreen, ushort addBlue, byte multRed, byte multGreen, byte multBlue) {
                Name = name;
                AddRed = addRed;
                AddGreen = addGreen;
                AddBlue = addBlue;
                MultiplyRed = multRed;
                MultiplyGreen = multGreen;
                MultiplyBlue = multBlue;
            }
        }

        public unsafe static void SetColor(AtkResNode* node, ElementColor color) {
            node->AddRed = color.AddRed;
            node->AddGreen = color.AddGreen;
            node->AddBlue = color.AddBlue;
            node->AddRed_2 = color.AddRed;
            node->AddGreen_2 = color.AddGreen;
            node->AddBlue_2 = color.AddBlue;

            node->MultiplyRed = color.MultiplyRed;
            node->MultiplyGreen = color.MultiplyGreen;
            node->MultiplyBlue = color.MultiplyBlue;
            node->MultiplyRed_2 = color.MultiplyRed;
            node->MultiplyGreen_2 = color.MultiplyGreen;
            node->MultiplyBlue_2 = color.MultiplyBlue;
        }

        // ======== COLORS ======
        public static readonly ElementColor MpPink = new("MP Pink", 120, 0, 60, 90, 75, 75);
        public static readonly ElementColor HealthGreen = new("Health Green", 20, 75, 0, 80, 80, 40);
        public static readonly ElementColor Purple = new("Purple", 50, 0, 150, 80, 75, 80);
        public static readonly ElementColor Red = new("Red", 150, 0, 0, 80, 80, 80);
        public static readonly ElementColor LightBlue = new("Light Blue", 0, 100, 140, 80, 100, 100);
        public static readonly ElementColor Orange = new("Orange", 120, 50, 65506, 100, 100, 100);
        public static readonly ElementColor PurplePink = new("Purple-Pink", 80, 65476, 50, 100, 100, 100);
        public static readonly ElementColor BlueGreen = new("Blue-Green", 65456, 50, 90, 80, 80, 40);
        public static readonly ElementColor BrightGreen = new("Bright Green", 65486, 100, 0, 90, 100, 100);
        public static readonly ElementColor Yellow = new("Yellow", 130, 100, 65516, 100, 100, 100);
        public static readonly ElementColor White = new("White", 150, 140, 140, 100, 100, 100);
        public static readonly ElementColor DarkBlue = new("Dark Blue", 65516, 65516, 120, 100, 100, 100);
        public static readonly ElementColor NoColor = new("No Color", 0, 0, 0, 100, 100, 100);

        public static Dictionary<string, ElementColor> AllColors { get; private set; } = new();

        public static void SetupColors() {
            AllColors = new();
            AllColors.Add(MpPink.Name, MpPink);
            AllColors.Add(HealthGreen.Name, HealthGreen);
            AllColors.Add(Purple.Name, Purple);
            AllColors.Add(Red.Name, Red);
            AllColors.Add(LightBlue.Name, LightBlue);
            AllColors.Add(Orange.Name, Orange);
            AllColors.Add(PurplePink.Name, PurplePink);
            AllColors.Add(BlueGreen.Name, BlueGreen);
            AllColors.Add(BrightGreen.Name, BrightGreen);
            AllColors.Add(Yellow.Name, Yellow);
            AllColors.Add(White.Name, White);
            AllColors.Add(DarkBlue.Name, DarkBlue);
        }
    }
}
