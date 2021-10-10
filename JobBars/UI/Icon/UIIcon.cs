using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System;

namespace JobBars.UI {
    public enum UIIconComboType {
        Combo_Or_Active,
        Combo_And_Active,
        Only_When_Combo,
        Only_When_Active,
        Never
    }

    public struct UIIconProps {
        public bool IsTimer;
        public UIIconComboType ComboType;
    }

    public unsafe abstract class UIIcon {
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

        protected readonly UIIconComboType ComboType;

        protected IconState State = IconState.None;

        private bool Disposed = false;

        protected uint NodeIdx = 200;

        public UIIcon(uint adjustedId, uint slotId, int hotbarIdx, int slotIdx, AtkComponentNode* component, UIIconProps props) {
            ComboType = props.ComboType;

            AdjustedId = adjustedId;
            SlotId = slotId;
            HotbarIdx = hotbarIdx;
            SlotIdx = slotIdx;
            Component = component;
            IconComponent = (AtkComponentIcon*)Component->Component;
        }

        public abstract void SetProgress(float current, float max);

        public abstract void SetDone();

        public abstract void Tick(float dashPercent, bool border);

        public abstract void OnDispose();

        protected bool CalcShowBorder(bool active, bool border) => ComboType switch {
            UIIconComboType.Only_When_Combo => border,
            UIIconComboType.Only_When_Active => active,
            UIIconComboType.Combo_Or_Active => border || active,
            UIIconComboType.Combo_And_Active => border && active,
            UIIconComboType.Never => false,
            _ => false
        };

        public void Dispose() {
            if (Disposed) {
                return;
            }
            Disposed = true;

            OnDispose();

            Component = null;
            IconComponent = null;
        }
    }
}
