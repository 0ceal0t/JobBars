using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.GameStructs;
using JobBars.Helper;
using System;
using System.Collections.Generic;

namespace JobBars.UI {
    public struct UIIconProps {
        public bool IsTimer;
        public bool UseCombo;
        public bool IsGCD; // only matters with Timer
        public bool UseBorder;
    }

    public unsafe class UIIconManager {
        private class Icon {
            private enum IconState {
                None,
                TimerRunning,
                TimerDone,
                BuffRunning
            }

            public readonly uint AdjustedId;
            public readonly uint SlotId;
            public readonly int HotbarIdx;
            public readonly int SlotIdx;
            public AtkComponentNode* Component;

            private AtkResNode* OriginalOverlay;
            private AtkImageNode* OriginalImage;

            private AtkImageNode* Image;
            private AtkImageNode* Border;
            private AtkImageNode* Circle;
            private AtkImageNode* Ring;
            private AtkTextNode* Text;
            private AtkTextNode* BigText;
            private IconState State = IconState.None;

            private readonly bool IsTimer;
            private readonly bool UseCombo;
            private readonly bool IsGCD;
            private readonly bool UseBorder;

            public Icon(uint adjustedId, uint slotId, int hotbarIdx, int slotIdx, AtkComponentNode* component, UIIconProps props) {
                AdjustedId = adjustedId;
                SlotId = slotId;
                HotbarIdx = hotbarIdx;
                SlotIdx = slotIdx;
                Component = component;

                var nodeList = Component->Component->UldManager.NodeList;
                OriginalOverlay = nodeList[1];

                IsTimer = props.IsTimer;
                UseCombo = props.UseCombo;
                IsGCD = props.IsGCD;
                UseBorder = props.UseBorder;

                OriginalImage = (AtkImageNode*)nodeList[0];
                var originalBorder = (AtkImageNode*)nodeList[4];
                var originalCD = (AtkImageNode*)nodeList[5];
                var originalCircle = (AtkImageNode*)nodeList[7];

                uint nodeIdx = 200;

                Image = UIHelper.CleanAlloc<AtkImageNode>();
                Image->Ctor();
                Image->AtkResNode.NodeID = nodeIdx++;
                Image->AtkResNode.Type = NodeType.Image;
                Image->AtkResNode.X = 2;
                Image->AtkResNode.Y = 3;
                Image->AtkResNode.Width = 40;
                Image->AtkResNode.Height = 40;
                Image->AtkResNode.Flags = 8243;
                Image->AtkResNode.Flags_2 = 1;
                Image->AtkResNode.Flags_2 |= 4;
                Image->WrapMode = 1;
                Image->PartId = 0;
                Image->PartsList = OriginalImage->PartsList;

                Border = UIHelper.CleanAlloc<AtkImageNode>();
                Border->Ctor();
                Border->AtkResNode.NodeID = nodeIdx++;
                Border->AtkResNode.Type = NodeType.Image;
                Border->AtkResNode.X = -2;
                Border->AtkResNode.Width = 48;
                Border->AtkResNode.Height = 48;
                Border->AtkResNode.Flags = 8243;
                Border->AtkResNode.Flags_2 = 1;
                Border->AtkResNode.Flags_2 |= 4;
                Border->WrapMode = 1;
                Border->PartId = 0;
                Border->PartsList = originalBorder->PartsList;

                Circle = UIHelper.CleanAlloc<AtkImageNode>(); // for timer
                Circle->Ctor();
                Circle->AtkResNode.NodeID = nodeIdx++;
                Circle->AtkResNode.Type = NodeType.Image;
                Circle->AtkResNode.X = 0;
                Circle->AtkResNode.Y = 2;
                Circle->AtkResNode.Width = 44;
                Circle->AtkResNode.Height = 46;
                Circle->AtkResNode.Flags = 8243;
                Circle->AtkResNode.Flags_2 = 1;
                Circle->AtkResNode.Flags_2 |= 4;
                Circle->WrapMode = 1;
                Circle->PartId = 0;
                Circle->PartsList = originalCD->PartsList;

                Ring = UIHelper.CleanAlloc<AtkImageNode>(); // for timer
                Ring->Ctor();
                Ring->AtkResNode.NodeID = nodeIdx++;
                Ring->AtkResNode.Type = NodeType.Image;
                Ring->AtkResNode.X = 0;
                Ring->AtkResNode.Y = 2;
                Ring->AtkResNode.Width = 44;
                Ring->AtkResNode.Height = 46;
                Ring->AtkResNode.Flags = 8243;
                Ring->AtkResNode.Flags_2 = 1;
                Ring->AtkResNode.Flags_2 |= 4;
                Ring->WrapMode = 1;
                Ring->PartId = 0;
                Ring->PartsList = originalCircle->PartsList;

                Text = UIHelper.CleanAlloc<AtkTextNode>();
                Text->Ctor();
                Text->AtkResNode.NodeID = nodeIdx++;
                Text->AtkResNode.Type = NodeType.Text;
                Text->AtkResNode.X = 1;
                Text->AtkResNode.Y = 37;
                Text->AtkResNode.Width = 48;
                Text->AtkResNode.Height = 12;
                Text->AtkResNode.Flags = 8243;
                Text->AtkResNode.Flags_2 = 1;
                Text->AtkResNode.Flags_2 |= 4;
                Text->LineSpacing = 12;
                Text->AlignmentFontType = 3;
                Text->FontSize = 12;
                Text->TextFlags = 16;
                Text->TextColor = new ByteColor { R = 255, G = 255, B = 255, A = 255 };
                Text->EdgeColor = new ByteColor { R = 51, G = 51, B = 51, A = 255 };
                Text->SetText("");

                BigText = UIHelper.CleanAlloc<AtkTextNode>();
                BigText->Ctor();
                BigText->AtkResNode.NodeID = nodeIdx++;
                BigText->AtkResNode.Type = NodeType.Text;
                BigText->AtkResNode.X = 2;
                BigText->AtkResNode.Y = 3;
                BigText->AtkResNode.Width = 40;
                BigText->AtkResNode.Height = 40;
                BigText->AtkResNode.Flags = 8243;
                BigText->AtkResNode.Flags_2 = 1;
                BigText->AtkResNode.Flags_2 |= 4;
                BigText->LineSpacing = 40;
                BigText->AlignmentFontType = 20;
                BigText->FontSize = 16;
                BigText->TextFlags = 16;
                BigText->TextColor = new ByteColor { R = 255, G = 255, B = 255, A = 255 };
                BigText->EdgeColor = new ByteColor { R = 51, G = 51, B = 51, A = 255 };
                BigText->SetText("");

                var macroIcon = nodeList[15];
                var rootNode = (AtkResNode*)Component;

                Border->AtkResNode.ParentNode = rootNode;
                Circle->AtkResNode.ParentNode = rootNode;
                Ring->AtkResNode.ParentNode = rootNode;
                Text->AtkResNode.ParentNode = rootNode;

                UIHelper.Link(OriginalOverlay, (AtkResNode*)Image);
                UIHelper.Link((AtkResNode*)Image, (AtkResNode*)Circle);
                UIHelper.Link((AtkResNode*)Circle, (AtkResNode*)Ring);
                UIHelper.Link((AtkResNode*)Ring, (AtkResNode*)Border);
                UIHelper.Link((AtkResNode*)Border, (AtkResNode*)BigText);
                UIHelper.Link((AtkResNode*)BigText, (AtkResNode*)Text);
                UIHelper.Link((AtkResNode*)Text, macroIcon);

                Component->Component->UldManager.UpdateDrawNodeList();

                if (IsTimer) UIHelper.Hide(OriginalOverlay);
                if (!IsTimer) UIHelper.Hide(Image);

                UIHelper.Hide(Circle);
                UIHelper.Hide(Ring);
                UIHelper.Hide(Text);
                UIHelper.Hide(BigText);

                SetDimmed(false);
            }

            // ============================

            public void SetProgress(float current, float max) {
                if (IsTimer) SetTimerProgress(current, max);
                else SetBuffProgress(current);
            }

            public void SetDone() {
                if (IsTimer) SetTimerDone();
                else SetBuffDone();
            }

            // ======== FOR DoT TIMERS ========

            private void SetTimerProgress(float current, float max) {
                if (State == IconState.TimerDone && current <= 0) return;
                State = IconState.TimerRunning;

                UIHelper.Show(Text);
                Text->SetText(((int)Math.Round(current)).ToString());

                if (!UseCombo) Border->PartId = 0;

                UIHelper.Show(IsGCD ? Ring : Circle);
                (IsGCD ? Ring : Circle)->PartId = (ushort)(80 - (float)(current / max) * 80);
                if(IsGCD) SetDimmed(true);
            }

            private void SetTimerDone() {
                State = IconState.TimerDone;
                UIHelper.Hide(Text);

                UIHelper.Hide(IsGCD ? Ring : Circle);
                if (IsGCD) SetDimmed(false);
            }

            // ====== FOR BUFFS =============

            private void SetBuffProgress(float current) {
                if (State != IconState.BuffRunning) {
                    State = IconState.BuffRunning;
                    UIHelper.Hide(OriginalOverlay);
                    UIHelper.Show(BigText);
                }
                BigText->SetText(((int)Math.Round(current)).ToString());
            }

            private void SetBuffDone() {
                if (State == IconState.None) return;
                State = IconState.None;
                if (!UseCombo) Border->PartId = 0;

                UIHelper.Hide(BigText);
                UIHelper.Show(OriginalOverlay);
            }

            // =====================

            private void SetDimmed(bool dimmed) {
                var val = (byte)(dimmed || OriginalImage->AtkResNode.MultiplyRed == 50 ? 50 : 100);
                Image->AtkResNode.MultiplyRed = val;
                Image->AtkResNode.MultiplyRed_2 = val;
                Image->AtkResNode.MultiplyGreen = val;
                Image->AtkResNode.MultiplyGreen_2 = val;
                Image->AtkResNode.MultiplyBlue = val;
                Image->AtkResNode.MultiplyBlue_2 = val;
            }

            // =====================

            public void Tick(float dashPercent, float gcdPercent, bool border) {
                var useBorder = UseCombo ?
                    border :
                    UseBorder && (State == IconState.TimerDone || State == IconState.BuffRunning);

                Border->PartId = !useBorder ? (ushort)0 : (ushort)(6 + dashPercent * 7);

                if(IsTimer && IsGCD) {
                    Circle->PartId = (ushort)(gcdPercent * 80);
                    UIHelper.SetVisibility(Circle, gcdPercent > 0);
                }
            }

            public void Dispose() {
                var rootNode = (AtkResNode*)Component;

                UIHelper.Link(OriginalOverlay, Text->AtkResNode.PrevSiblingNode);
                Component->Component->UldManager.UpdateDrawNodeList();

                UIHelper.Show(OriginalOverlay);
                SetDimmed(false);

                if (Image != null) {
                    Image->AtkResNode.Destroy(true);
                    Image = null;
                }

                if (Border != null) {
                    Border->AtkResNode.Destroy(true);
                    Border = null;
                }

                if (Circle != null) {
                    Circle->AtkResNode.Destroy(true);
                    Circle = null;
                }

                if (Ring != null) {
                    Ring->AtkResNode.Destroy(true);
                    Ring = null;
                }

                if (Text != null) {
                    Text->AtkResNode.Destroy(true);
                    Text = null;
                }

                if (BigText != null) {
                    BigText->AtkResNode.Destroy(true);
                    BigText = null;
                }

                Component = null;
                OriginalOverlay = null;
                OriginalImage = null;
            }
        }

        // ==================
        private readonly string[] AllActionBars = {
            "_ActionBar",
            "_ActionBar01",
            "_ActionBar02",
            "_ActionBar03",
            "_ActionBar04",
            "_ActionBar05",
            "_ActionBar06",
            "_ActionBar07",
            "_ActionBar08",
            "_ActionBar09",
            "_ActionCross",
            //"_ActionDoubleCrossL",
            //"_ActionDoubleCrossR",
        };
        private static readonly int MILLIS_LOOP = 250;

        private readonly List<Icon> Icons = new();

        public UIIconManager() {
        }

        public void Setup(List<uint> triggers, UIIconProps props) {
            if (triggers == null) return;

            var hotbarData = UIHelper.GetHotbarUI();
            if (hotbarData == null) return;

            for (var hotbarIndex = 0; hotbarIndex < AllActionBars.Length; hotbarIndex++) {
                if (!hotbarData->IsLoaded(hotbarIndex)) continue;
                var hotbar = hotbarData->Hotbars[hotbarIndex];

                var actionBar = (AddonActionBarBase*)AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonByName(AllActionBars[hotbarIndex]);
                if (actionBar == null || actionBar->ActionBarSlotsAction == null) continue;

                for (var slotIndex = 0; slotIndex < actionBar->HotbarSlotCount; slotIndex++) {
                    var slot = actionBar->ActionBarSlotsAction[slotIndex];

                    var slotData = hotbar[slotIndex];
                    if (slotData.Type != HotbarSlotStructType.Action) continue;

                    var icon = slot.Icon;
                    var action = UIHelper.GetAdjustedAction(slotData.ActionId);
                    if (triggers.Contains(action) && !AlreadySetup(icon)) {
                        Icons.Add(new Icon(action, slotData.ActionId, hotbarIndex, slotIndex, icon, props));
                    }
                }
            }
        }

        private bool AlreadySetup(AtkComponentNode* node) {
            foreach (var icon in Icons) {
                if (icon.Component == node) return true;
            }
            return false;
        }

        public void Remove(List<uint> triggers) {
            if (triggers == null) return;
            List<Icon> toRemove = new();
            foreach(var icon in Icons) {
                if(triggers.Contains(icon.AdjustedId)) {
                    icon.Dispose();
                    toRemove.Add(icon);
                }
            }
            Icons.RemoveAll(x => toRemove.Contains(x));
        }

        public void SetProgress(List<uint> triggers, UIIconProps props, float current, float max, bool check = true) {
            var found = false;
            foreach (var icon in Icons) {
                if (!triggers.Contains(icon.AdjustedId)) continue;
                icon.SetProgress(current, max);
                found = true;
            }

            if (found || !check) return;
            Setup(triggers, props);
            SetProgress(triggers, props, current, max, false);
        }

        public void SetDone(List<uint> triggers, UIIconProps props, bool check = true) {
            var found = false;
            foreach (var icon in Icons) {
                if (!triggers.Contains(icon.AdjustedId)) continue;
                icon.SetDone();
                found = true;
            }

            if (found || !check) return;
            Setup(triggers, props);
            SetDone(triggers, props, false);
        }

        // ========================

        public void Tick() {
            var time = DateTime.Now;
            int millis = time.Second * 1000 + time.Millisecond;
            float percent = (float)(millis % MILLIS_LOOP) / MILLIS_LOOP;

            var gcdPercent = UIHelper.GetGCD();

            var hotbarData = UIHelper.GetHotbarUI();
            if (hotbarData == null) return;

            List<Icon> toRemove = new();
            foreach(var icon in Icons) {
                var slot = hotbarData->Hotbars[icon.HotbarIdx][icon.SlotIdx];
                if (slot.ActionId != icon.SlotId) { // Icon has been moved
                    toRemove.Add(icon);
                    icon.Dispose();
                }
                else {
                    icon.Tick(percent, gcdPercent, slot.YellowBorder);
                }
            }

            Icons.RemoveAll(x => toRemove.Contains(x));
        }

        public void Reset() {
            Icons.ForEach(x => x.Dispose());
            Icons.Clear();
        }

        public void Dispose() {
            Reset();
        }
    }
}
