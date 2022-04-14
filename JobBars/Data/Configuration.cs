using Dalamud.Configuration;
using ImGuiNET;
using System;
using System.Numerics;
using System.Linq;
using JobBars.Gauges;
using JobBars.UI;
using JobBars.Cursors;
using JobBars.Gauges.Rolling;
using System.Collections.Generic;
using JobBars.Cooldowns;

namespace JobBars.Data {
    public enum GaugePositionType {
        Global,
        PerJob,
        Split
    }

    public enum CursorPositionType {
        MouseCursor,
        Middle,
        CustomPosition
    }

    public enum AttachAddon {
        Chatbox,
        HP_MP_Bars,
        PartyList
    }

    public struct CustomCooldownProps {
        public string Name;
        public JobIds Job;
        public CooldownProps Props;
    }

    [Serializable]
    public class Configuration : IPluginConfiguration {
        public int Version { get; set; } = 2;

        public bool Use4K = true;

        public AttachAddon AttachAddon = AttachAddon.Chatbox;

        public AttachAddon CooldownAttachAddon = AttachAddon.PartyList;

        // ====== GAUGES ======

        public float GaugeScale = 1.0f;
        public bool GaugeHorizontal = false;
        public bool GaugeAlignRight = false;
        public bool GaugeBottomToTop = false;

        public bool GaugesEnabled = true;
        public bool GaugesHideOutOfCombat = false;
        public bool GaugesHideWeaponSheathed = false;
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
        public BoolValueConfig GaugeHideInactive = new(false);
        public BoolValueConfig GaugeInvert = new(false);
        public BoolValueConfig GaugeShowSegments = new(true);
        public BoolValueConfig GaugeReverseFill = new(false);
        public ColorConfig GaugeColor = new();
        public ComboValueConfig<GaugeVisualType> GaugeType = new();
        public BoolValueConfig GaugeShowText = new(true);
        public BoolValueConfig GaugeSwapText = new(false);
        public FloatValueConfig GaugeTimerOffset = new(0f);
        public ComboValueConfig<GaugeGCDRollingType> GaugeGCDRolling = new();
        public float GaugeSlidecastTime = 0.5f;

        public ComboValueConfig<GaugeCompleteSoundType> GaugeCompletionSound = new(); // GCD, stacks, charges
        public FloatValueConfig GaugeLowTimerWarning_2 = new(4.0f);
        public IntValueConfig GaugeSoundEffect_2 = new(0);
        public IntValueConfig GaugeCompletionSoundEffect_2 = new(78);

        public bool GaugePulse = true;

        // ===== BUFFS ======

        public Vector2 BuffPosition = new(200, 200);
        public float BuffScale = 1.0f;

        public bool BuffBarEnabled = true;
        public bool BuffPartyListASTText = false;
        public bool BuffHideOutOfCombat = false;
        public bool BuffHideWeaponSheathed = false;
        public bool BuffIncludeParty = true;
        public bool BuffOrderByActive = true;
        public float BuffDisplayTimer = 30f;
        public bool BuffThinBorder = false;
        public bool BuffSquare = false;
        public float BuffOnCDOpacity = 1.0f;

        public BoolValueConfig BuffEnabled = new(true);
        public BoolValueConfig BuffPartyListHighlight = new(true);

        public int BuffHorizontal = 5;
        public bool BuffRightToLeft = false;
        public bool BuffBottomToTop = false;
        public int BuffTextSize = 18;

        // ===== COOLDOWNS ======

        public Vector2 CooldownPosition = new(-40, 40);
        public float CooldownScale = 1.0f;
        public float CooldownsSpacing = 40f;

        public bool CooldownsEnabled = true;
        public bool CooldownsHideOutOfCombat = false;
        public bool CooldownsHideWeaponSheathed = false;
        public bool CooldownsHideActiveBuffDuration = false;
        public bool CooldownsShowPartyMembers = true;
        public float CooldownsOnCDOpacity = 1.0f;

        public bool CooldownsLeftAligned = false;

        public BoolValueConfig CooldownEnabled = new(true);
        public IntValueConfig CooldownOrder = new(-1);
        public BoolValueConfig CooldownShowBorderWhenActive = new(true);
        public BoolValueConfig CooldownShowBorderWhenOffCD = new(false);

        public List<CustomCooldownProps> CustomCooldown = new();

        // ===== CURSOR =======

        public bool CursorsEnabled = false;
        public bool CursorHideWhenHeld = false;
        public bool CursorHideWeaponSheathed = false;
        public bool CursorHideOutOfCombat = false;
        public CursorPositionType CursorPosition = CursorPositionType.MouseCursor;
        public Vector2 CursorCustomPosition = new(200, 200);
        public float CursorInnerScale = 1.5f;
        public float CursorOuterScale = 1.2f;
        public string CursorInnerColor = UIColor.MpPink.Name;
        public string CursorOuterColor = UIColor.HealthGreen.Name;

        public ComboValueConfig<CursorType> CursorType = new();
        public ComboValueConfig<Helper.ItemData> CursorStatus = new(true);
        public FloatValueConfig CursorStatusDuration = new(5f);

        // ===== ICONS ===========

        public bool IconsEnabled = true;
        public BoolValueConfig IconEnabled = new(true);
        public ComboValueConfig<UIIconComboType> IconComboType = new();
        public FloatValueConfig IconTimerOffset = new(0f);

        // =====================

        public void RemoveCustomCooldown(string name) {
            CustomCooldown.RemoveAll(x => x.Name == name);
            Save();
        }

        public void AddCustomCooldown(string name, JobIds job, CooldownProps props) {
            CustomCooldown.Add(new CustomCooldownProps {
                Job = job,
                Name = name,
                Props = props
            });
            Save();
        }

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
