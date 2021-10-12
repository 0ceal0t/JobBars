using Dalamud.Logging;
using JobBars.Helper;
using System.Collections.Generic;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        public static readonly ushort GAUGE_BG_PART = 0;
        public static readonly ushort GAUGE_FRAME_PART = 1;
        public static readonly ushort GAUGE_SEPARATOR = 2;
        public static readonly ushort GAUGE_BAR_MAIN = 3;
        public static readonly ushort GAUGE_TEXT_BLUR_PART = 4;
        public static readonly ushort ARROW_BG = 5;
        public static readonly ushort ARROW_FG = 6;
        public static readonly ushort DIAMOND_BG = 7;
        public static readonly ushort DIAMOND_FG = 8;
        public static readonly ushort BUFF_BORDER = 9;
        public static readonly ushort BUFF_OVERLAY = 10;

        public static readonly ushort CD_BORDER = 0;
        public static readonly ushort CD_DASH_START = 1;

        public Asset_PartList GaugeBuffAssets;
        public Asset_PartList CooldownAssets;
        public Asset_PartList CursorAssets;

        private void InitTextures() {
            PluginLog.Log("LOADING TEXTURES");

            var gaugeBuffLayout = new Dictionary<string, PartStruct[]>();
            gaugeBuffLayout.Add("ui/uld/Parameter_Gauge.tex", new[] {
                new PartStruct(0, 100, 160, 20), // GAUGE_BG_PART
                new PartStruct(0, 0, 160, 20),   // GAUGE_FRAME_PART
                new PartStruct(10, 3, 10, 5),    // GAUGE_SEPARATOR
                new PartStruct(6, 40, 148, 20),  // GAUGE_BAR_MAIN
            });

            gaugeBuffLayout.Add("ui/uld/JobHudNumBg.tex", new[] {
                new PartStruct(0, 0, 60, 40), // GAUGE_TEXT_BLUR_PART
            });

            gaugeBuffLayout.Add("ui/uld/JobHudSimple_StackB.tex", new[] {
                new PartStruct(0, 0, 32, 32),  // ARROW_BG
                new PartStruct(32, 0, 32, 32), // ARROW_FG
            });

            gaugeBuffLayout.Add("ui/uld/JobHudSimple_StackA.tex", new[] {
                new PartStruct(0, 0, 32, 32),  // DIAMOND_BG
                new PartStruct(32, 0, 32, 32), // DIAMOND_FG
            });

            gaugeBuffLayout.Add("ui/uld/IconA_Frame.tex", new[] {
                new PartStruct(252, 12, 47, 47),  // BUFF_BORDER
                new PartStruct(365, 4, 37, 37),   // BUFF_OVERLAY
            });

            GaugeBuffAssets = UIHelper.LoadLayout(gaugeBuffLayout);

            // ===================

            var cdLayout = new Dictionary<string, PartStruct[]>();
            cdLayout.Add("ui/uld/IconA_Frame.tex", new[] {
                new PartStruct(0, 96, 48, 48),  // CD_BORDER

                new PartStruct(96, 0, 48, 48),  // CD_DASH_START
                new PartStruct(144, 0, 48, 48),
                new PartStruct(192, 0, 48, 48),

                new PartStruct(96, 48, 48, 48),
                new PartStruct(144, 48, 48, 48),
                new PartStruct(192, 48, 48, 48),

                new PartStruct(96, 96, 48, 48),
            });

            CooldownAssets = UIHelper.LoadLayout(cdLayout);

            // ==================

            List<PartStruct> cursorParts = new();
            var cursorLayout = new Dictionary<string, PartStruct[]>();
            for (int idx = 0; idx < 80; idx++) {
                var row = idx % 9;
                var column = (idx - row) / 9;

                cursorParts.Add(new PartStruct((ushort)(44 * row), (ushort)(48 * column), 44, 46));
            }
            cursorLayout.Add("ui/uld/IconA_Recast2.tex", cursorParts.ToArray());

            cursorLayout.Add("ui/uld/CursorLocation.tex", new[] {
                new PartStruct(0, 0, 128, 128)
            });

            CursorAssets = UIHelper.LoadLayout(cursorLayout);
        }

        private void DisposeTextures() {
            UIHelper.DisposeLayout(GaugeBuffAssets);
            GaugeBuffAssets = new();

            UIHelper.DisposeLayout(CooldownAssets);
            CooldownAssets = new();

            UIHelper.DisposeLayout(CursorAssets);
            CursorAssets = new();
        }
    }
}
