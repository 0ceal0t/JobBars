using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System.Collections.Generic;
using System.Numerics;
using System;
using Dalamud.Logging;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        private AtkResNode* CooldownRoot = null;
        private List<UICooldown> Cooldowns = new();

        private void InitCooldowns() {
            var addon = UIHelper.PartyListAddon;

            CooldownRoot = CreateResNode();
            CooldownRoot->Width = 100;
            CooldownRoot->Height = 100;
            CooldownRoot->Flags = 9395;
            CooldownRoot->ParentNode = addon->AtkUnitBase.RootNode;
            UIHelper.SetPosition(CooldownRoot, -40, 40);

            UICooldown lastCooldown = null;
            for (int i = 0; i < 1; i++) {
                var newItem = new UICooldown();

                Cooldowns.Add(newItem);
                newItem.RootRes->ParentNode = CooldownRoot;

                if (lastCooldown != null) UIHelper.Link(lastCooldown.RootRes, newItem.RootRes);
                lastCooldown = newItem;
            }

            CooldownRoot->ChildCount = (ushort)((1 + Cooldowns[0].RootRes->ChildCount) * Cooldowns.Count);
            CooldownRoot->ChildNode = Cooldowns[0].RootRes;

            addon->AtkUnitBase.UldManager.NodeList[21]->PrevSiblingNode = CooldownRoot;
            addon->AtkUnitBase.UldManager.UpdateDrawNodeList();
        }

        private void DisposeCooldowns() {
            foreach (var cd in Cooldowns) cd.Dispose();
            Cooldowns = null;

            CooldownRoot->Destroy(true);
            CooldownRoot = null;

            var addon = UIHelper.PartyListAddon;
            addon->AtkUnitBase.UldManager.NodeList[21]->PrevSiblingNode = null;
            addon->AtkUnitBase.UldManager.UpdateDrawNodeList();
        }
    }
}
