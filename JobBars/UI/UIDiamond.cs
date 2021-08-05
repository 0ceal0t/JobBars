using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;
using System.Collections.Generic;
using static JobBars.UI.UIColor;

namespace JobBars.UI {
    public unsafe class UIDiamond : UIElement {

        private class TickStruct {
            public AtkResNode* MainTick;
            public AtkImageNode* Background;
            public AtkResNode* SelectedContainer;
            public AtkImageNode* Selected;
            public AtkTextNode* Text;

            public void Dispose() {
                if(Text != null) {
                    Text->AtkResNode.Destroy(true);
                    Text = null;
                }

                if (Selected != null) {
                    Selected->AtkResNode.Destroy(true);
                    Selected = null;
                }

                if (SelectedContainer != null) {
                    SelectedContainer->Destroy(true);
                    SelectedContainer = null;
                }

                if (Background != null) {
                    Background->AtkResNode.Destroy(true);
                    Background = null;
                }

                if (MainTick != null) {
                    MainTick->Destroy(true);
                    MainTick = null;
                }
            }
        }

        private static readonly int MAX = 12;
        private List<TickStruct> Ticks;
        private bool TextVisible = false;

        public UIDiamond(UIBuilder ui) : base(ui) {
            Ticks = new();

            var addon = UI.ADDON;

            RootRes = UI.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = 160;
            RootRes->Height = 46;

            TickStruct lastTick = null;

            for (int idx = 0; idx < MAX; idx++) {
                // ======= TICKS =========
                var tick = UI.CreateResNode();
                tick->X = 20 * idx;
                tick->Y = 0;
                tick->Width = 32;
                tick->Height = 32;

                var bg = UI.CreateImageNode();
                bg->AtkResNode.Width = 32;
                bg->AtkResNode.Height = 32;
                bg->AtkResNode.X = 0;
                bg->AtkResNode.Y = 0;
                bg->PartId = UIBuilder.DIAMOND_BG;
                bg->PartsList = addon->UldManager.PartsList;
                bg->Flags = 0;
                bg->WrapMode = 1;

                // ======== SELECTED ========
                var selectedContainer = UI.CreateResNode();
                selectedContainer->X = 0;
                selectedContainer->Y = 0;
                selectedContainer->Width = 32;
                selectedContainer->Height = 32;
                selectedContainer->OriginX = 16;
                selectedContainer->OriginY = 16;

                var text = UI.CreateTextNode();
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

                var selected = UI.CreateImageNode();
                selected->AtkResNode.Width = 32;
                selected->AtkResNode.Height = 32;
                selected->AtkResNode.X = 0;
                selected->AtkResNode.Y = 0;
                selected->AtkResNode.OriginX = 16;
                selected->AtkResNode.OriginY = 16;
                selected->PartId = UIBuilder.DIAMOND_FG;
                selected->PartsList = addon->UldManager.PartsList;
                selected->Flags = 0;
                selected->WrapMode = 1;

                // ======== SELECTED SETUP ========
                selectedContainer->ChildCount = 2;
                selectedContainer->ChildNode = (AtkResNode*)selected;

                UiHelper.Link((AtkResNode*)selected, (AtkResNode*)text);

                selected->AtkResNode.ParentNode = selectedContainer;
                text->AtkResNode.ParentNode = selectedContainer;

                // ======= SETUP TICKS =====
                tick->ChildCount = 4;
                tick->ChildNode = (AtkResNode*)bg;
                tick->ParentNode = RootRes;

                UiHelper.Link((AtkResNode*)bg, selectedContainer);

                bg->AtkResNode.ParentNode = tick;
                selectedContainer->ParentNode = tick;

                var newTick = new TickStruct {
                    MainTick = tick,
                    Background = bg,
                    Selected = selected,
                    SelectedContainer = selectedContainer,
                    Text = text
                };
                Ticks.Add(newTick);

                if (lastTick != null) UiHelper.Link(lastTick.MainTick, newTick.MainTick);
                lastTick = newTick;
            }

            // ====== SETUP ROOT =======
            RootRes->ChildNode = Ticks[0].MainTick;
            RootRes->ChildCount = (ushort)(5 * MAX);
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

        public override void SetColor(ElementColor color) {
            SetColor(color, 0, MAX);
        }

        public void SetColor(ElementColor color, int start, int count) {
            for (int idx = start; idx < (start + count); idx++) {
                SetColor(color, idx);
            }
        }

        public void SetColor(ElementColor color, int idx) {
            UIColor.SetColor(Ticks[idx].Selected, color);
        }

        public void SetMaxValue(int value, bool showText = false) {
            TextVisible = showText;
            SetSpacing(showText ? 5 : 0);

            for (int idx = 0; idx < MAX; idx++) {
                UiHelper.SetVisibility(Ticks[idx].MainTick, idx < value);
                if (idx < value) UiHelper.SetVisibility(Ticks[idx].Text, showText);
            }
        }

        private void SetSpacing(int space) {
            for (int idx = 0; idx < MAX; idx++) {
                Ticks[idx].MainTick->X = (20 + space) * idx;
            }
        }

        public void SetValue(int value) {
            SetValue(value, 0, MAX);
        }

        public void SetValue(int value, int start, int count) {
            for (int idx = start; idx < (start + count); idx++) {
                UiHelper.SetVisibility(Ticks[idx].SelectedContainer, idx < (value + start));
            }
        }

        public void SelectPart(int idx) {
            UiHelper.Show(Ticks[idx].SelectedContainer);
        }

        public void UnselectPart(int idx) {
            UiHelper.Hide(Ticks[idx].SelectedContainer);
        }

        public void SetText(int idx, string text) {
            Ticks[idx].Text->SetText(text);
        }

        public void ShowText(int idx) {
            UiHelper.Show(Ticks[idx].Text);
        }

        public void HideText(int idx) {
            UiHelper.Hide(Ticks[idx].Text);
        }

        public override int GetHeight(int param) {
            return TextVisible ? 40 : 32;
        }

        public override int GetWidth(int param) {
            return 32 + 20 * (param - 1);
        }

        public override int GetHorizontalYOffset() {
            return -3;
        }
    }
}
