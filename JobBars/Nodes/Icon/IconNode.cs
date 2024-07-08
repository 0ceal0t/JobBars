using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace JobBars.Nodes.Icon {
    public enum IconComboType {
        Combo_Or_Active,
        Combo_And_Active,
        Only_When_Combo,
        Only_When_Active,
        Never
    }

    public struct IconProps {
        public bool IsTimer;
        public IconComboType ComboType;
        public bool ShowRing;
    }

    public unsafe abstract class IconNode {
        protected enum IconState {
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
        public AtkComponentIcon* IconComponent;

        protected readonly IconComboType ComboType;
        protected readonly bool ShowRing;

        protected IconState State = IconState.None;

        private bool Disposed = false;

        public IconNode( uint adjustedId, uint slotId, int hotbarIdx, int slotIdx, AtkComponentNode* component, IconProps props ) {
            ComboType = props.ComboType;
            ShowRing = props.ShowRing;

            AdjustedId = adjustedId;
            SlotId = slotId;
            HotbarIdx = hotbarIdx;
            SlotIdx = slotIdx;
            Component = component;
            IconComponent = ( AtkComponentIcon* )Component->Component;
        }

        public abstract void SetProgress( float current, float max );

        public abstract void SetDone();

        public abstract void Tick( float dashPercent, bool border );

        public abstract void OnDispose();

        protected bool CalcShowBorder( bool active, bool border ) => ComboType switch {
            IconComboType.Only_When_Combo => border,
            IconComboType.Only_When_Active => active,
            IconComboType.Combo_Or_Active => border || active,
            IconComboType.Combo_And_Active => border && active,
            IconComboType.Never => false,
            _ => false
        };

        public abstract void RefreshVisuals();

        protected static void SetTextSmall( TextNode text ) {
            text.Size = new( 48, 12 );
            text.TextColor = new( 1f, 1f, 1f, 1f );
            text.TextOutlineColor = new( 51f / 255f, 51f / 255f, 51f / 255f, 1f );
            text.LineSpacing = 12;
            text.AlignmentFontType = 3;
            text.FontSize = 12;
            text.TextFlags = ( TextFlags )8;
        }

        protected static void SetTextLarge( TextNode text ) {
            text.Size = new( 40, 35 );
            text.TextColor = new( 1f, 1f, 1f, 1f );
            text.TextOutlineColor = new( 0f, 0f, 0f, 1f );
            text.LineSpacing = 12;
            text.AlignmentFontType = 52;
            text.FontSize = 24;
            text.TextFlags = ( TextFlags )8;
        }

        public void Dispose() {
            if( Disposed ) return;
            Disposed = true;
            OnDispose();
        }
    }
}
