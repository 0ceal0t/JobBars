using Dalamud.Logging;
using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UIIconComboType = JobBars.UI.UIIconComboType;
using UIIconProps = JobBars.UI.UIIconProps;

namespace JobBars.Icons {
    public struct IconProps {
        public bool IsTimer;
        public ActionIds[] Icons;
        public IconTriggerStruct[] Triggers;
    }

    public struct IconTriggerStruct {
        public Item Trigger;
        public float Duration;
    }

    public class IconReplacer {
        public static readonly UIIconComboType[] ValidComboTypes = (UIIconComboType[])Enum.GetValues(typeof(UIIconComboType));

        public enum IconState {
            Inactive,
            Active
        }

        public bool Enabled;

        public readonly string Name;
        private readonly bool IsTimer;
        private readonly List<uint> Icons;
        private readonly IconTriggerStruct[] Triggers;

        private IconState State = IconState.Inactive;
        private UIIconComboType ComboType;
        private UIIconProps IconProps;
        private float Offset;

        public IconReplacer(string name, IconProps props) {
            Name = name;
            Triggers = props.Triggers;
            IsTimer = props.IsTimer;
            Icons = new List<ActionIds>(props.Icons).Select(x => (uint)x).ToList();
            Enabled = JobBars.Config.IconEnabled.Get(Name);
            ComboType = JobBars.Config.IconComboType.Get(Name);
            Offset = JobBars.Config.IconTimerOffset.Get(Name);
            CreateIconProps();
        }

        private void CreateIconProps() {
            IconProps = new UIIconProps {
                IsTimer = IsTimer,
                ComboType = ComboType
            };
        }

        public void Setup() {
            State = IconState.Inactive;
            JobBars.IconBuilder.Setup(Icons, IconProps);
        }

        public void Tick() {
            var timeLeft = -1f;
            var maxDuration = 1f;
            foreach (var trigger in Triggers) {
                if (UIHelper.PlayerStatus.TryGetValue(trigger.Trigger, out var value)) {
                    timeLeft = value.RemainingTime - Offset;
                    maxDuration = trigger.Duration - Offset;
                    break;
                }
            }

            if (timeLeft > 0 && State == IconState.Inactive) {
                State = IconState.Active;
            }

            if (State == IconState.Active) {
                if (timeLeft <= 0) {
                    timeLeft = 0;
                    State = IconState.Inactive;
                }

                if (timeLeft == 0) ResetIcon();
                else SetIcon(timeLeft, maxDuration);
            }
        }

        private void SetIcon(float current, float duration) {
            JobBars.IconBuilder.SetProgress(Icons, current, duration);
        }

        private void ResetIcon() {
            JobBars.IconBuilder.SetDone(Icons);
        }

        public void Draw(string id, JobIds _) {
            var _ID = id + Name;
            var type = IsTimer ? "TIMER" : "BUFF";

            ImGui.TextColored(Enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1), $"{Name} [{type}]");

            if (JobBars.Config.IconEnabled.Draw($"Enabled{_ID}", Name, Enabled, out var newEnabled)) {
                Enabled = newEnabled;
                JobBars.IconManager.Reset();
            }

            if (JobBars.Config.IconComboType.Draw($"Dash border{_ID}", Name, ValidComboTypes, ComboType, out var newComboType)) {
                ComboType = newComboType;
                CreateIconProps();
                JobBars.IconManager.Reset();
            }

            if (IsTimer) {
                if (JobBars.Config.IconTimerOffset.Draw($"Time offset{_ID}", Name, Offset, out var newOffset)) {
                    Offset = newOffset;
                }
            }
        }
    }
}
