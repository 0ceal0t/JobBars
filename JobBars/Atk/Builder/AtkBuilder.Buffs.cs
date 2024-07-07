using System.Collections.Generic;

namespace JobBars.Atk {
    public unsafe partial class AtkBuilder {
        public List<AtkBuffPartyList> PartyListBuffs = [];

        private void InitBuffs() {
            for( var i = 0; i < 8; i++ ) PartyListBuffs.Add( new AtkBuffPartyList() );
        }

        private void DisposeBuffs() {
            // ========= PARTYLIST =============

            /*var partyListAddon = AtkHelper.PartyListAddon;
            for( var i = 0; i < PartyListBuffs.Count; i++ ) {
                if( partyListAddon != null ) {
                    var partyMember = partyListAddon->PartyMembers[i];
                    PartyListBuffs[i].DetachFrom( partyMember.TargetGlowContainer );
                    partyMember.PartyMemberComponent->UldManager.UpdateDrawNodeList();
                }
                PartyListBuffs[i].Dispose();
            }
            PartyListBuffs = null;*/
        }

        public void SetBuffPartyListVisible( int idx, bool visible ) => PartyListBuffs[idx].SetHighlightVisibility( visible );

        public void HideAllBuffPartyList() {
            foreach( var item in PartyListBuffs ) {
                item.SetHighlightVisibility( false );
            }
        }
    }
}
