using FFXIVClientStructs.FFXIV.Client.Game;
using JobBars.Data;
using JobBars.Helper;
using JobBars.Nodes.Cooldown;
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
            if( State != TrackerState.Running && UiHelper.CheckForTriggers( buffDict, Config.Triggers, out var trigger ) ) SetActive( trigger );

            if( State == TrackerState.Running ) {
                TimeLeft = UiHelper.TimeLeft( JobBars.Configuration.CooldownsHideActiveBuffDuration ? 0 : Config.Duration, buffDict, LastActiveTrigger, LastActiveTime );
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

        public void TickUi( CooldownNode node, float percent ) {
            if( node == null ) return;

            if( node?.IconId != Config.Icon ) node.LoadIcon( Config.Icon );

            node.IsVisible = true;

            if( State == TrackerState.None ) {
                node.SetOffCd();
                node.SetText( "" );
                node.SetNoDash();
            }
            else if( State == TrackerState.Running ) {
                node.SetOffCd();
                node.SetText( ( ( int )Math.Round( TimeLeft ) ).ToString() );
                if( Config.ShowBorderWhenActive ) {
                    node.SetDash( percent );
                }
                else {
                    node.SetNoDash();
                }
            }
            else if( State == TrackerState.OnCD ) {
                node.SetOnCd();
                node.SetText( ( ( int )Math.Round( TimeLeft ) ).ToString() );
                node.SetNoDash();
            }
            else if( State == TrackerState.OffCD ) {
                node.SetOffCd();
                node.SetText( "" );
                if( Config.ShowBorderWhenOffCD ) node.SetDash( percent );
                else node.SetNoDash();
            }
        }

        public void Reset() {
            State = TrackerState.None;
        }
    }
}