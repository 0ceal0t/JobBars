using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;

namespace JobBars.UI {
    public unsafe class UIBar : UIGaugeElement {
        private static readonly int MAX_SEGMENTS = 4;

        private AtkResNode* GaugeContainer;
        private AtkImageNode* Background;
        private AtkResNode* BarContainer;
        private AtkNineGridNode* BarMainNode;
        private AtkNineGridNode* BarSecondaryNode;
        private AtkImageNode* Frame;

        private AtkResNode* TextContainer;
        private AtkTextNode* TextNode;
        private AtkNineGridNode* TextBlurNode;

        private AtkImageNode*[] Separators;

        private string CurrentText;
        private float LastPercent = 1;
        private Animation Anim = null;
        private int Segments = 1;

        public UIBar(AtkUldPartsList* partsList) {

            // ======= CONTAINERS =========
            RootRes = UIBuilder.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = 160;
            RootRes->Height = 46;

            GaugeContainer = UIBuilder.CreateResNode();
            GaugeContainer->X = 0;
            GaugeContainer->Y = 0;
            GaugeContainer->Width = 160;
            GaugeContainer->Height = 32;

            Background = UIBuilder.CreateImageNode();
            Background->AtkResNode.Width = 160;
            Background->AtkResNode.Height = 20;
            Background->AtkResNode.X = 0;
            Background->AtkResNode.Y = 0;
            Background->PartId = UIBuilder.GAUGE_BG_PART;
            Background->PartsList = partsList;

            Background->Flags = 0;
            Background->WrapMode = 1;

            // ========= BAR ============
            BarContainer = UIBuilder.CreateResNode();
            BarContainer->X = 0;
            BarContainer->Y = 0;
            BarContainer->Width = 160;
            BarContainer->Height = 20;

            BarMainNode = UIBuilder.CreateNineNode();
            BarMainNode->AtkResNode.Width = 148;
            BarMainNode->AtkResNode.Height = 20;
            BarMainNode->AtkResNode.X = 6;
            BarMainNode->AtkResNode.Y = 0;
            BarMainNode->PartID = UIBuilder.GAUGE_BAR_MAIN;
            BarMainNode->PartsList = partsList;
            BarMainNode->TopOffset = 0;
            BarMainNode->BottomOffset = 0;
            BarMainNode->RightOffset = 0;
            BarMainNode->LeftOffset = 0;

            BarSecondaryNode = UIBuilder.CreateNineNode();
            BarSecondaryNode->AtkResNode.Width = 0;
            BarSecondaryNode->AtkResNode.Height = 20;
            BarSecondaryNode->AtkResNode.X = 6;
            BarSecondaryNode->AtkResNode.Y = 0;
            BarSecondaryNode->PartID = UIBuilder.GAUGE_BAR_MAIN;
            BarSecondaryNode->PartsList = partsList;
            BarSecondaryNode->TopOffset = 0;
            BarSecondaryNode->BottomOffset = 0;
            BarSecondaryNode->RightOffset = 0;
            BarSecondaryNode->LeftOffset = 0;
            UIColor.SetColor(BarSecondaryNode, UIColor.NoColor);

            Separators = new AtkImageNode*[MAX_SEGMENTS - 1];
            for (int i = 0; i < MAX_SEGMENTS - 1; i++) {
                Separators[i] = UIBuilder.CreateImageNode();
                Separators[i]->AtkResNode.Width = 10;
                Separators[i]->AtkResNode.Height = 5;
                Separators[i]->AtkResNode.Rotation = (float)(Math.PI / 2f);
                Separators[i]->AtkResNode.X = 0;
                Separators[i]->AtkResNode.Y = 5;
                Separators[i]->PartId = UIBuilder.GAUGE_SEPARATOR;
                Separators[i]->PartsList = partsList;
                Separators[i]->Flags = 0;
                Separators[i]->WrapMode = 1;
            }

            Frame = UIBuilder.CreateImageNode();
            Frame->AtkResNode.Width = 160;
            Frame->AtkResNode.Height = 20;
            Frame->AtkResNode.X = 0;
            Frame->AtkResNode.Y = 0;
            Frame->PartId = UIBuilder.GAUGE_FRAME_PART;
            Frame->PartsList = partsList;
            Frame->Flags = 0;
            Frame->WrapMode = 1;

            // ======== TEXT ==========
            TextContainer = UIBuilder.CreateResNode();
            TextContainer->X = 112;
            TextContainer->Y = 6;
            TextContainer->Width = 47;
            TextContainer->Height = 40;

            TextNode = UIBuilder.CreateTextNode();
            TextNode->AtkResNode.X = 14;
            TextNode->AtkResNode.Y = 5;
            TextNode->AtkResNode.Flags |= 0x10;
            TextNode->AtkResNode.Flags_2 = 1;

            TextBlurNode = UIBuilder.CreateNineNode();
            TextBlurNode->AtkResNode.Flags = 8371;
            TextBlurNode->AtkResNode.Width = 47;
            TextBlurNode->AtkResNode.Height = 40;
            TextBlurNode->AtkResNode.X = 0;
            TextBlurNode->AtkResNode.Y = 0;
            TextBlurNode->AtkResNode.OriginX = 0;
            TextBlurNode->AtkResNode.OriginY = 0;
            TextBlurNode->PartID = UIBuilder.GAUGE_TEXT_BLUR_PART;
            TextBlurNode->PartsList = partsList;
            TextBlurNode->TopOffset = 0;
            TextBlurNode->BottomOffset = 0;
            TextBlurNode->RightOffset = 28;
            TextBlurNode->LeftOffset = 28;

            List<LayoutNode> barNodes = new();
            barNodes.Add(new LayoutNode(BarSecondaryNode));
            barNodes.Add(new LayoutNode(BarMainNode));
            foreach(var sep in Separators) {
                barNodes.Add(new LayoutNode(sep));
            }

            var layout = new LayoutNode(RootRes, new[] {
                new LayoutNode(GaugeContainer, new[] {
                    new LayoutNode(Background),
                    new LayoutNode(BarContainer,
                        barNodes.ToArray()
                    ),
                    new LayoutNode(Frame)
                }),
                new LayoutNode(TextContainer, new[] {
                    new LayoutNode(TextNode),
                    new LayoutNode(TextBlurNode)
                })
            });;
            layout.Setup();
            layout.Cleanup();

            SetSegments(1); // Default 1 big segment
        }

        public override void Dispose() {
            if (GaugeContainer != null) {
                GaugeContainer->Destroy(true);
                GaugeContainer = null;
            }

            if (Background != null) {
                Background->UnloadTexture();
                Background->AtkResNode.Destroy(true);
                Background = null;
            }

            if (BarContainer != null) {
                BarContainer->Destroy(true);
                BarContainer = null;
            }

            if (BarMainNode != null) {
                // TODO
                BarMainNode->AtkResNode.Destroy(true);
                BarMainNode = null;
            }

            if (BarSecondaryNode != null) {
                // TODO
                BarSecondaryNode->AtkResNode.Destroy(true);
                BarSecondaryNode = null;
            }

            for (int i = 0; i < Separators.Length; i++) {
                Separators[i]->UnloadTexture();
                Separators[i]->AtkResNode.Destroy(true);
                Separators[i] = null;
            }

            if (Frame != null) {
                Frame->UnloadTexture();
                Frame->AtkResNode.Destroy(true);
                Frame = null;
            }

            if (TextContainer != null) {
                TextContainer->Destroy(true);
                TextContainer = null;
            }

            if (TextNode != null) {
                TextNode->AtkResNode.Destroy(true);
                TextNode = null;
            }

            if (TextBlurNode != null) {
                // TODO
                TextBlurNode->AtkResNode.Destroy(true);
                TextBlurNode = null;
            }

            if (RootRes != null) {
                RootRes->Destroy(true);
                RootRes = null;
            }
        }

        public void SetSegments(int segments) {
            segments = Math.Clamp(segments, 1, MAX_SEGMENTS);
            Segments = segments;

            var diff = 148 / segments;
            UIHelper.SetVisibility(BarSecondaryNode, segments > 1);

            for (int i = 0; i < segments - 1; i++) {
                UIHelper.Show(Separators[i]);
                Separators[i]->AtkResNode.X = 8 + (i + 1) * diff;
            }
            for(int i = segments - 1; i < MAX_SEGMENTS - 1; i++) {
                UIHelper.Hide(Separators[i]);
            }
        }

        public void SetText(string text) {
            if (text != CurrentText) {
                TextNode->SetText(text);
                CurrentText = text;
            }

            int size = text.Length * 17;
            UIHelper.SetPosition(TextContainer, 129 - size, 6);
            UIHelper.SetSize(TextContainer, 30 + size, 40);

            UIHelper.SetPosition(TextBlurNode, 0, 0);
            UIHelper.SetSize(TextBlurNode, 30 + size, 40);

            UIHelper.SetPosition(TextNode, 14, 5);
            UIHelper.SetSize(TextNode, size, 30);
        }

        public void SetTextColor(ElementColor color) {
            UIColor.SetColor(TextNode, color);
        }

        public void SetTextVisible(bool visible) => UIHelper.SetVisibility(TextContainer, visible);

        public void SetPercent(float value) {
            if (value > 1) value = 1;
            else if (value < 0) value = 0;

            var difference = Math.Abs(value - LastPercent);
            if (difference == 0) return;


            Anim?.Delete();
            if (difference > 0.1f) {
                Anim = Animation.AddAnim(f => SetPercentInternal(f), 0.2f, LastPercent, value);
            }
            else {
                SetPercentInternal(value);
            }
            LastPercent = value;
        }

        public void SetPercentInternal(float value) {
            if (Segments == 1) {
                UIHelper.SetSize(BarMainNode, (int)(148 * value), 20);
            }
            else {
                var segmentValue = 1f / Segments;

                var partialValue = value % segmentValue;
                var fullValue = value - partialValue;

                var fullWidth = (int)(148 * fullValue);
                var partialWidth = (int)(148 * value);

                UIHelper.SetSize(BarMainNode, fullWidth, 20);
                UIHelper.SetSize(BarSecondaryNode, partialWidth, 20);
                //UIHelper.SetPosition((AtkResNode*)BarSecondaryNode, 6 + fullWidth, 0);
            }
        }

        public override void SetColor(ElementColor color) {
            UIColor.SetColor(BarMainNode, color);
        }

        public override int GetHeight(int param) {
            return 46;
        }

        public override int GetWidth(int param) {
            return 160;
        }

        public override int GetHorizontalYOffset() {
            return 0;
        }
    }
}
