using Dalamud.Logging;
using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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
        public enum IconState {
            Inactive,
            Active
        }

        public readonly string Name;
        public bool Enabled;

        private IconState State = IconState.Inactive;

        private readonly bool IsTimer;
        private readonly List<uint> Icons;
        private readonly IconTriggerStruct[] Triggers;
        private bool UseCombo;
        private bool UseBorder;

        private UI.UIIconProps IconProps;
        
        public IconReplacer(string name, IconProps props) {
            Name = name;
            Enabled = JobBars.Config.IconEnabled.Get(Name);
            IsTimer = props.IsTimer;
            Icons = new List<ActionIds>(props.Icons).Select(x => (uint)x).ToList();
            Triggers = props.Triggers;
            UseCombo = JobBars.Config.IconUseCombo.Get(Name);
            UseBorder = JobBars.Config.IconUseBorder.Get(Name);
            CreateIconProps();
        }

        private void CreateIconProps() {
            IconProps = new UI.UIIconProps {
                IsTimer = IsTimer,
                UseCombo = UseCombo,
                UseBorder = UseBorder
            };
        }

        public void Setup() {
            State = IconState.Inactive;
            JobBars.IconBuilder.Setup(Icons, IconProps);
        }

        public void Tick() {
            var timeLeft = -1f;
            var maxDuration = 1f;
            foreach(var trigger in Triggers) {
                if(UIHelper.PlayerStatus.TryGetValue(trigger.Trigger, out var value)) {
                    timeLeft = value.RemainingTime;
                    maxDuration = trigger.Duration;
                    break;
                }
            }

            if(timeLeft > 0 && State == IconState.Inactive) {
                State = IconState.Active;
            }

            if(State == IconState.Active) {
                if(timeLeft <= 0) {
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

        public void Draw(string id, JobIds job) {
            var _ID = id + Name;
            var type = IsTimer ? "TIMER" : "BUFF";

            ImGui.TextColored(Enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1), $"{Name} [{type}]");

            if (JobBars.Config.IconEnabled.Draw($"Enabled{_ID}", Name, Enabled, out var newEnabled)) {
                Enabled = newEnabled;
                JobBars.IconManager.Reset();
            }

            if (JobBars.Config.IconUseCombo.Draw($"Show Original Dash Border{_ID}", Name, UseCombo, out var newCombo)) {
                UseCombo = newCombo;
                CreateIconProps();
                JobBars.IconManager.Reset();
            }

            if (JobBars.Config.IconUseBorder.Draw($"Show Dash Border When Done/Active{_ID}", Name, UseBorder, out var newBorder)) {
                UseBorder = newBorder;
                CreateIconProps();
                JobBars.IconManager.Reset();
            }
        }
    }
}
