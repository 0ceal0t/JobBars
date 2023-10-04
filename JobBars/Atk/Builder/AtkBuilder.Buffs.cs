using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System.Collections.Generic;
using System.Numerics;

namespace JobBars.Atk {
    public unsafe partial class AtkBuilder {
        private AtkResNode* BuffRoot = null;
        public List<AtkBuff> Buffs = new();
        public static readonly int MAX_BUFFS = 20;

        public List<AtkBuffPartyList> PartyListBuffs = new();

        private void InitBuffs() {
            BuffRoot = CreateResNode();
            BuffRoot->Width = 256;
            BuffRoot->Height = 100;
            BuffRoot->DrawFlags = 9395;

            AtkBuff lastBuff = null;
            for (var i = 0; i < MAX_BUFFS; i++) {
                var newBuff = new AtkBuff();

                Buffs.Add(newBuff);
                newBuff.RootRes->ParentNode = BuffRoot;

                if (lastBuff != null) AtkHelper.Link(lastBuff.RootRes, newBuff.RootRes);
                lastBuff = newBuff;
            }

            BuffRoot->ChildCount = (ushort)(5 * Buffs.Count);
            BuffRoot->ChildNode = Buffs[0].RootRes;

            RefreshBuffLayout();

            for (var i = 0; i < 8; i++) {
                PartyListBuffs.Add(new AtkBuffPartyList());
            }
        }

        private void DisposeBuffs() {
            Buffs.ForEach(x => x.Dispose());
            Buffs = null;

            BuffRoot->Destroy(true);
            BuffRoot = null;

            // ========= PARTYLIST =============

            var partyListAddon = AtkHelper.PartyListAddon;

            for (var i = 0; i < PartyListBuffs.Count; i++) {
                if (partyListAddon != null) {
                    var partyMember = partyListAddon->PartyMember[i];
                    PartyListBuffs[i].DetachFrom(partyMember.TargetGlowContainer, partyMember.IconBottomLeftText);
                    partyMember.PartyMemberComponent->UldManager.UpdateDrawNodeList();
                }
                PartyListBuffs[i].Dispose();
            }
            PartyListBuffs = null;
        }

        public void RefreshBuffLayout() {
            for (int i = 0; i < Buffs.Count; i++) {
                Buffs[i].SetPosition(i);
            }
        }

        public void UpdateBuffsTextSize() {
            foreach (var buff in Buffs) {
                buff.SetTextSize(JobBars.Configuration.BuffTextSize_v2);
            }
        }

        public void UpdateBorderThin() {
            foreach (var buff in Buffs) {
                buff.SetBorderThin(JobBars.Configuration.BuffThinBorder);
            }
        }

        public void SetBuffPartyListVisible(int idx, bool visible) => PartyListBuffs[idx].SetHighlightVisibility(visible);
        public void SetBuffPartyListText(int idx, string text) => PartyListBuffs[idx].SetText(text);
        public void HideAllBuffPartyList() {
            foreach (var item in PartyListBuffs) {
                item.SetHighlightVisibility(false);
                item.SetText("");
            }
        }

        public void SetBuffPosition(Vector2 pos) => SetPosition(BuffRoot, pos.X, pos.Y);
        public void SetBuffScale(float scale) => SetScale(BuffRoot, scale, scale);
        public void ShowBuffs() => AtkHelper.Show(BuffRoot);
        public void HideBuffs() => AtkHelper.Hide(BuffRoot);
        public void HideAllBuffs() => Buffs.ForEach(x => x.Hide());
        public void UpdateBuffsSize() => Buffs.ForEach(x => x.UpdateSize());
    }
}
