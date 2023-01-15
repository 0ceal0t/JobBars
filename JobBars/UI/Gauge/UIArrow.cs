using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;
using System.Collections.Generic;

namespace JobBars.UI {
    public unsafe class UIArrow : UIGauge {
        private class UIArrowTick {
            public AtkResNode* MainTick;
            public AtkImageNode* Background;
            public AtkResNode* SelectedContainer;
            public AtkImageNode* Selected;

            private ElementColor Color = UIColor.NoColor;

            public void Dispose() {
                if (Selected != null) {
                    UIHelper.UnloadTexture(Selected);
                    Selected->AtkResNode.Destroy(true);
                    Selected = null;
                }

                if (SelectedContainer != null) {
                    SelectedContainer->Destroy(true);
                    SelectedContainer = null;
                }

                if (Background != null) {
                    UIHelper.UnloadTexture(Background);
                    Background->AtkResNode.Destroy(true);
                    Background = null;
                }

                if (MainTick != null) {
                    MainTick->Destroy(true);
                    MainTick = null;
                }
            }

            public void SetColor(ElementColor color) {
                Color = color;
                UIColor.SetColor(Selected, color);
            }

            public void Tick(float percent) {
                UIColor.SetColorPulse((AtkResNode*)Selected, Color, percent);
            }
        }

        public static readonly int MAX = 12;
        private List<UIArrowTick> Ticks = new();

        public UIArrow() : base() {
            RootRes = UIBuilder.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = 160;
            RootRes->Height = 46;

            List<LayoutNode> tickNodes = new();

            for (int idx = 0; idx < MAX; idx++) {
                // ======= TICKS =========
                var tick = UIBuilder.CreateResNode();
                tick->X = 18 * idx;
                tick->Y = 0;
                tick->Width = 32;
                tick->Height = 32;

                var bg = UIBuilder.CreateImageNode();
                bg->AtkResNode.Width = 32;
                bg->AtkResNode.Height = 32;
                bg->AtkResNode.X = 0;
                bg->AtkResNode.Y = 0;
                UIHelper.SetupTexture(bg, "ui/uld/JobHudSimple_StackB.tex");
                UIHelper.UpdatePart(bg, 0, 0, 32, 32);
                bg->Flags = 0;
                bg->WrapMode = 1;

                // ======== SELECTED ========
                var selectedContainer = UIBuilder.CreateResNode();
                selectedContainer->X = 0;
                selectedContainer->Y = 0;
                selectedContainer->Width = 32;
                selectedContainer->Height = 32;
                selectedContainer->OriginX = 16;
                selectedContainer->OriginY = 16;

                var selected = UIBuilder.CreateImageNode();
                selected->AtkResNode.Width = 32;
                selected->AtkResNode.Height = 32;
                selected->AtkResNode.X = 0;
                selected->AtkResNode.Y = 0;
                selected->AtkResNode.OriginX = 16;
                selected->AtkResNode.OriginY = 16;
                UIHelper.SetupTexture(selected, "ui/uld/JobHudSimple_StackB.tex");
                UIHelper.UpdatePart(selected, 32, 0, 32, 32);
                selected->Flags = 0;
                selected->WrapMode = 1;

                Ticks.Add(new UIArrowTick {
                    MainTick = tick,
                    Background = bg,
                    Selected = selected,
                    SelectedContainer = selectedContainer
                });

                tickNodes.Add(new LayoutNode(tick, new[] {
                    new LayoutNode(bg),
                    new LayoutNode(selectedContainer, new[] {
                        new LayoutNode(selected)
                    })
                }));
            }

            var layout = new LayoutNode(RootRes, tickNodes.ToArray());
            layout.Setup();
            layout.Cleanup();
        }

        public override void Dispose() {
            for (int idx = 0; idx < MAX; idx++) {
                Ticks[idx].Dispose();
                Ticks[idx] = null;
            }
            Ticks = null;

            if (RootRes != null) {
                RootRes->Destroy(true);
                RootRes = null;
            }
        }

        public void SetMaxValue(int value) {
            for (int idx = 0; idx < MAX; idx++) {
                UIHelper.SetVisibility(Ticks[idx].MainTick, idx < value);
            }
        }

        public void SetColor(int idx, ElementColor color) {
            Ticks[idx].SetColor(color);
        }

        public void SetValue(int idx, bool value) {
            var prevVisible = Ticks[idx].Selected->AtkResNode.IsVisible;
            UIHelper.SetVisibility(Ticks[idx].Selected, value);

            if (value && !prevVisible) {
                var item = (AtkResNode*)Ticks[idx].Selected;
                Animation.AddAnim((float f) => UIHelper.SetScale(item, f, f), 0.2f, 2.5f, 1.0f);
            }
        }

        public void Clear() {
            for (int idx = 0; idx < MAX; idx++) SetValue(idx, false);
        }

        public void Tick(float percent) {
            Ticks.ForEach(t => t.Tick(percent));
        }
    }
}
