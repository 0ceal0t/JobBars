using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;
using System.Collections.Generic;

namespace JobBars.UI {
    public unsafe class UIArrow : UIGaugeElement {

        private class TickStruct {
            public AtkResNode* MainTick;
            public AtkImageNode* Background;
            public AtkResNode* SelectedContainer;
            public AtkImageNode* Selected;

            public void Dispose() {
                if (Selected != null) {
                    Selected->UnloadTexture();
                    Selected->AtkResNode.Destroy(true);
                    Selected = null;
                }

                if (SelectedContainer != null) {
                    SelectedContainer->Destroy(true);
                    SelectedContainer = null;
                }

                if (Background != null) {
                    Background->UnloadTexture();
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
        private List<TickStruct> Ticks = new();
        private int LastValue = 0;

        public UIArrow(AtkUldPartsList* partsList) : base() {
            RootRes = UIBuilder.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = 160;
            RootRes->Height = 46;

            TickStruct lastTick = null;

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
                bg->PartId = UIBuilder.ARROW_BG;
                bg->PartsList = partsList;
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
                selected->PartId = UIBuilder.ARROW_FG;
                selected->PartsList = partsList;
                selected->Flags = 0;
                selected->WrapMode = 1;

                // ======== SELECTED SETUP ========
                selectedContainer->ChildCount = 1;
                selectedContainer->ChildNode = (AtkResNode*)selected;
                selected->AtkResNode.ParentNode = selectedContainer;

                // ======= SETUP TICKS =====
                tick->ChildCount = 3;
                tick->ChildNode = (AtkResNode*)bg;
                tick->ParentNode = RootRes;

                UIHelper.Link((AtkResNode*)bg, selectedContainer);

                bg->AtkResNode.ParentNode = tick;
                selectedContainer->ParentNode = tick;

                var newTick = new TickStruct {
                    MainTick = tick,
                    Background = bg,
                    Selected = selected,
                    SelectedContainer = selectedContainer
                };
                Ticks.Add(newTick);

                if (lastTick != null) UIHelper.Link(lastTick.MainTick, newTick.MainTick);
                lastTick = newTick;
            }

            // ====== SETUP ROOT =======
            RootRes->ChildNode = Ticks[0].MainTick;
            RootRes->ChildCount = (ushort)(4 * MAX);
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
            foreach (var tick in Ticks) {
                UIColor.SetColor(tick.Selected, color);
            }
        }

        public void SetMaxValue(int value) {
            for (int idx = 0; idx < MAX; idx++) {
                UIHelper.SetVisibility(Ticks[idx].MainTick, idx < value);
            }
        }

        public void SetValue(int value) {
            for (int idx = 0; idx < MAX; idx++) {
                UIHelper.SetVisibility(Ticks[idx].Selected, idx < value);

                if (idx < value && idx >= LastValue) { // newly added
                    var item = (AtkResNode*)Ticks[idx].Selected;
                    Animation.AddAnim((float f) => UIHelper.SetScale(item, f, f), 0.2f, 2.5f, 1.0f);
                }
            }
            LastValue = value;
        }

        public override int GetHeight(int param) {
            return 32;
        }

        public override int GetWidth(int param) {
            return 32 + 18 * (param - 1);
        }

        public override int GetHorizontalYOffset() {
            return -3;
        }
    }
}
