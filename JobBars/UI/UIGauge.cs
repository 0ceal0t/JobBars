using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars.UI {
    public unsafe class UIGauge : UIElement {
        private AtkResNode* GaugeContainer;
        private AtkImageNode* Background;
        private AtkResNode* BarContainer;
        private AtkNineGridNode* BarMainNode;
        private AtkImageNode* Frame;
        private AtkResNode* TextContainer;
        private AtkTextNode* TextNode;
        private AtkNineGridNode* TextBlurNode;

        private string CurrentText;
        private float LastPercent = 1;
        private Animation Anim = null;

        public UIGauge(UIBuilder ui) : base(ui) {
            var addon = UI.ADDON;

            // ======= CONTAINERS =========
            RootRes = UI.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = 160;
            RootRes->Height = 46;

            GaugeContainer = UI.CreateResNode();
            GaugeContainer->X = 0;
            GaugeContainer->Y = 0;
            GaugeContainer->Width = 160;
            GaugeContainer->Height = 32;

            Background = UI.CreateImageNode();
            Background->AtkResNode.Width = 160;
            Background->AtkResNode.Height = 20;
            Background->AtkResNode.X = 0;
            Background->AtkResNode.Y = 0;
            Background->PartId = UIBuilder.GAUGE_BG_PART;
            Background->PartsList = addon->UldManager.PartsList;
            Background->Flags = 0;
            Background->WrapMode = 1;

            // ========= BAR ============
            BarContainer = UI.CreateResNode();
            BarContainer->X = 0;
            BarContainer->Y = 0;
            BarContainer->Width = 160;
            BarContainer->Height = 20;

            BarMainNode = UI.CreateNineNode();
            BarMainNode->AtkResNode.Width = 160;
            BarMainNode->AtkResNode.Height = 20;
            BarMainNode->AtkResNode.X = 0;
            BarMainNode->AtkResNode.Y = 0;
            BarMainNode->PartID = UIBuilder.GAUGE_BAR_MAIN;
            BarMainNode->PartsList = addon->UldManager.PartsList;
            BarMainNode->TopOffset = 0;
            BarMainNode->BottomOffset = 0;
            BarMainNode->RightOffset = 7;
            BarMainNode->LeftOffset = 7;

            // ======= BAR SETUP =========
            BarContainer->ChildCount = 1;
            BarContainer->ChildNode = (AtkResNode*)BarMainNode;
            BarMainNode->AtkResNode.ParentNode = BarContainer;

            Frame = UI.CreateImageNode();
            Frame->AtkResNode.Width = 160;
            Frame->AtkResNode.Height = 20;
            Frame->AtkResNode.X = 0;
            Frame->AtkResNode.Y = 0;
            Frame->PartId = UIBuilder.GAUGE_FRAME_PART;
            Frame->PartsList = addon->UldManager.PartsList;
            Frame->Flags = 0;
            Frame->WrapMode = 1;

            // ======== GAUGE CONTAINER SETUP ========
            Background->AtkResNode.ParentNode = GaugeContainer;
            BarContainer->ParentNode = GaugeContainer;
            Frame->AtkResNode.ParentNode = GaugeContainer;

            GaugeContainer->ChildCount = (ushort)(3 + BarContainer->ChildCount);
            GaugeContainer->ChildNode = (AtkResNode*)Background;

            UiHelper.Link((AtkResNode*)Background, BarContainer);
            UiHelper.Link(BarContainer, (AtkResNode*)Frame);

            // ======== TEXT ==========
            TextContainer = UI.CreateResNode();
            TextContainer->X = 112;
            TextContainer->Y = 6;
            TextContainer->Width = 47;
            TextContainer->Height = 40;

            TextNode = UI.CreateTextNode();
            TextNode->AtkResNode.X = 14;
            TextNode->AtkResNode.Y = 5;
            TextNode->AtkResNode.Flags |= 0x10;
            TextNode->AtkResNode.Flags_2 = 1;

            TextBlurNode = UI.CreateNineNode();
            TextBlurNode->AtkResNode.Flags = 8371;
            TextBlurNode->AtkResNode.Width = 47;
            TextBlurNode->AtkResNode.Height = 40;
            TextBlurNode->AtkResNode.X = 0;
            TextBlurNode->AtkResNode.Y = 0;
            TextBlurNode->AtkResNode.OriginX = 0;
            TextBlurNode->AtkResNode.OriginY = 0;
            TextBlurNode->PartID = UIBuilder.GAUGE_TEXT_BLUR_PART;
            TextBlurNode->PartsList = addon->UldManager.PartsList;
            TextBlurNode->TopOffset = 0;
            TextBlurNode->BottomOffset = 0;
            TextBlurNode->RightOffset = 28;
            TextBlurNode->LeftOffset = 28;

            // ====== TEXT SETUP =========
            TextContainer->ChildCount = 2;
            TextContainer->ChildNode = (AtkResNode*)TextNode;
            TextNode->AtkResNode.ParentNode = TextContainer;
            TextBlurNode->AtkResNode.ParentNode = TextContainer;

            UiHelper.Link((AtkResNode*)TextNode, (AtkResNode*)TextBlurNode);

            // ====== CONTAINER SETUP ======
            UiHelper.Link(GaugeContainer, TextContainer);

            // ====== ROOT SETUP =====
            RootRes->ChildNode = GaugeContainer;
            GaugeContainer->ParentNode = RootRes;
            TextContainer->ParentNode = RootRes;
            RootRes->ChildCount = (ushort)(GaugeContainer->ChildCount + TextContainer->ChildCount + 2);
        }

        public override void Dispose() {
            if (GaugeContainer != null) {
                GaugeContainer->Destroy(true);
                GaugeContainer = null;
            }

            if (Background != null) {
                Background->AtkResNode.Destroy(true);
                Background = null;
            }

            if (BarContainer != null) {
                BarContainer->Destroy(true);
                BarContainer = null;
            }

            if (BarMainNode != null) {
                BarMainNode->AtkResNode.Destroy(true);
                BarMainNode = null;
            }

            if (Frame != null) {
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
                TextBlurNode->AtkResNode.Destroy(true);
                TextBlurNode = null;
            }

            if (RootRes != null) {
                RootRes->Destroy(true);
                RootRes = null;
            }
        }

        public void SetText(string text) {
            if (text != CurrentText) {
                TextNode->SetText(text);
                CurrentText = text;
            }

            int size = text.Length * 17;
            UiHelper.SetPosition(TextContainer, 129 - size, 6);
            UiHelper.SetSize(TextContainer, 30 + size, 40);

            UiHelper.SetPosition(TextBlurNode, 0, 0);
            UiHelper.SetSize(TextBlurNode, 30 + size, 40);

            UiHelper.SetPosition(TextNode, 14, 5);
            UiHelper.SetSize(TextNode, size, 30);
        }

        public void SetTextColor(ElementColor color) {
            UIColor.SetColor(TextNode, color);
        }

        public void SetPercent(float value) {
            if (value > 1) {
                value = 1;
            }
            else if (value < 0) {
                value = 0;
            }

            var difference = Math.Abs(value - LastPercent);
            if (difference == 0) return;


            var item = (AtkResNode*)BarMainNode;
            Anim?.Delete();
            if (difference > 0.1f) {
                Anim = Animation.AddAnim(f => UiHelper.SetSize(item, (int)(160 * f), 20), 0.2f, LastPercent, value);
            }
            else {
                UiHelper.SetSize(item, (int)(160 * value), 20);
            }
            LastPercent = value;
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
