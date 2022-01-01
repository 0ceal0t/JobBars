using Dalamud.Configuration;
using ImGuiNET;
using System;
using System.Numerics;
using JobBars.Gauges;
using JobBars.UI;
using JobBars.Cursors;

namespace JobBars.Data {
    public enum GaugePositionType {
        Global,
        PerJob,
        Split
    }

    public enum AttachAddon {
        Chatbox,
        HP_MP_Bars
    }

    [Serializable]
    public class Configuration : IPluginConfiguration {
        public int Version { get; set; } = 2;

        public bool Use4K = true;

        public AttachAddon AttachAddon = AttachAddon.Chatbox;

        // ====== GAUGES ======

        public float GaugeScale = 1.0f;
        public bool GaugeHorizontal = false;
        public bool GaugeAlignRight = false;
        public bool GaugeBottomToTop = false;

        public bool GaugesEnabled = true;
        public bool GaugesHideOutOfCombat = false;
        public bool GaugeGCDTextVisible = true;

        public GaugePositionType GaugePositionType = GaugePositionType.Global;
        public Vector2 GaugePositionGlobal = new(200, 200); // global
        public VectorValueConfig GaugePerJobPosition = new(new Vector2(200, 200)); // per job
        public VectorValueConfig GaugeSplitPosition = new(new Vector2(200, 200)); // split

        public FloatValueConfig GaugeIndividualScale = new(1.0f);
        public BoolValueConfig GaugeEnabled = new(true);
        public IntValueConfig GaugeOrder = new(-1);
        public BoolValueConfig GaugeVertical = new(false);
        public IntValueConfig GaugeProcOrder = new(-1);
        public ColorConfig GaugeProcColor = new();
        public ComboValueConfig<GaugeCompleteSoundType> GaugeCompletionSound = new(); // GCD, stacks, charges
        public BoolValueConfig GaugeProgressSound = new(true); // Timer, GCD, proc, stacks, charges
        public BoolValueConfig GaugeHideInactive = new(false);
        public BoolValueConfig GaugeInvert = new(false);
        public BoolValueConfig GaugeShowSegments = new(true);
        public BoolValueConfig GaugeReverseFill = new(false);
        public ColorConfig GaugeColor = new();
        public ComboValueConfig<GaugeVisualType> GaugeType = new();
        public BoolValueConfig GaugeShowText = new(true);
        public BoolValueConfig GaugeSwapText = new(false);
        public FloatValueConfig GaugeTimerOffset = new(0f);

        public int GaugeSoundEffect = 0;
        public int GaugeCompletionSoundEffect = 78;
        public float GaugeLowTimerWarning = 4.0f;

        // ===== BUFFS ======

        public Vector2 BuffPosition = new(200, 200);
        public float BuffScale = 1.0f;

        public bool BuffBarEnabled = true;
        public bool BuffPartyListEnabled = true;
        public bool BuffHideOutOfCombat = false;
        public bool BuffIncludeParty = true;
        public bool BuffOrderByActive = true;
        public float BuffDisplayTimer = 30f;
        public bool BuffThinBorder = false;

        public BoolValueConfig BuffEnabled = new(true);

        public int BuffHorizontal = 5;
        public bool BuffRightToLeft = false;
        public bool BuffBottomToTop = false;
        public int BuffTextSize = 18;

        // ===== COOLDOWNS ======

        public Vector2 CooldownPosition = new(-40, 40);

        public bool CooldownsEnabled = true;
        public bool CooldownsHideOutOfCombat = false;
        public bool CooldownsShowBorderWhenActive = true;
        public bool CooldownsHideActiveBuffDuration = false;


        public bool CooldownsLeftAligned = false;

        public BoolValueConfig CooldownEnabled = new(true);
        public IntValueConfig CooldownOrder = new(-1);

        // ===== CURSOR =======

        public bool CursorsEnabled = false;
        public bool CursorHideWhenHeld = false;
        public bool CursorHideOutOfCombat = false;
        public bool CursorKeepInMiddle = false;
        public float CursorInnerScale = 1.5f;
        public float CursorOuterScale = 1.2f;
        public string CursorInnerColor = UIColor.MpPink.Name;
        public string CursorOuterColor = UIColor.HealthGreen.Name;

        public ComboValueConfig<CursorType> CursorType = new();
        public ComboValueConfig<Helper.StatusNameId> CursorStatus = new(true);
        public FloatValueConfig CursorStatusDuration = new(5f);

        // ===== ICONS ===========

        public bool IconsEnabled = true;
        public BoolValueConfig IconEnabled = new(true);
        public ComboValueConfig<UIIconComboType> IconComboType = new();
        public FloatValueConfig IconTimerOffset = new(0f);

        // =====================

        public void Save() {
            JobBars.PluginInterface.SavePluginConfig(this);
        }

        public static bool DrawColor(string id, ElementColor currentValue, out ElementColor value) {
            value = currentValue;
            if (ImGui.BeginCombo(id, value.Name)) {
                foreach (var entry in UIColor.AllColors) {
                    if (ImGui.Selectable($"{entry.Key}##Combo", value.Name == entry.Key)) {
                        value = entry.Value;
                        ImGui.EndCombo();
                        return true;
                    }
                }
                ImGui.EndCombo();
            }
            return false;
        }
    }
}
