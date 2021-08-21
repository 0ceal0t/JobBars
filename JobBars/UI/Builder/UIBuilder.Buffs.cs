using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;
using System.Collections.Generic;
using System;
using System.Numerics;
using JobBars.Buffs;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        private AtkResNode* BuffRoot = null;
        public List<UIBuff> Buffs = new();
        public Dictionary<ActionIds, UIBuff> IconToBuff = new();

        private void InitBuffs(AtkUldPartsList* partsList) {
            BuffRoot = CreateResNode();
            BuffRoot->Width = 256;
            BuffRoot->Height = 100;
            BuffRoot->Flags = 9395;

            UIBuff lastBuff = null;
            foreach (var icon in BuffManager.Manager.Icons) {
                var newBuff = new UIBuff(partsList, DataManager.GetIcon(icon));

                Buffs.Add(newBuff);
                newBuff.RootRes->ParentNode = BuffRoot;

                IconToBuff[icon] = newBuff;

                if (lastBuff != null) UIHelper.Link(lastBuff.RootRes, newBuff.RootRes);
                lastBuff = newBuff;
            }

            BuffRoot->ChildCount = (ushort)(5 * Buffs.Count);
            BuffRoot->ChildNode = Buffs[0].RootRes;
        }

        private void DisposeBuffs() {
            Buffs.ForEach(x => x.Dispose());
            Buffs = null;


            BuffRoot->Destroy(true);
            BuffRoot = null;
        }

        public void SetBuffPosition(Vector2 pos) => SetPosition(BuffRoot, pos.X, pos.Y);
        public void SetBuffScale(float scale) => SetScale(BuffRoot, scale, scale);
        public void ShowBuffs() => UIHelper.Show(BuffRoot);
        public void HideBuffs() => UIHelper.Hide(BuffRoot);
        public void HideAllBuffs() => Buffs.ForEach(x => x.Hide());
    }
}
