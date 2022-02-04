using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System.Collections.Generic;
using System.Numerics;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        public List<UICooldown> Cooldowns = new();
        private AtkResNode* CooldownRoot = null;

        private void InitCooldowns(AtkUldPartsList* partsList) {
            CooldownRoot = CreateResNode();
            CooldownRoot->Width = 100;
            CooldownRoot->Height = 100;
            CooldownRoot->Flags = 9395;
            UIHelper.SetPosition(CooldownRoot, -40, 40);

            UICooldown lastCooldown = null;
            for (int i = 0; i < 8; i++) {
                var newItem = new UICooldown(partsList);

                Cooldowns.Add(newItem);
                newItem.RootRes->ParentNode = CooldownRoot;

                if (lastCooldown != null) UIHelper.Link(lastCooldown.RootRes, newItem.RootRes);
                lastCooldown = newItem;
            }

            CooldownRoot->ChildCount = (ushort)((1 + Cooldowns[0].RootRes->ChildCount) * Cooldowns.Count);
            CooldownRoot->ChildNode = Cooldowns[0].RootRes;

            RefreshCooldownsLayout();
        }

        public void RefreshCooldownsLayout() {
            for (int i = 0; i < Cooldowns.Count; i++) {
                UIHelper.SetPosition(Cooldowns[i].RootRes, 0, JobBars.Config.CooldownsSpacing * i);
            }
        }

        private void DisposeCooldowns() {
            foreach (var cd in Cooldowns) cd.Dispose();
            Cooldowns = null;

            CooldownRoot->Destroy(true);
            CooldownRoot = null;
        }

        public void SetCooldownPosition(Vector2 pos) => UIHelper.SetPosition(CooldownRoot, pos.X, pos.Y);
        public void SetCooldownScale(float scale) => UIHelper.SetScale(CooldownRoot, scale, scale);
        public void SetCooldownRowVisible(int idx, bool visible) => UIHelper.SetVisibility(Cooldowns[idx].RootRes, visible);
        public void ShowCooldowns() => UIHelper.Show(CooldownRoot);
        public void HideCooldowns() => UIHelper.Hide(CooldownRoot);
    }
}
