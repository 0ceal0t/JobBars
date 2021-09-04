using Dalamud.Logging;
using FFXIVClientInterface;
using FFXIVClientInterface.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.GameStructs;
using JobBars.Helper;
using System;
using System.Collections.Generic;

namespace JobBars.UI {
    public unsafe class UIIconManager {
        private class Icon {
            private enum IconState {
                None,
                TimerRunning,
                TimerDone,
                BuffRunning
            }

            public readonly uint Id;
            public AtkComponentNode* Component;

            private AtkResNode* OriginalOverlay;
            private AtkImageNode* OriginalCombo;

            private AtkImageNode* Border;
            private AtkImageNode* CD;
            private AtkTextNode* Text;
            private AtkTextNode* BigText;
            private IconState State = IconState.None;

            private readonly bool DoTMode;
            private readonly bool UseCombo;

            public Icon(uint id, AtkComponentNode* component, bool dotMode, bool useCombo) {
                Id = id;
                Component = component;
                var nodeList = Component->Component->UldManager.NodeList;
                OriginalOverlay = nodeList[1];
                OriginalCombo = (AtkImageNode*)nodeList[9];

                DoTMode = dotMode;
                UseCombo = useCombo;

                var originalBorder = (AtkImageNode*)nodeList[4];
                var originalCD = (AtkImageNode*)nodeList[5];

                Border = UIHelper.CleanAlloc<AtkImageNode>();
                Border->Ctor();
                Border->AtkResNode.NodeID = 200;
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

                CD = UIHelper.CleanAlloc<AtkImageNode>();
                CD->Ctor();
                CD->AtkResNode.NodeID = 201;
                CD->AtkResNode.Type = NodeType.Image;
                CD->AtkResNode.X = 0;
                CD->AtkResNode.Y = 2;
                CD->AtkResNode.Width = 44;
                CD->AtkResNode.Height = 46;
                CD->AtkResNode.Flags = 8243;
                CD->AtkResNode.Flags_2 = 1;
                CD->AtkResNode.Flags_2 |= 4;
                CD->WrapMode = 1;
                CD->PartId = 0;
                CD->PartsList = originalCD->PartsList;

                Text = UIHelper.CleanAlloc<AtkTextNode>();
                Text->Ctor();
                Text->AtkResNode.NodeID = 202;
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
                BigText->AtkResNode.NodeID = 203;
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
                CD->AtkResNode.ParentNode = rootNode;
                Text->AtkResNode.ParentNode = rootNode;

                UIHelper.Link(OriginalOverlay, (AtkResNode*)CD);
                UIHelper.Link((AtkResNode*)CD, (AtkResNode*)Border);
                UIHelper.Link((AtkResNode*)Border, (AtkResNode*)BigText);
                UIHelper.Link((AtkResNode*)BigText, (AtkResNode*)Text);
                UIHelper.Link((AtkResNode*)Text, macroIcon);

                Component->Component->UldManager.UpdateDrawNodeList();

                if(dotMode) UIHelper.Hide(OriginalOverlay);
                UIHelper.Hide(CD);
                UIHelper.Hide(Text);
                UIHelper.Hide(BigText);
            }

            // ============================

            public void SetProgress(float current, float max) {
                if (DoTMode) SetTimerProgress(current, max);
                else SetBuffProgress(current);
            }

            public void SetDone() {
                if (DoTMode) SetTimerDone();
                else SetBuffDone();
            }

            // ======== FOR DoT TIMERS ========

            private void SetTimerProgress(float current, float max) {
                if (State == IconState.TimerDone && current <= 0) return;
                State = IconState.TimerRunning;

                UIHelper.Show(CD);
                UIHelper.Show(Text);
                if(!UseCombo) Border->PartId = 0;

                CD->PartId = (ushort)(80 - (float)(current / max) * 80);
                Text->SetText(((int)Math.Round(current)).ToString());
            }

            private void SetTimerDone() {
                State = IconState.TimerDone;
                UIHelper.Hide(CD);
                UIHelper.Hide(Text);
            }

            // ====== FOR BUFFS =============

            private void SetBuffProgress(float current) {
                if(State != IconState.BuffRunning) {
                    State = IconState.BuffRunning;
                    UIHelper.Hide(OriginalOverlay);
                    UIHelper.Show(BigText);
                }
                BigText->SetText(((int)Math.Round(current)).ToString());
            }

            private void SetBuffDone() {
                if (State == IconState.None) return;
                State = IconState.None;
                if(!UseCombo) Border->PartId = 0;
                UIHelper.Hide(BigText);
                UIHelper.Show(OriginalOverlay);
            }

            // =====================

            public void Tick(float percent, HashSet<uint> yellowBorder) {
                if (!UseCombo && State != IconState.TimerDone && State != IconState.BuffRunning) {
                    Border->PartId = 0;
                    return;
                }
                if (UseCombo && !yellowBorder.Contains(Id)) {
                    Border->PartId = 0;
                    return;
                }
                Border->PartId = (ushort) (6 + percent* 7);
            }

            public void Dispose() {
                var rootNode = (AtkResNode*)Component;

                UIHelper.Link(OriginalOverlay, Text->AtkResNode.PrevSiblingNode);
                Component->Component->UldManager.UpdateDrawNodeList();

                UIHelper.Show(OriginalOverlay);

                if (Border != null) {
                    Border->AtkResNode.Destroy(true);
                    Border = null;
                }

                if (CD != null) {
                    CD->AtkResNode.Destroy(true);
                    CD = null;
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
                OriginalCombo = null;
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
        private readonly ClientInterface Client;

        public UIIconManager() {
            Client = new ClientInterface();
        }

        public void Setup(List<uint> triggers, bool dotMode, bool useCombo) {
            if (triggers == null) return;
            var hotbarModule = Client.UiModule.RaptureHotbarModule;

            for (var abIndex = 0; abIndex < AllActionBars.Length; abIndex++) {
                if (hotbarModule == null) return;
                var actionBar = (AddonActionBarBase*)AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonByName(AllActionBars[abIndex]);
                if (actionBar == null || actionBar->ActionBarSlotsAction == null) continue;
                HotBar* bar = (abIndex < 10) ? hotbarModule.GetBar(abIndex, HotBarType.Normal) : hotbarModule.GetBar(abIndex - 10, HotBarType.Cross);

                for (var slotIndex = 0; slotIndex < actionBar->HotbarSlotCount; slotIndex++) {
                    var slot = actionBar->ActionBarSlotsAction[slotIndex];
                    var slotStruct = hotbarModule.GetBarSlot(bar, slotIndex);
                    if (slotStruct == null) continue;
                    if (slotStruct->CommandType != HotbarSlotType.Action) continue;

                    var icon = slot.Icon;
                    var action = UIHelper.GetAdjustedAction(slotStruct->CommandId);
                    if (triggers.Contains(action) && !AlreadySetup(icon)) {
                        Icons.Add(new Icon(action, icon, dotMode, useCombo));
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
                if(triggers.Contains(icon.Id)) {
                    icon.Dispose();
                    toRemove.Add(icon);
                }
            }
            Icons.RemoveAll(x => toRemove.Contains(x));
        }

        public void SetProgress(List<uint> triggers, bool dotMode, bool useCombo, float current, float max, bool check = true) {
            var found = false;
            foreach (var icon in Icons) {
                if (!triggers.Contains(icon.Id)) continue;
                icon.SetProgress(current, max);
                found = true;
            }

            if (found || !check) return;
            Setup(triggers, dotMode, useCombo);
            SetProgress(triggers, dotMode, useCombo, current, max, false);
        }

        public void SetDone(List<uint> triggers, bool dotMode, bool useCombo, bool check = true) {
            var found = false;
            foreach (var icon in Icons) {
                if (!triggers.Contains(icon.Id)) continue;
                icon.SetDone();
                found = true;
            }

            if (found || !check) return;
            Setup(triggers, dotMode, useCombo);
            SetDone(triggers, dotMode, useCombo, false);
        }

        // ========================

        public void Tick() {
            var time = DateTime.Now;
            int millis = time.Second * 1000 + time.Millisecond;
            float percent = (float)(millis % MILLIS_LOOP) / MILLIS_LOOP;

            HashSet<uint> yellowBorder = new();
            var hotbarStruct = UIHelper.GetHotbarUI();
            for(int i = 0; i < 10; i++) {
                for(int j = 0; j < 12; j++) {
                    var item = hotbarStruct->Hotbars[i][j];
                    if(item.YellowBorder && item.ActionId > 0) yellowBorder.Add(item.ActionId);
                }
            }

            Icons.ForEach(x => x.Tick(percent, yellowBorder));
        }

        public void Reset() {
            Icons.ForEach(x => x.Dispose());
            Icons.Clear();
        }

        public void Dispose() {
            Reset();
            Client.Dispose();
        }
    }
}
