using FFXIVClientStructs.FFXIV.Client.Game;
using JobBars.Atk;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBars.Cooldowns {
    public class CooldownTracker {
        public enum TrackerState {
            None,
            Running,
            OnCD,
            OffCD
        }

        private readonly CooldownConfig Config;

        private TrackerState State = TrackerState.None;
        private DateTime LastActiveTime;
        private Item LastActiveTrigger;
        private float TimeLeft;

        private UICooldownItem UI;

        public TrackerState CurrentState => State;
        public ActionIds Icon => Config.Icon;

        public CooldownTracker( CooldownConfig config ) {
            Config = config;
        }

        public void ProcessAction( Item action ) {
            if( Config.Triggers.Contains( action ) ) SetActive( action );
        }

        private void SetActive( Item trigger ) {
            State = Config.Duration == 0 ? TrackerState.OnCD : TrackerState.Running;
            LastActiveTime = DateTime.Now;
            LastActiveTrigger = trigger;
        }

        public void Tick( Dictionary<Item, Status> buffDict ) {
            if( State != TrackerState.Running && AtkHelper.CheckForTriggers( buffDict, Config.Triggers, out var trigger ) ) SetActive( trigger );

            if( State == TrackerState.Running ) {
                TimeLeft = AtkHelper.TimeLeft( JobBars.Configuration.CooldownsHideActiveBuffDuration ? 0 : Config.Duration, buffDict, LastActiveTrigger, LastActiveTime );
                if( TimeLeft <= 0 ) {
                    TimeLeft = 0;
                    State = TrackerState.OnCD; // mitigation needs to have a CD
                }
            }
            else if( State == TrackerState.OnCD ) {
                TimeLeft = ( float )( Config.CD - ( DateTime.Now - LastActiveTime ).TotalSeconds );

                if( TimeLeft <= 0 ) {
                    State = TrackerState.OffCD;
                }
            }
        }

        public void TickUI( UICooldownItem ui, float percent ) {
            if( UI != ui || UI?.IconId != Config.Icon ) {
                UI = ui;
                SetupUI();
            }

            UI.Show();

            if( State == TrackerState.None ) {
                ui.SetOffCD();
                ui.SetText( "" );
                ui.SetNoDash();
            }
            else if( State == TrackerState.Running ) {
                ui.SetOffCD();
                ui.SetText( ( ( int )Math.Round( TimeLeft ) ).ToString() );
                if( Config.ShowBorderWhenActive ) {
                    ui.SetDash( percent );
                }
                else {
                    ui.SetNoDash();
                }
            }
            else if( State == TrackerState.OnCD ) {
                ui.SetOnCD( JobBars.Configuration.CooldownsOnCDOpacity );
                ui.SetText( ( ( int )Math.Round( TimeLeft ) ).ToString() );
                ui.SetNoDash();
            }
            else if( State == TrackerState.OffCD ) {
                ui.SetOffCD();
                ui.SetText( "" );
                if( Config.ShowBorderWhenOffCD ) {
                    ui.SetDash( percent );
                }
                else {
                    ui.SetNoDash();
                }
            }
        }

        private void SetupUI() {
            UI.LoadIcon( Config.Icon );
        }

        public void Reset() {
            State = TrackerState.None;
        }
    }
}