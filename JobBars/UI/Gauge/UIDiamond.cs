﻿using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System.Collections.Generic;

namespace JobBars.UI {
    public unsafe class UIDiamond : UIGauge {
        private class UIDiamondTick {
            public AtkResNode* MainTick;
            public AtkImageNode* Background;
            public AtkResNode* SelectedContainer;
            public AtkImageNode* Selected;
            public AtkTextNode* Text;

            private ElementColor Color = UIColor.NoColor;

            public void Dispose() {
                if (Text != null) {
                    Text->AtkResNode.Destroy(true);
                    Text = null;
                }

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
        private List<UIDiamondTick> Ticks = new();

        public UIDiamond() {
            RootRes = UIBuilder.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = 160;
            RootRes->Height = 46;

            List<LayoutNode> tickNodes = new();

            for (int idx = 0; idx < MAX; idx++) {
                // ======= TICKS =========
                var tick = UIBuilder.CreateResNode();
                tick->X = 20 * idx;
                tick->Y = 0;
                tick->Width = 32;
                tick->Height = 32;

                var bg = UIBuilder.CreateImageNode();
                bg->AtkResNode.Width = 32;
                bg->AtkResNode.Height = 32;
                bg->AtkResNode.X = 0;
                bg->AtkResNode.Y = 0;
                UIHelper.SetupTexture(bg, "ui/uld/JobHudSimple_StackA.tex");
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

                var text = UIBuilder.CreateTextNode();
                text->FontSize = 14;
                text->LineSpacing = 14;
                text->AlignmentFontType = 4;
                text->AtkResNode.Width = 32;
                text->AtkResNode.Height = 32;
                text->AtkResNode.Y = 20;
                text->AtkResNode.X = 0;
                text->AtkResNode.Flags |= 0x10;
                text->AtkResNode.Flags_2 = 1;
                text->EdgeColor = new FFXIVClientStructs.FFXIV.Client.Graphics.ByteColor {
                    R = 40,
                    G = 40,
                    B = 40,
                    A = 255
                };

                var selected = UIBuilder.CreateImageNode();
                selected->AtkResNode.Width = 32;
                selected->AtkResNode.Height = 32;
                selected->AtkResNode.X = 0;
                selected->AtkResNode.Y = 0;
                selected->AtkResNode.OriginX = 16;
                selected->AtkResNode.OriginY = 16;
                UIHelper.SetupTexture(selected, "ui/uld/JobHudSimple_StackA.tex");
                UIHelper.UpdatePart(selected, 32, 0, 32, 32);
                selected->Flags = 0;
                selected->WrapMode = 1;

                Ticks.Add(new UIDiamondTick {
                    MainTick = tick,
                    Background = bg,
                    Selected = selected,
                    SelectedContainer = selectedContainer,
                    Text = text
                });

                tickNodes.Add(new LayoutNode(tick, new[] {
                    new LayoutNode(bg),
                    new LayoutNode(selectedContainer, new[] {
                        new LayoutNode(selected),
                        new LayoutNode(text)
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

        public void SetColor(int idx, ElementColor color) {
            Ticks[idx].SetColor(color);
        }

        public void SetMaxValue(int value) {
            for (int idx = 0; idx < MAX; idx++) {
                UIHelper.SetVisibility(Ticks[idx].MainTick, idx < value);
            }
        }

        public void SetTextVisible(bool showText) {
            SetSpacing(showText ? 5 : 0);
            for (int idx = 0; idx < MAX; idx++) {
                UIHelper.SetVisibility(Ticks[idx].Text, showText);
            }
        }

        private void SetSpacing(int space) {
            for (int idx = 0; idx < MAX; idx++) {
                Ticks[idx].MainTick->X = (20 + space) * idx;
            }
        }

        public void SetValue(int idx, bool value) {
            UIHelper.SetVisibility(Ticks[idx].SelectedContainer, value);
        }

        public void SetText(int idx, string text) {
            Ticks[idx].Text->SetText(text);
        }

        public void ShowText(int idx) {
            UIHelper.Show(Ticks[idx].Text);
        }

        public void HideText(int idx) {
            UIHelper.Hide(Ticks[idx].Text);
        }

        public void Clear() {
            for (int idx = 0; idx < MAX; idx++) SetValue(idx, false);
        }

        public void Tick(float percent) {
            Ticks.ForEach(t => t.Tick(percent));
        }
    }
}
