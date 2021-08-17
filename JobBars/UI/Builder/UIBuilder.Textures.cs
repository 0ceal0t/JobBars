using Dalamud.Logging;
using JobBars.Helper;
using System.Collections.Generic;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        public static readonly ushort GAUGE_BG_PART = 0;
        public static readonly ushort GAUGE_FRAME_PART = 1;
        public static readonly ushort GAUGE_BAR_MAIN = 2;
        public static readonly ushort GAUGE_TEXT_BLUR_PART = 3;
        public static readonly ushort ARROW_BG = 4;
        public static readonly ushort ARROW_FG = 5;
        public static readonly ushort DIAMOND_BG = 6;
        public static readonly ushort DIAMOND_FG = 7;
        public static readonly ushort BUFF_BORDER = 8;
        public static readonly ushort BUFF_OVERLAY = 9;

        public Asset_PartList GaugeBuffAssets;

        private void InitTextures() {
            PluginLog.Log("LOADING TEXTURES");

            var layout = new Dictionary<string, PartStruct[]>();
            layout.Add("ui/uld/Parameter_Gauge.tex", new[] {
                new PartStruct(0, 100, 160, 20), // GAUGE_BG_PART
                new PartStruct(0, 0, 160, 20),   // GAUGE_FRAME_PART
                new PartStruct(0, 40, 160, 20),  // GAUGE_BAR_MAIN
            });

            layout.Add("ui/uld/JobHudNumBg.tex", new[] {
                new PartStruct(0, 0, 60, 40), // GAUGE_TEXT_BLUR_PART
            });

            layout.Add("ui/uld/JobHudSimple_StackB.tex", new[] {
                new PartStruct(0, 0, 32, 32),  // ARROW_BG
                new PartStruct(32, 0, 32, 32), // ARROW_FG
            });

            layout.Add("ui/uld/JobHudSimple_StackA.tex", new[] {
                new PartStruct(0, 0, 32, 32),  // DIAMOND_BG
                new PartStruct(32, 0, 32, 32), // DIAMOND_FG
            });

            layout.Add("ui/uld/IconA_Frame.tex", new[] {
                new PartStruct(252, 12, 47, 47),  // BUFF_BORDER
                new PartStruct(365, 4, 37, 37),   // BUFF_OVERLAY
            });

            GaugeBuffAssets = UIHelper.LoadLayout(layout);
        }

        private void DisposeTextures() {
            UIHelper.DisposeLayout(GaugeBuffAssets);
            GaugeBuffAssets = new();
        }
    }
}
