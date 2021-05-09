using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JobBars.UI.UIColor;

namespace JobBars.UI {
    public unsafe class UIGauge : UIElement {
        AtkTextNode* TextNode;
        AtkNineGridNode* TextBlurNode;
        AtkNineGridNode* BarMainNode;
        string CurrentText = "";

        public UIGauge(UIBuilder _ui, AtkResNode* node = null) : base(_ui, 46, node) {
        }

        public override void Pickup(AtkResNode* node) {
            RootRes = node;
            TextNode = (AtkTextNode*)RootRes->ChildNode->PrevSiblingNode->ChildNode;
            TextBlurNode = (AtkNineGridNode*)RootRes->ChildNode->PrevSiblingNode->ChildNode->PrevSiblingNode;
            BarMainNode = (AtkNineGridNode*)RootRes->ChildNode->ChildNode->PrevSiblingNode->ChildNode;
        }

        public override void Init() {
            var nameplateAddon = _UI._ADDON;

            // ======== TEXT ==========
            TextBlurNode = _UI.CreateNineNode();
            TextBlurNode->AtkResNode.Flags = 8371;
            TextBlurNode->AtkResNode.Width = 47;
            TextBlurNode->AtkResNode.Height = 40;
            TextBlurNode->AtkResNode.X = 0;
            TextBlurNode->AtkResNode.Y = 0;
            TextBlurNode->PartID = UIBuilder.GAUGE_TEXT_BLUR_PART;
            TextBlurNode->PartsList = nameplateAddon->UldManager.PartsList;
            TextBlurNode->TopOffset = 0;
            TextBlurNode->BottomOffset = 0;
            TextBlurNode->RightOffset = 28;
            TextBlurNode->LeftOffset = 28;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)TextBlurNode;

            TextNode = _UI.CreateTextNode();
            TextNode->AtkResNode.X = 14;
            TextNode->AtkResNode.Y = 5;
            TextNode->AtkResNode.Flags |= 0x10;
            TextNode->AtkResNode.Flags_2 = 1;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)TextNode;

            var textContainer = _UI.CreateResNode();
            textContainer->X = 112;
            textContainer->Y = 6;
            textContainer->Width = 47;
            textContainer->Height = 40;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = textContainer;
            //
            textContainer->ChildCount = 2;
            textContainer->ChildNode = (AtkResNode*)TextNode;
            TextNode->AtkResNode.ParentNode = textContainer;
            TextBlurNode->AtkResNode.ParentNode = textContainer;
            TextNode->AtkResNode.PrevSiblingNode = (AtkResNode*)TextBlurNode;
            TextBlurNode->AtkResNode.NextSiblingNode = (AtkResNode*)TextNode;

            // ========= BAR ============
            var bg = _UI.CreateImageNode();
            bg->AtkResNode.Width = 160;
            bg->AtkResNode.Height = 20;
            bg->AtkResNode.X = 0;
            bg->AtkResNode.Y = 0;
            bg->PartId = UIBuilder.GAUGE_BG_PART;
            bg->PartsList = nameplateAddon->UldManager.PartsList;
            bg->Flags = 0;
            bg->WrapMode = 1;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)bg;

            BarMainNode = _UI.CreateNineNode();
            BarMainNode->AtkResNode.Width = 160;
            BarMainNode->AtkResNode.Height = 20;
            BarMainNode->AtkResNode.X = 0;
            BarMainNode->AtkResNode.Y = 0;
            BarMainNode->PartID = UIBuilder.GAUGE_BAR_MAIN;
            BarMainNode->PartsList = nameplateAddon->UldManager.PartsList;
            BarMainNode->TopOffset = 0;
            BarMainNode->BottomOffset = 0;
            BarMainNode->RightOffset = 7;
            BarMainNode->LeftOffset = 7;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)BarMainNode;

            var barContainer = _UI.CreateResNode();
            barContainer->X = 0;
            barContainer->Y = 0;
            barContainer->Width = 160;
            barContainer->Height = 20;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = barContainer;
            //
            barContainer->ChildCount = 1;
            barContainer->ChildNode = (AtkResNode*)BarMainNode;
            BarMainNode->AtkResNode.ParentNode = barContainer;

            var frame = _UI.CreateImageNode();
            frame->AtkResNode.Width = 160;
            frame->AtkResNode.Height = 20;
            frame->AtkResNode.X = 0;
            frame->AtkResNode.Y = 0;
            frame->PartId = UIBuilder.GAUGE_FRAME_PART;
            frame->PartsList = nameplateAddon->UldManager.PartsList;
            frame->Flags = 0;
            frame->WrapMode = 1;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = (AtkResNode*)frame;

            var gaugeContainer = _UI.CreateResNode();
            gaugeContainer->X = 0;
            gaugeContainer->Y = 0;
            gaugeContainer->Width = 160;
            gaugeContainer->Height = 32;
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = gaugeContainer;
            //
            gaugeContainer->ChildCount = (ushort)(3 + barContainer->ChildCount);
            gaugeContainer->ChildNode = (AtkResNode*)bg;
            bg->AtkResNode.ParentNode = gaugeContainer;
            barContainer->ParentNode = gaugeContainer;
            frame->AtkResNode.ParentNode = gaugeContainer;
            bg->AtkResNode.PrevSiblingNode = barContainer;
            barContainer->PrevSiblingNode = (AtkResNode*)frame;
            barContainer->NextSiblingNode = (AtkResNode*)bg;
            frame->AtkResNode.NextSiblingNode = barContainer;
            //
            gaugeContainer->PrevSiblingNode = textContainer;
            textContainer->NextSiblingNode = gaugeContainer;

            // ======= CONTAINERS =========
            RootRes = _UI.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = 160;
            RootRes->Height = 46;
            RootRes->ChildCount = (ushort)(gaugeContainer->ChildCount + textContainer->ChildCount + 2);
            nameplateAddon->UldManager.NodeList[nameplateAddon->UldManager.NodeListCount++] = RootRes;
            //
            RootRes->ChildNode = gaugeContainer;
            gaugeContainer->ParentNode = RootRes;
            textContainer->ParentNode = RootRes;
        }

        public void SetText(string text) {
            if(text != CurrentText) {
                UiHelper.SetText(TextNode, text);
                CurrentText = text;
            }
            UiHelper.SetSize((AtkResNode*)TextBlurNode, 30 + 16 * text.Length, 40);
            UiHelper.SetPosition((AtkResNode*)TextBlurNode, -16 * (text.Length - 1), 0);
        }

        public void SetPercent(float value) {
            UiHelper.SetSize((AtkResNode*)BarMainNode, (int)(160 * value), 20);
        }

        public override void SetColor(ElementColor color) {
            UIColor.SetColor((AtkResNode*)BarMainNode, color);
        }
    }
}
