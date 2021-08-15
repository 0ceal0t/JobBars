using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Plugin;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Game;
using JobBars.Helper;
using JobBars.UI;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Buffs;
using JobBars.Cooldown;
using JobBars.PartyList;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Threading;

namespace JobBars {
    public unsafe partial class JobBars : IDalamudPlugin {
        public string Name => "JobBars";
        public DalamudPluginInterface PluginInterface { get; private set; }
        public string AssemblyLocation { get; private set; } = Assembly.GetExecutingAssembly().Location;

        private GaugeManager GManager;
        private BuffManager BManager;
        private CooldownManager CDManager;
        private Configuration Config;
        private readonly HashSet<uint> GCDs = new();

        private JobIds CurrentJob = JobIds.OTHER;

        private delegate void ReceiveActionEffectDelegate(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private Hook<ReceiveActionEffectDelegate> receiveActionEffectHook;

        private delegate void ActorControlSelfDelegate(uint entityId, uint id, uint arg0, uint arg1, uint arg2, uint arg3, uint arg4, uint arg5, UInt64 targetId, byte a10);
        private Hook<ActorControlSelfDelegate> actorControlSelfHook;

        private PList Party; // TEMP
        private int LastPartyCount = 0;

        private bool PlayerExists => PluginInterface.ClientState?.LocalPlayer != null;
        private bool Initialized = false;

        private Vector2 LastPosition;
        private Vector2 LastScale;

        /*
         * SIG LIST:
         *  Lord have mercy when 6.0 comes
         * =================================
         * 
         *  FFXIVClientInterface/ClientInterface.cs
         *      Get UI Module (E8 ?? ?? ?? ?? 48 8B C8 48 8B 10 FF 52 40 80 88 ?? ?? ?? ?? 01 E9)
         *  FFXIVClientInterface/Client/Game/ActionManager.cs
         *      BaseAddress (E8 ?? ?? ?? ?? 33 C0 E9 ?? ?? ?? ?? 8B 7D 0C)
         *      GetRecastGroup (E8 ?? ?? ?? ?? 8B D0 48 8B CD 8B F0) 
         *      GetGroupTimer (E8 ?? ?? ?? ?? 0F 57 FF 48 85 C0)
         *      GetAdjustedActionId (E8 ?? ?? ?? ?? 8B F8 3B DF) 
         *      
         *  Helper/UiHelper.GameFunctions.cs
         *      _gameAlloc (E8 ?? ?? ?? ?? 45 8D 67 23)
         *      _getGameAllocator (E8 ?? ?? ?? ?? 8B 75 08)
         *      _playSe (E8 ?? ?? ?? ?? 4D 39 BE ?? ?? ?? ??)
         *      
         *  JobBars.cs
         *      receiveActionEffectFuncPtr (4C 89 44 24 18 53 56 57 41 54 41 57 48 81 EC ?? 00 00 00 8B F9)
         *      actorControlSelfPtr (E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64)
         *  UIBuilder.cs
         *      loadAssetsPtr (E8 ?? ?? ?? ?? 48 8B 84 24 ?? ?? ?? ?? 41 B9 ?? ?? ?? ??)
         *      loadTexAllocPtr (48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8B 01 49 8B D8 48 8B FA 48 8B F1 FF 50 48)
         *      texUnallocPtr (E8 ?? ?? ?? ?? C6 43 10 02)
         *      
         *  PartyList/PartyList.cs
         *      GroupManager (48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 80 B8 ?? ?? ?? ?? ?? 76 50)
         *      CrossRealmGroupManagerPtr (77 71 48 8B 05)
         *      CompanionManagerPtr (4C 8B 15 ?? ?? ?? ?? 4C 8B C9)
         *      GetCrossRealmMemberCountPtr (E8 ?? ?? ?? ?? 3C 01 77 4B)
         *      GetCrossMemberByGrpIndexPtr (E8 ?? ?? ?? ?? 44 89 7C 24 ?? 4C 8B C8)
         *      GetCompanionMemberCountPtr (E8 ?? ?? ?? ?? 8B D3 85 C0)
         *      
         *  GaugeManager (TargetManager from Dalamud)
         *      48 8B 05 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? FF 50 ?? 48 85 DB
         */

        public void Initialize(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;

            if (!FFXIVClientStructs.Resolver.Initialized) FFXIVClientStructs.Resolver.Initialize();

            UIIconManager.Initialize(pluginInterface);
            UIHelper.Setup(pluginInterface);
            UIColor.SetupColors();

            Party = new PList(pluginInterface, pluginInterface.TargetModuleScanner); // TEMP

            Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Config.Initialize(PluginInterface);
            SetupActions();

            IntPtr receiveActionEffectFuncPtr = PluginInterface.TargetModuleScanner.ScanText("4C 89 44 24 18 53 56 57 41 54 41 57 48 81 EC ?? 00 00 00 8B F9");
            receiveActionEffectHook = new Hook<ReceiveActionEffectDelegate>(receiveActionEffectFuncPtr, ReceiveActionEffect);
            receiveActionEffectHook.Enable();

            IntPtr actorControlSelfPtr = pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64");
            actorControlSelfHook = new Hook<ActorControlSelfDelegate>(actorControlSelfPtr, ActorControlSelf);
            actorControlSelfHook.Enable();

            PluginInterface.UiBuilder.Draw += BuildSettingsUI;
            PluginInterface.UiBuilder.Draw += Animate;
            PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfig;
            PluginInterface.Framework.OnUpdateEvent += FrameworkOnUpdate;
            PluginInterface.ClientState.TerritoryChanged += ZoneChanged;
            SetupCommands();
        }

        public void Dispose() {
            receiveActionEffectHook?.Disable();
            actorControlSelfHook?.Disable();

            Thread.Sleep(500);

            receiveActionEffectHook?.Dispose();
            actorControlSelfHook?.Dispose();
            receiveActionEffectHook = null;
            actorControlSelfHook = null;

            PluginInterface.UiBuilder.Draw -= BuildSettingsUI;
            PluginInterface.UiBuilder.Draw -= Animate;
            PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfig;
            PluginInterface.Framework.OnUpdateEvent -= FrameworkOnUpdate;
            PluginInterface.ClientState.TerritoryChanged -= ZoneChanged;

            GManager = null;
            BManager = null;
            CDManager = null; // TODO: dispose

            Animation.Cleanup();
            UIIconManager.Dispose();
            UIBuilder.Dispose();
            UIHelper.Dispose();

            RemoveCommands();
        }

        private void SetupActions() {
            var sheet = PluginInterface.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where(
                x => !string.IsNullOrEmpty(x.Name) && (x.IsPlayerAction || x.ClassJob.Value != null) && !x.IsPvP // weird conditions to catch things like enchanted RDM spells
            );
            foreach (var item in sheet) {
                var name = item.Name.ToString();
                var attackType = item.ActionCategory.Value.Name.ToString();
                var actionId = item.ActionCategory.Value.RowId;
                if (actionId == 2 || actionId == 3) { // spell or weaponskill
                    GCDs.Add(item.RowId);
                }
            }
        }

        private void Animate() {
            Animation.Tick();
        }

        private void FrameworkOnUpdate(Framework framework) {
            var addon = UIHelper.ParameterAddon;

            if (!PlayerExists) {
                if (Initialized && addon == null) Logout();
                return;
            }

            if (addon == null || addon->RootNode == null) return;

            if (!Initialized) {
                InitializeUI();
                return;
            }

            CheckForJobChange();
            Tick();
            CheckForPartyChange();
            CheckForHUDChange(addon);
        }

        private void Logout() {
            PluginLog.Log("==== LOGOUT ===");
            UIIconManager.Manager.Reset();
            Animation.Cleanup();

            Initialized = false;
            CurrentJob = JobIds.OTHER;
        }

        private void InitializeUI() {
            PluginLog.Log("==== INIT ====");
            UIIconManager.Manager.Reset();

            UIBuilder.Initialize(PluginInterface);
            GManager = new GaugeManager(PluginInterface, Party);
            BManager = new BuffManager();
            CDManager = new CooldownManager(PluginInterface, Party);
            UIBuilder.Builder.HideAllBuffs();
            UIBuilder.Builder.HideAllGauges();

            Initialized = true;
        }

        private void CheckForPartyChange() {
            if (Party.Count < LastPartyCount) { // someone left the party. this means that some buffs might not make sense anymore, so reset it
                BManager.SetJob(CurrentJob);
            }
            LastPartyCount = Party.Count;
        }

        private void CheckForJobChange() {
            var jobId = PluginInterface.ClientState.LocalPlayer.ClassJob;
            JobIds job = jobId.Id < 19 ? JobIds.OTHER : (JobIds)jobId.Id;

            if (job != CurrentJob) {
                CurrentJob = job;
                PluginLog.Log($"SWITCHED JOB TO {CurrentJob}");
                Reset();
            }
        }

        private void Tick() {
            var inCombat = PluginInterface.ClientState.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat];
            GManager.Tick(inCombat);
            BManager.Tick(inCombat);
            CDManager.Tick();
        }

        private void CheckForHUDChange(AtkUnitBase* addon) {
            var currentPosition = UIHelper.GetNodePosition(addon->RootNode); // changing HP/MP in hud layout
            var currentScale = UIHelper.GetNodeScale(addon->RootNode);

            if (currentPosition != LastPosition || currentScale != LastScale) {
                GManager.UpdatePositionScale();
                BManager.UpdatePositionScale();
            }
            LastPosition = currentPosition;
            LastScale = currentScale;
        }

        private void Reset() {
            GManager?.SetJob(CurrentJob);
            BManager?.SetJob(CurrentJob);
        }

        // ======= COMMANDS ============

        private void SetupCommands() {
            PluginInterface.CommandManager.AddHandler("/jobbars", new Dalamud.Game.Command.CommandInfo(OnCommand) {
                HelpMessage = $"Open config window for {Name}",
                ShowInHelp = true
            });
        }

        private void OnOpenConfig(object sender, EventArgs eventArgs) {
            Visible = true;
        }

        public void OnCommand(object command, object args) {
            Visible = !Visible;
        }

        public void RemoveCommands() {
            PluginInterface.CommandManager.RemoveHandler("/jobbars");
        }
    }
}
