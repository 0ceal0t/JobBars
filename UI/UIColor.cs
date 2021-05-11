using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.UI {
    public class UIColor {
        public static ByteColor BYTE_White = new ByteColor
        {
            R = 255,
            G = 255,
            B = 255,
            A = 255
        };
        public static ByteColor BYTE_Transparent = new ByteColor
        {
            R = 0,
            G = 0,
            B = 0,
            A = 0
        };

        public struct ElementColor {
            public ushort AddRed;
            public ushort AddGreen;
            public ushort AddBlue;

            public byte MultiplyRed;
            public byte MultiplyGreen;
            public byte MultiplyBlue;
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
        public static ElementColor MpPink = new ElementColor
        {
            AddRed = 120,
            AddGreen = 0,
            AddBlue = 60,
            MultiplyRed = 90,
            MultiplyGreen = 75,
            MultiplyBlue = 75
        };
        public static ElementColor HealthGreen = new ElementColor
        {
            AddRed = 20,
            AddGreen = 75,
            AddBlue = 0,
            MultiplyRed = 80,
            MultiplyGreen = 80,
            MultiplyBlue = 40
        };
        public static ElementColor Purple = new ElementColor
        {
            AddRed = 50,
            AddGreen = 0,
            AddBlue = 150,
            MultiplyRed = 80,
            MultiplyGreen = 75,
            MultiplyBlue = 80
        };
        public static ElementColor Red = new ElementColor
        {
            AddRed = 150,
            AddGreen = 0,
            AddBlue = 0,
            MultiplyRed = 80,
            MultiplyGreen = 80,
            MultiplyBlue = 80
        };
        public static ElementColor LightBlue = new ElementColor
        {
            AddRed = 0,
            AddGreen = 100,
            AddBlue = 140,
            MultiplyRed = 80,
            MultiplyGreen = 100,
            MultiplyBlue = 100
        };
        public static ElementColor Orange = new ElementColor
        {
            AddRed = 120,
            AddGreen = 50,
            AddBlue = 65506,
            MultiplyRed = 100,
            MultiplyGreen = 100,
            MultiplyBlue = 100
        };
        public static ElementColor PurplePink = new ElementColor
        {
            AddRed = 80,
            AddGreen = 65476,
            AddBlue = 50,
            MultiplyRed = 100,
            MultiplyGreen = 100,
            MultiplyBlue = 100
        };
        public static ElementColor BlueGreen = new ElementColor
        {
            AddRed = 65456,
            AddGreen = 50,
            AddBlue = 90,
            MultiplyRed = 80,
            MultiplyGreen = 80,
            MultiplyBlue = 40
        };
        public static ElementColor BrightGreen = new ElementColor
        {
            AddRed = 65486,
            AddGreen = 100,
            AddBlue = 0,
            MultiplyRed = 90,
            MultiplyGreen = 100,
            MultiplyBlue = 100
        };
        public static ElementColor Yellow = new ElementColor
        {
            AddRed = 130,
            AddGreen = 100,
            AddBlue = 65516,
            MultiplyRed = 100,
            MultiplyGreen = 100,
            MultiplyBlue = 100
        };
        public static ElementColor White = new ElementColor
        {
            AddRed = 150,
            AddGreen = 140,
            AddBlue = 140,
            MultiplyRed = 100,
            MultiplyGreen = 100,
            MultiplyBlue = 100
        };
        public static ElementColor DarkBlue = new ElementColor
        {
            AddRed = 65496,
            AddGreen = 65516,
            AddBlue = 120,
            MultiplyRed = 100,
            MultiplyGreen = 100,
            MultiplyBlue = 100
        };
    }
}
