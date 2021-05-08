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
            public byte AddRed;
            public byte AddGreen;
            public byte AddBlue;
            public byte AddRed_2;
            public byte AddGreen_2;
            public byte AddBlue_2;

            public byte MultiplyRed;
            public byte MultiplyGreen;
            public byte MultiplyBlue;
            public byte MultiplyRed_2;
            public byte MultiplyGreen_2;
            public byte MultiplyBlue_2;
        }

        public unsafe static void SetColor(AtkResNode* node, ElementColor color) {
            node->AddRed = color.AddRed;
            node->AddGreen = color.AddGreen;
            node->AddBlue = color.AddBlue;
            node->AddRed_2 = color.AddRed_2;
            node->AddGreen_2 = color.AddGreen_2;
            node->AddBlue_2 = color.AddBlue_2;

            node->MultiplyRed = color.MultiplyRed;
            node->MultiplyGreen = color.MultiplyGreen;
            node->MultiplyBlue = color.MultiplyBlue;
            node->MultiplyRed_2 = color.MultiplyRed_2;
            node->MultiplyGreen_2 = color.MultiplyGreen_2;
            node->MultiplyBlue_2 = color.MultiplyBlue_2;
        }

        // ======== COLORS ======
        public static ElementColor Pink = new ElementColor
        {
            AddRed = 120,
            AddGreen = 0,
            AddBlue = 60,
            AddRed_2 =  120,
            AddGreen_2 = 0,
            AddBlue_2 = 60,

            MultiplyRed = 90,
            MultiplyGreen = 75,
            MultiplyBlue = 75,
            MultiplyRed_2 = 90,
            MultiplyGreen_2 = 75,
            MultiplyBlue_2 = 75
        };

        public static ElementColor LightGreen = new ElementColor
        {
            AddRed = 20,
            AddGreen = 75,
            AddBlue = 0,
            AddRed_2 = 20,
            AddGreen_2 = 75,
            AddBlue_2 = 0,

            MultiplyRed = 80,
            MultiplyGreen = 80,
            MultiplyBlue = 40,
            MultiplyRed_2 = 80,
            MultiplyGreen_2 = 80,
            MultiplyBlue_2 = 40
        };

        public static ElementColor Purple = new ElementColor
        {
            AddRed = 50,
            AddGreen = 0,
            AddBlue = 150,
            AddRed_2 = 50,
            AddGreen_2 = 0,
            AddBlue_2 = 150,

            MultiplyRed = 80,
            MultiplyGreen = 75,
            MultiplyBlue = 80,
            MultiplyRed_2 = 80,
            MultiplyGreen_2 = 75,
            MultiplyBlue_2 = 80
        };

        public static ElementColor Red = new ElementColor
        {
            AddRed = 50,
            AddGreen = 0,
            AddBlue = 150,
            AddRed_2 = 50,
            AddGreen_2 = 0,
            AddBlue_2 = 150,

            MultiplyRed = 80,
            MultiplyGreen = 75,
            MultiplyBlue = 80,
            MultiplyRed_2 = 80,
            MultiplyGreen_2 = 75,
            MultiplyBlue_2 = 80
        };

        public static ElementColor LightBlue = new ElementColor
        {
            AddRed = 0,
            AddGreen = 100,
            AddBlue = 140,
            AddRed_2 = 0,
            AddGreen_2 = 100,
            AddBlue_2 = 140,

            MultiplyRed = 80,
            MultiplyGreen = 100,
            MultiplyBlue = 100,
            MultiplyRed_2 = 80,
            MultiplyGreen_2 = 100,
            MultiplyBlue_2 = 100
        };
    }
}
