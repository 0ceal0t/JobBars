using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;

namespace JobBars.UI {
    public struct ElementColor {
        public string Name;

        public short AddRed;
        public short AddGreen;
        public short AddBlue;

        public byte MultiplyRed;
        public byte MultiplyGreen;
        public byte MultiplyBlue;

        public ElementColor(string name, short addRed, short addGreen, short addBlue, byte multRed, byte multGreen, byte multBlue) {
            Name = name;
            AddRed = addRed;
            AddGreen = addGreen;
            AddBlue = addBlue;
            MultiplyRed = multRed;
            MultiplyGreen = multGreen;
            MultiplyBlue = multBlue;
        }
    }

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

        public unsafe static void SetColor(AtkTextNode* node, ElementColor color) => SetColor((AtkResNode*)node, color);
        public unsafe static void SetColor(AtkNineGridNode* node, ElementColor color) => SetColor((AtkResNode*)node, color);
        public unsafe static void SetColor(AtkImageNode* node, ElementColor color) => SetColor((AtkResNode*)node, color);
        public unsafe static void SetColor(AtkResNode* node, ElementColor color) => SetColor(node, color.AddRed, color.AddGreen, color.AddBlue, color.MultiplyRed, color.MultiplyGreen, color.MultiplyBlue);
        public unsafe static void SetColor(AtkResNode* node, short addRed, short addGreen, short addBlue, byte multRed, byte multGreen, byte multBlue) {
            node->AddRed = (ushort)addRed;
            node->AddGreen = (ushort)addGreen;
            node->AddBlue = (ushort)addBlue;
            node->AddRed_2 = (ushort)addRed;
            node->AddGreen_2 = (ushort)addGreen;
            node->AddBlue_2 = (ushort)addBlue;

            node->MultiplyRed = multRed;
            node->MultiplyGreen = multGreen;
            node->MultiplyBlue = multBlue;
            node->MultiplyRed_2 = multRed;
            node->MultiplyGreen_2 = multGreen;
            node->MultiplyBlue_2 = multBlue;
        }
        public unsafe static void SetColorPulse(AtkResNode* node, ElementColor color, float percent) {
            // 0 = color
            // 50 = color + 100
            // 100 = color

            var add = (short)(75 * (1f - 2f * Math.Abs(percent - 0.5f))); // 0 -> 1 -> 0
            var currentRed = (short)(color.AddRed + add);
            var currentGreen = (short)(color.AddGreen + add);
            var currentBlue = (short)(color.AddBlue + add);

            SetColor(node, currentRed, currentGreen, currentBlue, color.MultiplyRed, color.MultiplyGreen, color.MultiplyBlue);
        }

        // ======== COLORS ======
        public static readonly ElementColor MpPink = new("MP Pink", 120, 0, 60, 90, 75, 75);
        public static readonly ElementColor HealthGreen = new("Health Green", 20, 75, 0, 80, 80, 40);
        public static readonly ElementColor Purple = new("Purple", 50, 0, 150, 80, 75, 80);
        public static readonly ElementColor Red = new("Red", 150, 0, 0, 80, 80, 80);
        public static readonly ElementColor LightBlue = new("Light Blue", 0, 100, 140, 80, 100, 100);
        public static readonly ElementColor Orange = new("Orange", 120, 50, -29, 100, 100, 100);
        public static readonly ElementColor PurplePink = new("Purple-Pink", 80, -59, 50, 100, 100, 100);
        public static readonly ElementColor BlueGreen = new("Blue-Green", -79, 50, 90, 80, 80, 40);
        public static readonly ElementColor BrightGreen = new("Bright Green", -49, 100, 0, 90, 100, 100);
        public static readonly ElementColor Yellow = new("Yellow", 130, 100, -19, 100, 100, 100);
        public static readonly ElementColor White = new("White", 150, 140, 140, 100, 100, 100);
        public static readonly ElementColor DarkBlue = new("Dark Blue", -19, -19, 120, 100, 100, 100);
        public static readonly ElementColor NoColor = new("No Color", 0, 0, 0, 100, 100, 100);

        public static Dictionary<string, ElementColor> AllColors { get; private set; } = new();

        public static void SetupColors() {
            AllColors = new();
            AllColors.Add(NoColor.Name, NoColor);
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

        public static ElementColor GetColor(string colorName, ElementColor defaultColor) {
            if (string.IsNullOrEmpty(colorName)) return defaultColor;
            return AllColors.TryGetValue(colorName, out var newColor) ? newColor : defaultColor;
        }
    }
}
