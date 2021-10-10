using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System;

namespace JobBars.UI {
    public struct UIIconProps {
        public bool IsTimer;
        public bool UseCombo;
        public bool UseBorder;
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

        protected readonly bool UseCombo;
        protected readonly bool UseBorder;

        protected IconState State = IconState.None;

        private bool Disposed = false;

        protected uint NodeIdx = 200;

        public UIIcon(uint adjustedId, uint slotId, int hotbarIdx, int slotIdx, AtkComponentNode* component, UIIconProps props) {
            UseCombo = props.UseCombo;
            UseBorder = props.UseBorder;

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
