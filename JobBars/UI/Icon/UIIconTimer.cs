using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System;

namespace JobBars.UI {
    public unsafe class UIIconTimer : UIIcon {
        private AtkImageNode* OriginalImage;

        private bool Dimmed = false;

        private AtkResNode* OriginalRecastContainer;
        private AtkResNode* OriginalPreCombo;
        private AtkResNode* OriginalComboContainer;
        private AtkImageNode* OriginalCombo;

        private AtkImageNode* Ring;
        private AtkTextNode* Text;
        private AtkImageNode* Combo;

        public UIIconTimer(uint adjustedId, uint slotId, int hotbarIdx, int slotIdx, AtkComponentNode* component, UIIconProps props) :
            base(adjustedId, slotId, hotbarIdx, slotIdx, component, props) {

            var nodeList = Component->Component->UldManager.NodeList;

            OriginalRecastContainer = nodeList[3];
            OriginalImage = (AtkImageNode*)nodeList[0];
            var originalRing = (AtkImageNode*)nodeList[7];

            OriginalPreCombo = IconComponent->Frame->PrevSiblingNode;
            OriginalComboContainer = IconComponent->ComboBorder;
            OriginalCombo = (AtkImageNode*)OriginalComboContainer->ChildNode;

            Combo = UIHelper.CleanAlloc<AtkImageNode>();
            Combo->Ctor();
            Combo->AtkResNode.NodeID = NodeIdx++;
            Combo->AtkResNode.Type = NodeType.Image;
            Combo->AtkResNode.X = 0;
            Combo->AtkResNode.Width = 48;
            Combo->AtkResNode.Height = 48;
            Combo->AtkResNode.Flags = 8243;
            Combo->AtkResNode.Flags_2 = 1;
            Combo->AtkResNode.Flags_2 |= 4;
            Combo->WrapMode = 1;
            Combo->PartId = 0;
            Combo->PartsList = OriginalCombo->PartsList;
            Combo->PartId = 0;

            Combo->AtkResNode.ParentNode = OriginalComboContainer->ParentNode;

            OriginalPreCombo->PrevSiblingNode = (AtkResNode*)Combo;
            Combo->AtkResNode.NextSiblingNode = OriginalPreCombo;

            Combo->AtkResNode.PrevSiblingNode = OriginalComboContainer->PrevSiblingNode;
            OriginalPreCombo->PrevSiblingNode->NextSiblingNode = (AtkResNode*)Combo;

            UIHelper.Show(Combo);

            Text = (AtkTextNode*)IconComponent->UnknownImageNode;
            IconComponent->UnknownImageNode = null;
            Text->TextColor = new ByteColor { R = 255, G = 255, B = 255, A = 255 };
            Text->EdgeColor = new ByteColor { R = 51, G = 51, B = 51, A = 255 };
            Text->AtkResNode.Alpha_2 = 0xFF;
            Text->SetText("");

            Ring = UIHelper.CleanAlloc<AtkImageNode>(); // for timer
            Ring->Ctor();
            Ring->AtkResNode.NodeID = NodeIdx++;
            Ring->AtkResNode.Type = NodeType.Image;
            Ring->AtkResNode.X = 2;
            Ring->AtkResNode.Y = 2;
            Ring->AtkResNode.Width = 44;
            Ring->AtkResNode.Height = 46;
            Ring->AtkResNode.Flags = 8243;
            Ring->AtkResNode.Flags_2 = 1;
            Ring->AtkResNode.Flags_2 |= 4;
            Ring->WrapMode = 1;
            Ring->PartId = 0;
            Ring->PartsList = originalRing->PartsList;

            Ring->AtkResNode.ParentNode = OriginalRecastContainer;
            OriginalRecastContainer->ChildNode->PrevSiblingNode->PrevSiblingNode = (AtkResNode*)Ring;

            Component->Component->UldManager.UpdateDrawNodeList();

            UIHelper.Hide(Text);
            UIHelper.Hide(Ring);
        }

        public override void SetProgress(float current, float max) {
            if (State == IconState.TimerDone && current <= 0) return;
            State = IconState.TimerRunning;

            UIHelper.Show(Text);
            Text->SetText(((int)Math.Round(current)).ToString());

            UIHelper.Show(Ring);
            Ring->PartId = (ushort)(80 - (float)(current / max) * 80);

            JobBars.IconBuilder.AddIconOverride(new IntPtr(OriginalImage));
            SetDimmed(true);
        }

        public override void SetDone() {
            State = IconState.TimerDone;
            UIHelper.Hide(Text);

            UIHelper.Hide(Ring);

            JobBars.IconBuilder.RemoveIconOverride(new IntPtr(OriginalImage));
            SetDimmed(false);
        }

        private void SetDimmed(bool dimmed) {
            Dimmed = dimmed;
            SetDimmed(OriginalImage, dimmed);
        }

        public static void SetDimmed(AtkImageNode* image, bool dimmed) {
            var val = (byte)(dimmed ? 50 : 100);
            image->AtkResNode.MultiplyRed = val;
            image->AtkResNode.MultiplyRed_2 = val;
            image->AtkResNode.MultiplyGreen = val;
            image->AtkResNode.MultiplyGreen_2 = val;
            image->AtkResNode.MultiplyBlue = val;
            image->AtkResNode.MultiplyBlue_2 = val;
        }

        public override void Tick(float dashPercent, bool border) {
            var showBorder = CalcShowBorder(State == IconState.TimerDone, border);
            Combo->PartId = !showBorder ? (ushort)0 : (ushort)(6 + dashPercent * 7);
        }

        public override void OnDispose() {
            OriginalRecastContainer->ChildNode->PrevSiblingNode->PrevSiblingNode = null;

            OriginalPreCombo->PrevSiblingNode = OriginalComboContainer;
            Combo->AtkResNode.PrevSiblingNode->NextSiblingNode = OriginalComboContainer;

            Component->Component->UldManager.UpdateDrawNodeList();

            IconComponent->UnknownImageNode = (AtkImageNode*)Text;
            Text = null;

            if (Combo != null) {
                Combo->AtkResNode.Destroy(true);
                Combo = null;
            }

            if (Ring != null) {
                Ring->AtkResNode.Destroy(true);
                Ring = null;
            }

            JobBars.IconBuilder.RemoveIconOverride(new IntPtr(OriginalImage));
            if (Dimmed) SetDimmed(false);

            OriginalPreCombo = null;
            OriginalComboContainer = null;
            OriginalCombo = null;
            OriginalRecastContainer = null;
            OriginalImage = null;
        }
    }
}
