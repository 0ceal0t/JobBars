using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;
using JobBars.Nodes.Buff;
using JobBars.Nodes.Builder;
using JobBars.Nodes.Highlight;
using KamiToolKit.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Buffs.Manager {
    public unsafe partial class BuffManager : PerJobManager<BuffConfig[]> {
        private Dictionary<ulong, BuffPartyMember> ObjectIdToMember = [];
        private readonly List<BuffConfig> ApplyToTargetBuffs = [];

        private readonly Dictionary<JobIds, List<BuffConfig>> CustomBuffs = [];
        private List<BuffConfig> ApplyToTargetCustomBuffs => [.. CustomBuffs.Values.SelectMany( x => x.Where( y => y.ApplyToTarget ) )];

        private AddonController? Controller;
        private AddonController? PartyListController;

        private BuffRoot? Root;
        private HighlightRoot? Highlight;

        public BuffManager() : base( "##JobBars_Buffs" ) {
            ApplyToTargetBuffs.AddRange( [.. JobToValue.Values.SelectMany( x => x.Where( y => y.ApplyToTarget ) )] );

            Controller = new AddonController {
                AddonName = UiHelper.BuffGaugeAttachAddonName,
                OnSetup = SetupAddon,
                OnFinalize = ResetAddon,
                OnUpdate = UpdateAddon
            };
            Controller.Enable();

            PartyListController = new AddonController {
                AddonName = "_PartyList",
                OnSetup = SetupPartyList,
                OnFinalize = ResetPartyList,
            };
            PartyListController.Enable();
        }

        public void Hide() {
            Root?.IsVisible = false;
        }

        private void SetupAddon( AtkUnitBase* addon ) {
            Root = new();
            Root.AttachNode( addon );
        }

        private void SetupPartyList( AtkUnitBase* addon ) {
            Highlight = new();
            Highlight.AttachNode( addon->GetNodeById( 21 ) );
        }

        private void UpdateAddon( AtkUnitBase* addon ) {
            Tick();
        }

        private void ResetAddon( AtkUnitBase* addon ) {
            Root?.Dispose();
            Root = null;
        }

        private void ResetPartyList( AtkUnitBase* addon ) {
            Highlight?.Dispose();
            Highlight = null;
        }

        public void Dispose() {
            Controller?.Dispose();
            Controller = null;

            PartyListController?.Dispose();
            PartyListController = null;
        }

        public BuffConfig[] GetBuffConfigs( JobIds job ) {
            List<BuffConfig> configs = [.. ApplyToTargetBuffs];
            if( JobToValue.TryGetValue( job, out var props ) ) configs.AddRange( props.Where( x => !x.ApplyToTarget ) ); // avoid double-adding

            configs.AddRange( ApplyToTargetCustomBuffs );
            if( CustomBuffs.TryGetValue( job, out var customProps ) ) configs.AddRange( customProps.Where( x => !x.ApplyToTarget ) );

            return [.. configs];
        }

        public void PerformAction( Item action, uint objectId ) {
            if( !JobBars.Configuration.BuffBarEnabled ) return;
            if( !JobBars.Configuration.BuffIncludeParty && objectId != Dalamud.Objects.LocalPlayer?.GameObjectId ) return;

            foreach( var member in ObjectIdToMember.Values ) member.ProcessAction( action, objectId );
        }

        private void Tick() {
            if( Root == null || Highlight == null ) return;

            // Visibility

            if( UiHelper.CalcDoHide( JobBars.Configuration.BuffBarEnabled, JobBars.Configuration.BuffHideOutOfCombat, JobBars.Configuration.BuffHideWeaponSheathed ) ) {
                Highlight!.HideAll();
                Root!.IsVisible = false;
                return;
            }
            else {
                Root!.IsVisible = true;
            }
            
            // Global position + scale

            NodeBuilder.SetPositionGlobal( Root, JobBars.Configuration.BuffPosition );
            NodeBuilder.SetScaleGlobal( Root, JobBars.Configuration.BuffScale );
            Root?.UpdateSettings();

            // Evaluate each buff

            Dictionary<ulong, BuffPartyMember> newObjectIdToMember = [];
            HashSet<BuffTracker> activeBuffs = [];

            if( JobBars.PartyMembers == null ) return;

            for( var idx = 0; idx < JobBars.PartyMembers!.Count; idx++ ) {
                var partyMember = JobBars.PartyMembers[idx];

                if( partyMember == null || partyMember?.Job == JobIds.OTHER || partyMember?.ObjectId == 0 ) continue;
                if( !JobBars.Configuration.BuffIncludeParty && partyMember?.ObjectId != Dalamud.Objects.LocalPlayer?.GameObjectId ) continue;

                var member = ObjectIdToMember.TryGetValue( partyMember!.ObjectId, out var _member ) ? _member : new BuffPartyMember( partyMember.ObjectId, partyMember.IsPlayer );
                member.Tick( activeBuffs, partyMember, out var highlight, out var partyText );
                Highlight!.Highlights[idx].IsVisible = highlight;
                newObjectIdToMember[partyMember.ObjectId] = member;
            }

            for( var idx = JobBars.PartyMembers!.Count; idx < 8; idx++ ) {
                Highlight!.Highlights[idx].IsVisible = false;
            }

            var buffIdx = 0;
            foreach( var buff in JobBars.Configuration.BuffOrderByActive ?
                activeBuffs.OrderBy( b => b.CurrentState ) :
                activeBuffs.OrderBy( b => b.Id )
            ) {
                if( buffIdx >= ( BuffRoot.MAX_BUFFS - 1 ) ) break;
                buff.TickUi( Root!.Buffs[buffIdx] );
                buffIdx++;
            }

            // Hide all the rest

            for( var i = buffIdx; i < BuffRoot.MAX_BUFFS; i++ ) {
                Root!.Buffs[i].IsVisible = false;
            }

            ObjectIdToMember = newObjectIdToMember;
        }

        public void ResetUi() => ObjectIdToMember.Clear();

        public void Reset() {
            foreach( var item in ObjectIdToMember.Values ) item.Reset();
        }
    }
}
