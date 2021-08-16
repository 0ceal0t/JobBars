using Dalamud.Logging;
using Dalamud.Plugin;
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
                Running,
                Done
            }

            public readonly uint Id;
            public AtkComponentNode* Component;

            private AtkResNode* OriginalOverlay;
            private AtkImageNode* Border;
            private AtkImageNode* CD;
            private AtkTextNode* Text;
            private IconState State = IconState.None;

            public Icon(uint id, AtkComponentNode* component) {
                Id = id;
                Component = component;
                var nodeList = Component->Component->UldManager.NodeList;
                OriginalOverlay = nodeList[1];

                var originalBorder = (AtkImageNode*)nodeList[4];
                var originalCD = (AtkImageNode*)nodeList[5];

                Border = UIHelper.CleanAlloc<AtkImageNode>();
                Border->Ctor();
                Border->AtkResNode.NodeID = 200;
                Border->AtkResNode.ChildCount = 0;
                Border->AtkResNode.ChildNode = null;
                Border->AtkResNode.PrevSiblingNode = null;
                Border->AtkResNode.NextSiblingNode = null;
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
                Border->AtkResNode.ChildCount = 0;
                Border->AtkResNode.ChildNode = null;
                Border->AtkResNode.PrevSiblingNode = null;
                Border->AtkResNode.NextSiblingNode = null;
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
                Border->AtkResNode.ChildCount = 0;
                Border->AtkResNode.ChildNode = null;
                Border->AtkResNode.PrevSiblingNode = null;
                Border->AtkResNode.NextSiblingNode = null;
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

                var macroIcon = nodeList[15];
                var rootNode = (AtkResNode*)Component;

                Border->AtkResNode.ParentNode = rootNode;
                CD->AtkResNode.ParentNode = rootNode;
                Text->AtkResNode.ParentNode = rootNode;

                UIHelper.Link(OriginalOverlay, (AtkResNode*)Border);
                UIHelper.Link((AtkResNode*)Border, (AtkResNode*)CD);
                UIHelper.Link((AtkResNode*)CD, (AtkResNode*)Text);
                UIHelper.Link((AtkResNode*)Text, macroIcon);

                Component->Component->UldManager.UpdateDrawNodeList();

                UIHelper.Hide(OriginalOverlay);
                UIHelper.Hide(CD);
                UIHelper.Hide(Text);
            }

            public void SetProgress(float current, float max) {
                if (State == IconState.Done && current <= 0) return;
                State = IconState.Running;

                UIHelper.Show(CD);
                UIHelper.Show(Text);
                Border->PartId = 0;

                CD->PartId = (ushort)(80 - (float)(current / max) * 80);
                Text->SetText(((int)Math.Round(current)).ToString());
            }

            public void SetDone() {
                State = IconState.Done;
                UIHelper.Hide(CD);
                UIHelper.Hide(Text);
            }

            public void Tick(float percent) {
                if (State != IconState.Done) return;

                Border->PartId = (ushort)(6 + percent * 7);
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

                Component = null;
                OriginalOverlay = null;
            }
        }

        public static UIIconManager Manager { get; private set; }
        public static void Initialize(DalamudPluginInterface pluginInterface) {
            Manager = new UIIconManager(pluginInterface);
        }

        public static void Dispose() {
            Manager?.DisposeInstance();
            Manager = null;
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
        private readonly DalamudPluginInterface PluginInterface;

        private UIIconManager(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;
            Client = new ClientInterface(pluginInterface.TargetModuleScanner, pluginInterface.Data);
        }

        public void Setup(List<uint> triggers) {
            if (triggers == null) return;
            var hotbarModule = Client.UiModule.RaptureHotbarModule;

            for (var abIndex = 0; abIndex < AllActionBars.Length; abIndex++) {
                if (hotbarModule == null) return;
                var actionBar = (AddonActionBarBase*)PluginInterface.Framework.Gui.GetUiObjectByName(AllActionBars[abIndex], 1);
                if (actionBar == null || actionBar->ActionBarSlotsAction == null) continue;
                HotBar* bar = (abIndex < 10) ? hotbarModule.GetBar(abIndex, HotBarType.Normal) : hotbarModule.GetBar(abIndex - 10, HotBarType.Cross);

                for (var slotIndex = 0; slotIndex < actionBar->HotbarSlotCount; slotIndex++) {
                    var slot = actionBar->ActionBarSlotsAction[slotIndex];
                    var slotStruct = hotbarModule.GetBarSlot(bar, slotIndex);
                    if (slotStruct == null) continue;
                    if (slotStruct->CommandType != HotbarSlotType.Action) continue;

                    var icon = slot.Icon;
                    var action = slotStruct->CommandId;
                    if (triggers.Contains(action) && !AlreadySetup(icon)) {
                        Icons.Add(new Icon(action, icon));
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

        public void SetIconProgress(List<uint> triggers, float current, float max, bool check = true) {
            var found = false;
            foreach(var icon in Icons) {
                if (!triggers.Contains(icon.Id)) continue;
                icon.SetProgress(current, max);
                found = true;
            }

            if (found || !check) return;
            Setup(triggers);
            SetIconProgress(triggers, current, max, false);
        }

        public void SetIconDone(List<uint> triggers, bool check = true) {
            var found = false;
            foreach (var icon in Icons) {
                if (!triggers.Contains(icon.Id)) continue;
                icon.SetDone();
                found = true;
            }

            if (found || !check) return;
            Setup(triggers);
            SetIconDone(triggers, false);
        }

        public void Tick() {
            var time = DateTime.Now;
            int millis = time.Second * 1000 + time.Millisecond;
            float percent = (float)(millis % MILLIS_LOOP) / MILLIS_LOOP;

            Icons.ForEach(x => x.Tick(percent));
        }

        public void Reset() {
            Icons.ForEach(x => x.Dispose());
            Icons.Clear();
        }

        private void DisposeInstance() {
            Reset();
            Client.Dispose();
        }
    }
}
